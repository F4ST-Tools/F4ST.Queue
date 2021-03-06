﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using F4ST.Common.Containers;
using F4ST.Common.Extensions;
using F4ST.Common.Tools;
using F4ST.Queue.Extensions;
using F4ST.Queue.QMessageModels;
using F4ST.Queue.QMessageModels.RequestMessages;
using F4ST.Queue.Tools;
using HttpMultipartParser;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace F4ST.Queue.Receivers
{
    public class BaseWebReceiver : Receiver<QWebRequestMessage, QWebResponse, QWebRequestMessage>
    {
        protected override bool HaveRequestMessage => true;
        protected override bool HaveSendMessage => false;

        private static readonly HttpClient RequestClient;

        static BaseWebReceiver()
        {
            RequestClient = new HttpClient(new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                AllowAutoRedirect = false,
                UseCookies = false,
            });
        }

        public BaseWebReceiver(QSettingModel qSetting) : base(qSetting)
        {
        }

        protected override async Task<QWebResponse> ProcessRequestMessage(QWebRequestMessage request)
        {
            var res = await SendRequest(request, SettingModel);

            return res;
        }

        private readonly List<string> _blockedHeader = new List<string>()
        {
            "content-length", "Content-Length", "content-type", "Content-Type"
        };

        private async Task<QWebResponse> SendRequest(QWebRequestMessage request, QSettingModel settingModel)
        {
            var res = new QWebResponse()
            {
                TraceId = request.TraceId
            };

            try
            {
                HttpMethod httpMethod;
                switch (request.HttpMethod.ToUpper())
                {
                    case "POST":
                        httpMethod = HttpMethod.Post;
                        break;
                    case "GET":
                        httpMethod = HttpMethod.Get;
                        break;
                    case "PUT":
                        httpMethod = HttpMethod.Put;
                        break;
                    case "DELETE":
                        httpMethod = HttpMethod.Delete;
                        break;
                    case "OPTIONS":
                        httpMethod = HttpMethod.Options;
                        break;
                    case "HEAD":
                        httpMethod = HttpMethod.Head;
                        break;
                    default:
                        httpMethod = HttpMethod.Get;
                        break;
                }

                var appSetting = IoC.Resolve<IAppSetting>();

                var url = new Uri(appSetting.Get("EngineUrl"));
                request.Domain = url.Authority;
                request.Scheme = url.Scheme;

                var message = new HttpRequestMessage(httpMethod,
                    new Uri(new Uri(request.BaseUrl),
                        $"{request.Arguments}{request.QueryStrings}"));

                //Debugger.Break();
                if (request.Headers?.Any() ?? false)
                {
                    if (request.Headers.Any(k => k.Key == "Host"))
                    {
                        request.Headers.Remove("Host");
                        request.Headers.Add("Host", new[] {request.Domain});
                    }

                    if (request.Headers.ContainsKey("Content-Type") &&
                        request.Headers["Content-Type"][0].StartsWith("application/x-www-form-urlencoded"))
                    {
                        //message.Content = new StringContent(request.Body);
                        var items = HttpUtility.ParseQueryString(request.Body.FromBytes());
                        message.Content = new FormUrlEncodedContent(items.ToNameValueCollection<string, string>());
                    }

                    if (request.Headers.ContainsKey("Content-Type") &&
                        request.Headers["Content-Type"][0].StartsWith("multipart/form-data; boundary"))
                    {
                        var parser = MultipartFormDataParser.Parse(new MemoryStream(request.Body), Encoding.UTF8);

                        var cont = new MultipartFormDataContent();
                        foreach (var file in parser.Files ?? new List<FilePart>())
                        {
                            var f = new StreamContent(file.Data);

                            f.Headers.Add("Content-Disposition",
                                new string(Encoding.UTF8
                                    .GetBytes($"form-data; name=\"{file.Name}\"; filename=\"{file.FileName}\"")
                                    .Select(b => (char) b).ToArray()));
                            f.Headers.Add("Content-Type", file.ContentType);

                            if (!string.IsNullOrWhiteSpace(file.Name) && !string.IsNullOrWhiteSpace(file.FileName))
                            {
                                cont.Add(f, file.Name, file.FileName);
                            }
                            else if (!string.IsNullOrWhiteSpace(file.Name))
                            {
                                cont.Add(f, file.Name);
                            }
                            else
                            {
                                cont.Add(f);
                            }
                        }

                        foreach (var item in parser.Parameters ?? new List<ParameterPart>())
                        {
                            cont.Add(new StringContent(item.Data), item.Name);
                        }

                        message.Content = cont;
                    }

                    if (request.Headers.ContainsKey("Content-Type") &&
                        request.Headers["Content-Type"][0].StartsWith("application/json"))
                    {
                        message.Content = new StringContent(request.Body.FromBytes(), Encoding.UTF8,
                            request.Headers["Content-Type"][0]);
                    }

                    foreach (var header in request.Headers)
                    {
                        if (_blockedHeader.Contains(header.Key.ToLower()))
                        {
                            continue;
                        }

                        if (header.Key == "Content-Type" &&
                            header.Value[0].StartsWith("application/x-www-form-urlencoded"))
                        {
                            continue;
                        }

                        if (!string.IsNullOrWhiteSpace(
                            message
                                .Headers
                                .FirstOrDefault(d =>
                                    string.Equals(d.Key, header.Key, StringComparison.CurrentCultureIgnoreCase)).Key))
                        {
                            message.Headers.Remove(header.Key);
                            message.Headers.Remove(header.Key.ToLower());
                        }

                        message.Headers.Add(header.Key, header.Value);
                    }
                }

                message.Headers.Add("MIP", request.IP);

                var imp = Globals.GetImplementedInterfaceOf<IWebReceiverExt>();
                if (imp?.Any() ?? false)
                {
                    foreach (var item in imp)
                    {
                        item.BeforeSend(message, request);
                    }
                }

                var cancellationToken = new CancellationTokenSource();
                cancellationToken.CancelAfter((settingModel.Timeout ?? 15 / 1000) * 1000);

                var wRes = await RequestClient.SendAsync(message, cancellationToken.Token);

                if (wRes == null)
                    return res;

                res.Status = (int) wRes.StatusCode;

                res.Headers = new Dictionary<string, string[]>();

                var headers =
                    wRes.Content.Headers.Select(k => new KeyValuePair<string, IEnumerable<string>>(k.Key, k.Value))
                        .ToList();
                if (wRes.Headers != null)
                {
                    var keys = headers.Select(c => c.Key);
                    var items = wRes.Headers.Where(c => !keys.Contains(c.Key));

                    headers.AddRange(items);
                }

                foreach (var item in headers)
                {
                    if (item.Key == "Transfer-Encoding")
                        continue;

                    res.Headers.Add(item.Key, item.Value.ToArray());
                }

                //var content = await wRes.Content.ReadAsStringAsync();
                var contB = await wRes.Content.ReadAsByteArrayAsync();
                var content = Convert.ToBase64String(contB);
                res.Response = content;
            }
            catch (Exception e)
            {
                var obj = new
                {
                    request,
                    settingModel,
                    e
                };
                res.Status = (int) HttpStatusCode.InternalServerError;

                if (Debugger.IsAttached)
                {
                    res.Response = JsonConvert.SerializeObject(obj).Base64Encode();
                }
            }

            return res;
        }

        protected override async Task ProcessSendMessage(QWebRequestMessage request)
        {
        }

        public void Start()
        {
        }
    }
}
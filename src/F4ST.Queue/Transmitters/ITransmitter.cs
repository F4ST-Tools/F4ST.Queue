using System;
using System.Threading.Tasks;
using F4ST.Common.Containers;
using F4ST.Queue.QMessageModels;
using F4ST.Queue.QMessageModels.RequestMessages;

namespace F4ST.Queue.Transmitters
{
    public interface ITransmitter : ITransient, IDisposable
    {
        /// <summary>
        /// ارسال درخواست و انتظار جهت دریافت پاسخ
        /// </summary>
        /// <param name="request">اطلاعات ارسالی</param>
        /// <param name="setting">اطلاعات صف</param>
        /// <returns>پاسخ دریافتی</returns>
        Task<QBaseResponse> Request(QSettingModel setting, QBaseRequest request);

        /// <summary>
        /// ارسال درخواست بدون پاسخ
        /// </summary>
        /// <param name="request">اطلاعات ارسالی</param>
        /// <param name="setting">اطلاعات صف</param>
        Task Send(QSettingModel setting, QBaseMessage request);
    }
}
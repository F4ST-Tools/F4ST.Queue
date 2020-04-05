using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using F4ST.Common.Tools;
using F4ST.Queue.QMessageModels;

namespace F4ST.Queue.Extensions
{
    public static class QueueExt
    {
        public static IEnumerable<QSettingModel> GetQSettings(this IAppSetting appSetting, string section = "QueueSettings")
        {
            return appSetting.Get<List<QSettingModel>>(section);
        }

        public static QSettingModel GetQSetting(this IAppSetting appSetting, string name,
            string section = "QueueSettings")
        {
            return appSetting.Get<List<QSettingModel>>(section)
                .First(s => s.Active && s.Name == name);
        }

        public static IEnumerable<KeyValuePair<TKey, TValue>> ToNameValueCollection<TKey, TValue>(this NameValueCollection col)
        {
            var dict = new List<KeyValuePair<TKey, TValue>>();
            var keyConverter = TypeDescriptor.GetConverter(typeof(TKey));
            var valueConverter = TypeDescriptor.GetConverter(typeof(TValue));

            foreach (string name in col)
            {
                var key = (TKey)keyConverter.ConvertFromString(name);

                foreach (var v in col.GetValues(name) ?? new string[0])
                {
                    //TValue value = (TValue) valueConverter.ConvertFromString(col[name]);
                    var value = (TValue)valueConverter.ConvertFromString(v);
                    dict.Add(new KeyValuePair<TKey, TValue>(key, value));
                }
            }

            return dict;
        }

    }
}
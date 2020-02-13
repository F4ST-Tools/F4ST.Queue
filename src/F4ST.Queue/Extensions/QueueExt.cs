﻿using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using F4ST.Common.Tools;
using F4ST.Queue.QMessageModels;

namespace F4ST.Queue.Extensions
{
    public static class QueueExt
    {
        public static QSettingModel GetSetting(this IAppSetting appSetting, string name,
            string section = "QueueSettings")
        {
            return appSetting.Get<List<QSettingModel>>(section)
                .First(s => s.Active && s.Name == name);
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this NameValueCollection col)
        {
            var dict = new Dictionary<TKey, TValue>();
            var keyConverter = TypeDescriptor.GetConverter(typeof(TKey));
            var valueConverter = TypeDescriptor.GetConverter(typeof(TValue));

            foreach (string name in col)
            {
                TKey key = (TKey)keyConverter.ConvertFromString(name);
                TValue value = (TValue)valueConverter.ConvertFromString(col[name]);
                dict.Add(key, value);
            }

            return dict;
        }

    }
}
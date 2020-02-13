using F4ST.Queue.QMessageModels;

namespace F4ST.Queue
{
    internal static class QCreateConnectionString
    {
        public static string CreateConnection(QSettingModel setting, bool isTransmitter)
        {
            var res = $"host={setting.ServerAddress}";

            if (isTransmitter)
                res += ";publisherConfirms=true";


            if (setting.Timeout != null)
                res += $";timeout={setting.Timeout.Value}";

            if (!string.IsNullOrWhiteSpace(setting.UserName))
            {
                res += $";username={setting.UserName};password={setting.Password}";
            }

            if (!string.IsNullOrWhiteSpace(setting.VirtualHost))
            {
                res += $";virtualHost={setting.VirtualHost}";
            }

            return res;
        }
    }
}
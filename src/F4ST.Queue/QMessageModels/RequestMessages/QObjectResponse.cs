namespace F4ST.Queue.QMessageModels.RequestMessages
{
    public class QObjectResponse:QBaseResponse
    {
        /// <summary>
        /// پاسخ ارسالی
        /// </summary>
        public object Response { get; set; }
    }
}
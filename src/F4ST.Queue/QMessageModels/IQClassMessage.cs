namespace F4ST.Queue.QMessageModels
{
    public interface IQClassMessage
    {
        string Lang { get; set; }
        string MethodName { get; set; }
        object[] Parameters { get; set; }
    }
}
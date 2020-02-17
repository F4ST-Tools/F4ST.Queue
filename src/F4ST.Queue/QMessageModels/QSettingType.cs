namespace F4ST.Queue.QMessageModels
{
    /// <summary>
    /// نوع های ارسال /دریافت اطلاعات 
    /// </summary>
    public enum QSettingType
    {
        /// <summary>
        /// درخواست های وب سرویسی
        /// </summary>
        WebService = 0,

        /// <summary>
        /// ارتباط با اینجن ها
        /// </summary>
        RPC = 1,

        /// <summary>
        /// درخواست های وب
        /// </summary>
        Web = 2,

        /// <summary>
        /// Resources, css, js, image
        /// </summary>
        Resource = 3
    }
}
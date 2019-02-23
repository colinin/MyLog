namespace Runtime.Command
{
    /// <summary>
    /// 执行命令类别
    /// </summary>
    public enum CommandType
    {
        /// <summary>
        /// 无操作
        /// </summary>
        None = 0,
        /// <summary>
        /// 通知
        /// </summary>
        Notification = 2,
        /// <summary>
        /// 文件操作
        /// </summary>
        File = 4,
        /// <summary>
        /// 应用程序相关
        /// </summary>
        Application = 8,
        /// <summary>
        /// 系统相关
        /// </summary>
        System = 16
    }
}

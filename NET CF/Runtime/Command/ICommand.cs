using System;
namespace Runtime.Command.Command
{
    /// <summary>
    /// 命令接口
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// 是否帮助命令
        /// </summary>
        bool IsHelp { get; }
        /// <summary>
        /// 帮助信息
        /// </summary>
        string Help { get; }
        /// <summary>
        /// 执行命令
        /// </summary>
        void Call();
        /// <summary>
        /// 执行完毕事件
        /// </summary>
        EventHandler<CommandEventArgs> ExecutedHandler { get; set; }
    }
}

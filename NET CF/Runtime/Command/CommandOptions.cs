using System.Collections.Generic;

namespace Runtime.Command
{
    /// <summary>
    /// 命令选项
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CommandOptions<T>
    {
        /// <summary>
        /// 命令类别
        /// </summary>
        public CommandType CommandType { get; set; }
        /// <summary>
        /// 命令参数列表
        /// </summary>
        public IEnumerable<Command> Command { get; set; }
        /// <summary>
        /// 连接信息
        /// </summary>
        public Connection Connection { get; set; }
        /// <summary>
        /// 传输数据
        /// </summary>
        public T Buffer { get; set; }
    }
    /// <summary>
    /// 命令行信息
    /// </summary>
    public class Command
    {
        /// <summary>
        /// 命令
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public string Value { get; set; }
    }
}

````C#

using System;
namespace Runtime.Command
{
    /// <summary>
    /// 命令执行结果
    /// </summary>
    public class CommandResult
    {
        private readonly CommandType _commandType = CommandType.None;

        private readonly bool _successed = false;

        private readonly Exception _exception = null;

        private bool _callback = false;

        private string _callbackMsg = string.Empty;
        /// <summary>
        /// 命令类别
        /// </summary>
        public CommandType CommandType { get { return _commandType; } }
        /// <summary>
        /// 是否执行成功
        /// </summary>
        public bool Success { get { return _successed; } }
        /// <summary>
        /// 异常信息
        /// </summary>
        public Exception Exception { get { return _exception; } }
        /// <summary>
        /// 是否反馈调用者
        /// </summary>
        public bool ServerCall
        {
            get
            {
                return _callback;
            }
            set
            {
                _callback = value;
            }
        }
        /// <summary>
        /// 返回调用者信息
        /// </summary>
        public string ServerCallMsg
        {
            get
            {
                return _callbackMsg;
            }
            set
            {
                _callbackMsg = value;
            }
        }

        public CommandResult()
            : this(CommandType.None, true)
        {

        }

        public CommandResult(CommandType commandType, bool success)
            : this(commandType, success, null)
        {

        }

        public CommandResult(CommandType commandType, bool success, Exception exception)
            : this(commandType, success, exception, false, null)
        {

        }

        public CommandResult(CommandType commandType, bool success, Exception exception, bool callback, string callbackMsg)
        {
            _commandType = commandType;
            _successed = success;
            _exception = exception;
            _callback = callback;
            _callbackMsg = callbackMsg;
        }
    }

    public class CommandEventArgs : EventArgs
    {
        private readonly CommandResult _commandResult = null;
        public CommandResult CommandResult { get { return _commandResult; } }
        public CommandEventArgs(CommandResult commandResult)
        {
            _commandResult = commandResult;
        }
    }
}

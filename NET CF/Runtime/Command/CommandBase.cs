````C#
using System;
using System.Linq;
using System.Collections.Generic;

namespace Runtime.Command
{
    public abstract class CommandBase<T> : ICommand
    {
        private readonly CommandOptions<T> _commandOptions;
        private readonly ILogger _logger;
        private Exception _tiggerException = null;
        private bool _isHelp = false;
        protected ILogger Logger
        {
            get
            {
                return _logger;
            }
        }

        public CommandBase(CommandOptions<T> commandOptions, ILogger logger)
        {
            _commandOptions = commandOptions;
            _logger = logger;
        }

        #region ICommand 成员
        public EventHandler<CommandEventArgs> ExecutedHandler { get; set; }

        protected T Buffer
        {
            get
            {
                return _commandOptions.Buffer;
            }
        }

        protected Exception TiggerException
        {
            get
            {
                return _tiggerException;
            }
        }

        public void Call()
        {
            try
            {
                _isHelp = HelpCommand(_commandOptions.Command);
                if (!IsHelp)
                {
                    OnBeforExecute();
                    OnExecute();
                }
                OnExecuted();
            }
            catch (Exception ex)
            {
                OnExecuteException(ex);
            }
        }
        public virtual bool IsHelp { get { return _isHelp; } }
        /// <summary>
        /// 子命令需实现命令帮助文本
        /// </summary>
        public abstract string Help { get; }
        /// <summary>
        /// 实现此方法以执行命令
        /// </summary>
        /// <param name="commands">命令参数列表</param>
        protected abstract void Execute(IEnumerable<Command> commands);

        private void OnExecute()
        {
            if (_commandOptions.Connection != null && _commandOptions.Connection.Server != null)
            {
                _logger.Info(GetType(), "执行命令,来自：" + _commandOptions.Connection.Server.ToString());
            }
            Execute(_commandOptions.Command);
        }

        public void OnExecuteException(Exception ex)
        {
            _logger.Info(GetType(), "执行命令失败:" + ex.Message + ", 命令类别：" + _commandOptions.CommandType.ToString());
            _tiggerException = ex;
            CommandResult commandResult = new CommandResult(_commandOptions.CommandType, true, _tiggerException, true, "客户端执行命令失败,错误消息：" + _tiggerException.Message);
            OnExecuteException(commandResult);
            TiggerExecutedHandler(commandResult);
        }
        /// <summary>
        /// 重写此方法以执行自定义异常操作
        /// </summary>
        /// <param name="commandResult"></param>
        protected virtual void OnExecuteException(CommandResult commandResult)
        {
            _logger.Exception(GetType(), commandResult.Exception);
        }


        public void OnBeforExecute()
        {
            _logger.Info(GetType(), "预执行命令开始");
            OnBeforExecute(_commandOptions.Command);
        }
        /// <summary>
        /// 重写此方法以执行自定义预处理操作
        /// </summary>
        /// <param name="commands"></param>
        protected virtual void OnBeforExecute(IEnumerable<Command> commands)
        {

        }

        public void OnExecuted()
        {
            _logger.Info(GetType(), "执行命令完毕!");
            CommandResult commandResult = new CommandResult(_commandOptions.CommandType, _tiggerException == null, _tiggerException);
            if (commandResult.Success)
            {
                if (IsHelp)
                {
                    commandResult.ServerCall = true;
                    commandResult.ServerCallMsg = Help;
                    TiggerExecutedHandler(commandResult);
                    return;
                }
            }
            OnExecuted(commandResult);
            TiggerExecutedHandler(commandResult);
        }
        /// <summary>
        /// 重写此方法以执行自定义命令执行完毕操作
        /// </summary>
        /// <param name="commandResult"></param>
        protected virtual void OnExecuted(CommandResult commandResult)
        {

        }

        private void TiggerExecutedHandler(CommandResult commandResult)
        {
            if (ExecutedHandler != null)
            {
                ExecutedHandler.Invoke(this, new CommandEventArgs(commandResult));
            }
        }

        #endregion
        /// <summary>
        /// 重写此方法以自定义帮助命令行
        /// </summary>
        protected virtual bool HelpCommand(IEnumerable<Command> commands)
        {
            return commands.Any(x => x.Key.ToLower().Contains("-help") || x.Key.ToLower().Contains("-h"));
        }
    }
}

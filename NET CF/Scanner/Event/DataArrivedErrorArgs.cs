using System;

namespace Scanner.Event
{

    public sealed class DataArrivedErrorArgs : EventArgs
    {
        private Exception _exception;
        public Exception Exception
        {
            get
            {
                return _exception;
            }
        }
        public DataArrivedErrorArgs(Exception exception)
        {
            _exception = exception;
        }
    }
}

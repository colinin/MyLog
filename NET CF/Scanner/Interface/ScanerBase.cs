using System;
using Scanner.Event;
namespace Scanner.Interface
{
    public abstract class ScanerBase : IScaner
    {
        public abstract bool Ready { get; }

        public abstract bool Init();

        public abstract bool EnableScan();

        public abstract bool DisbledScan();

        public virtual void RefreshScan()
        {
            DisbledScan();
            EnableScan();
        }

        protected virtual void OnBarCodeInitialFailed(object sender, DataArrivedErrorArgs e)
        {
            if (OnBarCodeInitFailed != null)
            {
                OnBarCodeInitFailed.Invoke(sender, e);
            }
        }

        protected virtual void OnBarCodeFailed(object sender, DataArrivedErrorArgs e)
        {
            if (OnBarCodeChangedFailed != null)
            {
                OnBarCodeChangedFailed.Invoke(sender, e);
            }
        }

        protected virtual void OnBarCodeChanegd(object sender, DataArrivedArgs e)
        {
            if (OnBarCodeChanegdSuccess != null)
            {
                OnBarCodeChanegdSuccess.Invoke(sender, e);
            }
        }

        /// <summary>
        /// 条码扫描时事件
        /// </summary>
        public event EventHandler<DataArrivedArgs> OnBarCodeChanegdSuccess;
        /// <summary>
        /// 条码扫描错误事件
        /// </summary>
        public event EventHandler<DataArrivedErrorArgs> OnBarCodeChangedFailed;
        /// <summary>
        /// 条码扫描控件初始化错误事件
        /// </summary>
        public event EventHandler<DataArrivedErrorArgs> OnBarCodeInitFailed;
    }
}

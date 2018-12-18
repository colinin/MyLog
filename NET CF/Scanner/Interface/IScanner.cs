using System;
using Scanner.Event;
namespace Scanner.Interface
{
    /// <summary>
    /// 条码扫描控件接口
    /// </summary>
    public interface IScaner
    {
        /// <summary>
        /// 是否就绪
        /// </summary>
        bool Ready { get; }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        bool Init();
        /// <summary>
        /// 启用控件
        /// </summary>
        /// <returns></returns>
        bool EnableScan();
        /// <summary>
        /// 禁用控件
        /// </summary>
        /// <returns></returns>
        bool DisbledScan();
        /// <summary>
        /// 刷新控件状态
        /// </summary>
        /// <returns></returns>
        void RefreshScan();
        /// <summary>
        /// 条码扫描时事件
        /// </summary>
        event EventHandler<DataArrivedArgs> OnBarCodeChanegdSuccess;
        /// <summary>
        /// 条码扫描错误事件
        /// </summary>
        event EventHandler<DataArrivedErrorArgs> OnBarCodeChangedFailed;
        /// <summary>
        /// 条码扫描控件初始化错误事件
        /// </summary>
        event EventHandler<DataArrivedErrorArgs> OnBarCodeInitFailed;
    }
}

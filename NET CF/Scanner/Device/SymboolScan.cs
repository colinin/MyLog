using System;
using Barcode;
using Symbol.Barcode;
using Scanner.Event;
using Scanner.Interface;

namespace Scanner.Device
{
    /// <summary>
    /// 摩托罗拉扫描仪接口实现
    /// </summary>
    public class SymboolScan : ScanerBase
    {
        private Barcode.Barcode _barcode;

        private bool _isReady = false;
        /// <summary>
        /// 是否就绪
        /// </summary>
        public override bool Ready
        {
            get { return _isReady; }
        }
        #region IScaner 成员

        public override bool Init()
        {
            try
            {
                if (_barcode == null)
                {
                    _barcode = new Barcode.Barcode();
                    _barcode.OnRead += (sender, e) =>
                    {
                        try
                        {
                            OnBarCodeChanegd(sender, new DataArrivedArgs(e.IsText ? e.Text : ""));
                        }
                        catch (Exception ex)
                        {
                            OnBarCodeFailed(sender, new DataArrivedErrorArgs(ex));
                        }
                    };
                    _barcode.OnStatus += (sender, e) =>
                    {
                        if (e.State != States.READY)
                        {
                            _isReady = false;
                        }
                    };
                }
                _isReady = true;
                return true;
            }
            catch (Exception ex)
            {
                OnBarCodeInitialFailed(this, new DataArrivedErrorArgs(ex));
            }
            return false;
        }

        public override bool EnableScan()
        {
            try
            {
                Init();
                _barcode.EnableScanner = true;
                return true;
            }
            catch (Exception ex)
            {
                OnBarCodeInitialFailed(this, new DataArrivedErrorArgs(ex));
            }
            return false;
        }

        public override bool DisbledScan()
        {
            try
            {
                _barcode.EnableScanner = false;
                _barcode.Dispose();
                _barcode = null;
                return true;
            }
            catch (Exception ex)
            {
                OnBarCodeInitialFailed(this, new DataArrivedErrorArgs(ex));
            }
            finally
            {
                _isReady = false;
            }
            return false;
        }
        #endregion
    }
}

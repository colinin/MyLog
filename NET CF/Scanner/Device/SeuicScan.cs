using System;
using SeuicSCAN;
using Scanner.Interface;
using Scanner.Event;

namespace Scanner.Device
{
    /// <summary>
    /// 东大扫描仪接口实现
    /// </summary>
    public class SeuicScan : ScanerBase
    {
        private bool _isReady = false;
        /// <summary>
        /// 是否就绪
        /// </summary>
        public override bool Ready
        {
            get { return _isReady; }
        }

        public override bool Init()
        {
            try
            {
                if (SeuicPDA_A8.Init())
                {
                    SeuicPDA_A8.BeeperEnable(false);
                    SeuicPDA_A8.ScannerContinue(false);
                    if (SeuicPDA_A8._Open())
                    {
                        SeuicPDA_A8.DataArrivedEvent += new SeuicPDA_A8.DataArrivedEventHandler(SeuicPDA_A8_DataArrivedEvent);
                        _isReady = true;
                    }
                    else
                    {
                        _isReady = false;
                    }
                }
                else
                {
                    _isReady = false;
                }
                return _isReady;
            }
            catch (Exception ex)
            {
                OnBarCodeInitialFailed(this, new DataArrivedErrorArgs(ex));
            }
            return false;
        }

        private void SeuicPDA_A8_DataArrivedEvent(string e)
        {
            try
            {
                OnBarCodeChanegd(null, new DataArrivedArgs(e));
            }
            catch (Exception ex)
            {
                OnBarCodeFailed(null, new DataArrivedErrorArgs(ex));
            }
        }

        public override bool EnableScan()
        {
            try
            {
                if (_isReady)
                {
                    _isReady = SeuicPDA_A8.ScannerEnable(true);
                }
                return _isReady;
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
                SeuicPDA_A8.DataArrivedEvent -= new SeuicPDA_A8.DataArrivedEventHandler(SeuicPDA_A8_DataArrivedEvent);
                SeuicPDA_A8._Close();
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
    }
}

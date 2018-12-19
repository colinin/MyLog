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
                IntPtr hwndBarcode = Win32API.FindWindowA(null, "2dscan");
                if (hwndBarcode != IntPtr.Zero)
                {
                    /// 关闭东大系统自带扫描器程序
                    Win32API.SendMessage(hwndBarcode, 0x0002, 0, 0);
                }
                _isReady = true;
                return _isReady;
            }
            catch (Exception ex)
            {
                OnBarCodeInitialFailed(this, new DataArrivedErrorArgs(ex));
            }
            return false;
        }

        private void SeuicPDA_A8_DataArrivedEvent(Scanner.CodeInfo codeInfo)
        {
            try
            {
                Debug.Write("Scanner barcode:"  + codeInfo.barcode);
                Debug.Write("Length:" + codeInfo.len.ToString());
                Debug.WriteLine(",Type:" + codeInfo.codetype);
                OnBarCodeChanegd(null, new DataArrivedArgs(codeInfo.barcode.Trim()));
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
                Scanner.Instance().OnScanedEvent += new Action<Scanner.CodeInfo>(SeuicPDA_A8_DataArrivedEvent);
                _isReady = Scanner.Enable();
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
                Scanner.Instance().OnScanedEvent -= new Action<Scanner.CodeInfo>(SeuicPDA_A8_DataArrivedEvent);
                return Scanner.Disable();
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

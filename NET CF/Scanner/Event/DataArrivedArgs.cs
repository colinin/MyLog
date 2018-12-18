using System;

namespace Scanner.Event
{
    public sealed class DataArrivedArgs : EventArgs
    {
        private string _barcode;
        public string BarCode
        {
            get
            {
                return _barcode;
            }
        }
        public DataArrivedArgs(string barcode)
        {
            _barcode = barcode;
        }
    }
}

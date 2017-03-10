using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nsoftware.IPWorks;

namespace Spar
{
    public class SparUdpport : Udpport
    {
        Aes _aes = null;
        public event Action<string> OnAesDataIn;
        public SparUdpport(string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
                _aes = new Aes(key);

            this.OnDataIn += (o, e) =>
            {
                /*if (OnAesDataIn != null)
                    OnAesDataIn(_aes == null ? e.Datagram : _aes.Decode(e.DatagramB));*/
                OnAesDataIn?.Invoke(_aes == null ? e.Datagram : _aes.Decode(e.DatagramB));
            };
        }

        public string AesDataToSend
        {
            set
            {
                //var data = value + "\n\u0000";
                if (_aes == null)
                {
                    this.DataToSend = value;
                    return;
                }
                this.DataToSendB = _aes.Encode(value);
            }
        }
    }
}

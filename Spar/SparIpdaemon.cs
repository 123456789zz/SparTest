using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nsoftware.IPWorks;

namespace Spar
{
    public class SparIpdaemon : Ipdaemon
    {
        Aes _aes = null;
        public event Action<object, IpdaemonDataInEventArgs, string> OnAesDataIn;

        public SparIpdaemon(string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
                _aes = new Aes(key);

            this.OnDataIn += (o, e) =>
            {
                OnAesDataIn?.Invoke(o, e, _aes == null ? e.Text : _aes.Decode(e.TextB));
                /*if (OnAesDataIn != null)
                    OnAesDataIn(o, e, _aes == null ? e.Text : _aes.Decode(e.TextB));*/
            };
        }
    }
}

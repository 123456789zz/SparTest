using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spar
{
    public class SparListener
    {
        const int SPAR_PORT_2 = 5159;
        //readonly string _key;
        SparIpdaemon _ipdaemon;
        public event Action<string> Rebooted;
        public event Action<string, string> CardScanned;
        public event Action<string> Error;
        //public event Action<string> Ready; //sparIP
        string getSparIP(string connectionId)
        {
            return _ipdaemon.Connections[connectionId].RemoteHost;
        }
        public SparListener(string key = "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f")
        {
            //_key = key;
            _ipdaemon = new SparIpdaemon(key) { LocalPort = SPAR_PORT_2 };
            _ipdaemon.OnAesDataIn += (o, e, msg) => {
                if (msg.Contains("Reboot"))
                {
                    /*if (Rebooted != null)
                        Rebooted(getSparIP(e.ConnectionId));*/
                    Rebooted?.Invoke(getSparIP(e.ConnectionId));
                    return;
                }

                /*if (CardScanned != null)
                    CardScanned(getSparIP(e.ConnectionId), Util.GetCardData(msg));*/
                CardScanned?.Invoke(getSparIP(e.ConnectionId), Util.GetCardData(msg));
            };
            _ipdaemon.OnError += (o, e) => {
                /*if (Error != null)
                    Error(e.Description);*/
                Error?.Invoke(e.Description);
            };
            _ipdaemon.OnConnectionRequest += (o, e) =>{
                e.Accept = true;
            };
            /*_ipdaemon.OnConnected += (o, e) =>
            {
                Console.WriteLine("Spar listener is connected");
            };
            _ipdaemon.OnReadyToSend += (o, e) =>
            {
                Console.WriteLine("Ready to send");
            };*/
            //_ipdaemon.OnReadyToSend += (o, e) =>
            //{
            //    //_ipdaemon.Connections[e.ConnectionId].RemoteHost;
            //    if (Ready != null)
            //        Ready(_ipdaemon.Connections[e.ConnectionId].RemoteHost);
            //};
        }
        public void Start()
        {
            _ipdaemon.Listening = true;
        }
    }
    /*public class SparListenerEventArg
    {
        public string SparIP;
        public string Data;
    }*/
}

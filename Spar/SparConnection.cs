using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Spar
{
    public class SparConnection
    {
        const int SPAR_PORT_1 = 10004;
        //readonly string _ip;
        readonly string _key;
        readonly string _usr1;
        readonly string _pwd1;
        readonly string _usr2;
        readonly string _pwd2;
        readonly SparUdpport _udp;
        string _result;
        bool _done = false;
        public SparConnection(
            string ip,
            string usr1 = "LSSYSTEM",
            string pwd1 = "LSSYSTEM_password",
            string usr2 = "user2_user_name",
            string pwd2 = "user2_password",
            string key = "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f")
        {
            _key = key;
            _usr1 = usr1;
            _pwd1 = pwd1;
            _usr2 = usr2;
            _pwd2 = pwd2;
            _udp = new SparUdpport(key)
            {
                RemoteHost = ip,
                RemotePort = SPAR_PORT_1,
                AcceptData = true,
                Active = true
            };
            _udp.OnAesDataIn += data =>
            {
                //Console.WriteLine(data);
                _result = data;
                _done = true;
            };
        }
        public string Send(string message)
        {
            _done = false;
            
            _udp.AesDataToSend = new StringBuilder(message)
                .Append(createTag("User1", string.Format("{0}, {1}", _usr1, _pwd1)))
                .Append(createTag("User2", string.Format("{0}, {1}", _usr2, _pwd2)))
                .ToString();

            if (!_done)
            {
                _udp.DoEvents();
                Thread.Sleep(600);
            }
            return _result;
        }
        public string Query()
        {
            return this.Send(createTag("Config", "?"));
        }
        public string Reboot()
        {
            return this.Send(createTag("SPARReboot", "Reboot"));
        }
        string createTag(string name, string value)
        {
            return string.Format("<{0}>{1}</{0}>\n", name, value);
        }
    }
}
 
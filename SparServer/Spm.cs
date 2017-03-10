using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Config;

namespace SparServer
{
    public class Spm
    {
        Dictionary<string, string> _card2User = new Dictionary<string, string>();
        Dictionary<string, Phone> _spar2Device = new Dictionary<string, Phone>();
        // alternatively, implement two <string,string> dictionaries with spar2mac and mac2ip

        object _locker = new object();
        HashSet<string> _inprogress = new HashSet<string>();


        public void AddDevice(string sparIP, Phone phone)
        {
            _spar2Device.Add(sparIP, phone);
        }

        public void AddUser(string cardID, string userID)
        {
            _card2User.Add(cardID, userID);
        }

        public ReqInfo Process(string sparIP, string data)
        {
            if (!_spar2Device.ContainsKey(sparIP))
            {
                Console.WriteLine("[S:{0}] Spar does not have associated phone", sparIP);
                return null;
            }

            if (!_card2User.ContainsKey(data))
            {
                Console.WriteLine("[S:{0}] Card {1} is not registered", sparIP, data);
                return new ReqInfo()
                {
                    Phone = _spar2Device[sparIP],
                    User = null
                };
            }

            return new ReqInfo() {
                Phone = _spar2Device[sparIP],
                User = _card2User[data]
            };
        }

        public string[] GetSparIPs()
        {
            return _spar2Device.Keys.ToArray();
        }
    }
}

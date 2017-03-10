using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Config
{
    public class ConfigManager
    {
        XDocument _xml = null;
        string _path = null;
        public ConfigManager(string path)
        {
            _path = path;
            _xml = XDocument.Load(_path); 
            this.Root = _xml.Root;
        }
        public XElement Root { get; private set; }

        static Spar Spar(XElement elem)
        {
            return new Spar() {
                IP = elem.GetString("ip"),
                PhoneID = elem.GetString("phoneId"),
                HostIP = elem.GetString("hostIp")
            };
        }
        public IEnumerable<Spar> Spars()
        {
            return Root.ListOf("spar").To(Spar);
        }

        static Card Card(XElement elem)
        {
            return new Card() {
                ID = elem.GetString("id"),
                UserID = elem.GetString("userId")
            };
        }
        public IEnumerable<Card> Cards()
        {
            return Root.ListOf("card").To(Card);
        }

        static Phone Phone(XElement elem)
        {
            return new Phone() {
                ID = elem.GetString("id"),
                IP = elem.GetString("ip"),
                Profile = elem.GetString("profile")
            };
        }
        public Phone Phone(string id)
        {
            return Root.ListOf("phone").ByID(id).To(Phone);
        }

        public Host Host()
        {
            return Root.ListOf("host").First().To(Host);
        }

        static Host Host(XElement elem)
        {
            return new Host()
            {
                IP = elem.GetString("ip"),
                AppID = elem.GetString("appID"),
                AppCert = elem.GetString("appCert")
            };
        }

        public Host Host(string id)
        {
            return Root.ListOf("host").ByID(id).To(Host);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Net;
using RestSharp;
using Config;

namespace ExtMob
{
    public class ExtensionMobility
    {
        public const string _encodeType = "application/x-www-form-urlencoded";
        public enum ReqType { login, logout, checklogin, checkdevice };

        public static bool isLoggedIn(string user, Phone device, Host host)
        {
            return execute(ReqType.checklogin, user, device, host).Contains(device.ID);
        }

        public static bool isBusyDevice(string user, Phone device, Host host) {
            return execute(ReqType.checkdevice, user, device, host).Contains("</userID>");
        }

        public static string logIn(string user, Phone device, Host host)
        {
            return execute(ReqType.login, user, device, host);
        }

        public static string logOut(string user, Phone device, Host host)
        {
            return execute(ReqType.logout, user, device, host);
        } 

        private static string execute(ReqType reqType, string user, Phone device, Host host)
        {
            var client = new RestClient("http://" + host.IP + ":8080/emservice/EMServiceServlet");
            var request = new RestRequest(Method.POST);
            request.AddParameter(_encodeType, encodeReq(reqType, user, device, host),
                ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.Content;
        }

        private static string encodeReq(ReqType reqType, string user, Phone device, Host host)
        {
            var encodedReq = "";
            switch (reqType)
            {
                case ReqType.login:
                    XElement xmlLogIn = new XElement("request",
                        new XElement("appInfo",
                            new XElement("appID", host.AppID),
                            new XElement("appCertificate", host.AppCert)
                        ),
                        new XElement("login",
                            new XElement("deviceName", device.ID),
                            new XElement("userID", user),
                            new XElement("deviceProfile", device.Profile),
                            new XElement("exclusiveDuration",
                                new XElement("indefinite", null)
                            )
                        )
                    );
                    encodedReq = WebUtility.UrlEncode(xmlLogIn.ToString());
                    //Console.WriteLine(encodedReq);
                    break;

                case ReqType.logout:
                    XElement xmlLogOut = new XElement("request",
                        new XElement("appInfo",
                            new XElement("appID", host.AppID),
                            new XElement("appCertificate", host.AppCert)
                        ),
                        new XElement("logout",
                            new XElement("deviceName", device.ID)
                        )
                    );
                    encodedReq = WebUtility.UrlEncode(xmlLogOut.ToString());
                    break;

                case ReqType.checklogin:
                    XElement xmlDeviceQuery = new XElement("query",
                        new XElement("appInfo",
                            new XElement("appID", host.AppID),
                            new XElement("appCertificate", host.AppCert)
                        ),
                        new XElement("userDevicesQuery",
                            new XElement("userID", user)
                        )
                    );
                    //Console.WriteLine(xmlDeviceQuery);
                    encodedReq = WebUtility.UrlEncode(xmlDeviceQuery.ToString());
                    break;

                case ReqType.checkdevice:
                    XElement xmlUserQuery = new XElement("query",
                        new XElement("appInfo",
                            new XElement("appID", host.AppID),
                            new XElement("appCertificate", host.AppCert)
                        ),
                        new XElement("deviceUserQuery",
                            new XElement("deviceName", device.ID)
                        )
                    );
                    //Console.WriteLine(xmlUserQuery);
                    encodedReq = WebUtility.UrlEncode(xmlUserQuery.ToString());
                    break;

            }
            return "xml=" + encodedReq;
        }
    }
}

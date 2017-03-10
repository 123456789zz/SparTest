using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;

namespace XCom
{
    public class XCommand
    {
        public const string _contentType = "text/xml";
        public static string PushMsg(string msg, string phoneIP)
        {
            var client = new RestClient("http://" + phoneIP);
            client.CookieContainer = new CookieContainer();
            
            var beginReq = new RestRequest("/xmlapi/session/begin", Method.POST);
            beginReq.AddHeader("Authorization", "Basic YWRtaW46");
            IRestResponse response = client.Execute(beginReq);

            /*var cookie = "";
            foreach (var c in response.Headers)
            {
                if (c.Name.ToString().Contains("Cookie"))
                    cookie = c.Value.ToString().Split(';')[0];
            }
            Console.WriteLine(cookie);*/

            var msgReq = new RestRequest("/putxml", Method.POST);
            msgReq.AddParameter(_contentType, "<Command>\r\n<UserInterface>\r\n<Message>\r\n<Alert>\r\n<Display command=\"True\">\r\n<Duration>10</Duration>\r\n<Title>Important</Title>\r\n<Text>" + msg + "</Text>\r\n</Display>\r\n</Alert>\r\n</Message>\r\n</UserInterface>\r\n</Command>",
                ParameterType.RequestBody);
            IRestResponse msgRes = client.Execute(msgReq);
            foreach (var c in msgRes.Headers)
                Console.WriteLine(c.Name.ToString() + ": " + c.Value.ToString());

            var endReq = new RestRequest("/xmlapi/session/end", Method.POST);
            IRestResponse endRes = client.Execute(endReq);
            foreach (var c in endRes.Headers)
                Console.WriteLine(c.Name.ToString() + ": " + c.Value.ToString());

            return "cat";
        }
    }
}

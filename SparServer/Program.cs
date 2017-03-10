using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spar;
using ExtMob;
using XCom;
using Config;

namespace SparServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Hit enter to start listening:");
            Console.Read();

            var cm = new ConfigManager("..\\..\\Config.xml");
            var spm = new Spm();

            foreach (var spar in cm.Spars())
            {
                // phone may not exist yet
                var phone = cm.Phone(spar.PhoneID);
                Console.WriteLine("Associate SPAR {0} with phone {1}", spar.IP, phone.ID);
                spm.AddDevice(spar.IP, phone);
            }

            foreach (var card in cm.Cards())
            {
                Console.WriteLine("Associate card {0} with {1}", card.ID, card.UserID);
                spm.AddUser(card.ID, card.UserID);
            }

            var host = cm.Host();
            var sparListener = new SparListener();

            sparListener.Rebooted += (data) =>
            {
                Console.WriteLine("Rebooted SPAR with ip: " + data);
            };

            sparListener.CardScanned += (sparIp, data) =>
            {
                //Console.WriteLine("test: " + sparIp + " test 2: " +  data);
                var reqInfo = spm.Process(sparIp, data);
                var response = "";

                // assume all spars have associated phones
                if (reqInfo.User == null)
                {
                    //Console.WriteLine("Failed swipe. Please try again.");
                    Console.WriteLine(XCommand.PushMsg("Invalid swipe", reqInfo.Phone.IP));
                    return;
                }

                if (ExtensionMobility.isLoggedIn(reqInfo.User, reqInfo.Phone, host))
                {
                    response = ExtensionMobility.logOut(reqInfo.User, reqInfo.Phone, host);
                    Console.WriteLine(response.Contains("success") ? "Goodbye " + reqInfo.User : "Logout failed");
                    return;
                }

                if (ExtensionMobility.isBusyDevice(reqInfo.User, reqInfo.Phone, host))
                {
                    Console.WriteLine("Someone else is currently logged in to the phone");
                    return;
                }

                response = ExtensionMobility.logIn(reqInfo.User, reqInfo.Phone, host);
                Console.WriteLine(response.Contains("success") ? "Welcome " + reqInfo.User : "Login failed");
            };

            sparListener.Error += (data) =>
            {
                Console.WriteLine("Error: " + data);
            };

            //sparListener.Rebooted += SparListener_Rebooted;
            //sparListener.CardScanned += SparListener_CardScanned;
            //sparListener.Error += SparListener_Error;

            try
            {
                sparListener.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: Unable to connect to start SPAR listener");
                Console.Write(e.Message);
            }

            var working = 0;
            foreach (var sparIP in spm.GetSparIPs())
            {
                try
                {
                    var con = new SparConnection(sparIP);
                    var msg = con.Reboot();
                    if (string.IsNullOrWhiteSpace(msg))
                        throw new Exception("No message coming in from SPAR " + sparIP);

                    if (msg.Contains("<SPARReboot>Reboot"))
                    {
                        Console.WriteLine("Rebooting SPAR " + sparIP);
                        working++;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: Error connecting to SPAR " + sparIP);
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }

            if (working == 0)
            {
                Console.WriteLine("No working SPAR");
                Console.WriteLine("Make sure the SPARs are set up properly");
                Console.WriteLine("End process");
                return;
            }

            Console.Read();
            Console.Read();
        }

       /* private static void SparListener_Error(string obj)
        {
            Console.WriteLine("Error " + obj);
        }


        private static void SparListener_CardScanned(string sparID, string cardID)
        {
            Console.WriteLine("Card " + cardID + " has been scanned at IP: " + sparID);
        }

        private static void SparListener_Rebooted(string obj)
        {
            Console.WriteLine("spar has been rebooted " + obj);
        }*/
    }
}

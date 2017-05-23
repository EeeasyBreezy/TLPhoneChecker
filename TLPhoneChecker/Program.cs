using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TLSharp.Core;

namespace TLPhoneChecker
{
    internal class Program
    {
        private static int API_ID = 129062;
        private static string API_HASH = "7bb45a0b8bea42bcd20e6750e1f9c8d8";
        private static FileSessionStore store = null;
        private static TelegramClient client = null;
        private static async void LaunchChecker()
        {
            var reader = new StreamReader("phones.txt");

            await client.ConnectAsync();
            while (client.IsConnected == false)
            {
            }
            while (reader.EndOfStream == false)
            {
                var number = reader.ReadLine();
                bool isRegistered = await client.IsPhoneRegisteredAsync(number);
                Console.WriteLine("{0} : {1}", number, isRegistered.ToString());
                Thread.Sleep(500);
            }
        }

        private static async Task ConnectToAPI()
        {
            await client.ConnectAsync();
        }

        private static async Task CheckPhone(string phone)
        {
            try
            {
                bool result = await client.IsPhoneRegisteredAsync(phone);
                Console.WriteLine("{0}, {1}", phone, result.ToString());
            }
            catch(InvalidOperationException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private static void Main(string[] args)
        {
            StreamReader reader = new StreamReader("phones.txt");

            store = new FileSessionStore();
            client = new TelegramClient(API_ID, API_HASH, store, "session");
            ConnectToAPI().Wait();
            while (reader.EndOfStream == false)
            {
                string number = string.Empty;
                try
                {
                    number = string.Format("+7{0}", reader.ReadLine());
                    CheckPhone(number).Wait();
                }
                catch(Exception e)
                {
                    Console.WriteLine("Number {0}: {1}", number, e.Message);
                    continue;
                }
                finally
                {
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }
            }

        }
    }
}
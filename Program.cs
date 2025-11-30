using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Detect detector = new Detect();
            detector.PanelDetect();
        }
    }

    class Detect
    {
        private List<string> panel;

        public Detect()
        {
            panel = new List<string>();
        }

        public void PanelDetect()
        {
            var panels = File.ReadAllLines("admin.txt");
            List<string> activePanel = new List<string>();

            foreach (var pan in panels)
            {
                panel.Add(pan);
            }

            Console.Write("Input the website: ");
            string website = Console.ReadLine();

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(10);

                if (File.Exists("headers.txt"))
                {
                    string[] headerz = File.ReadAllLines("headers.txt");
                    foreach (var item in headerz)
                    {
                        client.DefaultRequestHeaders.UserAgent.ParseAdd(item);
                    }
                }
                else
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                }

                int index = 0;

                while (true)
                {
                    Thread.Sleep(2000);  // web site can block you, !Don't change
                    try
                    {
                        if (index >= panel.Count)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("[-] Admin panel not found.");
                            break;
                        }

                        string site = website.TrimEnd('/') + "/" + panel[index];

                        HttpResponseMessage response = client.GetAsync(site).Result;

                        if ((int)response.StatusCode == 200)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"[+] Panel found: {site}");
                            activePanel.Add(site);
                            break;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine($"Called URL: {site}");
                        }

                        index++;
                        
                    }
                    catch (HttpRequestException e)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"[!] Request error: {e.Message}");
                        Thread.Sleep(2000); // retry after wait
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[!] Unexpected error: {e.Message}");
                        break;
                    }
                }

                if (activePanel.Count > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("\nActive Panels:");
                    foreach (var ap in activePanel)
                    {
                        Console.WriteLine(ap);
                    }
                }

                Console.ResetColor();
            }
        }
    }
}

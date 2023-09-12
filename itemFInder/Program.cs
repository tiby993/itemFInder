using System;
using System.Net;
using System.Xml;
using System.Media;

namespace RssFeedMonitor
{
    class Program
    {
        private static DateTime lastCheckDate = DateTime.Now;

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Kérlek adj meg egy RSS URL-t és egy keresendő stringet a script futtatásához.");
                Console.WriteLine("Példa: ./itemFinder <rssUrl> <KeresendoStringek>");
                return;
            }

            string rssUrl = args[0];

            Console.WriteLine("RSS feed figyelése...");

            while (true)
            {
                try
                {
                    using (XmlReader reader = XmlReader.Create(rssUrl))
                    {
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "item")
                            {
                                string title = ReadElementContent(reader, "title");
                                string link = ReadElementContent(reader, "link");
                                DateTime pubDate = DateTime.Parse(ReadElementContent(reader, "pubDate"));

                                if (IsNewEntry(title, pubDate, args))
                                {
                                    Console.WriteLine("Új elem:");
                                    Console.WriteLine("Cím: " + title);
                                    Console.WriteLine("Link: " + link);
                                    Console.WriteLine("Dátum: " + pubDate);
                                    Console.WriteLine();

                                    // Figyelmeztető hang lejátszása
                                    PlayAlertSound();
                                }
                            }
                        }
                    }

                    lastCheckDate = DateTime.Now;
                    Console.WriteLine("Következő ellenőrzés " + DateTime.Now.AddMinutes(2).ToString("HH:mm:ss") + "-kor.");
                    System.Threading.Thread.Sleep(2 * 60 * 1000); // Vár 2 percet az újabb ellenőrzésig
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hiba történt: " + ex.Message);
                }
            }
        }

        static string ReadElementContent(XmlReader reader, string elementName)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == elementName)
                {
                    return reader.ReadElementContentAsString();
                }
            }
            return string.Empty;
        }

        static bool IsNewEntry(string title, DateTime pubDate, string[] args)
        {
            foreach (string arg in args.Skip(1))

            {

                // Ellenőrizze, hogy a title tartalmazza "Ryzen9" vagy "Ryzen 9"-et, kis- és nagybetűk érzékenytlensége mellett
                if (title.IndexOf("Ryzen9", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    title.IndexOf("Ryzen 9", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    // Ellenőrizze, hogy a hirdetés újabb-e az utolsó ellenőrzés óta
                    return pubDate > lastCheckDate;
                }

            }


            return false;
        }

        static void PlayAlertSound()
        {
            try
            {
                using (SoundPlayer player = new SoundPlayer("AlertSound.wav"))
                {
                    player.Play();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Figyelmeztető hang lejátszása közben hiba történt: " + ex.Message);
            }
        }
    }
}
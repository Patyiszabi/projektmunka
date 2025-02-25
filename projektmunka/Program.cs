using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    class Message
    {
        public DateTime ido { get; set; }
        public string uzenet { get; set; }
        public string kuldoIP { get; set; }
        public string fogadoIP { get; set; }
    }

    static void Main()
    {
        string filePath = "adatforgalom.txt"; // A fájl elérési útja
        List<Message> messages = ReadMessagesFromFile(filePath);
        
        Console.Write("Adj meg egy IP-címet (pl. 192.168.0.231): ");
        string inputIP = Console.ReadLine();
        DisplayCommunicationForIP(messages, inputIP);
    }

    static List<Message> ReadMessagesFromFile(string filePath)
    {
        List<Message> messages = new List<Message>();
        string content = File.ReadAllText(filePath);
        string[] rawMessages = content.Split(".-.+.", StringSplitOptions.RemoveEmptyEntries);
        
        foreach (string rawMessage in rawMessages)
        {
            string[] parts = rawMessage.Split(".x.", StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 4) continue;

            DateTime timestamp = DateTime.Parse(parts[0].Trim());
            string text = parts[1].Trim();
            string senderIP = ConvertBinaryToIP(parts[2].Trim());
            string receiverIP = ConvertBinaryToIP(parts[3].Trim());
            
            messages.Add(new Message { ido = timestamp, uzenet = text, kuldoIP = senderIP, fogadoIP = receiverIP });
        }
        return messages;
    }

    static string ConvertBinaryToIP(string binaryIP)
    {
        string[] octets = new string[4];
        for (int i = 0; i < 4; i++)
        {
            octets[i] = Convert.ToInt32(binaryIP.Substring(i * 8, 8), 2).ToString();
        }
        return string.Join(".", octets);
    }

    static void DisplayCommunicationForIP(List<Message> messages, string ipAddress)
    {
        var relatedMessages = messages.Where(m => m.kuldoIP == ipAddress || m.fogadoIP == ipAddress).OrderBy(m => m.ido).ToList();
        if (relatedMessages.Count == 0)
        {
            Console.WriteLine("Nincs találat az adott IP-címre.");
            return;
        }

        Console.WriteLine($"Kommunikációs adatok az IP-címhez: {ipAddress}\n");
        foreach (var msg in relatedMessages)
        {
            Console.WriteLine($"[{msg.ido}] {msg.kuldoIP} -> {msg.fogadoIP}: {msg.uzenet}");
        }
    }
}

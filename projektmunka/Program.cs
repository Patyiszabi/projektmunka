using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

class Program
{
    class Uzenet
    {
        public DateTime Ido { get; set; }
        public string Szoveg { get; set; }
        public string KuldoIP { get; set; }
        public string FogadoIP { get; set; }
    }

    static void Main()
    {
        string fajl = File.ReadAllText("adatforgalom.txt", Encoding.UTF8); // A fájl elérési útja
        List<Uzenet> uzenetek = new List<Uzenet>();
        string[] formazatlanUzenetek = fajl.Split(".-.+.", StringSplitOptions.RemoveEmptyEntries);
        
        foreach (string formazatlanUzenet in formazatlanUzenetek)
        {
            string[] darabok = formazatlanUzenet.Split(".x.", StringSplitOptions.RemoveEmptyEntries);
            if (darabok.Length < 4) continue;

            DateTime ido = DateTime.Parse(darabok[0].Trim());
            string szoveg = darabok[1].Trim();
            string kuldoIP = IPCimAtvaltas(darabok[2].Trim());
            string fogadoIP = IPCimAtvaltas(darabok[3].Trim());
            
            uzenetek.Add(new Uzenet { Ido = ido, Szoveg = szoveg, KuldoIP = kuldoIP, FogadoIP = fogadoIP });
        }
        
        Console.Write("Adj meg egy IP-címet (pl. 192.168.0.231): ");
        string ipcimBekeres = Console.ReadLine();
        IPCimKereses(uzenetek, ipcimBekeres);
    }

    public static string IPCimAtvaltas(string eredetiIpcim)
    {
        string[] oktettek = new string[4];
        for (int i = 0; i < 4; i++)
        {
            oktettek[i] = Convert.ToInt32(eredetiIpcim.Substring(i * 8, 8), 2).ToString();
        }
        return string.Join(".", oktettek);
    }

    static void IPCimKereses(List<Uzenet> uzenetek, string ipcim)
    {
        List<Uzenet> talalatok = new List<Uzenet>();
        foreach (var uzenet in uzenetek)
        {
            if (uzenet.KuldoIP == ipcim || uzenet.FogadoIP == ipcim)
            {
                talalatok.Add(uzenet);
            }
        }

        talalatok.Sort((m1, m2) => m1.Ido.CompareTo(m2.Ido));

        if (talalatok.Count == 0)
        {
            Console.WriteLine("Nincs találat az adott IP-címre.");
            return;
        }

        Console.WriteLine($"Kommunikációs adatok az IP-címhez: {ipcim}\n");
        foreach (var uzenet in talalatok)
        {
            Console.WriteLine($"[{uzenet.Ido}] {uzenet.KuldoIP} -> {uzenet.FogadoIP}: {uzenet.Szoveg}");
        }
    }
}

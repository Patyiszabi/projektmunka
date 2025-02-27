using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

class Uzenet
{
    public DateTime Idopont { get; set; }
    public string Szoveg { get; set; }
    public string KuldoIP { get; set; }
    public string FogadoIP { get; set; }

    public Uzenet(DateTime idopont, string szoveg, string kuldoIP, string fogadoIP)
    {
        Idopont = idopont;
        Szoveg = szoveg;
        KuldoIP = IPCimAtvaltas(kuldoIP);
        FogadoIP = IPCimAtvaltas(fogadoIP);
    }

    private static string IPCimAtvaltas(string ipcim)
    {
        if (ipcim.Length != 32)
        {
            throw new ArgumentException("Az IP címnek 32 bites binárisnak kell lennie.");
        }

        string[] oktettek = new string[4];
        for (int i = 0; i < 4; i++)
        {
            oktettek[i] = Convert.ToInt32(ipcim.Substring(i * 8, 8), 2).ToString();
        }

        return string.Join(".", oktettek);
    }
}

class Program
{
    static List<Uzenet> Uzenetek = new List<Uzenet>();

    static void Main()
    {
        string fajlNev = "uzenetek.txt";

        if (!File.Exists(fajlNev))
        {
            Console.WriteLine("A fájl nem található!");
            return;
        }

        BeolvasFajlbol(fajlNev);
        Feladat1();
        Feladat2();
        Feladat3();
    }

    static void BeolvasFajlbol(string fajlNev)
    {
        string[] sorok = File.ReadAllLines(fajlNev, Encoding.UTF8);
        
        foreach (string sor in sorok)
        {
            if (sor.Contains(".-.-.+.+."))
                break; // Az extra adatok figyelmen kívül hagyása
            
            string[] adatok = sor.Split(".x.");
            
            if (adatok.Length >= 4)
            {
                try
                {
                    string idopontStr = adatok[0].Replace("xxx", ""); // Az elején lévő jelzés eltávolítása
                    DateTime idopont = DateTime.Parse(idopontStr);
                    
                    string szoveg = adatok[1];
                    string kuldoIP = adatok[2];
                    string fogadoIP = adatok[3].Replace(".-+.", ""); // Az üzenet végét jelző karakter levágása

                    Uzenetek.Add(new Uzenet(idopont, szoveg, kuldoIP, fogadoIP));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Hiba történt az üzenet feldolgozása során: {ex.Message}");
                }
            }
        }
    }

    static void Feladat1()
    {
        Console.WriteLine("1. Feladat: Üzenetek szűrése 10 másodperces különbséggel");
        List<Uzenet> szurtUzenetek = new List<Uzenet>();

        for (int i = 0; i < Uzenetek.Count; i++)
        {
            if (i == 0 || (Uzenetek[i].Idopont - Uzenetek[i - 1].Idopont).TotalSeconds >= 10)
            {
                szurtUzenetek.Add(Uzenetek[i]);
            }
        }

        foreach (var uzenet in szurtUzenetek)
        {
            Console.WriteLine($"{uzenet.Idopont} | {uzenet.Szoveg} | {uzenet.KuldoIP} | {uzenet.FogadoIP}");
        }
    }

    static void Feladat2()
    {
        Console.WriteLine("\n2. Feladat: IP-címek listázása");
        foreach (var uzenet in Uzenetek)
        {
            Console.WriteLine($"Küldő IP: {uzenet.KuldoIP}, Fogadó IP: {uzenet.FogadoIP}");
        }
    }

    static void Feladat3()
    {
        Console.Write("\n3. Feladat: Adjon meg egy IP-címet: ");
        string ipcim = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(ipcim))
        {
            Console.WriteLine("Érvénytelen IP-cím!");
            return;
        }

        IPCimKereses(Uzenetek, ipcim);
    }

    static void IPCimKereses(List<Uzenet> uzenetek, string ipcim)
    {
        List<Uzenet> talalatok = uzenetek.FindAll(u => u.KuldoIP == ipcim || u.FogadoIP == ipcim);

        talalatok.Sort((a, b) => a.Idopont.CompareTo(b.Idopont));

        if (talalatok.Count == 0)
        {
            Console.WriteLine("Nincs találat az adott IP-címre.");
            return;
        }

        Console.WriteLine($"Kommunikációs adatok az IP-címhez: {ipcim}\n");
        foreach (var uzenet in talalatok)
        {
            Console.WriteLine($"[{uzenet.Idopont}] {uzenet.KuldoIP} -> {uzenet.FogadoIP}: {uzenet.Szoveg}");
        }
    }
}

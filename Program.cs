using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Diagnostics;

class Program
{
    public static Dictionary<string,(string,string)> dnslist = new Dictionary<string,(string,string)>{
        {"Google DNS",("8.8.8.8","8.8.4.4")},
        {"CloudFare DNS",("1.1.1.1","1.0.0.1")},
        {"Open DNS",("208.67.222.222","208.67.220.220")}, 
        {"Open DNS 2",("208.67.222.222","208.67.220.220")}, 
        {"Quad9 DNS (Security)",("9.9.9.9","149.112.112.112")},
        {"Quad9 DNS (NoSecurity)",("9.9.9.10","149.112.112.110")},
        {"AdGuard DNS",("94.140.14.14","94.140.15.15")},
        {"CleanBrowsing",("185.228.168.9","185.228.169.9")},
        {"Alternate DNS",("76.76.19.19","76.223.122.150")},
        {"Control D",("76.76.2.0","76.76.10.0")},
        {"Norton DNS",("192.153.192.1","192.153.194.1")},
        {"Norton ConnectSafe",("199.85.126.10","199.85.127.10")},
        {"Ultra DNS",("204.69.234.1","204.74.101.1")},
        {"VeriSing Public DNS",("64.6.65.6","64.6.64.6")},
        {"Dyn",("216.146.36.36","216.146.35.35")},
        {"Safe DNS",("195.46.39.40","195.46.39.39")},
        {"Next DNS",("45.90.30.230","45.90.28.230")},
        {"Comodo",("156.154.70.22","156.154.71.22")},
        {"Neustar 2",("156.154.70.5","156.154.71.5")},
        {"Neustar 1",("156.154.70.1","156.154.71.1")},
        {"Qwest",("205.171.3.65","205.171.2.65")},
        {"Hurricane Electric",("","74.82.42.42")},
        {"Comodo Secure",("8.26.56.56","8.20.247.20")},
        {"Level 3 - A",("209.244.0.3","209.244.0.4")},
        {"Level 3 - B",("4.2.2.1","4.2.2.2")},
        {"Level 3 - C",("4.2.2.3","4.2.2.4")},
        {"Level 3 - D",("4.2.2.6","4.2.2.5")},
        {"Freenom World",("80.80.81.81","80.80.80.80")},
        {"Orange DNS",("195.92.195.94","195.92.195.95")},
        {"FDN",("80.67.169.12","80.67.169.40")},
        {"Zen Internet",("212.23.3.1","212.23.8.1")},
        {"SprintLink",("204.97.212.10","199.2.252.10")},
        {"Yandex",("77.88.8.8","77.88.8.1")},
    };

    public static readonly string pingDomain = "www.facebook.com"; //change this if you want
    public static void Main()
    {
        Console.Clear();
        Console.WriteLine("Starting optimizer...");
        CurrentDNS();
        Optimize();
    }

    public static void Optimize(){
        var ping = new Ping();
        var pingReply = ping.Send(pingDomain);
        var bestDNS = "Google DNS";
        long currentPing;
        long bestPing = pingReply.RoundtripTime;
        foreach (var dnsType in dnslist.Keys){
            SetDNS("Ethernet", dnslist[dnsType].Item1,dnslist[dnsType].Item2);
            for (int i=0; i < 4; i++){
                pingReply = ping.Send(pingDomain);
                currentPing = pingReply.RoundtripTime;
                if (bestPing > currentPing){
                    bestPing = currentPing;
                    bestDNS = dnsType;
                }
            }
            Debug.Info($"Status: {pingReply.Status}    | Ping: {pingReply.RoundtripTime}ms    | DNS: {dnsType}");
        }

        Debug.Info($"Best ping: {bestPing}    | DNS: {bestDNS}");
        SetDNS("Ethernet", dnslist[bestDNS].Item1,dnslist[bestDNS].Item2);
        CurrentDNS();
    }
    
    public static void CurrentDNS(){
        foreach (var dns in GetCurrentDNS()){
            Console.WriteLine($"Actual DNS: {dns}");
        }
    }

    public static void SetDNS(string interfaceName, string primaryDns, string secondaryDns)
    {
        try
        {
            var setPrimaryDnsCmd = $@"netsh interface ip set dns name=""{interfaceName}"" static {primaryDns}";
            var addSecondaryDnsCmd = $@"netsh interface ip add dns name=""{interfaceName}"" {secondaryDns} index=2";

            var processInfo = new ProcessStartInfo("cmd.exe", $"/c {setPrimaryDnsCmd} && {addSecondaryDnsCmd}")
            {
                Verb = "runas",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            using (var process = Process.Start(processInfo))
            {
                var output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    //Console.WriteLine("DNS Changed Successfully!");
                }
                else
                {
                    Console.WriteLine($"[{process.ExitCode}] Failed to change DNS. {output}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error changing DNS: {ex.Message}");
        }
    }
    public static IEnumerable<string> GetCurrentDNS()
    {
        List<string> dnslist = new List<string>();

        foreach(var ni in NetworkInterface.GetAllNetworkInterfaces()){
            var dnsaddresses = ni.GetIPProperties().DnsAddresses;

            foreach(var dns in dnsaddresses){
                if(dns.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork){
                    dnslist.Add(dns.ToString());
                }
            }
        }
        return dnslist;
    }
}
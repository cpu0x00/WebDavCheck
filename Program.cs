// an extendation to https://github.com/G0ldenGunSec/GetWebDAVStatus/ that allows the use of subnets 


using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using NetTools;
using System.Threading.Tasks;

namespace WebDavCheck
{
    public class Program
    {

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WaitNamedPipeA(string lpNamedPipeName, uint nTimeOut);





        public static void DisplayHlp() {

            Console.WriteLine("\n-subnet                    subnet to scan for webdav [EX: 10.10.10.0/24]");
            Console.WriteLine("-ip                          a single ip to scan with webdav");
            Console.WriteLine("-threads                     a number of threads to use with subnet scanning, default: 20");
            Console.WriteLine("-verbose                     diplays failed attempts in subnet mode");

            Console.WriteLine("\nusage: .\\WebDavCheck.exe -subnet 10.10.10.0/24 [-threads 50] [-verbose] ");
            Console.WriteLine("usage: .\\WebDavCheck.exe -ip 10.10.10.5");

        }


        public static List<string> SubnetParser(string subnet) {
            
            List<string> ipAddresses = new List<string>();

            IPAddressRange range = IPAddressRange.Parse(subnet);

            foreach (IPAddress ip in range)
            {
                ipAddresses.Add(ip.ToString());
            }

            return ipAddresses;

        }


        public static void SubnetChecker(string subnet, int threads, bool verbose=false) { 
            List<string> targets = SubnetParser(subnet);


            Parallel.ForEach(targets.ToArray(), new ParallelOptions { MaxDegreeOfParallelism = threads }, singleTarget =>
            {
                string pipename = @"\\" + singleTarget + @"\pipe\DAV RPC SERVICE";
                bool davActive = WaitNamedPipeA(pipename, 3000);

                if (davActive)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[+] WebClient service is active on " + singleTarget);
                    Console.ResetColor();
                }
                else
                {
                    if (verbose)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[x] Unable to reach DAV pipe on {0}, system is either unreachable or does not have WebClient service running", singleTarget);
                        Console.ResetColor();
                    }
                }
            });


        }

        public static void SingleHostCheck(string HostIp) {

            string pipename = @"\\" + HostIp + @"\pipe\DAV RPC SERVICE";
            bool davActive = WaitNamedPipeA(pipename, 3000);

            if (davActive)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[+] WebClient service is active on " + HostIp);
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[x] Unable to reach DAV pipe on {0}, system is either unreachable or does not have WebClient service running", HostIp);
                Console.ResetColor();
            }
            

        }

        public static void Main(string[] args)
        {
            string subnet = null;
            string ip = null;
            int threads = 20;
            bool verbose = false;

            if (args.Length == 0) { DisplayHlp(); return; }

            for (int arg = 0; arg < args.Length; arg++)
            {
                if (args[arg] == "-subnet") { subnet = args[arg + 1]; }
                if (args[arg] == "-ip") { ip = args[arg + 1]; }
                if (args[arg] == "-threads") { threads = int.Parse(args[arg + 1]); }
                if (args[arg] == "-verbose") { verbose = true; }
            }

            if (subnet == null && ip == null) { DisplayHlp(); return; }

            if (subnet != null && ip != null) { Console.WriteLine("[-] choose between subnet or ip modes, NOT BOTH ITS POINTLESS"); return; }

            if (subnet != null) {
                Console.WriteLine("[+] Starting in subnet mode");
                SubnetChecker(subnet, threads, verbose);
                return;
            }

            if (ip != null) {
                Console.WriteLine("[+] Starting in single-target mode");
                SingleHostCheck(ip);
                return;
            }
        }
    }
}

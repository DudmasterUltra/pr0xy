using ex0dusCLI;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace pr0xy {

    public class Runner {

        public static async Task<bool> Test(string address) {

            Console.Write("Testing " + address + "...");

            try {

                WebProxy proxy = new WebProxy(address) { UseDefaultCredentials = false };
                HttpClientHandler handler = new HttpClientHandler() { Proxy = proxy };
                using (HttpClient client = new HttpClient(handler)) {

                    String publicIP = client.GetAsync("https://api.ipify.org").Result.Content.ReadAsStringAsync().Result;
                    if (publicIP.Equals(address)) {

                        return true;

                    }

                }

            } catch (Exception e) {}

            return false;

        }

        public static void Main(string[] args) {

            CLI cli = new CLI(new CLI.Switch[] {

                new CLI.Switch("in", "The input file containing the proxy list")
                    { IsRequired = true, HasValue = true },
                new CLI.Switch("out", "The output file to return the working proxies")
                    { IsRequired = true, HasValue = true }

            });
            CLIResult parse = cli.Parse(args);

            if (parse.IsEmpty) {

                cli.Splash("pr0xy", "ex0dus");
                return;

            }

            Console.WriteLine(Properties.Resources.launch);

            string input;
            if (parse.ParsedContains("in")) {

                input = parse.GetParsed("in").Value;

            } else {

                Console.WriteLine("FAILURE = Missing -in file");
                return;

            }

            string output;
            if (parse.ParsedContains("out")) {

                output = parse.GetParsed("out").Value;

            } else {

                Console.WriteLine("FAILURE = Missing -out file");
                return;

            }

            Console.WriteLine("INPUT = " + input);
            Console.WriteLine("OUTPUT = " + output);
            if (File.Exists(input)) {

                using (StreamWriter writer = new StreamWriter(File.OpenWrite(output))) {

                    using (StreamReader reader = File.OpenText(input)) {

                        string line;
                        while ((line = reader.ReadLine()) != null) {

                            Task.Run(() => Test(line));

                        }

                    }

                }

            } else {
                
                Console.WriteLine("-in file does not exist!");

            }

        }

    }

}

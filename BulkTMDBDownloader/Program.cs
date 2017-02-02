using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BulkTMDBDownloader
{
    class Program
    {
        private const string Outputfolder = "output";
        public static int SearchCount = 0;
        public static int GetCount = 0;
        public static int WriteCount = 0;
        public static int LineCount = 0;

        private static Task _consoleUpdateTask;

        static void Main(string[] args)
        {
            Directory.CreateDirectory(Outputfolder);
            Console.WriteLine("Enter the path to the input file...");
            string filename = Console.ReadLine();
            Console.WriteLine("Enter delimiter...");
            ProcessInput(new RequestManager(), filename, ReadDelimiter());
            _consoleUpdateTask = new Task(UpdateProgress);
            _consoleUpdateTask.Start();
            Console.Read();
        }

        /// <summary>
        /// Updates the console line that indicates the progress.
        /// </summary>
        private static void UpdateProgress()
        {
            while (true)
            {
                Console.Write(
                    $"\rProgress: {SearchCount} Search requests handled | {GetCount} Get requests handled | {WriteCount} Files written | {LineCount} Items total");
                _consoleUpdateTask.Wait(250);
            }
        }

        /// <summary>
        /// Reads the file delimiter from the console. Retries until success.
        /// </summary>
        /// <returns>Delimiter</returns>
        private static char ReadDelimiter()
        {
            try
            {
                return Console.ReadLine().First();
            }
            catch (Exception e)
            {
                Console.WriteLine("You did not enter a delimiter, please try again");
                return ReadDelimiter();
            }
        }

        /// <summary>
        /// Processes the input file and adds requests for each line.
        /// </summary>
        /// <param name="rm">Request Manager</param>
        /// <param name="filename">Filename of the input file</param>
        /// <param name="delimiter">Delimiter</param>
        private static void ProcessInput(RequestManager rm, string filename, char delimiter)
        {
            using (StreamReader sr = File.OpenText(filename))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    AddSearchRequest(rm, line, delimiter);
                    LineCount++;
                }
            }
        }

        /// <summary>
        /// Adds a search request for one line
        /// </summary>
        /// <param name="rm">Request Manager</param>
        /// <param name="line">Current line of the input file</param>
        /// <param name="delimiter">Delimiter</param>
        private static void AddSearchRequest(RequestManager rm, string line, char delimiter)
        {
            // keywords ; year (can be extended for own use)
            string[] arguments = line.Split(delimiter);
            Query query = new Query(Query.RequestTypes.Search).Keywords(arguments[0]).Year(arguments[1]).Callback(result => AddGetRequest(rm, result));
            rm.AddRequest(query);
        }

        /// <summary>
        /// Takes the first result from the search request and adds a request to get the full information for that result.
        /// </summary>
        /// <param name="rm">Request Manager</param>
        /// <param name="result">Result from search request</param>
        private static void AddGetRequest(RequestManager rm, string result)
        {
            JObject jResult = (JObject) JsonConvert.DeserializeObject(result);
            if(jResult["total_results"].ToString() == "0")
                return;

            string id = jResult["results"].First()["id"].ToString();
            string type = jResult["results"].First()["media_type"].ToString();
            Query query = new Query(Query.RequestTypes.Get).Id(id).Type(type).Callback(content => WriteToFile(id + ".json", content));
            rm.AddRequest(query);
        }

        /// <summary>
        /// Writes the resulting information to a json file
        /// </summary>
        /// <param name="filename">File name to write to</param>
        /// <param name="content">Data to be written</param>
        private static void WriteToFile(string filename, string content)
        {
            StreamWriter file = new StreamWriter(Outputfolder + "\\" + filename);
            file.WriteLine(content);
            file.Close();
            WriteCount++;
        }        
    }
}

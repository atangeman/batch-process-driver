namespace ProcessExample
{
    using BatchProcessLibrary.Events;
    using BatchProcessLibrary.Processes;
    using BatchProcessLibrary.Utilities;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Example process to demonstrate use of class library
    /// </summary>
    public class ProcessExampleMain : ProcessBase, IProcess
    {
        private string _filePath;
        private string _outputPath;
        private DocumentStatistics docStats;

        /// <summary>
        /// Gets a value indicating whether boolean set to true if process is running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Gets name for process when reporting to message and debug delegates.
        /// </summary>
        public string ProcessName { get; private set; }

        /// <summary>
        /// Configuration for current process
        /// </summary>
        public IDictionary<string, object> Config { get; private set; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets or sets Document statistics object model
        /// </summary>
        public DocumentStatistics DocStats
        {
            get
            {
                return docStats;
            }

            set
            {
                docStats = value;
            }
        }

        /// <summary>
        /// Starts process
        /// </summary>
        public ProcessExampleMain(IDictionary<string, object> config)
        {
            Config = config;
            docStats = new DocumentStatistics();
        }

        /// <summary>
        /// Starts the process or sub routine
        /// </summary>
        public void StartProcess()
        {
            try
            {
                _filePath = Config["FilePath"].ToString();
                _outputPath = Config["OutputPath"].ToString();

                RaiseLogEvent($"Initializing project using {_filePath} as path");
                RaiseLogEvent("");

                RaiseLogEvent($"processing files..");
                RaiseLogEvent("");
                ProcessFiles(_filePath, ref docStats);
                if (docStats.WordCounts.Count() > 0)
                {
                    RaiseLogEvent("");
                    RaiseLogEvent("Word counts: ");
                    RaiseLogEvent("");
                    foreach (var item in docStats.WordCounts.OrderByDescending(key => key.Value))
                    {
                        Console.WriteLine($"{item.Key,-15} {item.Value,-5}");
                    }
                    DataSerializationTools.SerializeJSON<DocumentStatistics>($"{_outputPath}", ref docStats);
                }
                StopProcess();
            }
            catch (Exception ex)
            {
                RaiseProcessException(ex);
            }

        }

        /// <summary>
        /// Processes files contained in Resources directory
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="stats"></param>
        private void ProcessFiles(string filepath, ref DocumentStatistics stats)
        {
            try
            {
                string[] files = Directory.GetFiles(filepath);
                RaiseLogEvent($"{files.Count()} Files found ");
                if (files.Count() > 0)
                {
                    RaiseLogEvent("");
                    foreach (string file in files)
                    {
                        string fname = Path.GetFileName(file);
                        RaiseLogEvent(fname);
                        stats.Documents.Add(fname);
                        string[] result = ParseText(file);
                        stats.CountWords(result);
                    }
                }
                else
                {
                    Console.WriteLine($"No files found :/ ");
                }
            }
            catch (UnauthorizedAccessException)
            {
                RaiseDebugEvent("ACCESS DENIED");
            }
            catch (DirectoryNotFoundException)
            {
                RaiseDebugEvent("No Directory found");
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// Parses text from documents into compatible format
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string[] ParseText(string fileName)
        {
            string[] values = { " " };
            string SpecialChar = @"./?><,:';+=!@$%^&*()~`*{}_[]";
            char[] specialCharacters = SpecialChar.ToCharArray();
            StringBuilder sb = new StringBuilder();
            char ch;
            using (StreamReader sr = new StreamReader(fileName))
            {
                while (sr.Peek() >= 0)
                {
                    ch = (char)sr.Read();
                    if (!specialCharacters.Contains(ch))
                    {
                        if (char.IsWhiteSpace(ch)) sb.Append('|');
                        else if (ch != '\0') sb.Append(char.ToLower(ch));
                    }
                }
            }
            values = sb.ToString().Split('|');
            return values;
        }

        /// <summary>
        /// Performs logic to terminate process cleanly. 
        /// </summary>
        public void StopProcess()
        {
            IsRunning = false;
            RaiseProcessComplete(FireProcessReturnCodes.SUCCESS, "Process finished");
        }
    }
}

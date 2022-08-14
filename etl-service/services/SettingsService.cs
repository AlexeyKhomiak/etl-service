using etl_service.models;
using System.Text.Json;

namespace etl_service
{
    internal class SettingsService
    {
        string _settingsFilePath { get; set; }
        string _settingsFileName { get; set; }
        public Settings Settings { get; set; }

        public SettingsService(string settingsFileName)
        {
            _settingsFileName = settingsFileName;
            _settingsFilePath = Path.Combine(Directory.GetCurrentDirectory(), _settingsFileName);

            if (!File.Exists(_settingsFilePath))
            {
                SetSourceFolder();
            }
            else
            {
                Settings = GetConfiguration();
            }
        }

        void SetSourceFolder()
        {
            string sourceFolder = GetSourceFolder();
            string destinationFolder = Path.Combine(sourceFolder, "processed_files");
            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
                Console.WriteLine("Destination folder created.");
            }
            CreateConfigFile(new Settings() { SourcePath = sourceFolder, DestinationPath = destinationFolder });
        }

        string GetSourceFolder()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("To get started, you must enter the source folder with files to process. Example: c:\\folder");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"You can also set up a configuration file in the program folder: {_settingsFilePath}");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Please enter the path: ");
            Console.ForegroundColor = ConsoleColor.White;
            string answer = Console.ReadLine();

            while (answer != "exit" && !ValidUri(answer))
            {
                if (!ValidUri(answer))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid source folder");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Please enter the path or enter 'exit':");
                }

                answer = Console.ReadLine();
            }
            if (answer == "exit")
            {
                TerminateApp();
            }
            if (ValidUri(answer) && !Directory.Exists(answer))
            {
                Directory.CreateDirectory(answer);
                Console.WriteLine("Source folder created.");

            }
            return answer;
        }

        public Settings GetConfiguration()
        {
            using FileStream openStream = File.OpenRead(_settingsFileName);
            Settings? settings = JsonSerializer.Deserialize<Settings>(openStream);

            if (settings.SourcePath == null || settings.SourcePath == "")
            {
                TerminateApp();
            }

            return settings;
        }

        bool ValidUri(string path)
        {
            Uri res;
            return Uri.TryCreate(path, UriKind.Absolute, out res);
        }

        public void CreateConfigFile(Settings _data)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(_data, options);
            File.WriteAllText(_settingsFilePath, json);
            Console.WriteLine("Settings have been saved.\n");
            Console.WriteLine("Settings path: " + _settingsFilePath + "\n");
            Console.WriteLine("Settings data:");
            Console.WriteLine(json + "\n\n");
        }

        public void TerminateApp()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Set up a configuration file in the directory with the program.");
            Console.Write($"Configuration file: ");
            Console.WriteLine(_settingsFilePath);
            Console.ForegroundColor = ConsoleColor.White;
            Environment.Exit(0);
        }
    }
}

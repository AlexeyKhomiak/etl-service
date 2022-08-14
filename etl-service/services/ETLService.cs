using etl_service.models;

namespace etl_service.services
{
    class ETLService : IDisposable
    {
        SettingsService settingsService { get; set; }
        public Settings settings { get; set; }
        public FileService fileService { get; set; }
        FileSystemWatcher watcher { get; set; }

        public ETLService(string settingsFileName)
        {
            settingsService = new SettingsService(settingsFileName);
            settings = settingsService.GetConfiguration();
            fileService = new FileService();

            watcher = new FileSystemWatcher
            {
                Path = settings.SourcePath,
                NotifyFilter = NotifyFilters.FileName,
                Filters = { "*.txt", "*.csv" },
                EnableRaisingEvents = true
            };

        }
        public void Start()
        {
            watcher.Created += OnCreated;
            List<string> filesPaths = fileService.GetFiles(settings.SourcePath);
            foreach (string filePathItem in filesPaths)
            {
                ProcessFiles(filePathItem);
            }

            Console.WriteLine("Service running...\n");
        }

        public async Task ProcessFiles(string filePath)
        {
            List<Line> fileData = new List<Line>();
            List<CityData> cityData = new List<CityData>();

            fileData = fileService.ReadFile(filePath);

            cityData = fileService.TransformData(fileData);

            await fileService.WriteJson(settings.DestinationPath, cityData);
            

            string doneFolder = Path.Combine(settings.SourcePath, "Done");
            if (!Directory.Exists(doneFolder))
            {
                Directory.CreateDirectory(doneFolder);
            }
            fileService.FileMove(filePath, Path.Combine(doneFolder, Path.GetFileName(filePath)));
            
        }

        async void OnCreated(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"Get new file: {e.FullPath}");
            await ProcessFiles(e.FullPath);
        }

        public void Dispose()
        {
            watcher.Dispose();
        }
    }



}

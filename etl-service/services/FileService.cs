using CsvHelper;
using etl_service.models;
using System.Globalization;
using System.Text.Json;

namespace etl_service.services
{
    internal class FileService
    {
        public int parsed_files { get; set; } = 0;
        public int parsed_lines { get; set; } = 0;
        public int found_errors { get; set; } = 0;
        public List<string> invalid_files { get; set; } = new List<string>();
        public DateTime sysDate { get; set; }

        public FileService()
        {
            sysDate = DateTime.Now;
        }

        public List<string> GetFiles(string sourceFolderPath)
        {
            return Directory.EnumerateFiles(sourceFolderPath, "*.*")
                .Where(s => s.EndsWith(".txt") || s.EndsWith(".csv"))
                .ToList();
        }

        public List<Line> ReadFile(string path)
        {
            if (sysDate.Date != DateTime.Now.Date)
            {
                parsed_files = 0;
                parsed_lines = 0;
                found_errors = 0;
                invalid_files = new List<string>();
            }

            var records = new List<Line>();
            try
            {
                Console.WriteLine("Read file: " + path);

                using (var reader = new StreamReader(path))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    if (Path.GetExtension(path) == ".csv")
                    {
                        csv.Read();
                        csv.ReadHeader();
                    }
                    while (csv.Read())
                    {
                        try
                        {
                            Line record = new Line
                            {
                                Name = CheckEmpty(csv.GetField(0)),
                                LastName = CheckEmpty(csv.GetField(1).Replace(" ", "")),
                                Address = CheckEmpty(csv.GetField(2).Replace("“", "").Replace(" ", "")),
                                Payment = csv.GetField<decimal>(5),
                                Date = DateOnly.ParseExact(csv.GetField(6).Replace(" ", ""), "yyyy-dd-MM", CultureInfo.InvariantCulture),
                                Acount_Number = long.Parse(csv.GetField(7).Replace(" ", ""), CultureInfo.InvariantCulture),
                                Service = CheckEmpty(csv.GetField(8).Substring(1))
                            };

                            records.Add(record);
                            parsed_lines++;
                        }
                        catch (Exception e)
                        {
                            found_errors++;
                            continue;
                        }
                    }
                }
                parsed_files++;
            }
            catch (Exception)
            {
                invalid_files.Add(path);
            }

            return records;
        }

        public string CheckEmpty(string val)
        {
            if (String.IsNullOrEmpty(val))
            {
                throw new ArgumentNullException();
            }
            return val;
        }

        public List<CityData> TransformData(List<Line> fileData)
        {
            List<CityData> transData = fileData
                .GroupBy(g => g.Address)
                .Select(g => new CityData
                {
                    City = g.Key,
                    Total = g.Sum(s => s.Payment),
                    services = g.GroupBy(gb => gb.Service)
                        .Select(s => new Service
                        {
                            Name = s.Key,
                            Total = s.Sum(s => s.Payment),
                            payers = s.Select(s => new Payer
                            {
                                Name = s.Name,
                                Payment = s.Payment,
                                Date = s.Date,
                                Account_Number = s.Acount_Number

                            }).ToList()
                        }).ToList()
                }).ToList();

            return transData;
        }

        public async Task WriteJson(string destinationPath, List<CityData> report)
        {
            DateTime date = DateTime.Now;
            string folderPath = Path.Combine(destinationPath, date.ToString("MM-dd-yyyy"));

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fileDestPath = Path.Combine(folderPath, $"output{parsed_files}.json");

            await using FileStream createStream = File.Create(fileDestPath);
            await JsonSerializer.SerializeAsync(createStream, report, new JsonSerializerOptions { WriteIndented = true });

            Console.WriteLine($"File proccesed: {fileDestPath}\n");

            WriteMetaLog(destinationPath);
        }

        public void WriteMetaLog(string destinationPath)
        {
            string[] lines =
            {
                "parsed_files: " + parsed_files,
                "parsed_lines: " + parsed_lines,
                "found_errors: " + found_errors,
                "invalid_files: " + (invalid_files.Count == 0 ? "0" : String.Join(", ", invalid_files))
            };

            File.WriteAllLines(Path.Combine(destinationPath, DateTime.Now.ToString("MM-dd-yyyy"), "meta.log"), lines);
        }

        public void FileMove(string from, string to)
        {
           File.Move(from, to);
        }

    }




}

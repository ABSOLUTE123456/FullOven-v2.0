using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using polnuyaPetch.Models;

namespace polnuyaPetch.Services
{
    public class CsvMenuStorage
    {
        private readonly string _filePath;

        public CsvMenuStorage(string filePath)
        {
            _filePath = filePath;
        }

        public void Save(List<MenuItem> items)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
            var lines = new List<string> { "Id,Name,Category,Price,Description" };

            foreach (var item in items)
            {
                var id = item.Id.ToString();
                var name = EscapeCsv(item.Name ?? "");
                var cat = EscapeCsv(item.Category ?? "");
                var price = item.Price.ToString();
                var desc = EscapeCsv(item.Description ?? "");

                lines.Add($"{id},{name},{cat},{price},{desc}");
            }
            File.WriteAllLines(_filePath, lines);
        }

        public (List<MenuItem> items, int errors) Load()
        {
            if (!File.Exists(_filePath)) return (new List<MenuItem>(), 1);

            var lines = File.ReadAllLines(_filePath).ToList();
            var result = new List<MenuItem>();
            int errors = 0;

            if (lines.Count == 0) return (result, 0);

            int start = lines[0].StartsWith("Id,", StringComparison.OrdinalIgnoreCase) ? 1 : 0;

            for (int i = start; i < lines.Count; i++)
            {
                var line = lines[i].Trim();
                if (line.Length == 0) continue;

                try
                {
                    var parts = ParseCsvLine(line);
                    if (parts.Count < 5 || !int.TryParse(parts[0], out var id) || !decimal.TryParse(parts[3], out var price))
                    {
                        errors++;
                        continue;
                    }

                    result.Add(new MenuItem
                    {
                        Id = id,
                        Name = parts[1],
                        Category = parts[2],
                        Price = price,
                        ImagePath = "image 2.png",
                        Description = parts[4]
                    });
                }
                catch
                {
                    errors++;
                }
            }
            return (result, errors);
        }

        private static string EscapeCsv(string text)
        {
            bool mustQuote = text.Contains(',') || text.Contains('"') || text.Contains('\n') || text.Contains('\r');
            if (text.Contains('"')) text = text.Replace("\"", "\"\"");
            return mustQuote ? $"\"{text}\"" : text;
        }

        private static List<string> ParseCsvLine(string line)
        {
            var result = new List<string>();
            var current = "";
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (inQuotes)
                {
                    if (c == '"')
                    {
                        if (i + 1 < line.Length && line[i + 1] == '"')
                        {
                            current += '"';
                            i++;
                        }
                        else inQuotes = false;
                    }
                    else current += c;
                }
                else
                {
                    if (c == '"') inQuotes = true;
                    else if (c == ',')
                    {
                        result.Add(current);
                        current = "";
                    }
                    else current += c;
                }
            }
            result.Add(current);
            return result;
        }
    }
}

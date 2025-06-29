using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VismaTask.Models;

namespace VismaTask.Repositories;

public class ShortageRepository : IShortageRepository
{
    private readonly string _fileName;

    public ShortageRepository(string fileName)
    {
        _fileName = fileName;
    }

    public List<Shortage> ReadShortages()
    {
        try
        {
            if (!File.Exists(_fileName))
            {
                return new List<Shortage>();
            }

            var content = File.ReadAllText(_fileName);

            if (string.IsNullOrWhiteSpace(content))
            {
                return new List<Shortage>();
            }
            return JsonSerializer.Deserialize<List<Shortage>>(content);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading shortages: {ex.Message}");
            return new List<Shortage>();
        }
    }

    public void WriteShortages(List<Shortage> shortages)
    {
        var json = JsonSerializer.Serialize(shortages);
        File.WriteAllText(_fileName, json);
    }
}

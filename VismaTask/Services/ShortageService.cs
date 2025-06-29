using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VismaTask.Models;
using VismaTask.Models.Utility;
using VismaTask.Repositories;
using static VismaTask.Models.SD.SD;

namespace VismaTask.Services;

public class ShortageService : IShortageService
{
    private List<Shortage> _shortages;

    private readonly IShortageRepository _shortageRepository;

    public ShortageService(IShortageRepository shortageRepository)
    {
        _shortageRepository = shortageRepository;
        _shortages = _shortageRepository.ReadShortages();
    }


    public List<Shortage> GetShortages()
    {
        return _shortages;
    }

    public List<Shortage> GetShortages(FilterOptions filterOptions)
    {
        var query = _shortages.AsQueryable();

        if (!filterOptions.Name.ToLower().Equals("admin"))
        {
            query = query.Where(s => s.Name.Equals(filterOptions.Name));
        }

        if (!string.IsNullOrWhiteSpace(filterOptions.Title))
        {
            query = query.Where(s => s.Title.Contains(filterOptions.Title, StringComparison.OrdinalIgnoreCase));
        }

        if (filterOptions.Room.HasValue)
        {
            query = query.Where(s => s.Room.Equals(filterOptions.Room));
        }

        if (filterOptions.Category.HasValue)
        {
            query = query.Where(s => s.Category.Equals(filterOptions.Category));
        }

        if (filterOptions.CreatedStartDate.HasValue)
        {
            query = query.Where(s => s.CreatedOn.Date >= filterOptions.CreatedStartDate.Value.Date);
        }

        if (filterOptions.CreatedEndDate.HasValue)
        {
            query = query.Where(s => s.CreatedOn.Date <= filterOptions.CreatedEndDate.Value.Date);
        }

        return query.OrderByDescending(s => s.Priority).ToList();
    }

    public Result AddShortage(Shortage shortage)
    {
        var existing = _shortages.FirstOrDefault(s =>
                s.Title.Equals(shortage.Title)
                && s.Room == shortage.Room);

        if (existing != null)
        {
            if (shortage.Priority > existing.Priority)
            {
                _shortages.Remove(existing);
                _shortages.Add(shortage);
                _shortageRepository.WriteShortages(_shortages);
                return Result.Ok("Shortage with same title and room has been overriden.");
            }

            return Result.Fail($"Error: shortage with title: {shortage.Title} and room: {shortage.Room} already exists.");
        }
        else
        {
            _shortages.Add(shortage);
            _shortageRepository.WriteShortages(_shortages);
            return Result.Ok("Shortage registered successfully!");
        }
    }

    public Result DeleteShortage(Shortage shortage)
    {
        var existing = _shortages.FirstOrDefault(s =>
                s.Title.Equals(shortage.Title)
                && s.Room == shortage.Room);

        if (existing != null)
        {
            _shortages.Remove(existing);
            _shortageRepository.WriteShortages(_shortages);
            return Result.Ok("Shortage deleted successfully!");
        }
        
        return Result.Fail($"Shortage with title: {shortage.Title} and room: {shortage.Room} not found.");
    }
}

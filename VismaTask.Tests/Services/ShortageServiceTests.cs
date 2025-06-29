using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VismaTask.Models;
using VismaTask.Models.Utility;
using VismaTask.Repositories;
using VismaTask.Services;
using static VismaTask.Models.SD.SD;

namespace VismaTask.Tests.Services;

public class ShortageServiceTests
{
    private readonly IShortageRepository _repo;

    public ShortageServiceTests()
    {
        _repo = Substitute.For<IShortageRepository>();
    }

    [Fact]
    public void GetsAllShortages()
    {
        // Arrange
        var shortages = new List<Shortage>
        {
            new Shortage
            {
                Title = "Coffee Machine Broken",
                Name = "Alice",
                Room = Room.Kitchen,
                Category = Category.Electronics,
                Priority = 8,
                CreatedOn = DateTime.Today
            },
            new Shortage
            {
                Title = "Hand Dryer Not Working",
                Name = "Ian",
                Room = Room.Bathroom,
                Category = Category.Electronics,
                Priority = 6,
                CreatedOn = new DateTime(2025, 6, 13)
            },
            new Shortage
            {
                Title = "Whiteboard Markers Gone",
                Name = "Jill",
                Room = Room.MeetingRoom,
                Category = Category.Other,
                Priority = 5,
                CreatedOn = new DateTime(2025, 6, 14)
            },
            new Shortage
            {
                Title = "Microwave Dirty",
                Name = "Kevin",
                Room = Room.Kitchen,
                Category = Category.Other,
                Priority = 2,
                CreatedOn = new DateTime(2025, 6, 15)
            }
        };

        _repo.ReadShortages().Returns(shortages);

        var service = new ShortageService(_repo);

        // Act
        var resultShortages = service.GetShortages();

        // Assert
        Assert.Equal("Coffee Machine Broken", resultShortages[0].Title);
        Assert.Equal("Hand Dryer Not Working", resultShortages[1].Title);
        Assert.Equal("Whiteboard Markers Gone", resultShortages[2].Title);
        Assert.Equal("Microwave Dirty", resultShortages[3].Title);

    }

    [Fact]
    public void GetsAllShortagesByFilters()
    {
        // Arrange
        var shortages = new List<Shortage>
        {
            new Shortage
            {
                Title = "Coffee Machine Broken",
                Name = "Alice",
                Room = Room.Kitchen,
                Category = Category.Electronics,
                Priority = 8,
                CreatedOn = new DateTime(2025, 6, 05)
            },
            new Shortage
            {
                Title = "Hand Dryer Not Working",
                Name = "Ian",
                Room = Room.Bathroom,
                Category = Category.Electronics,
                Priority = 6,
                CreatedOn = new DateTime(2025, 6, 13)
            },
            new Shortage
            {
                Title = "Whiteboard Markers Gone",
                Name = "Jill",
                Room = Room.MeetingRoom,
                Category = Category.Other,
                Priority = 5,
                CreatedOn = new DateTime(2025, 6, 14)
            },
            new Shortage
            {
                Title = "Microwave Dirty",
                Name = "Kevin",
                Room = Room.Kitchen,
                Category = Category.Other,
                Priority = 2,
                CreatedOn = new DateTime(2025, 6, 15)
            }
        };

        _repo.ReadShortages().Returns(shortages);

        var service = new ShortageService(_repo);

        // Act
        var resultShortages = service.GetShortages(new FilterOptions { Name = "admin", Category = Category.Electronics,
                CreatedStartDate = new DateTime(2025, 6, 10), CreatedEndDate = new DateTime(2025, 6, 20) });

        // Assert
        Assert.Single(resultShortages);
        Assert.Equal("Hand Dryer Not Working", resultShortages[0].Title);
        Assert.Equal(Category.Electronics, resultShortages[0].Category);
    }

    [Fact]
    public void DoesNotAddShortage_WhenLowerPriorityDuplicateExists()
    {
        // Arrange
        var shortages = new List<Shortage>
        {
            new Shortage
            {
                Title = "Coffee Machine Broken",
                Name = "Alice",
                Room = Room.Kitchen,
                Category = Category.Electronics,
                Priority = 8,
                CreatedOn = DateTime.Today
            }
        };

        var newShortage = new Shortage
        {
            Title = "Coffee Machine Broken",
            Name = "Alice",
            Room = Room.Kitchen,
            Category = Category.Electronics,
            Priority = 5, // Lower priority than existing
            CreatedOn = DateTime.Today
        };

        _repo.ReadShortages().Returns(shortages);

        var service = new ShortageService(_repo);

        // Act
        var addResult = service.AddShortage(newShortage);
        var resultShortages = service.GetShortages();

        // Assert
        Assert.False(addResult.Success);  // Adding lower priority duplicate should fail
        Assert.Contains("already exists", addResult.Message);

        Assert.Single(resultShortages);   // Only original shortage remains
        Assert.Equal(8, resultShortages[0].Priority); // Priority remains unchanged
    }

    [Fact]
    public void AddsShortage_WhenHigherPriorityDuplicateExists()
    {
        // Arrange
        var shortages = new List<Shortage>
        {
            new Shortage
            {
                Title = "Coffee Machine Broken",
                Name = "Alice",
                Room = Room.Kitchen,
                Category = Category.Electronics,
                Priority = 4,
                CreatedOn = DateTime.Today
            }
        };

        var newShortage = new Shortage
        {
            Title = "Coffee Machine Broken",
            Name = "Alice",
            Room = Room.Kitchen,
            Category = Category.Electronics,
            Priority = 7, // Higher priority than existing
            CreatedOn = DateTime.Today
        };

        _repo.ReadShortages().Returns(shortages);

        var service = new ShortageService(_repo);

        // Act
        var addResult = service.AddShortage(newShortage);
        var resultShortages = service.GetShortages();

        // Assert
        Assert.True(addResult.Success);  // Adding higher priority duplicate should override
        Assert.Contains("overriden", addResult.Message);

        Assert.Single(resultShortages);   // Only one shortage remains
        Assert.Equal(7, resultShortages[0].Priority); // Priority has been changed
    }
}

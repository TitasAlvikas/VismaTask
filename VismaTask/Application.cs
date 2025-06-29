using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using VismaTask.Models;
using VismaTask.Models.Utility;
using VismaTask.Services;
using static VismaTask.Models.SD.SD;

namespace VismaTask;

public class Application
{
    private readonly IShortageService _shortageService;
    private string user;

    public Application(IShortageService shortageService)
    {
        _shortageService = shortageService;
    }

    public void Run()
    {
        Console.WriteLine("Welcome to Shortage Managment Application!");
        Login();
        while (true)
        {
            ShowMenu();
        }
    }

    private void Login()
    {
        Console.Write("Enter username: ");
        user = Console.ReadLine();
    }

    private void ShowMenu()
    {
        Console.Clear();
        Console.WriteLine($"Welcome {user}!");

        Console.WriteLine("1. List Shortages");
        Console.WriteLine("2. Register New Shortage");
        Console.WriteLine("3. Delete Shortage");
        Console.WriteLine("4. Exit");
        Console.Write("Select an option: ");
        var input = Console.ReadLine();


        switch (input)
        {
            case "1":
                ShowShortages();
                break;
            case "2":
                RegisterShortage();
                break;
            case "3":
                DeleteShortage();
                break;
            case "4":
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Invalid option, please try again.");
                break;
        }
    }

    private void ShowShortages()
    {
        Console.Clear();

        var filterOptions = new FilterOptions { Name = user };
        
        filterOptions.Title = Prompt("Would you like to filter by shortage title?\nEnter title or press enter to skip:");
        
        string roomInput = Prompt("Would you like to filter by room?\nEnter room(Kitchen, MeetingRoom, Bathroom) or press enter to skip:");
        
        string categoryInput = Prompt("Would you like to filter by category?\nEnter category(Electronics, Food, Other) or press enter to skip:");

        Enum.TryParse(roomInput, true, out Room room);

        Enum.TryParse(categoryInput, true, out Category category);

        filterOptions.Room = room;

        filterOptions.Category = category;

        var startDateInput = Prompt("Would you like to filter by creation date? Enter start date (dd/MM/yyyy) or press enter to skip:");
        var endDateInput = Prompt("Enter end date (dd/MM/yyyy) or press enter to skip:");

        if (DateTime.TryParse(startDateInput, out var startDate))
        {
            filterOptions.CreatedStartDate = startDate;
        }
        if (DateTime.TryParse(endDateInput, out var endDate))
        {
            filterOptions.CreatedEndDate = endDate;
        }

        var shortages = _shortageService.GetShortages(filterOptions);


        PrintShortages(shortages);
        Console.ReadKey();
    }

    private void PrintShortages(List<Shortage> shortages)
    {
        if (shortages.Count == 0)
        {
            Console.WriteLine("No shortages found.");
            Console.ReadKey();
        }

        foreach (var shortage in shortages)
        {
            Console.WriteLine($"Title: {shortage.Title}, Priority: {shortage.Priority}, Room: {shortage.Room}, Category: {shortage.Category}, Created On: {shortage.CreatedOn.ToString("dd/MM/yyyy")}");
        }
    }

    private string Prompt(string message)
    {
        Console.WriteLine(message);
        return Console.ReadLine();
    }

    private void RegisterShortage()
    {
        Console.Clear();
        Console.WriteLine("Register New Shortage");

        string title = Prompt("Enter title: ");
        string roomInput = Prompt("Enter room (Kitchen, MeetingRoom, Bathroom): ");
        string categoryInput = Prompt("Enter category (Electronics, Food, Other): ");
        string priorityInput = Prompt("Enter priority (1(not important) - 10(very important)): ");

        if (!Enum.TryParse(roomInput, true, out Room room))
        {
            Console.WriteLine("Invalid room entered.");
            Console.ReadKey();
            return;
        }

        if (!Enum.TryParse(categoryInput, true, out Category category))
        {
            Console.WriteLine("Invalid category entered.");
            Console.ReadKey();
            return;
        }

        if (!int.TryParse(priorityInput, out int priority) || priority < 1 || priority > 10)
        {
            Console.WriteLine("Priority must be between 1 and 10.");
            Console.ReadKey();
            return;
        }

        var shortage = new Shortage
        {
            Title = title,
            Name = user,
            Room = room,
            Category = category,
            Priority = priority,
            CreatedOn = DateTime.Now
        };

        var result = _shortageService.AddShortage(shortage);

        Console.WriteLine(result.Message);
        Console.ReadKey();
    }

    private void DeleteShortage()
    {
        Console.Clear();
        Console.WriteLine("Delete Shortage");
        var title = Prompt("Enter title of the shortage to delete: ");
        var roomInput = Prompt("Enter room of the shortage to delete: ");
        
        if (!Enum.TryParse(roomInput, true, out Room room))
        {
            Console.WriteLine("Invalid room entered.");
            Console.ReadKey();
            return;

        }

        var result = _shortageService.DeleteShortage(new Shortage { Name = user, Title = title, Room = room});

        Console.WriteLine(result.Message);
        Console.ReadKey();
    }
}

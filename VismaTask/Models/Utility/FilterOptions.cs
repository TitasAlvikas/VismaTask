using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VismaTask.Models.SD.SD;

namespace VismaTask.Models.Utility;

public class FilterOptions
{
    public string? Title { get; set; }

    public required string Name { get; set; }

    public Room? Room { get; set; }

    public Category? Category { get; set; }

    public DateTime? CreatedStartDate { get; set; }

    public DateTime? CreatedEndDate { get; set; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VismaTask.Models.SD.SD;

namespace VismaTask.Models;

public class Shortage
{
    public string Title { get; set; }
    public string Name { get; set; }
    public Room Room { get; set; }
    public Category Category { get; set; }
    public int Priority { get; set; }
    public DateTime CreatedOn { get; set; }
}

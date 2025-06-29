using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VismaTask.Models;
using VismaTask.Models.Utility;
using VismaTask.Repositories;

namespace VismaTask.Services;

public interface IShortageService
{

    List<Shortage> GetShortages();

    List<Shortage> GetShortages(FilterOptions filterOptions);

    Result AddShortage(Shortage shortage);

    Result DeleteShortage(Shortage shortage);
}

using VismaTask.Models;

namespace VismaTask.Repositories;

public interface IShortageRepository
{
    List<Shortage> ReadShortages();

    void WriteShortages(List<Shortage> shortages);
}
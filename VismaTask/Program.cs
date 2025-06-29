using VismaTask;
using VismaTask.Repositories;
using VismaTask.Services;

var projectDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
var filePath = Path.Combine(projectDir, "shortages.json");
IShortageRepository repository = new ShortageRepository(filePath);
IShortageService service = new ShortageService(repository);
var app = new Application(service);

app.Run();



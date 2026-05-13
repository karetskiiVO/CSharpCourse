using System.Threading;
using System.Threading.Tasks;

using TaskHub.Utils;

namespace TaskHub;

class Program {
    private static CancellationTokenSource cts = new();

    static async Task Main(string[] args) {
        using var manager = new TaskManager<TaskItem>("tasks.yaml");

        manager.OnNotification += message => Logging.Info($"[Уведомление] {message}");
        manager.OnTaskAdded += view => Logging.Debug($"Событие: добавлена задача ID {view.id.id}");
        manager.OnTaskRemoved += view => Logging.Debug($"Событие: удалена задача ID {view.id.id}");

        await manager.LoadFromFileAsync();

        _ = Task.Run(() => BackgroundOverdueChecker(manager, cts.Token));

        bool exit = false;
        while (!exit) {
            Console.WriteLine("\n=== MЕНЮ TASKHUB ===");
            Console.WriteLine("1. Добавить задачу");
            Console.WriteLine("2. Показать все задачи");
            Console.WriteLine("3. Показать выполненные задачи");
            Console.WriteLine("4. Показать с высоким приоритетом");
            Console.WriteLine("5. Найти задачу по статусу");
            Console.WriteLine("6. Редактировать задачу");
            Console.WriteLine("7. Удалить задачу");
            Console.WriteLine("8. Статистика");
            Console.WriteLine("9. Сохранить");
            Console.WriteLine("0. Выход");
            Console.Write("Выберите действие: ");

            var choice = Console.ReadLine();

            try {
                switch (choice) {
                    case "1":
                        AddNewTask(manager);
                        break;
                    case "2":
                        PrintTasks(manager.GetAllTasks());
                        break;
                    case "3":
                        PrintTasks(manager.FindTasks(t => t.Status == TaskItem.TaskStatus.Done));
                        break;
                    case "4":
                        PrintTasks(manager.FindTasks(t => t.Priority == TaskItem.PriorityLevel.High));
                        break;
                    case "5":
                        var statusTasks = manager.FindTasks(t => t.Status == TaskItem.TaskStatus.New);
                        PrintTasks(statusTasks);
                        break;
                    case "6":
                        EditTask(manager);
                        break;
                    case "7":
                        Console.Write("ID задачи для удаления: ");
                        if (int.TryParse(Console.ReadLine(), out int id)) manager.RemoveTask(id);
                        break;
                    case "8":
                        manager.PrintStatistics();
                        break;
                    case "9":
                        await manager.SaveToFileAsync();
                        break;
                    case "0":
                        await manager.SaveToFileAsync();
                        exit = true;
                        break;
                    default:
                        Logging.Warning("Неверный выбор.");
                        break;
                }
            } catch (Exception ex) {
                Logging.Error($"Произошла ошибка: {ex.Message}");
            }
        }

        cts.Cancel();
    }

    private static void AddNewTask(TaskManager<TaskItem> manager) {
        Console.Write("Название: ");
        string title = Console.ReadLine() ?? "Без названия";

        var task = new TaskItem {
            Title = title,
            Description = "Тестовое описание",
            Deadline = DateTime.Now.AddMinutes(2)
        };

        manager.AddTask(task);
    }

    private static void EditTask(TaskManager<TaskItem> manager) {
        Console.Write("ID задачи для редактирования: ");
        if (int.TryParse(Console.ReadLine(), out int id)) {
            var task = manager.GetTask(id);
            if (task != null) {
                task.Status = TaskItem.TaskStatus.InProgress;
                Logging.Info($"Статус задачи {id} изменен на InProgress");
            } else {
                Logging.Warning("Задача не найдена.");
            }
        }
    }

    private static void PrintTasks(IEnumerable<TaskItem> tasks) {
        foreach (var task in tasks) Console.WriteLine(task.ToString());
    }

    private static async Task BackgroundOverdueChecker(TaskManager<TaskItem> manager, CancellationToken token) {
        while (!token.IsCancellationRequested) {
            var overdueCount = manager.FindTasks(t => t.IsOverdue).Count();
            if (overdueCount > 0) {
                Logging.Warning($"[ФОНОВОЕ УВЕДОМЛЕНИЕ] Внимание! У вас {overdueCount} просроченных задач(и)!");
            }
            await Task.Delay(15000, token);
        }
    }
}

using TaskHub.Utils;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TaskHub;

public class TaskManager<T>(string dataFilePath) : IDisposable where T : TaskItem {
    private List<T> tasks = new();
    private int nextId = 1;
    private readonly string filePath = dataFilePath;
    private bool disposed = false;

    public struct TaskID {
        public int id;
        public TaskID(int id) { this.id = id; }
    }

    public struct TaskView {
        public T task;
        public TaskID id;
    }

    public delegate void AddTaskHandler(TaskView taskView);
    public delegate void RemoveTaskHandler(TaskView taskView);
    public delegate void UpdateTaskHandler(TaskView taskView);

    public event AddTaskHandler? OnTaskAdded;
    public event RemoveTaskHandler? OnTaskRemoved;
    public event UpdateTaskHandler? OnPreUpdateTask;
    public event UpdateTaskHandler? OnPostUpdateTask;

    public delegate void TaskNotification(string message);
    public event TaskNotification? OnNotification;

    public void AddTask(T task) {
        task.Id = nextId++;
        tasks.Add(task);
        OnTaskAdded?.Invoke(new TaskView { task = task, id = new TaskID(task.Id) });
        Logging.Info($"Задача добавлена: {task.Title}");
        OnNotification?.Invoke($"Задача добавлена: {task.Title}");
    }

    public bool RemoveTask(int id) {
        var task = tasks.FirstOrDefault(t => t.Id == id);
        if (task != null) {
            OnTaskRemoved?.Invoke(new TaskView { task = task, id = new TaskID(task.Id) });
            tasks.Remove(task);
            Logging.Info($"Задача удалена: {task.Title}");
            OnNotification?.Invoke($"Задача удалена: {task.Title}");
            return true;
        }
        return false;
    }

    public T? GetTask(int id) => tasks.FirstOrDefault(t => t.Id == id);

    public IEnumerable<T> GetAllTasks() => tasks;

    public IEnumerable<T> FindTasks(Func<T, bool> predicate) => tasks.Where(predicate);

    public void PrintStatistics() {
        Logging.Info("\n--- Статистика ---");
        Logging.Info($"Всего задач: {tasks.Count}");
        Logging.Info($"Выполнено: {tasks.Count(t => t.Status == TaskItem.TaskStatus.Done)}");
        Logging.Info($"Просрочено: {tasks.Count(t => t.IsOverdue)}");

        var byPriority = tasks
            .GroupBy(t => t.Priority)
            .ToDictionary(g => g.Key, g => g.Count());

        foreach (var p in Enum.GetValues<TaskItem.PriorityLevel>()) {
            Logging.Info($"Приоритет {p}: {byPriority.GetValueOrDefault(p, 0)}");
        }
        Logging.Info("------------------\n");
    }

    public async Task SaveToFileAsync() {
        try {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var yaml = serializer.Serialize(tasks);
            await File.WriteAllTextAsync(filePath, yaml);
            Logging.Info("Данные успешно сохранены в YAML.");
            OnNotification?.Invoke("Данные успешно сохранены.");
        } catch (Exception ex) {
            Logging.Error($"Ошибка сохранения: {ex.Message}");
        }
    }

    public async Task LoadFromFileAsync() {
        if (!File.Exists(filePath)) return;

        try {
            var yaml = await File.ReadAllTextAsync(filePath);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var loaded = deserializer.Deserialize<List<T>>(yaml);
            if (loaded != null) {
                tasks = loaded;
                nextId = (tasks.Count > 0 ? tasks.Max(t => t.Id) : 0) + 1;
                Logging.Info("Данные успешно загружены из YAML.");
                OnNotification?.Invoke("Данные успешно загружены.");
            }
        } catch (Exception ex) {
            Logging.Error($"Ошибка загрузки: {ex.Message}");
        }
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
        if (!disposed) {
            if (disposing) tasks.Clear();
            disposed = true;
        }
    }
}

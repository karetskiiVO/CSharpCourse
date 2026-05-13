namespace TaskHub;

public class TaskItem {
    public enum PriorityLevel : byte { Low, Medium, High }
    public enum TaskStatus : byte { New, InProgress, Done }

    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
    public TaskStatus Status { get; set; } = TaskStatus.New;
    public DateTime Deadline { get; set; }

    public bool IsOverdue => DateTime.Now > Deadline && Status != TaskStatus.Done;

    public override string ToString() {
        string overdueMark = IsOverdue ? " [ПРОСРОЧЕНО!]" : "";
        return $"[ID: {Id}] {Title} | Priority: {Priority} | Status: {Status} | Deadline: {Deadline:g}{overdueMark}\n  Desc: {Description}";
    }
}

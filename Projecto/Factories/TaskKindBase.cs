namespace Projecto
{
    public abstract class TaskKindBase<TSelf> : ITaskKind where TSelf : new()
    {
        public static TSelf Instance = new();

        public abstract string Name { get; }
        public abstract ITask Create(string name, TaskStatus taskStatus = default);
    }
}
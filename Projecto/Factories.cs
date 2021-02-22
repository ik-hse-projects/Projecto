namespace Projecto
{
    public class StoryFactory : ITaskKind
    {
        public static readonly StoryFactory Instance = new();
        public string Name => "История";

        public ITask Create(string name, TaskStatus taskStatus = default)
        {
            return new Story(name, taskStatus);
        }
    }

    public abstract class TaskFactoryBase<TSelf> : ITaskKind where TSelf : new()
    {
        public static TSelf Instance = new();

        public abstract string Name { get; }
        public abstract ITask Create(string name, TaskStatus taskStatus = default);
    }

    public class EpicFactory : TaskFactoryBase<EpicFactory>
    {
        public override string Name => "Тема";

        public override ITask Create(string name, TaskStatus taskStatus = default)
        {
            return new Epic(name, taskStatus);
        }
    }

    public class TaskFactory : TaskFactoryBase<TaskFactory>
    {
        public override string Name => "Задача";

        public override ITask Create(string name, TaskStatus taskStatus = default)
        {
            return new Task(name, taskStatus);
        }
    }

    public class BugFactory : TaskFactoryBase<BugFactory>
    {
        public override string Name => "Ошибка";

        public override ITask Create(string name, TaskStatus taskStatus = default)
        {
            return new Bug(name, taskStatus);
        }
    }
}
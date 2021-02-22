using System.Text.Json.Serialization;

namespace Projecto
{
    /// <summary>
    /// Общая часть для всех фабрик, которые создают различные ITask. 
    /// </summary>
    /// <typeparam name="TSelf"></typeparam>
    public abstract class TaskCreatorBase<TSelf> : ITaskKind where TSelf : new()
    {
        public static TSelf Instance = new();

        public abstract string Name { get; }
        public abstract ITask Create(string name, TaskStatus taskStatus = default);
    }

    public class EpicCreator : TaskCreatorBase<EpicCreator>
    {
        public override string Name => "Тема";

        public override ITask Create(string name, TaskStatus taskStatus = default) => new Epic(name, taskStatus);
    }

    public class TaskCreator : TaskCreatorBase<TaskCreator>
    {
        public override string Name => "Задача";
        public override ITask Create(string name, TaskStatus taskStatus = default) => new Task(name, taskStatus);
    }

    public class BugCreator : TaskCreatorBase<BugCreator>
    {
        public override string Name => "Ошибка";
        public override ITask Create(string name, TaskStatus taskStatus = default) => new Bug(name, taskStatus);
    }
}
using System;

namespace Projecto
{
    /// <inheritdoc />
    /// <summary>
    /// Общая для всех задач часть.
    /// </summary>
    public abstract class TaskBase : ITask
    {
        public abstract ITaskKind Kind { get; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public TaskStatus TaskStatus { get; set; }

        protected TaskBase(string name, TaskStatus taskStatus = default)
        {
            Name = name;
            TaskStatus = taskStatus;
            CreatedAt = DateTime.Now;
        }
    }

    public abstract class TaskFactoryBase<TSelf> : ITaskKind where TSelf : new()
    {
        public static TSelf Instance = new();

        protected TaskFactoryBase()
        {
        }

        public abstract string Name { get; }
        public abstract ITask Create(string name, TaskStatus taskStatus = default);
    }
}
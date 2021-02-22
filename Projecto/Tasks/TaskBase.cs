using System;

namespace Projecto
{
    /// <inheritdoc />
    /// <summary>
    /// Общая для всех задач часть.
    /// </summary>
    public abstract class TaskBase : ITask
    {
        protected TaskBase(string name, TaskStatus taskStatus = default)
        {
            Name = name;
            TaskStatus = taskStatus;
            CreatedAt = DateTime.Now;
        }

        public abstract ITaskKind Kind { get; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public TaskStatus TaskStatus { get; set; }
    }
}
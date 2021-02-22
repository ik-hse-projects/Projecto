using System;
using System.Collections.Generic;

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
    
    /// <summary>
    /// Задача, по объему работ меньшая, чем Epic. Может быть подзадачей Epic.
    /// </summary>
    public class Story : TaskBase, IHaveManyExecutors
    {
        IList<IUser> IHaveManyExecutors.Executors { get; } = new List<IUser>();

        public Story(string name, TaskStatus taskStatus = default) : base(name, taskStatus)
        {
        }

        public override ITaskKind Kind => StoryFactory.Instance;
    }

    public class StoryFactory : ITaskKind
    {
        public static readonly StoryFactory Instance = new();
        public string Name => "История";

        public ITask Create(string name, TaskStatus taskStatus = default) => new Story(name, taskStatus);
    }

    // Все классы ниже тривиально реализуют все перечисленные для них интерфейсы.
    // Вынести некоторый общий код в базовый класс возможно, но мне кажется, что это только усложнит понимание.

    /// <summary>
    /// Большая задача, для реализации которой нужно много времени, включает в себя несколько меньших подзадач.
    /// </summary>
    public class Epic : TaskBase, IHaveSubtasks
    {
        public List<ITask> Subtasks { get; } = new();

        public IReadOnlyList<ITaskKind> AllowedSubtasks => new ITaskKind[]
        {
            StoryFactory.Instance,
            TaskFactory.Instance,
        };

        public Epic(string name, TaskStatus taskStatus = default) : base(name, taskStatus)
        {
        }

        public override ITaskKind Kind => EpicFactory.Instance;
    }

    /// <summary>
    /// Задача, по объему работ меньшая, чем Story. Может быть подзадачей Epic.
    /// </summary>
    public class Task : TaskBase, IHaveSingleExecutor
    {
        public IUser? Executor { get; set; }
        public override ITaskKind Kind => TaskFactory.Instance;

        public Task(string name, TaskStatus taskStatus = default) : base(name, taskStatus)
        {
        }
    }

    /// <summary>
    /// Задача, описывающая проблему в проекте.
    /// </summary>
    public class Bug : TaskBase, IHaveSingleExecutor
    {
        public IUser? Executor { get; set; }

        public Bug(string name, TaskStatus taskStatus = default) : base(name, taskStatus)
        {
        }

        public override ITaskKind Kind => BugFactory.Instance;
    }
}
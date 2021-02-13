using System.Collections.Generic;

namespace Projecto
{
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

    public class EpicFactory : TaskFactoryBase<EpicFactory>
    {
        public override string Name => "Тема";

        public override ITask Create(string name, TaskStatus taskStatus = default) => new Epic(name, taskStatus);
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

    public class TaskFactory : TaskFactoryBase<TaskFactory>
    {
        public override string Name => "Задача";
        public override ITask Create(string name, TaskStatus taskStatus = default) => new Task(name, taskStatus);
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

    public class BugFactory : TaskFactoryBase<BugFactory>
    {
        public override string Name => "Ошибка";
        public override ITask Create(string name, TaskStatus taskStatus = default) => new Bug(name, taskStatus);
    }
}
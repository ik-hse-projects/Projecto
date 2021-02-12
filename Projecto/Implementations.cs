using System;
using System.Collections.Generic;

namespace Projecto
{
    /// <summary>
    /// Человек, который может быть назначен исполнителем задачи.
    /// </summary>
    public class User : IUser
    {
        public string Name { get; }

        public User(string name)
        {
            Name = name;
        }
    }
    
    /// <summary>
    /// Общая для всех задач часть.
    /// </summary>
    public abstract class TaskBase : ITask
    {
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
    public class Story : TaskBase, ISubtaskOf<Epic>, IHaveManyExecutors
    {
        private readonly List<IUser> Executors = new List<IUser>();
        IReadOnlyList<IUser> IHaveManyExecutors.Executors => Executors;

        public bool TryAddExecutor(IUser user)
        {
            Executors.Add(user);
            return true;
        }

        public bool TryRemoveExecutorAt(int index)
        {
            if (index < 0 || index >= Executors.Count)
            {
                return false;
            }

            Executors.RemoveAt(index);
            return true;
        }

        public Story(string name, TaskStatus taskStatus = default) : base(name, taskStatus)
        {
        }
    }

    // Все классы ниже тривиально реализуют все перечисленные для них интерфейсы.
    // Вынести некоторый общий код в базовый класс возможно, но мне кажется, что это только усложнит понимание.

    /// <summary>
    /// Большая задача, для реализации которой нужно много времени, включает в себя несколько меньших подзадач.
    /// </summary>
    public class Epic : TaskBase, IHaveSubtasks<Epic>
    {
        public List<ISubtaskOf<Epic>> Subtasks { get; } = new List<ISubtaskOf<Epic>>();

        public Epic(string name, TaskStatus taskStatus = default) : base(name, taskStatus)
        {
        }
    }

    /// <summary>
    /// Задача, по объему работ меньшая, чем Story. Может быть подзадачей Epic.
    /// </summary>
    public class Task : TaskBase, ISubtaskOf<Epic>, IHaveSingleExecutor
    {
        public IUser? Executor { get; set; }

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
    }
}
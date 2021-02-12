using System;
using System.Collections.Generic;

namespace Projecto
{
    /// <summary>
    /// Общая для всех задач часть.
    /// </summary>
    public abstract class TaskBase : ITask
    {
        public string Name { get; }
        public DateTime CreatedAt { get; }
        public TaskStatus TaskStatus { get; }
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
    }

    // Все классы ниже тривиально реализуют все перечисленные для них интерфейсы.
    // Вынести некоторый общий код в базовый класс возможно, но мне кажется, что это только усложнит понимание.

    /// <summary>
    /// Большая задача, для реализации которой нужно много времени, включает в себя несколько меньших подзадач.
    /// </summary>
    public class Epic : TaskBase, IHaveSubtasks<Epic>
    {
        public List<ISubtaskOf<Epic>> Subtasks { get; } = new List<ISubtaskOf<Epic>>();
    }

    /// <summary>
    /// Задача, по объему работ меньшая, чем Story. Может быть подзадачей Epic.
    /// </summary>
    public class Task : TaskBase, ISubtaskOf<Epic>, IHaveSingleExecutor
    {
        public IUser? Executor { get; set; }
    }

    /// <summary>
    /// Задача, описывающая проблему в проекте.
    /// </summary>
    public class Bug : TaskBase, IHaveSingleExecutor
    {
        public IUser? Executor { get; set; }
    }

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
}
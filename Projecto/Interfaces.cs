using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Projecto
{
    /// <summary>
    /// Если класс реализует этот интерфейс, то он задача. 
    /// </summary>
    public interface ITask
    {
        public ITaskKind Kind { get; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public TaskStatus TaskStatus { get; set; }
    }

    /// <summary>
    /// Означает, что задача может содержать подзадачи. 
    /// </summary>
    public interface IHaveSubtasks
    {
        /// <summary>
        /// Список подзадач. Можно менять.
        /// </summary>
        public List<ITask> Subtasks { get; }

        /// <summary>
        /// Список допустимых подзадач.
        /// </summary>
        public IReadOnlyList<ITaskKind> AllowedSubtasks { get; }
    }

    /// <summary>
    /// Представляет собой некоторый вид задач. Например, «тема», «история», «ошибка».
    /// </summary>
    public interface ITaskKind
    {
        /// <summary>
        /// Название этого вида задач.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Создаёт задачу.
        /// </summary>
        public ITask Create(string name, TaskStatus taskStatus = default);
    }

    /// <summary>
    /// Означает, что задача может иметь 0 или 1, или много исполнителей.
    /// </summary>
    public interface IHaveExecutors
    {
        /// <summary>
        /// Список исполнителей. Только читать.
        /// </summary>
        public IReadOnlyList<IUser> Executors { get; }

        /// <summary>
        /// Добавляет исполнителя. Если это почему-то невозможно, то возвращает false.
        /// Например, эта задача разрешает только одного исполнителя.
        /// </summary>
        public bool TryAddExecutor(IUser user);

        /// <summary>
        /// Добавляет исполнителя и выбрасывает исключение, если почему-то это невозможно.
        /// </summary>
        public void AddExecutor(IUser user)
        {
            if (!TryAddExecutor(user))
            {
                throw new InvalidOperationException("Can't add executor");
            }
        }

        /// <summary>
        /// Удаляет исполнителя и возвращает false, если это почему-то невозможно.
        /// Например, указан несуществующий индекс.
        /// </summary>
        public bool TryRemoveExecutorAt(int index);

        /// <summary>
        /// Удаляет исполнителя и выбрасывает исключение, если почему-то это невозможно.
        /// </summary>
        public void RemoveExecutorAt(int index)
        {
            if (!TryRemoveExecutorAt(index))
            {
                throw new InvalidOperationException("Can't remove executor");
            }
        }
    }

    /// <summary>
    /// Означает, что задача точно может иметь много исполнителей (в т.ч. больше одного). 
    /// </summary>
    public interface IHaveManyExecutors : IHaveExecutors
    {
        /// <summary>
        /// Список исполнителей. Можно менять
        /// </summary>
        public new IList<IUser> Executors { get; }

        /// <inheritdoc />
        IReadOnlyList<IUser> IHaveExecutors.Executors => Executors.AsReadOnly();

        /// <inheritdoc />
        bool IHaveExecutors.TryAddExecutor(IUser user)
        {
            Executors.Add(user);
            return true;
        }

        /// <inheritdoc />
        bool IHaveExecutors.TryRemoveExecutorAt(int index)
        {
            if (index < 0 || index >= Executors.Count)
            {
                return false;
            }

            Executors.RemoveAt(index);
            return true;
        }
    }

    /// <summary>
    /// Означает, что задача может имеет не более одного исполнителя. 
    /// </summary>
    /// <remarks>
    /// Если класс имеет не более одного исполнителя, то он очевидно имеет «0 или 1, или много исполнителей».
    /// Поэтому этот интерфейс содержит реализацию по-умолчанию для интерфейса IHaveManyExecutors.
    /// </remarks>
    public interface IHaveSingleExecutor : IHaveExecutors
    {
        /// <summary>
        /// Единственный исполнитель. null, если он не назначен. Можно менять.
        /// </summary>
        public IUser? Executor { get; set; }

        // Далее следует реализация IHaveManyExecutors.

        IReadOnlyList<IUser> IHaveExecutors.Executors => Executor == null ? Array.Empty<IUser>() : new[] {Executor};

        bool IHaveExecutors.TryAddExecutor(IUser user)
        {
            if (Executor == null)
            {
                return false;
            }

            Executor = user;
            return true;
        }

        bool IHaveExecutors.TryRemoveExecutorAt(int index)
        {
            if (index != 0 || Executor == null)
            {
                return false;
            }

            Executor = null;
            return true;
        }
    }

    /// <summary>
    /// Человек, который может быть назначен исполнителем задачи.
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// Имя человека.
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Статус, который позволяет определить, что происходит с задачей в данный момент.
    /// </summary>
    public enum TaskStatus
    {
        /// <summary>
        /// Открытая задача - статус по умолчанию.
        /// </summary>
        Open = 0,

        /// <summary>
        /// Задача в работе.
        /// </summary>
        InProcess,

        /// <summary>
        /// Завершенная задача.
        /// </summary>
        Completed,
    }

    /// <summary>
    /// Методы для перечисления <see cref="TaskStatus"/>
    /// </summary>
    public static class TaskStatusExt
    {
        public static TaskStatus[] Statuses => new[] {TaskStatus.Open, TaskStatus.InProcess, TaskStatus.Completed};
    }
}
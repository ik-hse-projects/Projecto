using System;
using System.Collections.Generic;

namespace Projecto
{
    /// <summary>
    /// Если класс реализует этот интерфейс, то он задача. 
    /// </summary>
    public interface ITask
    {
        public string Name { get; }
        public DateTime CreatedAt { get; }
        public TaskStatus TaskStatus { get; }
    }

    /// <summary>
    /// Пометка, что реализующий этот интерфейс класс может быть подзадачей другого класса.
    /// </summary>
    /// <typeparam name="T">Класс, который является надзадачей по отношению к реализующему интерфейс классу.</typeparam>
    public interface ISubtaskOf<T> : ITask where T : ITask
    {
    }

    /// <summary>
    /// Означает, что задача может содержать подзадачи. 
    /// </summary>
    /// <typeparam name="TSelf">
    /// Класс, который является надзадачей для всех подзадач.
    /// Ожидается, что здесь будет тот же тип, который реализует этот интерфейс.
    /// </typeparam>
    public interface IHaveSubtasks<TSelf> where TSelf : ITask
    {
        /// <summary>
        /// Список подзадач. Можно менять.
        /// </summary>
        public List<ISubtaskOf<TSelf>> Subtasks { get; }
    }

    /// <summary>
    /// Означает, что задача может иметь 0 или 1, или много исполнителей.
    /// </summary>
    public interface IHaveManyExecutors
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
    /// Означает, что задача может имеет не более одного исполнителя. 
    /// </summary>
    /// <remarks>
    /// Если класс имеет не более одного исполнителя, то он очевидно имеет «0 или 1, или много исполнителей».
    /// Поэтому этот интерфейс содержит реализацию по-умолчанию для интерфейса IHaveManyExecutors.
    /// </remarks>
    public interface IHaveSingleExecutor : IHaveManyExecutors
    {
        /// <summary>
        /// Единственный исполнитель. null, если он не назначен. Можно менять.
        /// </summary>
        public IUser? Executor { get; set; }
        
        // Далее следует реализация IHaveManyExecutors.
        
        IReadOnlyList<IUser> IHaveManyExecutors.Executors => Executor == null ? Array.Empty<IUser>() : new[] {Executor};

        bool IHaveManyExecutors.TryAddExecutor(IUser user)
        {
            if (Executor == null)
            {
                return false;
            }

            Executor = user;
            return true;
        }

        bool IHaveManyExecutors.TryRemoveExecutorAt(int index)
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
        public string Name { get; }
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
}
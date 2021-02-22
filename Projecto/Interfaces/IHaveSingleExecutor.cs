using System;
using System.Collections.Generic;

namespace Projecto
{
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
        public User? Executor { get; set; }

        // Далее следует реализация по-умолчанию IHaveManyExecutors.

        /// <inheritdoc />
        IReadOnlyList<User> IHaveExecutors.Executors => Executor == null ? Array.Empty<User>() : new[] {Executor};

        /// <inheritdoc />
        bool IHaveExecutors.TryAddExecutor(User user)
        {
            if (Executor == null)
            {
                return false;
            }

            Executor = user;
            return true;
        }

        /// <inheritdoc />
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
}
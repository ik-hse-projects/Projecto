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
}
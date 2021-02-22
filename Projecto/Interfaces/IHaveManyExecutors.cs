using System.Collections.Generic;

namespace Projecto
{
    /// <summary>
    /// Означает, что задача точно может иметь много исполнителей (в т.ч. больше одного).
    /// </summary>
    public interface IHaveManyExecutors : IHaveExecutors
    {
        /// <summary>
        /// Список исполнителей. Можно менять.
        /// </summary>
        // Основное отличие от IHaveExecutors.Executors — то, что это уже не IReadOnlyList, а полноценный IList.
        public new IList<User> Executors { get; }

        // Далее следует реализация по-умолчанию IHaveManyExecutors.
        
        /// <inheritdoc />
        IReadOnlyList<User> IHaveExecutors.Executors => Executors.AsReadOnly();

        /// <inheritdoc />
        bool IHaveExecutors.TryAddExecutor(User user)
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
}
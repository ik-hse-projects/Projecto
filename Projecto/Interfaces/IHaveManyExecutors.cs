using System.Collections.Generic;

namespace Projecto
{
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
}
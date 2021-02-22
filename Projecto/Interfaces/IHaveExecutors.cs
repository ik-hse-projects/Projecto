using System;
using System.Collections.Generic;

namespace Projecto
{
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
}
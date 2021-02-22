using System;

namespace Projecto
{
    /// <summary>
    /// Если класс реализует этот интерфейс, то он задача.
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// Тип задачи.
        /// </summary>
        /// <seealso cref="ITaskKind"/>
        public ITaskKind Kind { get; }

        /// <summary>
        /// Название задачи.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Когда эта задача была создана.
        /// Не меняется.
        /// </summary>
        public DateTime CreatedAt { get; }

        /// <summary>
        /// Текущий статус задачи.
        /// </summary>
        public TaskStatus TaskStatus { get; set; }
    }
}
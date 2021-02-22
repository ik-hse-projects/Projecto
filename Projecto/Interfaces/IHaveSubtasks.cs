using System.Collections.Generic;

namespace Projecto
{
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
}
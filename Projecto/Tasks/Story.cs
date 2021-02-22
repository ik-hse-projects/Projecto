using System.Collections.Generic;

namespace Projecto
{
    /// <summary>
    /// Задача, по объему работ меньшая, чем Epic. Может быть подзадачей Epic.
    /// </summary>
    public class Story : TaskBase, IHaveManyExecutors
    {
        public Story(string name, TaskStatus taskStatus = default) : base(name, taskStatus)
        {
        }

        /// <inheritdoc />
        public override ITaskKind Kind => StoryTaskKind.Instance;

        /// <inheritdoc />
        IList<User> IHaveManyExecutors.Executors { get; } = new List<User>();
    }
}
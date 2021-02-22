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

        public override ITaskKind Kind => StoryTaskKind.Instance;
        IList<IUser> IHaveManyExecutors.Executors { get; } = new List<IUser>();
    }
}
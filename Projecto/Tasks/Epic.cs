using System.Collections.Generic;

namespace Projecto
{
    /// <summary>
    /// Большая задача, для реализации которой нужно много времени, включает в себя несколько меньших подзадач.
    /// </summary>
    public class Epic : TaskBase, IHaveSubtasks
    {
        public Epic(string name, TaskStatus taskStatus = default) : base(name, taskStatus)
        {
        }

        public override ITaskKind Kind => EpicTaskKind.Instance;
        public List<ITask> Subtasks { get; } = new();

        public IReadOnlyList<ITaskKind> AllowedSubtasks => new ITaskKind[]
        {
            StoryTaskKind.Instance,
            TaskTaskKind.Instance
        };
    }
}
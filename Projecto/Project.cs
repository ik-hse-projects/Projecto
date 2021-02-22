using System.Collections.Generic;

namespace Projecto
{
    public class Project : IHaveSubtasks
    {
        public Project(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public List<ITask> Subtasks { get; } = new();

        public IReadOnlyList<ITaskKind> AllowedSubtasks => new ITaskKind[]
        {
            EpicTaskKind.Instance,
            BugTaskKind.Instance
        };
    }
}
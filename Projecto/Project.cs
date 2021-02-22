using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Projecto
{
    public class Project : IHaveSubtasks
    {
        public string Name { get; set; }
        public List<ITask> Subtasks { get; } = new();

        public IReadOnlyList<ITaskKind> AllowedSubtasks => new ITaskKind[]
        {
            EpicFactory.Instance,
            BugFactory.Instance
        };

        public Project(string name)
        {
            Name = name;
        }
    }
}
using System.Collections.Generic;

namespace Projecto
{
    /// <summary>
    /// Проект 
    /// </summary>
    public class Project : IHaveSubtasks
    {
        /// <summary>
        /// Создаёт пустой проект с указанным названием.
        /// </summary>
        public Project(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Название проекта.
        /// </summary>
        public string Name { get; set; }

        /// <inheritdoc />
        public List<ITask> Subtasks { get; } = new();

        /// <inheritdoc />
        public IReadOnlyList<ITaskKind> AllowedSubtasks => new ITaskKind[]
        {
            EpicTaskKind.Instance,
            BugTaskKind.Instance
        };
    }
}
using System.Collections.Generic;

namespace Projecto
{
    public class Project
    {
        public string Name { get; set; }
        public List<ITask> Tasks { get; } = new List<ITask>();

        public Project(string name)
        {
            Name = name;
        }
    }
}
using System;

namespace Projecto
{
    /// <summary>
    /// Если класс реализует этот интерфейс, то он задача.
    /// </summary>
    public interface ITask
    {
        public ITaskKind Kind { get; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public TaskStatus TaskStatus { get; set; }
    }
}
namespace Projecto
{
    /// <summary>
    /// <see cref="ITaskKind"/> для <see cref="Task"/>.
    /// </summary>
    public class TaskTaskKind : Singleton<TaskTaskKind>, ITaskKind
    {
        /// <inheritdoc />
        public string Name => "Задача";

        /// <inheritdoc />
        public ITask Create(string name, TaskStatus taskStatus = default)
        {
            return new Task(name, taskStatus);
        }
    }
}
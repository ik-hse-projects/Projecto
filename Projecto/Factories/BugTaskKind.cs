namespace Projecto
{
    /// <summary>
    /// <see cref="ITaskKind" /> для <see cref="Task" />.
    /// </summary>
    public class BugTaskKind : Singleton<BugTaskKind>, ITaskKind
    {
        /// <inheritdoc />
        public string Name => "Ошибка";

        /// <inheritdoc />
        public ITask Create(string name, TaskStatus taskStatus = default)
        {
            return new Bug(name, taskStatus);
        }
    }
}
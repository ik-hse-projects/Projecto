namespace Projecto
{
    /// <summary>
    /// <see cref="ITaskKind"/> для <see cref="Epic"/>.
    /// </summary>
    public class EpicTaskKind : Singleton<EpicTaskKind>, ITaskKind
    {
        /// <inheritdoc />
        public string Name => "Тема";

        /// <inheritdoc />
        public ITask Create(string name, TaskStatus taskStatus = default)
        {
            return new Epic(name, taskStatus);
        }
    }
}
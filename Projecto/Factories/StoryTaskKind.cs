namespace Projecto
{
    /// <summary>
    /// <see cref="ITaskKind"/> для <see cref="Story"/>.
    /// </summary>
    public class StoryTaskKind : Singleton<StoryTaskKind>, ITaskKind
    {
        /// <inheritdoc />
        public string Name => "История";

        /// <inheritdoc />
        public ITask Create(string name, TaskStatus taskStatus = default)
        {
            return new Story(name, taskStatus);
        }
    }
}
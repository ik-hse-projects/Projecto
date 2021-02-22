namespace Projecto
{
    /// <summary>
    /// Представляет собой некоторый вид задач. Например, «тема», «история», «ошибка».
    /// </summary>
    public interface ITaskKind
    {
        /// <summary>
        /// Название этого вида задач.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Создаёт задачу.
        /// </summary>
        public ITask Create(string name, TaskStatus taskStatus = default);
    }
}
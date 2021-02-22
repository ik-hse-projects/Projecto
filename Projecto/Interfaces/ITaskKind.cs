namespace Projecto
{
    /// <summary>
    /// Представляет собой некоторый вид задач. Например, «тема», «история», «ошибка».
    ///
    /// Важно, что реализации этого интерфейса умеют создавать экземпляры соответсвующих им задач (<see cref="Create"/>).
    /// По большей части, именно для этого и был создан этот интерфейс.
    /// Он активно применяется в <see cref="IHaveSubtasks.AllowedSubtasks"/>,
    /// чтобы создавать только корректные подзадачи для некоторой задачи.
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
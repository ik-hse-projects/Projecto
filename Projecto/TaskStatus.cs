namespace Projecto
{
    /// <summary>
    /// Статус, который позволяет определить, что происходит с задачей в данный момент.
    /// </summary>
    public enum TaskStatus
    {
        /// <summary>
        /// Открытая задача - статус по умолчанию.
        /// </summary>
        Open = 0,

        /// <summary>
        /// Задача в работе.
        /// </summary>
        InProcess,

        /// <summary>
        /// Завершенная задача.
        /// </summary>
        Completed
    }
    
    /// <summary>
    /// Методы для перечисления <see cref="TaskStatus" />
    /// </summary>
    public static class TaskStatusExt
    {
        public static TaskStatus[] Statuses => new[] {TaskStatus.Open, TaskStatus.InProcess, TaskStatus.Completed};
    }
}
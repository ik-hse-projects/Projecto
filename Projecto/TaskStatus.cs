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
        
        public static string RuString(this TaskStatus status)
        {
            return status switch
            {
                TaskStatus.Open => "Открыта",
                TaskStatus.InProcess => "В работе",
                TaskStatus.Completed => "Завершена"
            };
        }
    }

    // Я оставил эти два типа в одном файле, поскольку разделять их было бы очень странно и неприятно.
    // Как жаль, что C# не позволяет добавлять методы к enum'ам.
}
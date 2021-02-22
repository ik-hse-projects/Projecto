namespace Projecto.Tui
{
    public static class ProjectoExtension
    {
        /// <summary>
        /// Переводит статус задачи в строку на русском языке.
        /// </summary>
        public static string RuString(this TaskStatus status)
        {
            return status switch
            {
                TaskStatus.Open => "Открыта",
                TaskStatus.InProcess => "В работе",
                TaskStatus.Completed => "Завершена"
            };
        }

        /// <summary>
        /// Определяет тип переданной задачи и переводит его на русский язык.
        /// </summary>
        public static string Kind(this ITask task)
        {
            return task switch
            {
                Epic _ => "Тема",
                Story _ => "История",
                Task _ => "Задача",
                Bug _ => "Ошибка",
                _ => "Другое"
            };
        }
    }
}
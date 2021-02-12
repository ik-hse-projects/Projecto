using System;

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
                TaskStatus.Completed => "Завершена",
            };
        }
    }
}
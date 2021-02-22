namespace Projecto
{
    /// <summary>
    /// Человек, который может быть назначен исполнителем задачи.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Создаёт <see cref="User"/>, задавая ему имя.
        /// </summary>
        public User(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Имя пользователя/исполнителя.
        /// </summary>
        public string Name { get; set; }
    }
}
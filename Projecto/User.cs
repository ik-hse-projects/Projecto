namespace Projecto
{
    /// <summary>
    /// Человек, который может быть назначен исполнителем задачи.
    /// </summary>
    public class User : IUser
    {
        public User(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
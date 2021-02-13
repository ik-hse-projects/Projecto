namespace Projecto
{
    /// <summary>
    /// Человек, который может быть назначен исполнителем задачи.
    /// </summary>
    public class User : IUser
    {
        public string Name { get; }

        public User(string name)
        {
            Name = name;
        }
    }
}
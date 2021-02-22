using Newtonsoft.Json;

namespace Projecto
{
    /// <summary>
    /// Человек, который может быть назначен исполнителем задачи.
    /// </summary>
    public class User : IUser
    {
        public string Name { get; set; }

        public User(string name)
        {
            Name = name;
        }
    }
}
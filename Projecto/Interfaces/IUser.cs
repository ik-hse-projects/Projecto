namespace Projecto
{
    /// <summary>
    /// Человек, который может быть назначен исполнителем задачи.
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// Имя человека.
        /// </summary>
        public string Name { get; set; }
    }
}
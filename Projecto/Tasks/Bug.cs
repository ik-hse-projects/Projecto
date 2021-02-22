namespace Projecto
{ 
    /// <summary>
    /// Задача, описывающая проблему в проекте.
    /// </summary>
    public class Bug : TaskBase, IHaveSingleExecutor
    {
        public Bug(string name, TaskStatus taskStatus = default) : base(name, taskStatus)
        {
        }

        /// <inheritdoc />
        public override ITaskKind Kind => BugTaskKind.Instance;

        /// <inheritdoc />
        public User? Executor { get; set; }
    }
}
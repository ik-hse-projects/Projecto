namespace Projecto
{
    /// <summary>
    /// Задача, по объему работ меньшая, чем Story. Может быть подзадачей Epic.
    /// </summary>
    public class Task : TaskBase, IHaveSingleExecutor
    {
        public Task(string name, TaskStatus taskStatus = default) : base(name, taskStatus)
        {
        }

        public override ITaskKind Kind => TaskTaskKind.Instance;
        public IUser? Executor { get; set; }
    }
}
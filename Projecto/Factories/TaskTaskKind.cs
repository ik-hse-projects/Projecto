namespace Projecto
{
    public class TaskTaskKind : TaskKindBase<TaskTaskKind>
    {
        public override string Name => "Задача";

        public override ITask Create(string name, TaskStatus taskStatus = default)
        {
            return new Task(name, taskStatus);
        }
    }
}
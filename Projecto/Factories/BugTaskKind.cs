namespace Projecto
{
    public class BugTaskKind : TaskKindBase<BugTaskKind>
    {
        public override string Name => "Ошибка";

        public override ITask Create(string name, TaskStatus taskStatus = default)
        {
            return new Bug(name, taskStatus);
        }
    }
}
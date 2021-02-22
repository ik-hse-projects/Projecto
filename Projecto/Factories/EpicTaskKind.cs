namespace Projecto
{
    public class EpicTaskKind : TaskKindBase<EpicTaskKind>
    {
        public override string Name => "Тема";

        public override ITask Create(string name, TaskStatus taskStatus = default)
        {
            return new Epic(name, taskStatus);
        }
    }
}
namespace Projecto
{
    public class StoryTaskKind : TaskKindBase<StoryTaskKind>
    {
        public override string Name => "История";

        public override ITask Create(string name, TaskStatus taskStatus = default)
        {
            return new Story(name, taskStatus);
        }
    }
}
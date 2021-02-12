using System;
using Thuja;
using Thuja.Widgets;

namespace Projecto.Tui
{
    public class OpenedProject
    {
        private ListOf<ITask> tasks;
        private BaseContainer root;
        public IWidget Widget { get; }

        public OpenedProject(BaseContainer root, Project project)
        {
            this.root = root;
            tasks = new StackContainer().ListOf<ITask>(TaskToWidget);
            foreach (var task in project.Tasks)
            {
                tasks.Add(task);
            }

            Widget = new StackContainer()
                .Add(new Button("Удалить проект").OnClick(() => Deleted?.Invoke()))
                .Add(new Button("Добавить задачу").OnClick(AddTask))
                .Add(tasks.Widget);
        }

        private void AddTask()
        {
            var radios = new RadioSetBuilder<string>()
                .Add("Тема", "theme")
                .Add("История", "story")
                .Add("Задача", "task")
                .Add("Ошибка", "bug");
            var nameField = new InputField();
            new Popup()
                .Add(new Label("Вид задачи:"))
                .Add(radios.ToStack())
                .Add(new StackContainer(Orientation.Horizontal)
                    .Add(new Label("Имя: "))
                    .Add(nameField))
                .AddWith(popup => new Button("Создать").OnClick(() =>
                {
                    if (radios.Checked != null)
                    {
                        popup.Close();
                        var name = nameField.Text.ToString();
                        ITask result = radios.Checked switch
                        {
                            "theme" => new Epic(name),
                            "story" => new Story(name),
                            "task" => new Task(name),
                            "bug" => new Bug(name),
                        };
                        tasks.Add(result);
                    }
                }))
                .AddClose("Отмена")
                .Show(root);
        }

        private IWidget TaskToWidget(ITask task)
        {
            return new Expandable(
                new Label($"Тема: {task.Name}"),
                new StackContainer()
                    .Add(new Button($"Тема: {task.Name}").OnClick(() => OpenTask(task)))
                    .Add(new Label($"   Статус: {task.TaskStatus.RuString()}"))
                    .Add(new Label($"   Создана: {task.CreatedAt}"))
                    .AsIKeyHandler()
            );
        }

        private void OpenTask(ITask task)
        {
            throw new NotImplementedException();
        }

        public event Action Deleted;
    }
}
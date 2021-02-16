using System;
using Thuja;
using Thuja.Widgets;

namespace Projecto.Tui
{
    public static partial class OpenedExt
    {
        private class SubtasksContext
        {
            public Opened<IHaveSubtasks> Opened;
            public ListOf<ITask> Subtasks;

            public SubtasksContext(Opened<IHaveSubtasks> opened)
            {
                Opened = opened;
                Subtasks = new StackContainer().FromList(opened.Object.Subtasks, TaskToWidget);
            }

            internal void AddSubtask()
            {
                var radios = new RadioSetBuilder<ITaskKind>();
                foreach (var taskKind in Opened.Object.AllowedSubtasks)
                {
                    radios.Add(taskKind.Name, taskKind);
                }

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
                            var subtask = radios.Checked.Create(name);
                            Subtasks.Add(subtask);
                        }
                    }))
                    .AddClose("Отмена")
                    .Show(Opened.container);
            }

            private IWidget TaskToWidget(ITask task)
            {
                var title = task.Kind.Name;
                return new Expandable(
                    new Label($"{title}: {task.Name}"),
                    new StackContainer()
                        .Add(new Button($"{title}: {task.Name}").OnClick(() => OpenTask(task)))
                        .Add(new Label($"   Статус: {task.TaskStatus.RuString()}"))
                        .Add(new Label($"   Создана: {task.CreatedAt}"))
                        .AsIKeyHandler()
                );
            }

            private void OpenTask(ITask task)
            {
                var opened = new Opened<ITask>(Opened.container, task);
                var widget = opened.Setup();
                var popup = new Popup().Add(widget);
                opened.Closed.Value += () =>
                {
                    popup.Close();
                    Subtasks.Update(Subtasks.IndexOf(task));
                };
                opened.Deleted.Value += () => Subtasks.Remove(task);
                popup.Show(opened.root);
            }
        }

        private static void SetupSubtasks(this Opened<IHaveSubtasks> opened)
        {
            var context = new SubtasksContext(opened);
            var addButton = new Button("Добавить задачу")
                .OnClick(() => context.AddSubtask());
            opened.content
                .Add(addButton)
                .Add(context.Subtasks.Widget);
        }

        private static void SetupTask(this Opened<ITask> opened)
        {
            var task = opened.Object;

            opened.content
                .Add(new StackContainer(Orientation.Horizontal, 1)
                    .Add(new Label($"{task.Kind.Name}:"))
                    .Add(new InputField(task.Name)
                        .OnChanged(field => task.Name = field.Text.ToString())))
                .Add(new StackContainer(Orientation.Horizontal, 1)
                    .Add(new Label("Статус:"))
                    .Add(new Button(task.TaskStatus.RuString())
                        .OnClick(btn => opened.AskForStatus(task,
                            status => btn.Text = status.RuString()))))
                .Add(new StackContainer(Orientation.Horizontal, 1)
                    .Add(new Label("Создана:"))
                    .Add(new Label(task.CreatedAt.ToString())));
        }

        private static void AskForStatus(this Opened<ITask> opened, ITask task, Action<TaskStatus> callback)
        {
            var radios = new RadioSetBuilder<TaskStatus>()
                .Add(TaskStatus.Open.RuString(), TaskStatus.Open)
                .Add(TaskStatus.InProcess.RuString(), TaskStatus.InProcess)
                .Add(TaskStatus.Completed.RuString(), TaskStatus.Completed);
            radios.Check(task.TaskStatus);

            var popup = new Popup();
            radios.OnChecked += status =>
            {
                task.TaskStatus = status;
                popup.Close();
                callback(status);
            };

            popup
                .Add(new Label("Выбрать статус:"))
                .Add(radios.ToStack())
                .AddClose("отмена")
                .Show(opened.container);
        }
    }
}
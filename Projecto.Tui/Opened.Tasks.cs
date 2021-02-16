using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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

            public ImmutableHashSet<TaskStatus> Filter { get; set; } = TaskStatusExt.Statuses.ToImmutableHashSet();

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
                if (!Filter.Contains(task.TaskStatus))
                {
                    return new BaseContainer();
                }

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

            public string FilterString() => string.Join(", ", Filter.Select(x => x.RuString()));
        }

        private static void SetupSubtasks(this Opened<IHaveSubtasks> opened)
        {
            var context = new SubtasksContext(opened);
            var filterLabel = new Label(context.FilterString());

            opened.content
                .Add(new Button("Добавить задачу")
                    .OnClick(() => context.AddSubtask()))
                .Add(new StackContainer(Orientation.Horizontal, 1)
                    .Add(new Button("Фильтр:")
                        .OnClick(() => opened.AskForManyStatuses(context.Filter,
                            newFilter =>
                            {
                                context.Filter = newFilter;
                                filterLabel.Text = context.FilterString();
                                context.Subtasks.Update();
                            })))
                    .Add(filterLabel))
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
                        .OnClick(btn => opened.AskForStatus(task.TaskStatus,
                            status =>
                            {
                                task.TaskStatus = status;
                                btn.Text = status.RuString();
                            }))))
                .Add(new StackContainer(Orientation.Horizontal, 1)
                    .Add(new Label("Создана:"))
                    .Add(new Label(task.CreatedAt.ToString())));
        }

        private static void AskForStatus<T>(this Opened<T> opened, TaskStatus defaultStatus,
            Action<TaskStatus> callback)
        {
            var radios = new RadioSetBuilder<TaskStatus>();
            foreach (var status in TaskStatusExt.Statuses)
            {
                radios.Add(status.RuString(), status);
            }

            radios.Check(defaultStatus);

            var popup = new Popup();
            radios.OnChecked += status =>
            {
                popup.Close();
                callback(status);
            };

            popup
                .Add(new Label("Выбрать статус:"))
                .Add(radios.ToStack())
                .AddClose("отмена")
                .Show(opened.container);
        }

        private static void AskForManyStatuses<T>(this Opened<T> opened, ImmutableHashSet<TaskStatus> defaultStatus,
            Action<ImmutableHashSet<TaskStatus>> callback)
        {
            var choices = new StackContainer();
            var checkboxes = new Dictionary<TaskStatus, Checkbox>();
            foreach (var status in TaskStatusExt.Statuses)
            {
                var checkbox = new Checkbox(status.RuString(), defaultStatus.Contains(status));
                checkboxes[status] = checkbox;
                choices.Add(checkbox.Auto());
            }

            new Popup()
                .Add(new Label("Выбрать статусы:"))
                .Add(choices)
                .AddWith(popup => new StackContainer(Orientation.Horizontal, 3)
                    .Add(new Button("ОК")
                        .OnClick(() =>
                        {
                            var chosen = checkboxes
                                .Where(kv => kv.Value.Checked)
                                .Select(kv => kv.Key)
                                .ToImmutableHashSet();
                            callback(chosen);
                            popup.Close();
                        }))
                    .Add(new Button("Отмена").OnClick(popup.Close)))
                .Show(opened.container);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Thuja;
using Thuja.Widgets;

namespace Projecto.Tui
{
    public static partial class OpenedExt
    {
        /// <summary>
        /// Добавляет список подзадач.
        /// </summary>
        private static void SetupSubtasks(this Opened<IHaveSubtasks> opened)
        {
            var context = new SubtasksContext(opened);
            var filterLabel = new Label(context.FilterString());

            opened.content
                .Add(new Button("Добавить задачу")
                    .OnClick(() => context.AddSubtask()))
                .Add(new StackContainer(Orientation.Horizontal, 1)
                    .Add(new Button("Группировать и фильтровать:")
                        .OnClick(() => opened.AskForManyStatuses(context.Filter,
                            newFilter =>
                            {
                                context.Filter = newFilter;
                                filterLabel.Text = context.FilterString();
                                context.SortAndUpdate();
                            })))
                    .Add(filterLabel))
                .Add(context.Subtasks.Widget);
        }

        /// <summary>
        /// Добавляет виджеты, соответсвующие любой задаче. Не проверяет, есть ли исполнители, подзадачи и всё такое.
        /// </summary>
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

        /// <summary>
        /// Запрашивает у пользователя статус задачи.
        /// </summary>
        /// <param name="defaultStatus">Статус, который будет отмечен как выбранный при открыти окна.</param>
        /// <param name="callback">Функция, в которую передаётся выбранный статус, если пользователь его выбрал.</param>
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

        /// <summary>
        /// Полностью аналогично <see cref="AskForStatus{T}"/>, но позволяет отмечать несколько статусов одновременно.
        /// </summary>
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

        /// <summary>
        /// Вспомогательный класс, который хранит информацию о подзадачах.
        /// </summary>
        private class SubtasksContext
        {
            private readonly Opened<IHaveSubtasks> opened;

            /// <summary>
            /// Список виджетов, соответсвующий списку подзадач из opened.Object.Subtasks.
            /// </summary>
            public readonly ListOf<ITask> Subtasks;

            public SubtasksContext(Opened<IHaveSubtasks> opened)
            {
                this.opened = opened;
                Subtasks = new StackContainer(maxVisibleCount: 10).FromList(opened.Object.Subtasks, TaskToWidget);
            }

            /// <summary>
            /// Сортирует задачи по статусу и обновляет Subtasks.
            /// </summary>
            public void SortAndUpdate()
            {
                // https://stackoverflow.com/a/5037815
                var list = Subtasks.GetInner();
                var comparer = Comparer<ITask>.Create((l, r) => l.TaskStatus.CompareTo(r.TaskStatus));
                ArrayList.Adapter((IList) list).Sort(comparer);
                Subtasks.Update();
            }

            /// <summary>
            /// Текущий фильтр подзадач.
            /// </summary>
            public ImmutableHashSet<TaskStatus> Filter { get; set; } = TaskStatusExt.Statuses.ToImmutableHashSet();

            /// <summary>
            /// Добавляет новую подзадачу. Всю необходимую информацию запрашивает у пользователя, открывая попап.
            /// </summary>
            internal void AddSubtask()
            {
                var radios = new RadioSetBuilder<ITaskKind>();
                foreach (var taskKind in opened.Object.AllowedSubtasks)
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
                    .Show(opened.container);
            }

            /// <summary>
            /// Преобразовывает задачу в красиывый виджет.
            /// </summary>
            private IWidget TaskToWidget(ITask task)
            {
                if (!Filter.Contains(task.TaskStatus))
                {
                    return new BaseContainer();
                }

                var color = task.TaskStatus switch
                {
                    TaskStatus.Completed => MyColor.Green,
                    TaskStatus.InProcess => MyColor.Yellow,
                    TaskStatus.Open => MyColor.Cyan,
                    _ => MyColor.Default
                };
                var label = new Label(task.Name)
                {
                    CurrentStyle = new Style(color, MyColor.Default)
                };

                var title = task.Kind.Name;
                return new Expandable(
                    new StackContainer(Orientation.Horizontal, 1)
                        .Add(new Label(title))
                        .Add(label),
                    new StackContainer()
                        .Add(new StackContainer(Orientation.Horizontal, 1)
                            .Add(new Button(title).OnClick(() => OpenTask(task)))
                            .Add(label))
                        .Add(new Label($"   Статус: {task.TaskStatus.RuString()}"))
                        .Add(new Label($"   Создана: {task.CreatedAt}"))
                        .AsIKeyHandler()
                );
            }

            /// <summary>
            /// Открывает подробную информацию о задаче.
            /// </summary>
            private void OpenTask(ITask task)
            {
                var newOpened = new Opened<ITask>(task);
                var widget = newOpened.Setup();
                var popup = new Popup().Add(widget);
                newOpened.Closed.Value += () =>
                {
                    popup.Close();
                    Subtasks.Update(Subtasks.IndexOf(task));
                };
                newOpened.Deleted.Value += () => Subtasks.Remove(task);
                popup.Show(opened.container);
            }

            /// <summary>
            /// Преобразовывает текущий фильтр подзадач в читабельную строку.
            /// </summary>
            public string FilterString()
            {
                return string.Join(", ", Filter.Select(x => x.RuString()));
            }
        }
    }
}
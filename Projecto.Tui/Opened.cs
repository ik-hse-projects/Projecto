using System;
using System.Linq;
using Thuja;
using Thuja.Widgets;

namespace Projecto.Tui
{
    /// <summary>
    ///     Простейшая обертка над любым типом.
    ///     Очень полезна, когда требуется копировать ссылку на значимый тип. Например, делегаты.
    /// </summary>
    class Box<T>
    {
        public T Value;

        public Box(T value)
        {
            Value = value;
        }
    }

    public class Opened<T>
    {
        internal readonly T Object;
        internal BaseContainer root { get; private init; }
        internal BaseContainer container { get; private init; }
        internal StackContainer content { get; private init; }

        internal Box<Action?> Closed { get; private init; } = new(null);
        internal Box<Action?> Deleted { get; private init; } = new(null);
        internal Box<Action?> Modified { get; private init; } = new(null);

        public Opened(BaseContainer root, T obj) : this(obj)
        {
            this.root = root;
            content = new StackContainer();
            container = new BaseContainer().Add(content);
            Deleted.Value += () => Closed.Value?.Invoke();
        }

        private Opened(T obj)
        {
            Object = obj;
        }

        public Opened<T2>? Cast<T2>()
        {
            if (Object is not T2 casted)
            {
                return null;
            }

            return new Opened<T2>(casted)
            {
                root = root,
                content = content,
                container = container,
                Closed = Closed,
                Deleted = Deleted,
                Modified = Modified,
            };
        }
    }

    public static class OpenedExt
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
                opened.Closed.Value += () => popup.Close();
                opened.Deleted.Value += () => Subtasks.Remove(task);
                popup.Show(opened.root);
            }
        }

        private class ExecutorsContext
        {
            public Opened<IHaveManyExecutors> Opened;
            public ListOf<IUser> Executors;

            public ExecutorsContext(Opened<IHaveManyExecutors> opened)
            {
                Opened = opened;
                Executors = new StackContainer().FromList(opened.Object.Executors, ExecutorToWidget);
            }

            private IWidget ExecutorToWidget(IUser executor)
            {
                return new Button(executor.Name)
                    .OnClick(() => AskForUser(Opened.container, user =>
                    {
                        if (user == null)
                        {
                            Executors.Remove(executor);
                        }
                        else
                        {
                            var idx = Executors.IndexOf(executor);
                            Executors[idx] = user;
                        }
                    }));
            }
        }

        public static IWidget Setup<T>(this Opened<T> opened)
        {
            if (opened.Cast<ITask>() is { } task)
                task.SetupTask();
            if (opened.Cast<Project>() is { } project)
                project.SetupProject();
            if (opened.Cast<IUser>() is { } user)
                user.SetupUser();
            opened.SetupCloseDelete();
            if (opened.Cast<IHaveSingleExecutor>() is { } singleExecutor)
                singleExecutor.SetupSingleExecutor();
            if (opened.Cast<IHaveManyExecutors>() is { } manyExecutors)
                manyExecutors.SetupManyExecutors();
            if (opened.Cast<IHaveSubtasks>() is { } subtasks)
                subtasks.SetupSubtasks();
            opened.Modified.Value?.Invoke();
            return opened.container;
        }

        private static void SetupUser(this Opened<IUser> opened)
        {
            new Popup()
                .Add(new StackContainer(Orientation.Horizontal)
                    .Add(new Label("Имя:"))
                    .Add(new InputField(opened.Object.Name)))
                .Add(new StackContainer());
        }

        private static void SetupManyExecutors(this Opened<IHaveManyExecutors> opened)
        {
            var context = new ExecutorsContext(opened);
            opened.content
                .Add(new Label(""))
                .Add(new Label("Исполнители:"))
                .Add(new Button("Добавить").OnClick(() => AskForUser(opened.container, user =>
                {
                    if (user != null)
                    {
                        context.Executors.Add(user);
                    }
                })))
                .Add(context.Executors.Widget);
        }

        private static void SetupSingleExecutor(this Opened<IHaveSingleExecutor> opened)
        {
            var button = new Button("...")
                .OnClick(() => AskForUser(opened.container, u =>
                {
                    opened.Object.Executor = u;
                    opened.Modified.Value?.Invoke();
                }));
            opened.Modified.Value += () => button.Text = opened.Object.Executor?.Name ?? "<не назначен>";
            opened.content.Add(new StackContainer(Orientation.Horizontal, 1)
                .Add(new Label("Исполнитель:"))
                .Add(button));
        }

        private static void AskForUser(BaseContainer root, Action<IUser?> callback)
        {
            new Popup()
                .AddWith(popup => new Button("никто").OnClick(() =>
                {
                    callback(null);
                    popup.Close();
                }))
                .Add(new Label(""))
                .AddWith(popup => new FuzzySearch<IUser>(Program.Instance.Users, u => u.Name)
                    .OnChosen(search =>
                    {
                        callback(search.Choice);
                        popup.Close();
                    }))
                .Add(new Label(""))
                .AddClose("Отмена")
                .Show(root);
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

        private static void SetupTask<T>(this Opened<T> opened) where T : ITask
        {
            var task = opened.Object;
            opened.content
                .Add(new Button($"{task.Kind.Name}: {task.Name}"))
                .Add(new Button($"   Статус: {task.TaskStatus.RuString()}"))
                .Add(new Button($"   Создана: {task.CreatedAt}"));
        }

        private static void SetupProject(this Opened<Project> opened)
        {
            opened.content
                .Add(new Label($"Проект: {opened.Object.Name}"));
        }

        private static void SetupCloseDelete<T>(this Opened<T> opened)
        {
            opened.content
                .Add(new StackContainer(Orientation.Horizontal, 3)
                    .Add(new Button("Закрыть").OnClick(() => opened.Closed.Value?.Invoke()))
                    .Add(new Button("Удалить!").OnClick(() => opened.Deleted.Value?.Invoke())));
        }
    }
}
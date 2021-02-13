using System;
using Thuja;
using Thuja.Widgets;

namespace Projecto.Tui
{
    public class Opened<T>
    {
        internal T Object;
        internal BaseContainer root { get; init; }
        internal BaseContainer container { get; init; }
        internal StackContainer content { get; init; }

        internal Action? Closed;
        internal Action? Deleted;

        public Opened(BaseContainer root, T obj) : this(obj)
        {
            this.root = root;
            content = new StackContainer();
            container = new BaseContainer().Add(content);
            Deleted += () => Closed?.Invoke();
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
                Deleted = Deleted
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
                Subtasks = new StackContainer().FromList(opened.Object.Subtasks,
                    task => TaskToWidget(this, task));
            }
        }

        public static IWidget Setup<T>(this Opened<T> opened)
        {
            if (opened.Cast<ITask>() is { } task)
                task.SetupTask();
            if (opened.Cast<Project>() is { } project)
                project.SetupProject();
            opened.SetupCloseDelete();
            if (opened.Cast<IHaveSubtasks>() is { } subtasks)
                subtasks.SetupSubtasks();
            return opened.container;
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

        private static void AddSubtask(this SubtasksContext context)
        {
            var radios = new RadioSetBuilder<ITaskKind>();
            foreach (var taskKind in context.Opened.Object.AllowedSubtasks)
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
                        context.Subtasks.Add(subtask);
                    }
                }))
                .AddClose("Отмена")
                .Show(context.Opened.root);
        }

        private static IWidget TaskToWidget(SubtasksContext context, ITask task)
        {
            var title = task.Kind.Name;
            return new Expandable(
                new Label($"{title}: {task.Name}"),
                new StackContainer()
                    .Add(new Button($"{title}: {task.Name}").OnClick(() => OpenTask(context, task)))
                    .Add(new Label($"   Статус: {task.TaskStatus.RuString()}"))
                    .Add(new Label($"   Создана: {task.CreatedAt}"))
                    .AsIKeyHandler()
            );
        }

        private static void OpenTask(SubtasksContext context, ITask task)
        {
            var opened = new Opened<ITask>(context.Opened.container, task);
            var widget = opened.Setup();
            var popup = new Popup(x: 2, y: 2).Add(widget);
            opened.Closed += () => popup.Close();
            opened.Deleted += () => context.Subtasks.Remove(task);
            popup.Show(opened.root);
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
                    .Add(new Button("Закрыть").OnClick(() => opened.Closed?.Invoke()))
                    .Add(new Button("Удалить!").OnClick(() => opened.Deleted?.Invoke())));
        }
    }
}
using Thuja;
using Thuja.Widgets;

namespace Projecto.Tui
{
    public static partial class OpenedExt
    {
        /// <summary>
        /// Перебирет все известные интерфейсы и добавляет необходимые виджеты, если T их реализует (интерфейсы).
        /// </summary>
        /// <returns>Виджет, который содержит все добавленные виджеты.</returns>
        // Здесь вместо T мог бы быть просто `Opened<object>`, но это бы потребовало дополнительного
        // вызова Cast, что не очень краисво, приятно и удобно. См. описание Opened.Cast, почему так приходится делать.
        public static IWidget Setup<T>(this Opened<T> opened)
        {
            if (opened.Cast<ITask>() is { } task)
            {
                task.SetupTask();
            }

            if (opened.Cast<Project>() is { } project)
            {
                project.SetupProject();
            }

            if (opened.Cast<User>() is { } user)
            {
                user.SetupUser();
            }

            opened.SetupCloseDelete();
            if (opened.Cast<IHaveSingleExecutor>() is { } singleExecutor)
            {
                singleExecutor.SetupSingleExecutor();
            }

            if (opened.Cast<IHaveManyExecutors>() is { } manyExecutors)
            {
                manyExecutors.SetupManyExecutors();
            }

            if (opened.Cast<IHaveSubtasks>() is { } subtasks)
            {
                subtasks.SetupSubtasks();
            }

            return opened.container;
        }

        /// <summary>
        /// Добавляет виджеты, соответсвующие пользователю.
        /// </summary>
        private static void SetupUser(this Opened<User> opened)
        {
            opened.content
                .Add(new StackContainer(Orientation.Horizontal)
                    .Add(new Label("Имя:"))
                    .Add(new InputField(opened.Object.Name)
                        .OnChanged(field => opened.Object.Name = field.Text.ToString())))
                .Add(new StackContainer());
        }

        /// <summary>
        /// Добавляет виджеты, соответсвующие проекту.
        /// </summary>
        private static void SetupProject(this Opened<Project> opened)
        {
            opened.content
                .Add(new StackContainer(Orientation.Horizontal, 1)
                    .Add(new Label("Проект: "))
                    .Add(new InputField(opened.Object.Name)
                        .OnChanged(field => opened.Object.Name = field.Text.ToString())));
        }

        /// <summary>
        /// Добавляет кнопки закрытия и удаления. 
        /// </summary>
        private static void SetupCloseDelete<T>(this Opened<T> opened)
        {
            opened.content
                .Add(new StackContainer(Orientation.Horizontal, 3)
                    .Add(new Button("Закрыть").OnClick(() => opened.Closed.Value?.Invoke()))
                    .Add(new Button("Удалить!").OnClick(() => opened.Deleted.Value?.Invoke())));
        }
    }
}
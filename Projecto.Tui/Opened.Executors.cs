using System;
using Thuja;
using Thuja.Widgets;

namespace Projecto.Tui
{
    public static partial class OpenedExt
    {
        /// <summary>
        /// Добавляет список большого количества исполнителей (не один).
        /// </summary>
        private static void SetupManyExecutors(this Opened<IHaveManyExecutors> opened)
        {
            var context = new ExecutorsContext(opened);
            opened.content
                .Add(new Label(""))
                .Add(new Label("Исполнители:"))
                .Add(new Button("Добавить").OnClick(() => opened.AskForUser(user =>
                {
                    if (user != null)
                    {
                        context.Executors.Add(user);
                    }
                })))
                .Add(context.Executors.Widget);
        }

        /// <summary>
        /// Добавляет кнопку для выбора одного исполнителя.
        /// </summary>
        private static void SetupSingleExecutor(this Opened<IHaveSingleExecutor> opened)
        {
            var button = new Button(opened.Object.Executor?.Name ?? "<не назначен>")
                .OnClick(btn => opened.AskForUser(u =>
                {
                    opened.Object.Executor = u;
                    btn.Text = opened.Object.Executor?.Name ?? "<не назначен>";
                }));
            opened.content.Add(new StackContainer(Orientation.Horizontal, 1)
                .Add(new Label("Исполнитель:"))
                .Add(button));
        }
        
        /// <summary>
        /// Открывает попап и предлагает человеку выбрать пользователя.
        /// </summary>
        /// <param name="callback">Будет вызвана после получения ответа от пользователя.</param>
        private static void AskForUser<T>(this Opened<T> opened, Action<User?> callback)
        {
            new Popup()
                .AddWith(popup => new Button("никто").OnClick(() =>
                {
                    callback(null);
                    popup.Close();
                }))
                .Add(new Label(""))
                .AddWith(popup => new FuzzySearch<User>(Program.Instance.Users, u => u.Name)
                    .OnChosen(search =>
                    {
                        callback(search.Choice);
                        popup.Close();
                    }))
                .Add(new Label(""))
                .AddClose("Отмена")
                .Show(opened.container);
        }

        /// <summary>
        /// Вспомогательный класс, который хранит информацию о многих исполнителях и работает с ней.
        /// </summary>
        private class ExecutorsContext
        {
            /// <summary>
            /// Список виджетов, соответсвующий списку исполнителей из opened.Object.Executors.
            /// </summary>
            public readonly ListOf<User> Executors;

            private readonly Opened<IHaveManyExecutors> opened;

            public ExecutorsContext(Opened<IHaveManyExecutors> opened)
            {
                this.opened = opened;
                Executors = new StackContainer(maxVisibleCount: 10).FromList(opened.Object.Executors, ExecutorToWidget);
            }

            /// <summary>
            /// Конвертирует исполнителя из списка в виджет.
            /// </summary>
            private IWidget ExecutorToWidget(User executor)
            {
                return new Button(executor.Name)
                    .OnClick(() => opened.AskForUser(user =>
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
    }
}
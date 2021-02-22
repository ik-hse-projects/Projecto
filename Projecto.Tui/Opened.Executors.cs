using System;
using Thuja;
using Thuja.Widgets;

namespace Projecto.Tui
{
    public static partial class OpenedExt
    {
        private static void SetupManyExecutors(this Opened<IHaveManyExecutors> opened)
        {
            var context = new ExecutorsContext(opened);
            opened.content
                .Add(new Label(""))
                .Add(new Label("Исполнители:"))
                .Add(new Button("Добавить").OnClick(() => ExecutorsContext.AskForUser(opened.container, user =>
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
            var button = new Button(opened.Object.Executor?.Name ?? "<не назначен>")
                .OnClick(btn => ExecutorsContext.AskForUser(opened.container, u =>
                {
                    opened.Object.Executor = u;
                    btn.Text = opened.Object.Executor?.Name ?? "<не назначен>";
                }));
            opened.content.Add(new StackContainer(Orientation.Horizontal, 1)
                .Add(new Label("Исполнитель:"))
                .Add(button));
        }

        private class ExecutorsContext
        {
            public readonly ListOf<User> Executors;
            public readonly Opened<IHaveManyExecutors> Opened;

            public ExecutorsContext(Opened<IHaveManyExecutors> opened)
            {
                Opened = opened;
                Executors = new StackContainer(maxVisibleCount: 10).FromList(opened.Object.Executors, ExecutorToWidget);
            }

            private IWidget ExecutorToWidget(User executor)
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

            public static void AskForUser(BaseContainer root, Action<User?> callback)
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
                    .Show(root);
            }
        }
    }
}
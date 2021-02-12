using System;
using Thuja;
using Thuja.Widgets;

namespace Projecto.Tui
{
    class Program
    {
        static void Main(string[] args)
        {
            var content = new StackContainer()
                .AddFocused(new Button("1"))
                .Add(new Button("2"));
            var tabs = new Tabs()
                .Add("F1|Справка", content, out var help)
                .AndFocus()
                .Add("F2|Пользователи", new Label("Users"), out var users)
                .Add("F3|Проекты", new Label("Projects"), out var projects);
            tabs.AsIKeyHandler()
                .Add(new KeySelector(ConsoleKey.F1), () => tabs.Focus(help))
                .Add(new KeySelector(ConsoleKey.F2), () => tabs.Focus(users))
                .Add(new KeySelector(ConsoleKey.F3), () => tabs.Focus(projects));
            new MainLoop(tabs).Start();
        }
    }
}
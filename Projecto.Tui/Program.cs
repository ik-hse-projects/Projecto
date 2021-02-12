using System;
using System.Collections.Generic;
using Thuja;
using Thuja.Widgets;

namespace Projecto.Tui
{
    class Program
    {
        private const string HELP =
            "Добро пожаловать в Projecto!\n" +
            "Эта программа самая консольная среди проектоуправляющих и самая проектоуправляющая среди консольных\n" +
            "\n" +
            "Некоторые вещи, которые могут быть не совсем очевидны:\n" +
            "1. Переключаться между вкладками можно нажимая F1-F4 или стрелками влево-вправо, когда это возможно.\n" +
            "2. В Меню можно корректно выйти из программы.";

        private readonly BaseContainer container = new BaseContainer();

        static void Main()
        {
            new Program().Start();
        }

        void Start()
        {
            var mainLoop = new MainLoop(container);

            var projects = new StackContainer().ListOf<Project>(project => new Button(project.Name));
            var addProjectButton =
                new Button("Создать").OnClick(() => AskForProject(project => projects.Insert(0, project)));
            var users = new StackContainer().ListOf<User>(user => new Button(user.Name));
            var addUserButton = new Button("Добавить").OnClick(() => AskForUser(user => users.Insert(0, user)));
            var tabs = new Tabs()
                .Add("F1|Справка", new MultilineLabel(HELP, 78), out var help)
                .AndFocus()
                .Add("F2|Пользователи", new StackContainer(margin: 1)
                    .Add(addUserButton)
                    .Add(users.Widget), out var usersTab)
                .Add("F3|Проекты", new StackContainer(margin: 1)
                    .Add(addProjectButton)
                    .Add(projects.Widget), out var projectsTab)
                .Add("F4|Меню", new StackContainer()
                    .Add(new Button("Сохранить как"))
                    .Add(new Button("Заугрузить откуда"))
                    .Add(new Button("Выйти").OnClick(() => mainLoop.OnStop = () => { })), out var menuTab);
            tabs.AsIKeyHandler()
                .Add(new KeySelector(ConsoleKey.F1), () => tabs.Focus(help))
                .Add(new KeySelector(ConsoleKey.F2), () => tabs.Focus(usersTab))
                .Add(new KeySelector(ConsoleKey.F3), () => tabs.Focus(projectsTab))
                .Add(new KeySelector(ConsoleKey.F4), () => tabs.Focus(menuTab));
            container.Add(tabs);

            mainLoop.Start();
        }

        private void AskForUser(Action<User> callback)
        {
            var input = new InputField();
            new Popup()
                .Add(new Label("Добавление пользователя"))
                .Add(new StackContainer(Orientation.Horizontal, 1)
                    .Add(new Label("Имя:"))
                    .Add(input))
                .AndFocus()
                .AddWith(popup => new StackContainer(Orientation.Horizontal, 3)
                    .Add(new Button("Создать!").OnClick(() =>
                    {
                        var user = new User(input.Text.ToString());
                        callback(user);
                        popup.Close();
                    }))
                    .Add(new Button("Отмена").OnClick(popup.Close))
                )
                .Show(container);
        }

        private void AskForProject(Action<Project> callback)
        {
            new Popup()
                .Add(new Label("Создание проекта"))
                .AddClose("Отмена")
                .Show(container);
        }
    }
}
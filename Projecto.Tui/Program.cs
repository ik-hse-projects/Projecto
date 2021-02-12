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
            "2. В Меню можно корректно выйти из программы.\n" +
            "3. Это не приложение на WinForms, мышка здесь не пригодится.";

        private readonly MainLoop mainLoop;
        private readonly BaseContainer container = new BaseContainer();
        private readonly Tabs tabs;
        private readonly TabPage<IWidget> projectsTab;

        private readonly ListOf<Project> projects;
        private readonly ListOf<User> users;

        static void Main()
        {
            new Program().mainLoop.Start();
        }

        Program()
        {
            mainLoop = new MainLoop(container);
            projects = new StackContainer().ListOf<Project>(project => new Button(project.Name)
                .OnClick(() => OpenProject(project)));
            users = new StackContainer().ListOf<User>(user => new Button(user.Name)
                .OnClick(() => users!.Remove(user)));
            tabs = new Tabs()
                .Add("F1|Справка", new MultilineLabel(HELP, 78), out var helpTab)
                .AndFocus()
                .Add("F2|Пользователи", new StackContainer(margin: 1)
                    .Add(new Button("Добавить").OnClick(
                        () => AskForName("Добавление пользователя",
                            name => users.Insert(0, new User(name)))))
                    .Add(new Label(""))
                    .Add(users.Widget), out var usersTab)
                .Add("F3|Проекты", new StackContainer(margin: 1)
                    .Add(new Button("Создать").OnClick(
                        () => AskForName("Создание проекта",
                            name => projects.Insert(0, new Project(name)))))
                    .Add(projects.Widget), out projectsTab)
                .Add("F4|Меню", new StackContainer()
                    .Add(new Button("Сохранить как"))
                    .Add(new Button("Заугрузить откуда"))
                    .Add(new Button("Выйти").OnClick(
                        () => mainLoop.OnStop = () => { })), out var menuTab);
            tabs.AsIKeyHandler()
                .Add(new KeySelector(ConsoleKey.F1), () => tabs.Focus(helpTab))
                .Add(new KeySelector(ConsoleKey.F2), () => tabs.Focus(usersTab))
                .Add(new KeySelector(ConsoleKey.F3), () => tabs.Focus(projectsTab))
                .Add(new KeySelector(ConsoleKey.F4), () => tabs.Focus(menuTab));
            container.Add(tabs);
        }

        private TabPage? OpenedProject;

        private void OpenProject(Project? project)
        {
            if (OpenedProject != null)
            {
                tabs.Remove(OpenedProject);
            }

            if (project == null)
            {
                OpenedProject = null;
                tabs.Focus(projectsTab);
            }
            else
            {
                var opened = new OpenedProject(container, project);
                opened.Deleted += () =>
                {
                    projects.Remove(project);
                    OpenProject(null);
                };
                OpenedProject = tabs.Insert(3, $"[{project.Name}]", opened.Widget);
                tabs.Focus(OpenedProject);
            }
        }

        private void AskForName(string title, Action<string> callback)
        {
            var name = new InputField();
            new Popup()
                .Add(new Label(title))
                .Add(new StackContainer(Orientation.Horizontal, 1)
                    .Add(new Label("Имя:"))
                    .Add(name))
                .AndFocus()
                .AddWith(popup => new StackContainer(Orientation.Horizontal, 3)
                    .Add(new Button("Создать!").OnClick(() =>
                    {
                        callback(name.Text.ToString());
                        popup.Close();
                    }))
                    .Add(new Button("Отмена").OnClick(popup.Close))
                )
                .Show(container);
        }
    }
}
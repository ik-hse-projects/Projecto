using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Thuja;
using Thuja.Video;
using Thuja.Widgets;

namespace Projecto.Tui
{
    class Program
    {
        public static readonly Program Instance = new Program();

        private const string HELP =
            "Добро пожаловать в Projecto!\n" +
            "Эта программа самая консольная среди проектоуправляющих и самая проектоуправляющая среди консольных\n" +
            "\n" +
            "Некоторые вещи, которые могут быть не совсем очевидны:\n" +
            "1. Переключаться между вкладками можно нажимая F1-F4 или стрелками влево-вправо, когда это возможно.\n" +
            "2. В Меню можно корректно выйти из программы (сохранив все проекты).\n" +
            "3. Списки умеют прокручиваться. " +
            "Если вы добавили 20 проектов и видите только часть, то надо просто пройтись по списку и всё будет.\n" +
            "4. Если вы переименовываете пользователя, то " +
            "он будет переименован везде: и в списке пользователей, и во всех проектах.\n" +
            "5. Это не приложение на WinForms, мышку можно отложить в сторону.";

        private readonly MainLoop mainLoop;
        private readonly BaseContainer container = new BaseContainer();
        private readonly Tabs tabs;
        private readonly TabPage<IWidget> projectsTab;

        private readonly State State = LoadState();

        public readonly ListOf<Project> Projects;
        public readonly ListOf<IUser> Users;

        static void Main()
        {
            Instance.mainLoop.Start();
        }

        private Program()
        {
            mainLoop = new MainLoop(container);
            Projects = new StackContainer(maxVisibleCount: 15).FromList(State.Projects,
                project => new Button(project.Name).OnClick(() => OpenProject(project)));
            Users = new StackContainer(maxVisibleCount: 15).FromList(State.Users, UserToWidget);
            tabs = new Tabs()
                .Add("F1|Справка", new MultilineLabel(HELP, 78), out var helpTab)
                .AndFocus()
                .Add("F2|Пользователи", new StackContainer(margin: 1)
                        .Add(new Button("Добавить").OnClick(
                            () => AskForName("Добавление пользователя",
                                name => Users.Insert(0, new User(name)))))
                        .Add(Users.Widget),
                    out var usersTab)
                .Add("F3|Проекты", new StackContainer(margin: 1)
                        .Add(new Button("Создать").OnClick(
                            () => AskForName("Создание проекта",
                                name => Projects.Insert(0, new Project(name)))))
                        .Add(Projects.Widget),
                    out projectsTab)
                .Add("F4|Меню", new StackContainer()
                        .Add(new StackContainer(Orientation.Horizontal, 3)
                            .Add(new Button("Выйти").OnClick(() => mainLoop.OnStop = SaveState)))
                        .Add(new Label(""))
                        .Add(Logo()),
                    out var menuTab);

            // Когда меняется вкладка обновляем текущий проект, т.к. его имя могло измениться.
            projectsTab.Focused += () =>
            {
                if (CurrentProject is var (_, project))
                {
                    Projects.Update(Projects.IndexOf(project));
                }
            };

            tabs.AsIKeyHandler()
                .Add(new KeySelector(ConsoleKey.F1), () => tabs.Focus(helpTab))
                .Add(new KeySelector(ConsoleKey.F2), () => tabs.Focus(usersTab))
                .Add(new KeySelector(ConsoleKey.F3), () => tabs.Focus(projectsTab))
                .Add(new KeySelector(ConsoleKey.F4), () => tabs.Focus(menuTab));
            container.Add(tabs);
        }

        private const string STATE_JSON = "state.json";

        private void SaveState()
        {
            try
            {
                var state = State.Serialize();
                File.WriteAllText(STATE_JSON, state);
            }
            catch (Exception e)
            {
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
            }
        }

        private static State LoadState()
        {
            try
            {
                var state = File.ReadAllText(STATE_JSON);
                return State.Deserialize(state);
            }
            catch (Exception e)
            {
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
                return new State();
            }
        }

        private static IWidget Logo()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = assembly.GetManifestResourceNames()
                    .Single(str => str.EndsWith("logo.br"));
                var stream = assembly.GetManifestResourceStream(resourceName);
                return new VideoPlayer(stream);
            }
            catch (Exception)
            {
                Debugger.Break();
                return new BaseContainer();
            }
        }

        private IWidget UserToWidget(IUser user)
        {
            return new Button(user.Name).OnClick(() =>
            {
                var popup = new Popup();
                var opened = new Opened<IUser>(popup.Container, user);
                opened.Deleted.Value += () => Users.Remove(user);
                opened.Closed.Value += () =>
                {
                    popup.Close();
                    Users.Update(Users.IndexOf(user));
                };
                popup.Add(opened.Setup());
                popup.Show(container);
            });
        }

        private (TabPage tab, Project project)? CurrentProject;

        private void OpenProject(Project? project)
        {
            if (CurrentProject != null)
            {
                tabs.Remove(CurrentProject.Value.tab);
            }

            if (project == null)
            {
                CurrentProject = null;
                tabs.Focus(projectsTab);
            }
            else
            {
                var opened = new Opened<Project>(container, project);
                opened.Deleted.Value += () => Projects.Remove(project);
                opened.Closed.Value += () =>
                {
                    Projects.Update(Projects.IndexOf(project));
                    OpenProject(null);
                };
                var widget = opened.Setup();
                var tab = tabs.Insert(3, $"[{project.Name}]", widget);
                CurrentProject = (tab, project);
                tabs.Focus(tab);
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
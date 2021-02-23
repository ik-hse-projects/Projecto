using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Thuja;
using Thuja.Video;
using Thuja.Widgets;

namespace Projecto.Tui
{
    internal class Program
    {
        /// <summary>
        /// Максимальная ширина разных полей ввода.
        /// Иначе интерфейс перестаёт влезать на древние терминалы 80x25 символов.
        /// </summary>
        internal const int MaxWidth = 60; 
        
        /// <summary>
        /// Справка по программе, отображается на первом экране.
        /// </summary>
        private static readonly string Help =
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
            $"5. Максимальная длина полей ввода ограничена {MaxWidth} символами. Это не должно помешать." +
            "6. Это не приложение на WinForms, мышку можно отложить в сторону.";

        /// <summary>
        /// Путь к файлу, в котором сохраняется состояние.
        /// </summary>
        private const string STATE_JSON = "state.json";

        /// <summary>
        /// Класс Program — Singleton. И вот его единственный экземпляр:
        /// </summary>
        public static readonly Program Instance = new();

        /// <summary>
        /// Контейнер, в котором происходит вся отрисовка.
        /// </summary>
        private readonly BaseContainer container = new();

        private readonly MainLoop mainLoop;

        /// <summary>
        /// Список проектов в интерфейсе.
        /// </summary>
        private readonly ListOf<Project> projects;

        private readonly TabPage<IWidget> projectsTab;

        private readonly State state = LoadState();
        private readonly Tabs tabs;
        public readonly ListOf<User> Users;

        private (TabPage tab, Project project)? currentProject;

        private Program()
        {
            mainLoop = new MainLoop(container);
            projects = new StackContainer(maxVisibleCount: 15).FromList(state.Projects,
                project => new Button(project.Name).OnClick(() => OpenProject(project)));
            Users = new StackContainer(maxVisibleCount: 15).FromList(state.Users, UserToWidget);
            tabs = new Tabs()
                .Add("F1|Справка", new MultilineLabel(Help, 78), out var helpTab)
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
                                name => projects.Insert(0, new Project(name)))))
                        .Add(projects.Widget),
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
                if (currentProject is var (_, project))
                {
                    projects.Update(projects.IndexOf(project));
                }
            };

            tabs.AsIKeyHandler()
                .Add(new KeySelector(ConsoleKey.F1), () => tabs.Focus(helpTab))
                .Add(new KeySelector(ConsoleKey.F2), () => tabs.Focus(usersTab))
                .Add(new KeySelector(ConsoleKey.F3), () => tabs.Focus(projectsTab))
                .Add(new KeySelector(ConsoleKey.F4), () => tabs.Focus(menuTab));
            container.Add(tabs);
        }

        /// <summary>
        /// Точка входа, которая запускает mainLoop.
        /// </summary>
        private static void Main()
        {
            Instance.mainLoop.Start();
        }

        /// <summary>
        /// Сохраняет состояние в файл.
        /// </summary>
        private void SaveState()
        {
            try
            {
                var saved = this.state.Serialize();
                File.WriteAllText(STATE_JSON, saved);
            }
            catch (Exception e)
            {
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
            }
        }

        /// <summary>
        /// Загружает состояние из файла.
        /// </summary>
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

        /// <summary>
        /// Загружает логотип из embedded resources.
        /// </summary>
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

        /// <summary>
        /// Преобразовывает пользователя в виджет для списка.
        /// </summary>
        private IWidget UserToWidget(User user)
        {
            return new Button(user.Name) {MaxWidth = MaxWidth}.OnClick(() =>
            {
                var popup = new Popup();
                var opened = new Opened<User>(user);
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

        /// <summary>
        /// Открывает проект, закрывая предыдущий при необходимости.
        /// </summary>
        private void OpenProject(Project? project)
        {
            if (currentProject != null)
            {
                tabs.Remove(currentProject.Value.tab);
            }

            if (project == null)
            {
                currentProject = null;
                tabs.Focus(projectsTab);
            }
            else
            {
                var opened = new Opened<Project>(project);
                opened.Deleted.Value += () => projects.Remove(project);
                opened.Closed.Value += () =>
                {
                    projects.Update(projects.IndexOf(project));
                    OpenProject(null);
                };
                var widget = opened.Setup();
                var tab = tabs.Insert(3, $"[{project.Name}]", widget);
                currentProject = (tab, project);
                tabs.Focus(tab);
            }
        }

        /// <summary>
        /// Спршивает у пользователя имя чего-либо.
        /// </summary>
        /// <param name="title">Вопрос к пользователю, чтобы он понимал, чьё имя нужно вводить.</param>
        /// <param name="callback">Функция вызовется, когда пользователб всё введёт и нажмет кнопку.</param>
        private void AskForName(string title, Action<string> callback)
        {
            var name = new InputField {MaxLength = MaxWidth};
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
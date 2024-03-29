namespace Projecto
{
    /// <summary>
    /// Иногда бывают классы, которые не имеет смысл создавать много.
    /// Для них удобно иметь единственный статический экземпляр.
    /// Этот абстрактный класс позваоляет делать это легко и приятно.
    ///
    /// Используется, например, так: <c>EpicTaskKind kind = EpicTaskKind.Instance;</c>
    ///
    /// Важное замечание, что этот конкретный класс требует существование публичного конструктора без параметров.
    /// Соответсвенно невозможно гарантировать, что никто не создаст ещё один экземпляр класса.
    /// В какой-то степени это отличается от "традиционного" Singleton, который создаётся ровно один раз.
    /// </summary>
    /// <typeparam name="TSelf">
    /// Тип класса, который будет создан.
    /// В целом ожидается, что это будет тот же класс, который наследуется от этого, но это не обязательно.
    ///
    /// Например, <c>class MyClass: Singleton&lt;MyClass&gt; { ...</c>
    /// </typeparam>
    public abstract class Singleton<TSelf> where TSelf : new()
    {
        /// <summary>
        /// Экземпляр <see cref="TSelf"/>.
        /// </summary>
        public static readonly TSelf Instance = new();
    }

    // Мне показалось забавным, что для класса из двух строчек написано 14 строк комментариев (не считая этой).
}
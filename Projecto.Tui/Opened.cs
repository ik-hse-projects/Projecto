using System;
using Thuja;
using Thuja.Widgets;

namespace Projecto.Tui
{
    /// <summary>
    /// Представляет собой некоторый открытый в интерфейсе объект.
    /// </summary>
    /// <typeparam name="T">Какой объект открыт.</typeparam>
    public class Opened<T>
    {
        /// <summary>
        /// Объект, который открыт.
        /// </summary>
        internal readonly T Object;

        public Opened(T obj)
        {
            Object = obj;
            content = new StackContainer();
            container = new BaseContainer().Add(content);
            Deleted.Value += () => Closed.Value?.Invoke();
        }

        /// <summary>
        /// Контейнер, в котором находится <see cref="content"/>. Можно добавлять что-то ещё при необходимости.
        /// </summary>
        internal BaseContainer container { get; private init; }

        /// <summary>
        /// Виджеты, которые позволяют взаимодействовать с объектом.
        /// </summary>
        internal StackContainer content { get; private init; }

        /// <summary>
        /// Делегат вызывается, когда этот объъект закрывается.
        /// </summary>
        internal Box<Action?> Closed { get; private init; } = new(null);

        /// <summary>
        /// Делегат вызывается, когда этот объект следует удалить. После него всегда вызывается <see cref="Closed"/>.
        /// </summary>
        internal Box<Action?> Deleted { get; private init; } = new(null);

        /// <summary>
        /// Преобразовывает Opened{T} в Opened{T2}, если это возможно. Если нет, то возвращает null.
        ///
        /// Этот метод необходим, поскольку невозможно указать ковариантность для класса, чтобы, например,
        ///     Opened{List{int}} был приводим к типу Opened{IList{int}}, поскольку List{int} приводим к IList{int}. 
        /// </summary>
        public Opened<T2>? Cast<T2>()
        {
            if (Object is not T2 casted)
            {
                return null;
            }

            return new Opened<T2>(casted)
            {
                content = content,
                container = container,
                Closed = Closed,
                Deleted = Deleted
            };
        }
    }
}
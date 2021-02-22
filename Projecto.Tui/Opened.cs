using System;
using Thuja;
using Thuja.Widgets;

namespace Projecto.Tui
{
    public class Opened<T>
    {
        internal readonly T Object;

        public Opened(T obj)
        {
            Object = obj;
            content = new StackContainer();
            container = new BaseContainer().Add(content);
            Deleted.Value += () => Closed.Value?.Invoke();
        }

        internal BaseContainer container { get; private init; }
        internal StackContainer content { get; private init; }

        internal Box<Action?> Closed { get; private init; } = new(null);
        internal Box<Action?> Deleted { get; private init; } = new(null);

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
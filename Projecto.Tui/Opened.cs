using System;
using System.Linq;
using Thuja;
using Thuja.Widgets;

namespace Projecto.Tui
{
    public class Opened<T>
    {
        internal readonly T Object;
        internal BaseContainer root { get; private init; }
        internal BaseContainer container { get; private init; }
        internal StackContainer content { get; private init; }

        internal Box<Action?> Closed { get; private init; } = new(null);
        internal Box<Action?> Deleted { get; private init; } = new(null);

        public Opened(BaseContainer root, T obj) : this(obj)
        {
            this.root = root;
            content = new StackContainer();
            container = new BaseContainer().Add(content);
            Deleted.Value += () => Closed.Value?.Invoke();
        }

        private Opened(T obj)
        {
            Object = obj;
        }

        public Opened<T2>? Cast<T2>()
        {
            if (Object is not T2 casted)
            {
                return null;
            }

            return new Opened<T2>(casted)
            {
                root = root,
                content = content,
                container = container,
                Closed = Closed,
                Deleted = Deleted,
            };
        }
    }
}
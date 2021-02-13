using System;
using System.Collections;
using System.Collections.Generic;

namespace Projecto
{
    // https://stackoverflow.com/a/34362585
    public static class ReadOnlyListExtensions
    {
        public static IReadOnlyList<T> AsReadOnly<T>(this IList<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return source as IReadOnlyList<T> ?? new ReadOnlyListAdapter<T>(source);
        }

        sealed class ReadOnlyListAdapter<T> : IReadOnlyList<T>
        {
            readonly IList<T> source;
            public ReadOnlyListAdapter(IList<T> source) => this.source = source;
            public int Count => source.Count;
            public IEnumerator<T> GetEnumerator() => source.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public T this[int index] => source[index];
        }
    }
}
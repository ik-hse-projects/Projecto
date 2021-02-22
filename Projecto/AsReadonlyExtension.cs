using System;
using System.Collections;
using System.Collections.Generic;

namespace Projecto
{
    /// <summary>
    /// Расширение <see cref="IList{T}"/> для конвертации его в <see cref="IReadOnlyList{T}"/>.
    /// </summary>
    // https://stackoverflow.com/a/34362585
    public static class AsReadonlyExtension
    {
        /// <summary>
        /// Превращает переданный список в реализацию интерфейса <see cref="IReadOnlyList{T}"/>
        /// </summary>
        public static IReadOnlyList<T> AsReadOnly<T>(this IList<T> source)
        {
            return source as IReadOnlyList<T> ?? new ReadOnlyListAdapter<T>(source);
        }

        /// <summary>
        /// Вспомогательный класс, который реализует <see cref="IReadOnlyList{T}"/> при помощи <see cref="IList"/>.
        /// </summary>
        private sealed class ReadOnlyListAdapter<T> : IReadOnlyList<T>
        {
            private readonly IList<T> source;

            public ReadOnlyListAdapter(IList<T> source)
            {
                this.source = source;
            }

            /// <inheritdoc />
            public int Count => source.Count;

            /// <inheritdoc />
            public IEnumerator<T> GetEnumerator()
            {
                return source.GetEnumerator();
            }

            /// <inheritdoc />
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            /// <inheritdoc />
            public T this[int index] => source[index];
        }
    }
}
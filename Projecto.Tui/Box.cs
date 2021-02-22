namespace Projecto.Tui
{
    /// <summary>
    /// Простейшая обертка над любым типом.
    /// Очень полезна, когда требуется копировать ссылку на значимый тип. Например, делегаты.
    /// </summary>
    internal class Box<T>
    {
        public T Value;

        public Box(T value)
        {
            Value = value;
        }
    }
}
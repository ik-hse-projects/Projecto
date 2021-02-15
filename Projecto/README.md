# NuGet
Используется только `Newtonsoft.Json`,
поскольку стандартный `System.Text.Json` не умеет _нормально_ десериализовывать интерфейсы.

Подробнее:
- https://github.com/dotnet/runtime/issues/30083
- https://stackoverflow.com/q/58074304
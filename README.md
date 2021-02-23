# Projecto

Эта программа состоит из трёх проектов:
- `Projecto` — библиотека классов, которая содержит задачи, проекты и пользователей. Очень хорошо документирована.
- `Projecto.Tui` — консольное приложение для работы со всем вышеперечисленным. Документирована хорошо, но сильно зависит от Thuja.
- `Thuja` — библиотека, которая позволяет делать красивые консольные приложения. Документация есть, но разобраться может быть тяжело.
К каждому проекту приложен README.md, в котором поверхностно описывается, как вообще код устроен и как оно работает.

Просматривать код рекомендую именно в таком порядке, причём желательно не забывать про README к каждому проекту.

Thuja — большой и обширный проект. Я приложил некоторые усилия, чтобы его документировать, но боюсь, что этого совершенно недостаточно. В целом, его можно рассматривать как какой-нибудь NuGet проект, что-то вроде вроде WinForms (но поменьше, конечно). Очень рекомендую прочитать его README, я надеюсь, что оно сильно поможет.

# C#9 и .NET 5
Проект написан под .NET5 и с использованием C#9. Использовались некоторые новые фичи языка, о которых я на всякий случай здесь напишу:
- `if (somevar is {} notNull) ...` идентично `if (somevar != null) { var notNull = somevar; ... }`
- `public bool Property { get; init; }` — позволяет устанавливать свойство только в конструкторе *или* при создании класса таким образом:
```c#
var foo = new MyClass(...) {
    Property = true,    // Можно.
};
foo.Property = false;    // Нельзя.
```
- `protected internal Dictionary<(int, bool), List<MyClass<int>>> myCrazyField = new()` — идентично
  `protected internal Dictionary<(int, bool), List<MyClass<int>>> myCrazyField = new Dictionary<(int, bool), List<MyClass<int>>>()`.
  Здорово, правда?

Ну и как всегда активно использовались nullable-аннотации, так что если видите `public List<int>? myField;`, то это означает, что поле может быть null. А если вопросительного знака нет, то там точно есть какой-нибудь список.

# Про дебаггер
Для удобства **отладки**, в некоторых местах стоят явные вызовы подключенного отладчика, которые приостанавливают работу программы. На это корректно никак не реагировать, а просто продолжить работу. А ещё лучше, просто запустить вовсе программу без отладки (<kbd>Ctrl+F5</kbd>).

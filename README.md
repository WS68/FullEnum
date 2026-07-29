# FullEnum

FullEnum — библиотека на .NET для полного перебора сценариев выполнения нескольких независимых акторов. Проект полезен для проверки конкурентных алгоритмов, MVCC-состояний, транзакционных протоколов и других систем, где важно исследовать все возможные порядки действий.

## Возможности

- полный перебор последовательностей шагов нескольких акторов;
- изоляция ветвей за счёт копирования контекста;
- параллельное выполнение большого количества ветвей;
- отдельные результаты для успешных, отрицательных и аварийных сценариев;
- обработка остановленных шагов и исключений;
- финальная проверка состояния через валидатор;
- готовые демонстрационные сценарии синхронизации корневых папок и двухфазного коммита.

## Структура решения

- [`RunAlgorithm.Core`](RunAlgorithm.Core) — основной движок перебора: [`Runner`](RunAlgorithm.Core/Runner.cs:13), интерфейсы акторов, шагов, контекста и валидатора, а также исполнители.
- [`SyncRootFolders`](SyncRootFolders) — сценарий проверки согласованности признака корневой папки и содержимого.
- [`TwoPhaseCommit`](TwoPhaseCommit) — сценарий двухфазного коммита.
- [`CheckAlgorithm`](CheckAlgorithm) — консольное приложение, собирающее зависимости и запускающее демонстрационный сценарий.
- [`Tests`](Tests) — модульные тесты движка и сценариев.

## Как работает перебор

Каждый актор предоставляет имя и список шагов через [`IActor`](RunAlgorithm.Core/IActor.cs:3). Шаг реализует [`IStep`](RunAlgorithm.Core/IActor.cs:10): он изменяет контекст и возвращает признак возможности продолжения.

Для каждой следующей комбинации движок:

1. выбирает очередной шаг одного из незавершённых акторов;
2. вызывает [`IContext.Clone()`](RunAlgorithm.Core/IContext.cs:5), чтобы создать независимую ветвь;
3. выполняет шаг в клонированном контексте;
4. сохраняет результат шага и добавляет новую ветвь в очередь исполнения;
5. после завершения всех акторов передаёт контекст и результаты в [`IValidator`](RunAlgorithm.Core/IValidator.cs:3).

Возвращаемое шагом `false` значение означает остановку ветви. Исключение переводит ветвь в состояние `Failed`, после чего оно учитывается валидатором и статистикой, а не выбрасывается наружу из запуска.

> Реализация [`IContext.Clone()`](RunAlgorithm.Core/IContext.cs:5) должна действительно изолировать изменяемое состояние ветвей. Повторное использование одного изменяемого экземпляра контекста может привести к смешиванию результатов разных сценариев.

## Быстрый старт

### Требования

- .NET 6 SDK или новее с поддержкой таргета `net6.0`;
- Windows, Linux или macOS;
- NuGet-доступ для восстановления зависимостей.

### Сборка

```bash
dotnet restore CheckAlgorithm.sln
dotnet build CheckAlgorithm.sln
```

### Запуск демонстрации

```bash
dotnet run --project CheckAlgorithm/CheckAlgorithm.csproj
```

Приложение регистрирует акторов, контекст, валидатор и [`Runner`](RunAlgorithm.Core/Runner.cs:13) через DI. По завершении выводится статистика вида `OK`, `Negative` и `Failures`.

## Создание собственного сценария

Минимальный сценарий состоит из контекста, шага, актора и валидатора:

```csharp
using RunAlgorithm.Core;

public sealed class ExampleContext : IContext
{
    public int Value { get; set; }

    public IContext Clone() => new ExampleContext { Value = Value };
}

public sealed class IncrementStep : IStep
{
    public string Name => "Increment";
    public bool IsList => false;
    public IReadOnlyList<IStep> Variants => Array.Empty<IStep>();

    public bool Execute(IContext context)
    {
        ((ExampleContext)context).Value++;
        return true;
    }
}

public sealed class ExampleActor : IActor
{
    public string Name => "Example actor";
    public IReadOnlyList<IStep> Steps { get; } = new[] { new IncrementStep() };
}
```

Затем зарегистрируйте реализации в контейнере зависимостей:

```csharp
services.AddSingleton<IActor, ExampleActor>();
services.AddSingleton<IContext, ExampleContext>();
services.AddSingleton<IValidator, ExampleValidator>();
services.AddTransient<Runner>();
```

На практике валидатор должен реализовать [`IValidator`](RunAlgorithm.Core/IValidator.cs:3) и определить, является ли конечное состояние положительным, отрицательным или ошибочным.

## Варианты шагов

[`VariantsStep`](RunAlgorithm.Core/Steps/VariantsStep.cs:3) используется как маркер ветвления, а не как обычный исполняемый шаг. Его не следует вызывать напрямую и вкладывать один составной вариант в другой. Для обычных шагов достаточно возвращать список вариантов в соответствии с контрактом [`IStep`](RunAlgorithm.Core/IActor.cs:10).

## Выполнение и производительность

[`Runner.Run()`](RunAlgorithm.Core/Runner.cs:43) запускает перебор и возвращает [`RunStatistics`](RunAlgorithm.Core/RunStatistics.cs:3). Для небольших наборов используется прямое выполнение, а [`SmartExecutor`](RunAlgorithm.Core/Runtime/SmartExecutor.cs:3) переключается на пул потоков, когда размер пакета превышает `Environment.ProcessorCount * 2`.

Акторы, шаги и контексты должны быть безопасны для такого режима. Не используйте общее изменяемое состояние без синхронизации и не храните в экземпляре шага данные, относящиеся к конкретной ветви.

## Тесты

Запуск всех тестов:

```bash
dotnet test Tests/Tests.csproj
```

Запуск отдельного теста:

```bash
dotnet test Tests/Tests.csproj --filter "FullyQualifiedName=Tests.RunTests.BasicRunTest"
```

Сбор покрытия:

```bash
dotnet test Tests/Tests.csproj --collect:"XPlat Code Coverage"
```

## Лицензия

Проект распространяется в соответствии с условиями файла [`LICENSE`](LICENSE).

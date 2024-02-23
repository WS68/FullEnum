namespace RunAlgorithm.Core;

public enum RunResult
{
    /// <summary>
    /// Незавершен
    /// </summary>
    Running,
    /// <summary>
    /// Завершено успешно
    /// </summary>
    RunToEnd,
    /// <summary>
    /// Остановлен 
    /// </summary>
    Stopped,
    /// <summary>
    /// Возникло исключение
    /// </summary>
    Failed,
}
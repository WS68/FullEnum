namespace RunAlgorithm.Core;

/// <summary>
/// Варианты результата
/// </summary>
public enum CheckResult
{
    /// <summary>
    /// Успешно, положительный результат алгоритма
    /// </summary>
    SuccessPositive,
    /// <summary>
    /// Успешно, отрицательный исход алгоритма
    /// </summary>
    SuccessNegative,
    /// <summary>
    /// Ошибка в алгоритме
    /// </summary>
    Failure,
}
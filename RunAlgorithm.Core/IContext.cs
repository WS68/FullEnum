namespace RunAlgorithm.Core;

public interface IContext
    {
    /// <summary>
    /// Выполняет глубокое клонирование  
    /// </summary>
    IContext Clone();
    }
namespace RunAlgorithm.Core;

public record RunStatistics( long PositiveResults, long NegativeResults, long Failures )
{
    public long Total => PositiveResults + NegativeResults + Failures;
}
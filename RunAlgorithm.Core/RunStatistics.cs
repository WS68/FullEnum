namespace RunAlgorithm.Core;

public record RunStatistics( int PositiveResults, int NegativeResults, int Failures )
{
    public int Total => PositiveResults + NegativeResults + Failures;
}
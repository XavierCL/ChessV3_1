using System;
using System.Threading;

public class Ai8TimeManagement
{
  private readonly TimeSpan remainingTime;
  private readonly TimeSpan increment;
  private readonly CancellationToken cancellationToken;
  private readonly DateTime startTime;
  private readonly int forcedDepth;

  public Ai8TimeManagement(TimeSpan remainingTime, TimeSpan increment, CancellationToken cancellationToken, int forcedDepth)
  {
    this.remainingTime = remainingTime;
    this.increment = increment;
    this.cancellationToken = cancellationToken;
    startTime = DateTime.UtcNow;
    this.forcedDepth = forcedDepth;
  }

  public bool ShouldStop(int depth)
  {
    if (cancellationToken.IsCancellationRequested) return true;
    if (forcedDepth != -1)
    {
      return depth > forcedDepth;
    }

    if (remainingTime < MINIMUM) return true;

    var elapsedTime = GetElapsed();

    if (remainingTime < increment * 2) return elapsedTime >= remainingTime / 2;

    var bank = remainingTime - increment;
    var relativeBank = TimeSpan.FromMilliseconds(Math.Ceiling(bank.TotalMilliseconds / 40.0));
    var allotedTime = increment + relativeBank;
    return elapsedTime >= allotedTime;
  }

  public TimeSpan GetElapsed()
  {
    return DateTime.UtcNow - startTime;
  }

  private readonly TimeSpan MINIMUM = TimeSpan.FromMilliseconds(100);
}
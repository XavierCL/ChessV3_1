using System;
using System.Threading;

public class Ai10TimeManagement
{
  private readonly TimeSpan remainingTime;
  private readonly TimeSpan increment;
  private readonly CancellationToken cancellationToken;
  private readonly DateTime startTime;
  private readonly int forcedDepth;

  public Ai10TimeManagement(TimeSpan remainingTime, TimeSpan increment, CancellationToken cancellationToken, int forcedDepth)
  {
    this.remainingTime = remainingTime;
    this.increment = increment;
    this.cancellationToken = cancellationToken;
    startTime = DateTime.UtcNow;
    this.forcedDepth = forcedDepth;
  }

  public bool ShouldStop(int depth)
  {
    if (depth <= 1) return false;
    if (cancellationToken.IsCancellationRequested) return true;
    if (forcedDepth != -1)
    {
      return depth > forcedDepth;
    }

    if (remainingTime < MINIMUM) return true;

    var safeRemainingTime = remainingTime - MINIMUM;
    var elapsedTime = GetElapsed();

    if (safeRemainingTime < increment * 2) return elapsedTime >= safeRemainingTime / 2;

    var bank = safeRemainingTime - increment;
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
using System;
using System.Threading;

public class Ai4TimeManagement
{
  private readonly TimeSpan remainingTime;
  private readonly TimeSpan increment;
  private readonly CancellationToken cancellationToken;
  private readonly DateTime startTime;

  public Ai4TimeManagement(TimeSpan remainingTime, TimeSpan increment, CancellationToken cancellationToken)
  {
    this.remainingTime = remainingTime;
    this.increment = increment;
    this.cancellationToken = cancellationToken;
    startTime = DateTime.UtcNow;
  }

  public bool ShouldStop()
  {
    if (cancellationToken.IsCancellationRequested) return true;

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
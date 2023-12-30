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
    var elapsedTime = GetElapsed();
    var allotedTime = remainingTime / 40 + increment - TimeSpan.FromMilliseconds(10);
    return elapsedTime >= allotedTime;
  }

  public TimeSpan GetElapsed()
  {
    return DateTime.UtcNow - startTime;
  }
}
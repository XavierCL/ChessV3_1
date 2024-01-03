using System;
using System.Threading;

public class Ai7TimeManagement
{
  private readonly TimeSpan remainingTime;
  private readonly TimeSpan increment;
  private readonly CancellationToken cancellationToken;
  private readonly DateTime startTime;
  private readonly int forcedDepth;

  public Ai7TimeManagement(TimeSpan remainingTime, TimeSpan increment, CancellationToken cancellationToken, int forcedDepth)
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
    var elapsedTime = GetElapsed();
    var bank = remainingTime - increment;
    var positiveBank = bank < TimeSpan.Zero ? TimeSpan.Zero : bank;
    var allotedTime = positiveBank / 40 + increment / 2;
    return elapsedTime >= allotedTime;
  }

  public TimeSpan GetElapsed()
  {
    return DateTime.UtcNow - startTime;
  }
}
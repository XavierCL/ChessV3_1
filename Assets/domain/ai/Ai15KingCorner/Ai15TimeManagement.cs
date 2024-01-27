using System;
using System.Threading;

public class Ai15TimeManagement
{
  private readonly double startOfTurnRemainingTime;
  private readonly double increment;
  private readonly CancellationToken cancellationToken;
  private readonly DateTime startTime;
  private readonly int forcedDepth;
  private int maxDepthSeen = 0;

  public Ai15TimeManagement(TimeSpan remainingTime, TimeSpan increment, CancellationToken cancellationToken, int forcedDepth)
  {
    this.startOfTurnRemainingTime = remainingTime.TotalSeconds;
    this.increment = increment.TotalSeconds;
    this.cancellationToken = cancellationToken;
    startTime = DateTime.UtcNow;
    this.forcedDepth = forcedDepth;
  }

  public bool ShouldStop(int depth, bool dontStartNextDepthAfterHalfTime = true)
  {
    if (depth <= 1) return false;
    if (cancellationToken.IsCancellationRequested) return true;
    if (forcedDepth != -1) return depth > forcedDepth;
    if (startOfTurnRemainingTime < MINIMUM) return true;
    var elapsedTime = GetElapsed().TotalSeconds;
    var currentRemainingTime = startOfTurnRemainingTime - elapsedTime - MINIMUM;
    if (currentRemainingTime <= 0) return true;

    var bank = startOfTurnRemainingTime - increment - MINIMUM;
    var positiveBank = bank <= 0 ? 0 : bank;
    var allotedTime = positiveBank / 40 + increment;

    if (depth > this.maxDepthSeen)
    {
      // New depth, don't start it if alloted time is < 2x elapsed time
      this.maxDepthSeen = depth;

      if (dontStartNextDepthAfterHalfTime)
      {
        return elapsedTime * 2 >= allotedTime;
      }
    }

    return elapsedTime >= allotedTime;
  }

  public TimeSpan GetElapsed()
  {
    return DateTime.UtcNow - startTime;
  }

  private readonly double MINIMUM = TimeSpan.FromMilliseconds(100).TotalSeconds;
}
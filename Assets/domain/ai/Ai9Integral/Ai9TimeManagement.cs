using System;
using System.Threading;

public class Ai9TimeManagement
{
  private readonly TimeSpan startOfTurnRemainingTime;
  private readonly TimeSpan increment;
  private readonly CancellationToken cancellationToken;
  private readonly DateTime startTime;
  private readonly int forcedDepth;
  private int maxDepthSeen = 0;

  public Ai9TimeManagement(TimeSpan remainingTime, TimeSpan increment, CancellationToken cancellationToken, int forcedDepth)
  {
    this.startOfTurnRemainingTime = remainingTime;
    this.increment = increment;
    this.cancellationToken = cancellationToken;
    startTime = DateTime.UtcNow;
    this.forcedDepth = forcedDepth;
  }

  public bool ShouldStop(int depth)
  {
    if (depth <= 1) return false;
    if (cancellationToken.IsCancellationRequested) return true;
    if (forcedDepth != -1) return depth > forcedDepth;
    if (startOfTurnRemainingTime < MINIMUM) return true;
    var elapsedTime = GetElapsed();
    var currentRemainingTime = startOfTurnRemainingTime - elapsedTime - MINIMUM;
    if (currentRemainingTime < TimeSpan.Zero) return true;

    var bank = startOfTurnRemainingTime - increment - MINIMUM;
    var positiveBank = bank < TimeSpan.Zero ? TimeSpan.Zero : bank;
    var allotedTime = positiveBank / 40 + increment;

    if (depth > this.maxDepthSeen) {
      // New depth, don't start it if alloted time is < 2x elapsed time
      this.maxDepthSeen = depth;
      return elapsedTime * 2 >= allotedTime;
    }
    
    return elapsedTime >= allotedTime;
  }

  public TimeSpan GetElapsed()
  {
    return DateTime.UtcNow - startTime;
  }

  private readonly TimeSpan MINIMUM = TimeSpan.FromMilliseconds(100);
}
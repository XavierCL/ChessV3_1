using System;
using UnityEngine;

public class Clocks : MonoBehaviour
{
    public string initialTime = "5:00";
    public string increment = "5";
    public Color InitialGrayColor = Color.green;
    public Color TickingDownGreenColor = Color.green;
    public Color NoMoreTimeColor = Color.green;
    private bool isSwapped = false;

    void Start()
    {
        InitializeClocks();
    }

    public void Restart(bool topFirst)
    {
        var topClock = StaticReferences.topClock.Value.GetComponent<SingleClock>();
        var bottomClock = StaticReferences.bottomClock.Value.GetComponent<SingleClock>();

        InitializeClocks();
        topClock.ResetClock(initialTime);
        bottomClock.ResetClock(initialTime);
        ResetSwap();

        if (topFirst)
        {
            topClock.TickDown();
        }
        else
        {
            bottomClock.TickDown();
        }
    }

    public void MovePlayed()
    {
        var topClock = StaticReferences.topClock.Value.GetComponent<SingleClock>();
        var bottomClock = StaticReferences.bottomClock.Value.GetComponent<SingleClock>();

        if (topClock.TickingDown)
        {
            topClock.StopTicking(increment);
            bottomClock.TickDown();
        }
        else
        {
            bottomClock.StopTicking(increment);
            topClock.TickDown();
        }
    }

    public void Stop()
    {
        var topClock = StaticReferences.topClock.Value.GetComponent<SingleClock>();
        var bottomClock = StaticReferences.bottomClock.Value.GetComponent<SingleClock>();

        topClock.ForceStop();
        bottomClock.ForceStop();
    }

    public void ResetSwap()
    {
        if (!isSwapped) return;
        Swap();
    }

    public void Swap()
    {
        var topClock = StaticReferences.topClock.Value;
        var bottomClock = StaticReferences.bottomClock.Value;

        (bottomClock.transform.position, topClock.transform.position) = (topClock.transform.position, bottomClock.transform.position);
        isSwapped = !isSwapped;
    }

    private void InitializeClocks()
    {
        var topClock = StaticReferences.topClock.Value.GetComponent<SingleClock>();
        var bottomClock = StaticReferences.bottomClock.Value.GetComponent<SingleClock>();

        topClock.InitialGrayColor = InitialGrayColor;
        topClock.TickingDownGreenColor = TickingDownGreenColor;
        topClock.NoMoreTimeColor = NoMoreTimeColor;
        topClock.IsTop = true;

        bottomClock.InitialGrayColor = InitialGrayColor;
        bottomClock.TickingDownGreenColor = TickingDownGreenColor;
        bottomClock.NoMoreTimeColor = NoMoreTimeColor;
        bottomClock.IsTop = false;
    }
}

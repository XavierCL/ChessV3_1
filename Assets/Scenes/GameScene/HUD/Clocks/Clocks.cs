using System;
using UnityEngine;

public class Clocks : MonoBehaviour
{
    public string initialTime = "5:00";
    public string increment = "5";
    public Color InitialGrayColor = Color.green;
    public Color TickingDownGreenColor = Color.green;
    public Color NoMoreTimeColor = Color.green;

    private GameObject topClock;
    private GameObject bottomClock;
    private bool isSwapped = false;

    void Awake()
    {
        topClock = GameObject.Find("TopTime");
        bottomClock = GameObject.Find("BottomTime");
    }

    void Start()
    {
        var topClock = this.topClock.GetComponent<SingleClock>();
        var bottomClock = this.bottomClock.GetComponent<SingleClock>();

        topClock.InitialGrayColor = InitialGrayColor;
        topClock.TickingDownGreenColor = TickingDownGreenColor;
        topClock.NoMoreTimeColor = NoMoreTimeColor;
        topClock.IsTop = true;

        bottomClock.InitialGrayColor = InitialGrayColor;
        bottomClock.TickingDownGreenColor = TickingDownGreenColor;
        bottomClock.NoMoreTimeColor = NoMoreTimeColor;
        bottomClock.IsTop = false;
    }

    public void Restart(bool topFirst)
    {
        var topClock = this.topClock.GetComponent<SingleClock>();
        var bottomClock = this.bottomClock.GetComponent<SingleClock>();

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
        var topClock = this.topClock.GetComponent<SingleClock>();
        var bottomClock = this.bottomClock.GetComponent<SingleClock>();

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
        var topClock = this.topClock.GetComponent<SingleClock>();
        var bottomClock = this.bottomClock.GetComponent<SingleClock>();

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
        (bottomClock.transform.position, topClock.transform.position) = (topClock.transform.position, bottomClock.transform.position);
        isSwapped = !isSwapped;
    }
}

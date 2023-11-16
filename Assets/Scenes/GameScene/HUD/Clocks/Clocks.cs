using UnityEngine;

public class Clocks : MonoBehaviour
{
    public string initialTime = "5:00";
    public string increment = "5";
    public Color InitialGrayColor = Color.green;
    public Color TickingDownGreenColor = Color.green;

    private SingleClock topClock;
    private SingleClock bottomClock;

    void Awake()
    {
        topClock = GameObject.Find("TopTime").GetComponent<SingleClock>();
        bottomClock = GameObject.Find("BottomTime").GetComponent<SingleClock>();
    }

    void Start()
    {
        topClock.InitialGrayColor = InitialGrayColor;
        topClock.TickingDownGreenColor = TickingDownGreenColor;
        bottomClock.InitialGrayColor = InitialGrayColor;
        bottomClock.TickingDownGreenColor = TickingDownGreenColor;
    }

    public void Restart(bool topFirst)
    {
        topClock.ResetClock(initialTime);
        bottomClock.ResetClock(initialTime);

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
        topClock.ForceStop();
        bottomClock.ForceStop();
    }
}

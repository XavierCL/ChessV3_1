using UnityEngine;

public class Clocks : MonoBehaviour
{
    public string initialTime = "5:00";
    public string increment = "5";

    private SingleClock topClock;
    private SingleClock bottomClock;

    void Awake()
    {
        topClock = GameObject.Find("TopTime").GetComponent<SingleClock>();
        bottomClock = GameObject.Find("BottomTime").GetComponent<SingleClock>();
    }

    void Restart(bool topFirst)
    {
        topClock.Reset(initialTime);
        bottomClock.Reset(initialTime);

        if (topFirst)
        {
            topClock.TickDown();
        }
        else
        {
            bottomClock.TickDown();
        }
    }

    void MovePlayed()
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

    void Stop()
    {
        topClock.ForceStop();
        bottomClock.ForceStop();
    }
}

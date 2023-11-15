using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleClock : MonoBehaviour
{
    public string initialTime = "";
    public string increment = "";

    private TimeSpan remainingTimeSinceLastTickDown = TimeSpan.Zero;
    private DateTime lastTickDownDateTime = DateTime.MinValue;
    public bool TickingDown { get; private set; }

    public void Reset(string rootInitialTime)
    {
        TickingDown = false;
        getComponent<Text>().SetColor(InitialGray);
        remainingTimeSinceLastTickDown = format own or root
    }

    public void TickDown()
    {
        getComponent<Text>().SetColor(Green);
        TickingDown = true;
        lastTickDownDateTime = Time.now;
    }

    public void StopTicking(string rootIncrement)
    {
        TickingDown = false;
        remainingTimeSinceLastTickDown = remainingTimeSinceLastTickDown - (Time.now - lastTickDownDateTime) + format own or root increment
        getComponent<Text>().SetColor(InitialGray);
    }

    public void ForceStop()
    {
        if (!TickingDown) return;

        TickingDown = false;
        remainingTimeSinceLastTickDown = remainingTimeSinceLastTickDown - (Time.now - lastTickDownDateTime);
        getComponent<Text>().SetColor(InitialGray);
    }

    public void Update()
    {
        if (!TickingDown) return;

        TimeSpan timeLeft = remainingTimeSinceLastTickDown - (Time.now - lastTickDownDateTime);

        if (timeLeft == 0)
        {
            TickingDown = false;
            remainingTimeSinceLastTickDown = TimeSpan.Zero;
            gameController.NoMoreTime(top or bottom);
            getComponent<Text>().SetColor(NoMoreTimeRed);
        }

        getComponent<Text>().SetText(timeLeft);
    }
}

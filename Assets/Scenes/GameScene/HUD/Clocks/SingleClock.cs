using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SingleClock : MonoBehaviour
{
    public Color InitialGrayColor = Color.green;
    public Color TickingDownGreenColor = Color.green;
    public string initialTime = "";
    public string increment = "";

    private TimeSpan remainingTimeSinceLastTickDown = TimeSpan.Zero;
    private float lastTickDownDateTime = 0;
    private Regex stringToTimeSpanRegex = new Regex(@"(?:(\d+):)?(\d{2})", RegexOptions.Compiled);
    public bool TickingDown { get; private set; }

    public void ResetClock(string rootInitialTime)
    {
        TickingDown = false;
        gameObject.GetComponent<TextMeshPro>().color = InitialGrayColor;

        var initialTime = string.IsNullOrEmpty(this.initialTime) ? rootInitialTime : this.initialTime;
        gameObject.GetComponent<TextMeshPro>().SetText(initialTime);
        remainingTimeSinceLastTickDown = stringToTimeSpan(initialTime);
    }

    public void TickDown()
    {
        gameObject.GetComponent<TextMeshPro>().color = TickingDownGreenColor;
        TickingDown = true;
        lastTickDownDateTime = Time.time;
    }

    public void StopTicking(string rootIncrement)
    {
        TickingDown = false;
        remainingTimeSinceLastTickDown = remainingTimeSinceLastTickDown - TimeSpan.FromSeconds(Time.time - lastTickDownDateTime) + format own or root increment
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

    private TimeSpan stringToTimeSpan(string duration)
    {
        var matches = stringToTimeSpanRegex.Match(duration);
        var minutes = matches.Groups[0].Value;
        var seconds = matches.Groups[1].Value;

        var minutesNumber = string.IsNullOrEmpty(minutes) ? 0 : Int32.Parse(minutes);
        var secondsNumber = string.IsNullOrEmpty(seconds) ? 0 : Int32.Parse(seconds);
        return new TimeSpan(0, minutesNumber, secondsNumber);
    }

    private string timeSpanToString(TimeSpan duration)
    {
        return $"{Math.Floor(duration.TotalMinutes)}:{duration.Seconds}";
    }
}

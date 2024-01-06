using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class SingleClock : MonoBehaviour
{
    public Color InitialGrayColor = Color.green;
    public Color TickingDownGreenColor = Color.green;
    public Color NoMoreTimeColor = Color.green;
    public bool IsTop = false;
    public string initialTime = "";
    public string initialIncrement = "";

    private TimeSpan remainingTimeSinceLastTickDown = TimeSpan.Zero;
    private TimeSpan increment = TimeSpan.Zero;
    private float lastTickDownDateTime = 0;
    private static Regex stringToTimeSpanRegex = new Regex(@"(?:(\d+):)?(\d{1,2})(?:\.(\d{1,2}))?", RegexOptions.Compiled);
    private GameController gameController;

    public bool TickingDown { get; private set; }

    public void Awake()
    {
        gameController = StaticReferences.gameController.Value;
    }

    public void ResetClock(string rootInitialTime, string rootIncrement)
    {
        TickingDown = false;
        gameObject.GetComponent<TextMeshProUGUI>().color = InitialGrayColor;

        var initialTime = string.IsNullOrEmpty(this.initialTime) ? rootInitialTime : this.initialTime;
        gameObject.GetComponent<TextMeshProUGUI>().SetText(initialTime);
        remainingTimeSinceLastTickDown = stringToTimeSpan(initialTime);
        increment = stringToTimeSpan(string.IsNullOrEmpty(initialIncrement) ? rootIncrement : initialIncrement);
    }

    public void TickDown()
    {
        gameObject.GetComponent<TextMeshProUGUI>().color = TickingDownGreenColor;
        TickingDown = true;
        lastTickDownDateTime = Time.time;
    }

    public void StopTicking()
    {
        remainingTimeSinceLastTickDown = GetTimeLeft() + GetIncrement();
        TickingDown = false;
        gameObject.GetComponent<TextMeshProUGUI>().color = InitialGrayColor;
        gameObject.GetComponent<TextMeshProUGUI>().SetText(timeSpanToString(remainingTimeSinceLastTickDown));
    }

    public TimeSpan GetTimeLeft()
    {
        if (TickingDown) return remainingTimeSinceLastTickDown - TimeSpan.FromSeconds(Time.time - lastTickDownDateTime);
        return remainingTimeSinceLastTickDown;
    }

    public TimeSpan GetIncrement()
    {
        return increment;
    }

    public void ForceStop()
    {
        if (!TickingDown) return;

        remainingTimeSinceLastTickDown = GetTimeLeft();
        TickingDown = false;
        gameObject.GetComponent<TextMeshProUGUI>().color = InitialGrayColor;
    }

    public void Update()
    {
        if (!TickingDown) return;

        TimeSpan timeLeft = GetTimeLeft();

        if (timeLeft <= TimeSpan.Zero)
        {
            TickingDown = false;
            remainingTimeSinceLastTickDown = TimeSpan.Zero;
            timeLeft = remainingTimeSinceLastTickDown;
            gameController.NoMoreTime(IsTop);
            gameObject.GetComponent<TextMeshProUGUI>().color = NoMoreTimeColor;
        }

        gameObject.GetComponent<TextMeshProUGUI>().SetText(timeSpanToString(timeLeft));
    }

    public static TimeSpan stringToTimeSpan(string duration)
    {
        var matches = stringToTimeSpanRegex.Match(duration);
        var minutes = matches.Groups[1].Value;
        var seconds = matches.Groups[2].Value;
        var milliseconds = matches.Groups[3].Value;

        var minutesNumber = string.IsNullOrEmpty(minutes) ? 0 : Int32.Parse(minutes);
        var secondsNumber = string.IsNullOrEmpty(seconds) ? 0 : Int32.Parse(seconds);
        var millisecondsNumber = string.IsNullOrEmpty(milliseconds) ? 0 : (int)Math.Round(float.Parse($"0.{milliseconds}") * 1000);
        return new TimeSpan(0, 0, minutesNumber, secondsNumber, millisecondsNumber);
    }

    private string timeSpanToString(TimeSpan duration)
    {
        var totalMinutes = Math.Floor(duration.TotalMinutes);

        if (totalMinutes > 0) return $"{totalMinutes}:{duration.Seconds:00}";

        var totalSeconds = duration.TotalSeconds;

        if (totalSeconds >= 10) return $"{Math.Floor(totalSeconds):00}";

        return $"{totalSeconds:0.0}";
    }
}

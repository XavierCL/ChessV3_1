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
    public string increment = "";

    private TimeSpan remainingTimeSinceLastTickDown = TimeSpan.Zero;
    private float lastTickDownDateTime = 0;
    private static Regex stringToTimeSpanRegex = new Regex(@"(?:(\d+):)?(\d{1,2})", RegexOptions.Compiled);
    private GameController gameController;

    public bool TickingDown { get; private set; }

    public void Awake()
    {
        gameController = StaticReferences.gameController.Value;
    }

    public void ResetClock(string rootInitialTime)
    {
        TickingDown = false;
        gameObject.GetComponent<TextMeshProUGUI>().color = InitialGrayColor;

        var initialTime = string.IsNullOrEmpty(this.initialTime) ? rootInitialTime : this.initialTime;
        gameObject.GetComponent<TextMeshProUGUI>().SetText(initialTime);
        remainingTimeSinceLastTickDown = stringToTimeSpan(initialTime);
    }

    public void TickDown()
    {
        gameObject.GetComponent<TextMeshProUGUI>().color = TickingDownGreenColor;
        TickingDown = true;
        lastTickDownDateTime = Time.time;
    }

    public void StopTicking(string rootIncrement)
    {
        TickingDown = false;
        var increment = string.IsNullOrEmpty(this.increment) ? rootIncrement : this.increment;
        remainingTimeSinceLastTickDown = remainingTimeSinceLastTickDown - TimeSpan.FromSeconds(Time.time - lastTickDownDateTime) + stringToTimeSpan(increment);
        gameObject.GetComponent<TextMeshProUGUI>().color = InitialGrayColor;
        gameObject.GetComponent<TextMeshProUGUI>().SetText(timeSpanToString(remainingTimeSinceLastTickDown));
    }

    public void ForceStop()
    {
        if (!TickingDown) return;

        TickingDown = false;
        remainingTimeSinceLastTickDown = remainingTimeSinceLastTickDown - TimeSpan.FromSeconds(Time.time - lastTickDownDateTime);
        gameObject.GetComponent<TextMeshProUGUI>().color = InitialGrayColor;
    }

    public void Update()
    {
        if (!TickingDown) return;

        TimeSpan timeLeft = remainingTimeSinceLastTickDown - TimeSpan.FromSeconds(Time.time - lastTickDownDateTime);

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

        var minutesNumber = string.IsNullOrEmpty(minutes) ? 0 : Int32.Parse(minutes);
        var secondsNumber = string.IsNullOrEmpty(seconds) ? 0 : Int32.Parse(seconds);
        return new TimeSpan(0, minutesNumber, secondsNumber);
    }

    private string timeSpanToString(TimeSpan duration)
    {
        var totalMinutes = Math.Floor(duration.TotalMinutes);

        if (totalMinutes <= 0) return $"{duration.Seconds:00}";
        return $"{Math.Floor(duration.TotalMinutes)}:{duration.Seconds:00}";
    }
}

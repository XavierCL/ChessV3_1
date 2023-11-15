using UnityEngine;

public class ObserveAisHandler : MonoBehaviour
{
    public void ButtonPressed()
    {
        GameObject.Find(nameof(GameController)).GetComponent<GameController>().StartNewGame(
            Random.value >= 0.5 ? GameType.Ai1WhiteAi2Black : GameType.Ai1BlackAi2White
        );
    }
}

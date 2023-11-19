using UnityEngine;

public class ObserveAisHandler : MonoBehaviour
{
    public void ButtonPressed()
    {
        StaticReferences.gameController.Value.StartNewGame(
            Random.value >= 0.5 ? GameType.Ai1WhiteAi2Black : GameType.Ai1BlackAi2White
        );
    }
}

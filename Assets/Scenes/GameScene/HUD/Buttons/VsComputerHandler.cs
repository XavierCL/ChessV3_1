using UnityEngine;

public class VsComputerHandler : MonoBehaviour
{
    public void ButtonPressed()
    {
        StaticReferences.gameController.Value.StartNewGame(
            Random.value >= 0.5 ? GameType.HumanWhiteAiBlack : GameType.HumanBlackAiWhite
        );
    }
}

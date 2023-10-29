using UnityEngine;

public class VsComputerHandler : MonoBehaviour
{
    public void ButtonPressed()
    {
        GameObject.Find(nameof(BoardController)).GetComponent<BoardController>().StartNewGame(
            Random.value >= 0.5 ? GameType.HumanWhiteAiBlack : GameType.HumanBlackAiWhite
        );
    }
}

using UnityEngine;

public class PassAndPlayHandler : MonoBehaviour
{
    public void ButtonPressed()
    {
        GameObject.Find(nameof(BoardController)).GetComponent<BoardController>().StartNewGame(GameType.HumanHuman);
    }
}

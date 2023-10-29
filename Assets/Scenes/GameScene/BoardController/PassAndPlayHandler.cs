using UnityEngine;

public class PassAndPlayHandler : MonoBehaviour
{
    public void ButtonPressed()
    {
        GameObject.Find("BoardController").GetComponent<BoardController>().StartNewGame(GameType.HumanHuman);
    }
}

using UnityEngine;

public class PassAndPlayHandler : MonoBehaviour
{
    public void ButtonPressed()
    {
        GameObject.Find(nameof(GameController)).GetComponent<GameController>().StartNewGame(GameType.HumanHuman);
    }
}

using UnityEngine;

public class PassAndPlayHandler : MonoBehaviour
{
    public void ButtonPressed()
    {
        StaticReferences.gameController.Value.StartNewGame(GameType.HumanHuman);
    }
}

using TMPro;
using UnityEngine;

public class EndStateText : MonoBehaviour
{
    public void SetEndState(GameEndState endState)
    {
        gameObject.GetComponent<TextMeshProUGUI>().enabled = endState != GameEndState.Ongoing;

        switch (endState)
        {
            case GameEndState.Draw:
                gameObject.GetComponent<TextMeshProUGUI>().SetText("Draw!");
                return;
            case GameEndState.WhiteWin:
                gameObject.GetComponent<TextMeshProUGUI>().SetText("White wins!");
                return;
            case GameEndState.BlackWin:
                gameObject.GetComponent<TextMeshProUGUI>().SetText("Black wins!");
                return;
            case GameEndState.Ongoing:
                gameObject.GetComponent<TextMeshProUGUI>().SetText("Placeholder!");
                return;
        }
    }
}

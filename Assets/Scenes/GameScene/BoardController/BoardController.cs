using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public bool BoardRotated = false;
    public GameType gameType;
    private GameState gameState = new GameState();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private BoardPosition WorldPositionToBoardPosition(Vector2 worldPosition)
    {
        return new BoardPosition();
    }

    [ContextMenu("Rotate board")]
    public void RotateBoard()
    {
        var rotation = BoardRotated ? Quaternion.identity : Quaternion.Euler(new Vector3(0, 0, 180));
        GameObject.Find("TilesAndPieces").transform.rotation = rotation;
        getPieces().ForEach(piece => piece.transform.localRotation = rotation);
        BoardRotated = !BoardRotated;
    }

    public void PlayerPresses(Vector2 mouseWorldPosition)
    {
        if (gameType == GameType.Ai1WhiteAi2Black || gameType == GameType.Ai1BlackAi2White) return;
        if (gameType != GameType.HumanHuman)
        {
            if (gameType == GameType.HumanWhiteAiBlack && !gameState.whiteTurn) return;
            if (gameType == GameType.HumanBlackAiWhite && gameState.whiteTurn) return;
        }


    }

    public static void PlayerReleases()
    {

    }

    private List<GameObject> getPieces()
    {
        return new List<GameObject> {
            GameObject.Find("a1"),
            GameObject.Find("a2"),
            GameObject.Find("a7"),
            GameObject.Find("a8"),
            GameObject.Find("b2"),
            GameObject.Find("b7"),
            GameObject.Find("b8"),
            GameObject.Find("b1"),
            GameObject.Find("c1"),
            GameObject.Find("c2"),
            GameObject.Find("c7"),
            GameObject.Find("c8"),
            GameObject.Find("d1"),
            GameObject.Find("d2"),
            GameObject.Find("d7"),
            GameObject.Find("d8"),
            GameObject.Find("e1"),
            GameObject.Find("e2"),
            GameObject.Find("e7"),
            GameObject.Find("e8"),
            GameObject.Find("f1"),
            GameObject.Find("f2"),
            GameObject.Find("f7"),
            GameObject.Find("f8"),
            GameObject.Find("g1"),
            GameObject.Find("g2"),
            GameObject.Find("g7"),
            GameObject.Find("g8"),
            GameObject.Find("h1"),
            GameObject.Find("h2"),
            GameObject.Find("h7"),
            GameObject.Find("h8"),
        };
    }
}

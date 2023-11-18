using System;
using System.Threading.Tasks;
using UnityEngine;

public class AiController : MonoBehaviour
{
    public Guid gameId = Guid.NewGuid();

    public GameObject Ai1;

    public GameObject Ai2;

    private AiInterface Ai1Interface;
    private AiInterface Ai2Interface;

    public void Start()
    {
        Ai1Interface = Ai1.GetComponent<AiInterface>();
        Ai2Interface = Ai2.GetComponent<AiInterface>();
    }

    public void ResetAis()
    {
        gameId = Guid.NewGuid();
    }

    public async Task<Move> GetMove(GameState gameState)
    {
        var currentGuid = gameId;
        var moveOrEmpty = await Task.Run(async () =>
        {
            if (Ai1Interface == null || Ai2Interface == null) return null;
            if (gameState.whiteTurn) return await Ai1Interface.GetMove(gameState);
            return await Ai2Interface.GetMove(gameState);
        });

        if (currentGuid != gameId) return null;
        return moveOrEmpty;
    }
}

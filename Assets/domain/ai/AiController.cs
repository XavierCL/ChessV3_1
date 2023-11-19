using System;
using System.Threading.Tasks;
using UnityEngine;

public class AiController : MonoBehaviour
{
    public Guid gameId = Guid.NewGuid();

    public GameObject Ai1;

    public GameObject Ai2;

    public void ResetAis()
    {
        gameId = Guid.NewGuid();

        var ai1Interface = Ai1.GetComponent<AiInterface>();
        var ai2Interface = Ai2.GetComponent<AiInterface>();

        ai1Interface.ResetAi();
        ai2Interface.ResetAi();
    }

    public async Task<Move> GetMove(GameState gameState, bool ai1)
    {
        var ai1Interface = Ai1.GetComponent<AiInterface>();
        var ai2Interface = Ai2.GetComponent<AiInterface>();

        var currentGuid = gameId;
        var moveOrEmpty = await Task.Run(async () =>
        {
            if (ai1Interface == null || ai2Interface == null) return null;
            if (ai1) return await ai1Interface.GetMove(gameState);
            return await ai2Interface.GetMove(gameState);
        });

        if (currentGuid != gameId) return null;
        return moveOrEmpty;
    }
}

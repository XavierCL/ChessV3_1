using System;
using System.Threading.Tasks;
using UnityEngine;

public class AiController : MonoBehaviour
{
    public Guid gameId = Guid.NewGuid();

    public GameObject Ai1;

    private AiInterface ai1Interface;

    public GameObject Ai2;

    private AiInterface ai2Interface;

    public void Awake()
    {
        ai1Interface = Ai1.GetComponent<AiInterface>();
        ai2Interface = Ai2.GetComponent<AiInterface>();
    }

    public void ResetAis()
    {
        gameId = Guid.NewGuid();
        ai1Interface.ResetAi();
        ai2Interface.ResetAi();
    }

    public async Task<Move> GetMove(GameStateInterface gameState, bool ai1)
    {
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

    public async Task<Move> GetMoveSync(GameStateInterface gameState, bool ai1)
    {
        if (ai1Interface == null || ai2Interface == null) return null;
        if (ai1) return await ai1Interface.GetMove(gameState);
        return await ai2Interface.GetMove(gameState);
    }
}

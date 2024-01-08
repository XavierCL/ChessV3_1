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
        ai1Interface = Ai1.GetComponent<AiInterface>();
        ai2Interface = Ai2.GetComponent<AiInterface>();
        ai1Interface.ResetAi();
        ai2Interface.ResetAi();
    }

    public async Task<Move> GetMove(GameStateInterface gameState, bool ai1, TimeSpan remainingTime, TimeSpan increment)
    {
        var currentGuid = gameId;
        var moveOrEmpty = await Task.Run(async () =>
        {
            if (ai1Interface == null || ai2Interface == null) return null;
            if (ai1) return await ai1Interface.GetMove(gameState, remainingTime, increment);
            return await ai2Interface.GetMove(gameState, remainingTime, increment);
        });

        if (currentGuid != gameId) return null;
        return moveOrEmpty;
    }

    public async Task<Move> GetMoveSync(GameStateInterface gameState, bool ai1, TimeSpan remainingTime, TimeSpan increment)
    {
        if (ai1Interface == null || ai2Interface == null) return null;
        if (ai1) return await ai1Interface.GetMove(gameState, remainingTime, increment);
        return await ai2Interface.GetMove(gameState, remainingTime, increment);
    }

    public AiInterface GetAi1()
    {
        return ai1Interface;
    }
}

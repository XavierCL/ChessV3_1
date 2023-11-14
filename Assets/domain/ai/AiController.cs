using System.Threading.Tasks;
using UnityEngine;

public class AiController : MonoBehaviour
{
    public GameObject Ai1;

    public GameObject Ai2;

    private AiInterface Ai1Interface;
    private AiInterface Ai2Interface;

    public void Start()
    {
        Ai1Interface = Ai1.GetComponent<AiInterface>();
        Ai2Interface = Ai2.GetComponent<AiInterface>();
    }

    public async Task<Move> GetMove(GameState gameState)
    {
        return await Task.Run(async () =>
        {
            if (gameState.whiteTurn) return await Ai1Interface.GetMove(gameState);
            return await Ai2Interface.GetMove(gameState);
        });
    }
}

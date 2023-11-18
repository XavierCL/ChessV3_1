using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PremoveQueue : MonoBehaviour
{
    private readonly Queue<Move> queue = new Queue<Move> { };

    public void Push(Move move)
    {
        queue.Enqueue(move);
    }

    public Move Pop()
    {
        return queue.Dequeue();
    }

    public List<Move> GetMoves()
    {
        return queue.ToList();
    }

    public bool HasMoves()
    {
        return queue.Count > 0;
    }

    public void Clear()
    {
        queue.Clear();
    }
}

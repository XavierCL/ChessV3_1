using UnityEngine;

public class Tiles : MonoBehaviour
{
    public Color paleColor;
    public Color darkColor;

    public Vector3 position;
    public float size;
    public bool isWhiteBottom = true;

    void Update()
    {
        for (var x = 0; x < 8; ++x)
        {
            for (var y = 0; y < 8; ++y)
            {
                var isPaleSquare = ((x + y) % 2 == 0) ^ isWhiteBottom;
                Shapes.Rectangle(position + new Vector3(x * size, y * size, 0), size, size, isPaleSquare ? paleColor : darkColor);
            }
        }
    }
}

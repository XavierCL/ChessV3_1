using UnityEngine;

public class Tiles : MonoBehaviour
{
    public Color color;
    public Vector3 position;
    public float radius;

    void Update()
    {
        Shapes.Sphere(position, radius, color);
    }
}

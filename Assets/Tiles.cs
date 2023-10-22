using UnityEngine;

public class Tiles : MonoBehaviour
{
    public Color color;
    public Vector3 position;
    public float radius;

    void Update()
    {
        Shapes.Sphere(position, radius, color);
        Random.InitState(2);
        for (var i = 0; i < 1000; ++i)
        {
            Shapes.Sphere(Random.onUnitSphere, 0.01f, Random.ColorHSV());
        }
    }
}

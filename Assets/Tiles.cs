using UnityEngine;

public class Tiles : MonoBehaviour
{
    public Material material;

    void Update()
    {
        Shapes.Sphere(Vector3.zero, 1, material);
    }
}

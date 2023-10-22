using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shapes
{
    public static void Sphere(Vector3 position, double radius, Color color)
    {
        if (radius <= 0 || color.a == 0) return;

        Mesh mesh = new Mesh();
        mesh.SetColors()

        Graphics.DrawMesh(mesh
    }
}

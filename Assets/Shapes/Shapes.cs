using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shapes
{

    public static void Sphere(Vector3 position, float radius, Material material)
    {
        if (radius <= 0) return;

        var scale = new Vector3(radius * 2, radius * 2, 1);
        var meshPosition = Matrix4x4.TRS(position, Quaternion.identity, scale);

        var renderParams = new RenderParams(material);
        Graphics.RenderMesh(renderParams, DefaultMeshes.Quad, 0, meshPosition);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shapes
{
    private static Material material = new Material(Shader.Find("Unlit/PointShader"));

    public static void Sphere(Vector3 position, float radius, Color color)
    {
        if (radius <= 0 || color.a == 0) return;

        var scale = new Vector3(radius * 2, radius * 2, 1);
        var meshPosition = Matrix4x4.TRS(position, Quaternion.identity, scale);

        material.SetColor("_Color", color);
        var renderParams = new RenderParams(material);
        Graphics.RenderMesh(renderParams, DefaultMeshes.Quad, 0, meshPosition);
    }
}

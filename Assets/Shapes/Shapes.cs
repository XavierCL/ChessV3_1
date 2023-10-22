using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shapes
{
    private static MaterialPropertyBlock materialProperties = new MaterialPropertyBlock();
    private static Material pointMaterial = new Material(Shader.Find("Unlit/PointShader"));

    public static void Sphere(Vector3 position, float radius, Color color)
    {
        if (radius <= 0 || color.a == 0) return;

        var scale = new Vector3(radius * 2, radius * 2, 1);
        var meshPosition = Matrix4x4.TRS(position, Quaternion.identity, scale);

        materialProperties.SetColor("_Color", color);
        var renderParams = new RenderParams(pointMaterial) { matProps = materialProperties };
        Graphics.RenderMesh(renderParams, DefaultMeshes.Quad, 0, meshPosition);
    }
}

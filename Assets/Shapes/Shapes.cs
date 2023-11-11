using UnityEngine;

public class Shapes : MonoBehaviour
{
    private MaterialPropertyBlock colorMaterialProperties;
    public Material pointMaterial;
    public Material rectangleMaterial;

    public void Start()
    {
        colorMaterialProperties = new MaterialPropertyBlock();
    }

    public void Circle(Vector3 position, float radius, Color color)
    {
        if (radius <= 0 || color.a == 0) return;

        var scale = new Vector3(radius * 2, radius * 2, 1);
        var meshPosition = Matrix4x4.TRS(position, Quaternion.identity, scale);

        colorMaterialProperties.SetColor("_Color", color);
        var renderParams = new RenderParams(pointMaterial) { matProps = colorMaterialProperties };
        Graphics.RenderMesh(renderParams, DefaultMeshes.Quad, 0, meshPosition);
    }

    public void Rectangle(Vector3 position, float width, float height, Color color)
    {
        if (width <= 0 || height <= 0 || color.a == 0) return;

        var scale = new Vector3(width, height, 1);
        var meshPosition = Matrix4x4.TRS(position, Quaternion.identity, scale);

        colorMaterialProperties.SetColor("_Color", color);
        var renderParams = new RenderParams(rectangleMaterial) { matProps = colorMaterialProperties };
        Graphics.RenderMesh(renderParams, DefaultMeshes.Quad, 0, meshPosition);
    }
}

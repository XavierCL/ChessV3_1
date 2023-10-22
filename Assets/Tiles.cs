using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.RendererUtils;

public class Tiles : MonoBehaviour
{
    public Transform square;

    // Start is called before the first frame update
    void Start()
    {
        /* square = GameObject.CreatePrimitive(PrimitiveType.Plane).transform;
        MeshRenderer renderer = square.GetComponent<MeshRenderer>();
        renderer.material = new Material(Shader.Find("Unlit/Color"));
        renderer.sharedMaterial.color = Color.white;
        square.position = Vector3.zero;
        square.rotation = Quaternion.FromToRotation(new Vector3(0, 1, 0), new Vector3(0, 0, -1));
        square.localScale = Vector3.one / 2;*/
    }

    // Update is called once per frame
    void Update()
    {
        Shapes.Sphere(Vector3.zero, 1, Color.yellow);
    }
}

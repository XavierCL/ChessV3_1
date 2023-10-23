using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pieces : MonoBehaviour
{
    public GameObject[] pieces;
    // Start is called before the first frame update
    void Start()
    {
        pieces = new[] { GameObject.CreatePrimitive(PrimitiveType.Cube) };
    }

    // Update is called once per frame
    void Update()
    {

    }
}

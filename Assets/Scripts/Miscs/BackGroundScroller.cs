using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundScroller : MonoBehaviour
{
    [SerializeField]Vector2 scrollVelocity; 

    Material material;

    private void Awake()
    {
        material = GetComponent<Renderer>().material;
    }
    void Update()
    {
        material.mainTextureOffset += scrollVelocity * Time.deltaTime;
    }
}

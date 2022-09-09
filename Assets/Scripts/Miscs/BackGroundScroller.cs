using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundScroller : MonoBehaviour
{
    [SerializeField] Vector2 scrollVelocity;

    Material material;

    private void Awake()
    {
        material = GetComponent<Renderer>().material;
    }
    void Update()
    {
        if (GameManager.GameState != GameState.GameOver)
        {
            material.mainTextureOffset += scrollVelocity * Time.deltaTime;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viewport : Singleton<Viewport>
{

    float minX;
    float maxX;
    float minY;
    float maxY;
    float middleX;

    private void Start()
    {
        Camera mainCamera = Camera.main;

        Vector2 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0f));
        Vector2 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1f, 1f));

        middleX = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0f,0f)).x;

        minX = bottomLeft.x;
        minY = bottomLeft.y;
        maxX = topRight.x;
        maxY = topRight.y;

    }

    public Vector3 PlayerMoveablePostion(Vector3 playerPosition, float paddingX,float paddingY)
    {
        Vector3 postion = Vector3.zero;

        postion.x =  Mathf.Clamp(playerPosition.x, minX + paddingX, maxX - paddingX);
        postion.y = Mathf.Clamp(playerPosition.y, minY + paddingY, maxY - paddingY);

        return postion;
    }



    public Vector3 RandomEmemySpawnPosition(float paddingX, float paddingY)
    {
        Vector3 position = Vector3.zero;

        position.x = maxX + paddingX;
        position.y = Random.Range(minY + paddingY, maxY - paddingY);

        return position;
    }

    /// <summary>
    /// 敌人只在右半部分移动
    /// </summary>
    /// <param name="paddingX"></param>
    /// <param name="paddingY"></param>
    /// <returns></returns>
    public Vector3 RandomRightHalfPosition(float paddingX, float paddingY)
    {
        Vector3 position = Vector3.zero;

        position.x = Random.Range(middleX, maxX - paddingX);
        position.y = Random.Range(minY + paddingY, maxY - paddingY);

        return position;
    }


    /// <summary>
    /// 敌人在整个屏幕移动
    /// </summary>
    /// <param name="paddingX"></param>
    /// <param name="paddingY"></param>
    /// <returns></returns>
    public Vector3 RandomEnemyMovePosition(float paddingX, float paddingY)
    {
        Vector3 position = Vector3.zero;

        position.x = Random.Range(minX + paddingX, maxX - paddingX);
        position.y = Random.Range(minY + paddingY, maxY - paddingY);

        return position;
    }

}

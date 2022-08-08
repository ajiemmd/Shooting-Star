using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("---- Move ----")]
    [SerializeField] float paddingX;
    [SerializeField] float paddingY;
    [SerializeField] float moveSpeed = 2f;
    [SerializeField]float moveRotationAngle = 25f;

    [Header("---- Fire ----")]
    [SerializeField] GameObject[] projectiles;
    [SerializeField] AudioData[] projectileLaunchSFX;

    [SerializeField] Transform muzzle;

    [SerializeField] float minFireInterval;
    [SerializeField] float maxFireInterval;

    float maxMoveDistancePerFrame;

    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    private void Awake()
    {
        maxMoveDistancePerFrame = moveSpeed * Time.fixedDeltaTime;
    }

    private void OnEnable()
    {
        StartCoroutine(nameof(RandomlyMovingCoroutine));
        StartCoroutine(nameof(RandomlyFireCoroutine));
    }


    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator RandomlyMovingCoroutine()
    {
        transform.position = Viewport.Instance.RandomEmemySpawnPosition(paddingX, paddingY);

        Vector3 targetPosition = Viewport.Instance.RandomRightHalfPosition(paddingX, paddingY);

        while (gameObject.activeSelf)
        {
            //if has not arrived targetPosition
            if (Vector3.Distance(transform.position, targetPosition) >= maxMoveDistancePerFrame)
            {
                //keep moving to targetPosition
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, maxMoveDistancePerFrame);
                //make enemy rotate with x axis while moving
                transform.rotation = Quaternion.AngleAxis((targetPosition - transform.position).normalized.y * moveRotationAngle, Vector3.right);
            }
            else
            {
                //set a new targetPosition
                targetPosition = Viewport.Instance.RandomRightHalfPosition(paddingX, paddingY);
            }


            yield return waitForFixedUpdate;
        }
    }

        IEnumerator RandomlyFireCoroutine()
        {

            while (gameObject.activeSelf)
            {
                yield return new WaitForSeconds(Random.Range(minFireInterval, maxFireInterval));

                foreach (var projectile in projectiles)
                {
                    PoolManager.Release(projectile, muzzle.position);
                }

            AudioManager.Instance.PlayRandomSFX(projectileLaunchSFX);
            }
        }


    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField] Pool[] playerProjectilePools;

    [SerializeField] Pool[] enemyProjectilePools;

    [SerializeField] Pool[] vFXPools;

    static Dictionary<GameObject, Pool> dictionary;

    private void Start()
    {
        dictionary = new Dictionary<GameObject, Pool>();

        Initialize(playerProjectilePools);
        Initialize(enemyProjectilePools);
        Initialize(vFXPools);
    }

#if UNITY_EDITOR

    private void OnDestroy()//在编辑器停止运行时会自动调用
    {
        CheckPoolSize(playerProjectilePools);
        CheckPoolSize(enemyProjectilePools);
        CheckPoolSize(vFXPools);

    }
#endif

    private void CheckPoolSize(Pool[] pools)
    {
        foreach (var pool in pools)
        {
            if (pool.RuntimeSize > pool.Size)
            {
                Debug.LogWarning($"Pool: {pool.Prefab.name} has a runtime size {pool.RuntimeSize} bigger than its initial size {pool.Size}!");
            }
        }
    }

    private void Initialize(Pool[] pools)
    {
        foreach (var pool in pools)
        {
#if UNITY_EDITOR
            if (dictionary.ContainsKey(pool.Prefab))
            {
                Debug.LogError("Same prefab in Multiple pools! Prefab:" + pool.Prefab.name);
                continue;
            }
#endif

            dictionary.Add(pool.Prefab, pool);

            Transform poolParent = new GameObject("Pool: " + pool.Prefab.name).transform;

            poolParent.parent = transform;
            pool.Initialize(poolParent);
        }
    }

    /// <summary>
    /// 根据传入的<paramref name="prefab"></paramref>参数，返回对象池中预备好的游戏对象。
    /// </summary>
    /// <param name="prefab">指定的游戏对象预制体</param>
    /// <returns>游戏池中预备好的游戏对象</returns>
    public static GameObject Release(GameObject prefab)
    {
#if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager could NOT find prefab:" + prefab.name);
            return null;
        }
#endif

        return dictionary[prefab].prepareObject();
    }

    /// <summary>
    /// 根据传入的<paramref name="prefab"></paramref>参数，返回对象池中预备好的游戏对象。
    /// </summary>
    /// <param name="prefab">指定的游戏对象预制体</param>
    /// <param name="postion">指定释放位置</param>
    /// <returns>游戏池中预备好的游戏对象</returns>
    public static GameObject Release(GameObject prefab, Vector3 postion)
    {
#if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager could NOT find prefab:" + prefab.name);
            return null;
        }
#endif

        return dictionary[prefab].prepareObject(postion);
    }

    /// <summary>
    /// 根据传入的<paramref name="prefab"></paramref>参数，返回对象池中预备好的游戏对象。
    /// </summary>
    /// <param name="prefab">指定的游戏对象预制体</param>
    /// <param name="postion">指定释放位置</param>
    /// <param name="rotation">指定的旋转值</param>
    /// <returns>游戏池中预备好的游戏对象</returns>
    public static GameObject Release(GameObject prefab, Vector3 postion, Quaternion rotation)
    {
#if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager could NOT find prefab:" + prefab.name);
            return null;
        }
#endif

        return dictionary[prefab].prepareObject(postion,rotation);
    }

    /// <summary>
    /// 根据传入的<paramref name="prefab"></paramref>参数，返回对象池中预备好的游戏对象。
    /// </summary>
    /// <param name="prefab">指定的游戏对象预制体</param>
    /// <param name="postion">指定释放位置</param>
    /// <param name="rotation">指定的旋转值</param>
    /// <param name="localScale">指定的缩放值</param>
    /// <returns>游戏池中预备好的游戏对象</returns>
    public static GameObject Release(GameObject prefab, Vector3 postion, Quaternion rotation, Vector3 localScale)
    {
#if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager could NOT find prefab:" + prefab.name);
            return null;
        }
#endif
        return dictionary[prefab].prepareObject(postion, rotation, localScale);
    }

}

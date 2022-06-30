using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Pool
{
    public GameObject Prefab => prefab;
    public int Size => size;
    public int RuntimeSize => queue.Count;

    [SerializeField] GameObject prefab;

    [SerializeField] int size = 1;

    Queue<GameObject> queue;

    Transform parent;

    public void Initialize(Transform parent)
    {
        queue = new Queue<GameObject>();
        this.parent = parent;

        for (int i = 0; i < size; i++)
        {
            queue.Enqueue(Copy());
        }
    }


    private GameObject Copy()
    {
        var copy = GameObject.Instantiate(prefab, parent);

        copy.SetActive(false);

        return copy;
    }

    private GameObject AvailableObject()
    {
        GameObject availableObject = null;

        if (queue.Count > 0 && !queue.Peek().activeSelf)
        {
            availableObject = queue.Dequeue();
        }
        else
        {
            availableObject = Copy();
        }

        queue.Enqueue(availableObject);

        return availableObject;
    }

    public GameObject prepareObject()
    {
        GameObject prepareObject = AvailableObject();

        prepareObject.SetActive(true);

        return prepareObject;
    }

    public GameObject prepareObject(Vector3 position)
    {
        GameObject prepareObject = AvailableObject();

        prepareObject.SetActive(true);
        prepareObject.transform.position = position;

        return prepareObject;
    }

    public GameObject prepareObject(Vector3 position, Quaternion rotation)
    {
        GameObject prepareObject = AvailableObject();

        prepareObject.SetActive(true);
        prepareObject.transform.position = position;
        prepareObject.transform.rotation = rotation;

        return prepareObject;
    }

    public GameObject prepareObject(Vector3 position, Quaternion rotation, Vector3 localScale)
    {
        GameObject prepareObject = AvailableObject();

        prepareObject.SetActive(true);
        prepareObject.transform.position = position;
        prepareObject.transform.rotation = rotation;
        prepareObject.transform.localScale = localScale;

        return prepareObject;
    }

}

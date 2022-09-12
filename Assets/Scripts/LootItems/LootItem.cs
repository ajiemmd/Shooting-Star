using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootItem : MonoBehaviour
{
    [SerializeField] float minSpeed = 5f;
    [SerializeField] float maxSpeed = 15f;
    [SerializeField] protected AudioData defaultPickUpSFX;

    int pickUpStateId = Animator.StringToHash("PickUp");
    protected AudioData pickUpSFX;

    Animator animator;
    protected Player player;
    protected Text lootMessage;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        player = FindObjectOfType<Player>();
        lootMessage = GetComponentInChildren<Text>(true);
        pickUpSFX = defaultPickUpSFX;
    }

    private void OnEnable()
    {
        StartCoroutine(MoveCoroutine());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PickUp();
    }

    protected virtual void PickUp()
    {
        StopAllCoroutines();
        animator.Play(pickUpStateId);
        AudioManager.Instance.PlayRandomSFX(pickUpSFX);

    }

    IEnumerator MoveCoroutine()
    {
        float speed = Random.Range(minSpeed, maxSpeed);
        Vector3 direction = Vector3.left;
        while (true)
        {
            if (player.isActiveAndEnabled)
            {
                direction = (player.transform.position - transform.position).normalized;
            }

            transform.Translate(direction * speed * Time.deltaTime);

            yield return null;

        }


    }



}

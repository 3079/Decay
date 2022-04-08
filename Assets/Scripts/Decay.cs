using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decay : MonoBehaviour
{
    [SerializeField] private float frequency;
    [SerializeField] private float damage;
    [SerializeField] private GameObject mask;
    [SerializeField] private float maskSpawnInterval;
    [SerializeField] private float shrinkTime;
    // private Rigidbody2D rb;
    private float maskTimer;

    // private void Start()
    // {
        // rb = gameObject.GetComponentInParent<Rigidbody2D>();
    // }

    private void OnTriggerStay2D(Collider2D other)
    {
            Enemy enemy = other.GetComponent<Enemy>();
            if(enemy != null)
            {
                enemy.Decay(frequency, damage);
            }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
            Enemy enemy = other.GetComponent<Enemy>();
            if(enemy != null)
            {
                StartCoroutine(enemy.DecayEnd());
            }
    }

    private void Update()
    {
        maskTimer += Time.deltaTime;
        // if(rb.velocity.magnitude > 0.01f && maskTimer >= maskSpawnInterval)
        if(maskTimer >= maskSpawnInterval)
        {
            maskTimer = 0;
            DecayMask maskObj = GameObject.Instantiate(mask, transform.position, Quaternion.identity).GetComponent<DecayMask>();
            maskObj.setShrinkTime(shrinkTime);
        }

    }
}

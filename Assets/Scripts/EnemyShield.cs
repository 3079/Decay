using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShield : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] bool  hasTrace;
    [SerializeField] private GameObject trace;
    [SerializeField] private float traceSpawnInterval;
    [SerializeField] private float shrinkTime;
    private float traceTimer;
    // private Rigidbody2D rb;

    // private void Start()
    // {
    //     rb = gameObject.GetComponentInParent<Rigidbody2D>();
    // }

    private void OnTriggerStay2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if(player != null)
        {
            player.GetHit(damage);
        }
    }

    private void Update()
    {
        if(hasTrace)
        {
            traceTimer += Time.deltaTime;
            // if(rb.velocity.magnitude > 0.01f && traceTimer >= traceSpawnInterval)
            if(traceTimer >= traceSpawnInterval)
            {
                traceTimer = 0;
                DecayMask maskObj = GameObject.Instantiate(trace, transform.position, Quaternion.identity).GetComponent<DecayMask>();
                maskObj.setShrinkTime(shrinkTime);
            }
        }
    }
}

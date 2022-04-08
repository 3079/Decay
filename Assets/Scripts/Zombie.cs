using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy
{
    public enum EnemyState
    {
        Attacking,
        Approaching,
        Idle
    }
    private Vector2 movement;
    public GameObject melee;
    [SerializeField] private float meleeForce = 100f;
    [SerializeField] private float agroDistance;
    public EnemyState state;

    private AudioManager audioManager;
    [SerializeField] private float groanCooldown;
    [SerializeField] private float groanCooldownSpread;
    private float groanCD;

    void Start()
    {
        Spawn();
        rb = gameObject.GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player").transform;
        state = EnemyState.Idle;
        groanCD = 2f + Random.Range(0f, 2f);
        audioManager = FindObjectOfType<AudioManager>();
    }

    public IEnumerator Attack()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSeconds(0.3f);
        melee.GetComponent<EnemyMelee>().Attack(damage, movement, meleeForce);
        yield return new WaitForSeconds(0.1f);
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        yield return new WaitForSeconds(attackRate);
        state = EnemyState.Approaching;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == EnemyState.Idle)
        {
            if (DistanceToPlayer() < agroDistance)
                state = EnemyState.Approaching;
        }
        else
            if (DistanceToPlayer() >= agroDistance)
            {
                movement = Vector3.zero;
                state = EnemyState.Idle;
            }
            else
            {
                movement = player.position - rb.transform.position;
                movement = movement.normalized;

                if(DistanceToPlayer() < 5f && groanCD <= 0f && Random.Range(0f, 6f) < 1f)
                {
                    string audioName = "zombie" + Random.Range(0, 3);
                    audioManager.Play(audioName);
                    groanCD = groanCooldown + Random.Range(0, groanCooldownSpread);
                }
            }

        groanCD -= Time.deltaTime;

        Quaternion angle = Quaternion.Euler(0, 0, Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg);
        melee.transform.rotation = angle;
        animator.SetBool("right", movement.x > 0f);
    }

    void FixedUpdate() 
    {
        if(state != EnemyState.Idle)
        {
            rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
       
    }

    private float DistanceToPlayer()
    {
        return Vector3.Distance(transform.position, player.position);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEnemy : Enemy
{
    enum EnemyState 
    {
        Ready,
        Approaching,
        Fleeing,
        Idle
    }

    [Header("Enemy Specific Parameters")]
    [SerializeField] private float fleeRange;
    [SerializeField] private float approachRange;
    [SerializeField] private float idleRange;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private GameObject shield;
    [SerializeField] private float shieldHP;
    [SerializeField] private float shieldRechargeTime;
    [SerializeField] private float timeBeforeShieldStartsRecharging;
    [Space(10)]

    [Header("Shield Up Parameters")]
    [SerializeField] private GameObject projectileShieldUp;
    [SerializeField] private float projectileDamageShieldUp;
    [SerializeField] private float attackRateShieldUp;
    [Space(10)]

    [Header("Shield Down Parameters")]
    [SerializeField] private GameObject projectileShieldDown;
    [SerializeField] private float projectileDamageShieldDown;
    [SerializeField] private float attackRateShieldDown;

    private Vector2 movement;
    private EnemyState state;
    private GameObject projectile;
    
    private float projectileDamage;
    private float currentShieldHP;
    private float attackTimer = 0;
    private float shieldTimer = 0;
    private float shieldRechargeTimer = 0;
    private bool shieldUp = true;
    private bool decaying = false;

    private AudioManager audioManager;

    void Start()
    {
        Spawn();

        audioManager = FindObjectOfType<AudioManager>();

        rb = gameObject.GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player").transform;
        currentShieldHP = shieldHP;
        ShieldUp();
    }

    IEnumerator Attack()
    {
        if(state != EnemyState.Approaching)
        {
            //play audio
            string audioName = "fireball" + Random.Range(0, 3);
            audioManager.Play(audioName);

            animator.ResetTrigger("attack");
            animator.SetTrigger("attack");
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            Vector2 dir = (player.position - transform.position).normalized;
            Quaternion angle = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg); 
            // + 90 градусов по z, если фаербол будет ориентирован вертикально
            EnemyProjectile proj = Instantiate(projectile, transform.position, angle).GetComponent<EnemyProjectile>();
            proj.Launch(dir, projectileSpeed, projectileDamage);
            yield return new WaitForSeconds(0.1f);
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            //Add force
            // yield return new WaitForSeconds(attackRate);
        }
        else
            yield return null;
    }

    public override void Decay(float frequency, float damage)
    {
        decaying = true;
        if(shieldUp)
        {
            shieldTimer += Time.deltaTime;

            if (shieldTimer >= frequency)
            {
                shieldTimer = 0;
                currentShieldHP -= damage;
            }

            if (currentShieldHP <= 0)
            {
                currentShieldHP = 0;
                ShieldDown();
            }
        }
    }

    void ShieldDown()
    {
        shieldUp = false;

        shield.SetActive(false);

        projectile = projectileShieldDown;
        projectileDamage = projectileDamageShieldDown;
        attackRate = attackRateShieldDown;
    }

    void ShieldUp()
    {
        shieldUp = true;
        currentShieldHP = shieldHP;
        shieldRechargeTimer = 0;

        shield.SetActive(true);

        projectile = projectileShieldUp;
        projectileDamage = projectileDamageShieldUp;
        attackRate = attackRateShieldUp;
    }

    public override IEnumerator DecayEnd()
    {
        yield return new WaitForSeconds(timeBeforeShieldStartsRecharging);
        decaying = false;
    }

    void Update()
    {
        if(!shieldUp && !decaying)
        {
            shieldRechargeTimer += Time.deltaTime;

            currentShieldHP = shieldHP * shieldRechargeTimer / shieldRechargeTime;

            if(currentShieldHP >= shieldHP)
                ShieldUp();
        }

        attackTimer += Time.deltaTime;
        if((state == EnemyState.Ready || state == EnemyState.Fleeing) && attackTimer >= attackRate)
        {
            StartCoroutine(Attack());
            attackTimer = 0;
        }

        switch(state)
        {
            case EnemyState.Ready: 
            {
                movement = Vector2.zero;

                if ((player.transform.position - transform.position).magnitude > approachRange)
                    state = EnemyState.Approaching;

                if((player.transform.position - transform.position).magnitude < fleeRange)
                    state = EnemyState.Fleeing;

                break;
            }
            case EnemyState.Approaching: 
            {
                movement = player.position - rb.transform.position;
                movement = movement.normalized;

                if((player.transform.position - transform.position).magnitude <= approachRange)
                    state = EnemyState.Ready;

                if((player.transform.position - transform.position).magnitude >= idleRange)
                    state = EnemyState.Idle;

                break;
            }
            case EnemyState.Fleeing:
            {
                movement = rb.transform.position - player.position;
                movement = movement.normalized;

                if((player.transform.position - transform.position).magnitude >= fleeRange)
                    state = EnemyState.Ready;

                break;
            }
            case EnemyState.Idle:
            {
                    movement = Vector3.zero;
                    shield.SetActive(false);

                    if((player.transform.position - transform.position).magnitude <= idleRange)
                    {
                        state = EnemyState.Approaching;
                        shield.SetActive(true);
                    }

                break;
            }
        }

        Vector2 tmp = player.position - rb.transform.position;
        Quaternion angle = Quaternion.Euler(0, 0, Mathf.Atan2(tmp.y, tmp.x) * Mathf.Rad2Deg);
        animator.SetBool("right", tmp.x > 0);
        animator.SetBool("running right", movement.x > 0);
    }

    void FixedUpdate() 
    {
        rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
    }
}

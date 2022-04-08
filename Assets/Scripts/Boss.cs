using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
     enum EnemyState 
    {
        Ready,
        Approaching,
        Fleeing
    }

    [Header("Enemy Specific Parameters")]
    [SerializeField] private float fleeRange;
    [SerializeField] private float approachRange;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private GameObject shield;
    [SerializeField] private GameObject shieldFX;
    [SerializeField] private GameObject shieldBlinkFX;
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
    [SerializeField] private float shieldBlinkTime;
    [SerializeField] private GameObject projectileShieldDown;
    [SerializeField] private float projectileDamageShieldDown;
    [SerializeField] private float attackRateShieldDown;

    private Vector2 movement;
    private EnemyState state;
    private GameObject projectile;
    
    private float projectileDamage;
    [SerializeField] private float currentShieldHP;
    private float attackTimer = 0;
    private float shieldTimer = 0;
    private float shieldRechargeTimer = 0;
    private bool shieldUp = true;
    private bool decaying = false;

    void Awake()
    {
        Spawn();
        rb = gameObject.GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player").transform;
        currentShieldHP = shieldHP;

        invincible = true;
        shieldUp = true;
        
        currentShieldHP = shieldHP;
        shieldRechargeTimer = 0;

        shield.SetActive(true);
        shieldFX.SetActive(true);

        projectile = projectileShieldUp;
        projectileDamage = projectileDamageShieldUp;
        attackRate = attackRateShieldUp;
    }

    IEnumerator Attack()
    {
        if(state != EnemyState.Approaching)
        {
            animator.ResetTrigger("attack");
            animator.SetTrigger("attack");
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            Vector2 dir = (player.position - transform.position).normalized;
            Quaternion angle = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg); 
            // + 90 градусов по z, если фаербол будет ориентирован вертикально
            EnemyProjectile proj = Instantiate(projectile, transform.position, angle).GetComponent<EnemyProjectile>();
            proj.Launch(dir, projectileSpeed, projectileDamage);
            yield return new WaitForSeconds(0.1f);
            proj = Instantiate(projectile, transform.position, angle).GetComponent<EnemyProjectile>();
            proj.Launch(dir, projectileSpeed, projectileDamage);
            yield return new WaitForSeconds(0.1f);
            proj = Instantiate(projectile, transform.position, angle).GetComponent<EnemyProjectile>();
            proj.Launch(dir, projectileSpeed, projectileDamage);
            // yield return new WaitForSeconds(0.1f);
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

            if (shieldUp && currentShieldHP <= 0)
            {
                currentShieldHP = 0;
                StartCoroutine(ShieldDown());
            }
        }
    }

    public override void GetHit(float amount)
    {
        if(shieldUp)
            currentShieldHP -= amount;
        else
        {
            health -= amount;
            StartCoroutine(Blink());
        }

        if(health <= 0f)
            OnDeath();

        if(health <= maxHealth) 
        {
            HealthBar.SetActive(true);
            HealthBarFill.transform.localScale = new Vector3(Mathf.Clamp(health / maxHealth, 0 , 1), 1f, 1f);
        }

        if(health >= maxHealth) 
        {
            HealthBar.SetActive(false);
        }

        if (shieldUp && currentShieldHP <= 0)
        {
            currentShieldHP = 0;
            StartCoroutine(ShieldDown());
        }
    }

    IEnumerator ShieldDown()
    {
        invincible = false;
        shieldUp = false;

        shield.SetActive(false);
        shieldFX.SetActive(false);

        projectile = projectileShieldDown;
        projectileDamage = projectileDamageShieldDown;
        attackRate = attackRateShieldDown;

        yield return new WaitForSeconds(shieldRechargeTime);
        StartCoroutine(ShieldUp());
    }

    IEnumerator ShieldUp()
    {
        shieldFX.SetActive(true);
        shieldBlinkFX.SetActive(true);
        yield return new WaitForSeconds(shieldBlinkTime / 4f);
        shieldBlinkFX.SetActive(false);
        yield return new WaitForSeconds(shieldBlinkTime / 4f);
        shieldBlinkFX.SetActive(true);
        yield return new WaitForSeconds(shieldBlinkTime / 4f);
        shieldBlinkFX.SetActive(false);
        yield return new WaitForSeconds(shieldBlinkTime / 4f);
        invincible = true;
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
        // if(!shieldUp && !decaying)
        // {
        //     shieldRechargeTimer += Time.deltaTime;

        //     currentShieldHP = shieldHP * shieldRechargeTimer / shieldRechargeTime;

        //     if(currentShieldHP >= shieldHP)
        //         StartCoroutine(ShieldUp());
        // }

        attackTimer += Time.deltaTime;
        if(attackTimer >= attackRate)
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

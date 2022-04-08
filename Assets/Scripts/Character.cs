using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    protected float health;
    [SerializeField] protected float maxHealth;
    protected Rigidbody2D rb;
    [SerializeField] protected float speed;
    [SerializeField] protected ParticleSystem blood;
    [SerializeField] protected GameObject HealthBar;
    [SerializeField] protected Image HealthBarFill;
    protected Animator animator;
    protected bool invincible = false;

    public virtual void Spawn()
    {
        invincible = false;
        animator = gameObject.GetComponent<Animator>();
        HealthBar.SetActive(false);
        health = maxHealth;
    }

    public virtual void GetHit(float amount)
    {
        if(amount > 0)
        {
            if (!invincible)
            {
                health -= amount;
                StartCoroutine(Blink());
            }
        }
        else 
            health -= amount;

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
        
    }

    public IEnumerator Blink() 
    {
        invincible = true;
        Color temp = gameObject.GetComponent<SpriteRenderer>().color;
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        gameObject.GetComponent<SpriteRenderer>().color = temp;
        yield return new WaitForSeconds(0.1f);
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        gameObject.GetComponent<SpriteRenderer>().color = temp;
        invincible = false;
    }

    public void EmitBlood(Quaternion angle)
    {
        ParticleSystem particles = Instantiate(blood, transform.position, angle);
        Destroy(particles.gameObject, 1);
    }

    public virtual void OnDeath()
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }

    public void Knockback(Quaternion angle, float force)
    {
        if(!invincible)
            rb.AddForce(angle * Vector2.right * force);
    }
}

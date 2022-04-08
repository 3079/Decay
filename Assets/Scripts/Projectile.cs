using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private float damage;
    private Animator animator;
    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        Destroy(gameObject, 5);
        animator = gameObject.GetComponent<Animator>();
        animator.SetBool("stuck", false);
    }
    public void Launch(Vector2 direction, float speed, float dmg)
    {
        damage = dmg;
        // Debug.Log(direction + " " + speed + " " + damage);
        rb.AddForce(direction * speed);
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        rb.isKinematic = true;
        // rb.constraints = RigidbodyConstraints2D.FreezeAll;
        rb.transform.position += rb.transform.rotation * (Quaternion.Euler(0, 0, -45f) * Vector2.right) * 0.2f;
        rb.transform.SetParent(other.transform);
        animator.SetBool("stuck", true);
        Destroy (gameObject.GetComponent<Rigidbody2D>()); 
        Destroy (gameObject.GetComponent<CircleCollider2D>());

        Enemy enemy = other.collider.GetComponentInParent<Enemy>();
        if(enemy != null) 
        {
            enemy.GetHit(damage);
            enemy.EmitBlood(Quaternion.Euler(0, 0, transform.localEulerAngles.z));
        }

        Destroy(gameObject, 3f);
    }
}

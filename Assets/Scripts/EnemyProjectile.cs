using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private float damage;
    [SerializeField] protected ParticleSystem explosion;
    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        Destroy(gameObject, 5);
    }
    public void Launch(Vector2 direction, float speed, float dmg)
    {
        damage = dmg;
        // Debug.Log(direction + " " + speed + " " + damage);
        rb.AddForce(direction * speed);
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        ParticleSystem particles = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(particles.gameObject, 1);

        Player player = other.collider.GetComponentInParent<Player>();
        if(player != null) 
        {
            player.GetHit(damage);
            player.EmitBlood(Quaternion.Euler(0, 0, transform.localEulerAngles.z));
        }

        Destroy(gameObject);
    }
}

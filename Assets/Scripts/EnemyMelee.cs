using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : MonoBehaviour
{
    Player player = null;
    Zombie parent;

    private void Awake() 
    {
        parent = gameObject.GetComponentInParent<Zombie>();
    }
    public void Attack(float damage, Vector3 movement, float meleeForce)
    {
        if(player != null) {
            Quaternion angle = Quaternion.Euler(0, 0, Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg);
            player.Knockback(angle, meleeForce);
            player.GetHit(damage);
            player.EmitBlood(angle);
        }

        return;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        player = other.GetComponentInParent<Player>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        player = null;
    }

    private void OnTriggerStay2D(Collider2D other) 
    {
        if(parent.state == Zombie.EnemyState.Approaching)
        {
            player = other.GetComponentInParent<Player>();
            if(player != null)
            {
                parent.StartCoroutine(parent.Attack());
                parent.state = Zombie.EnemyState.Attacking;
            }
        }
    }
}

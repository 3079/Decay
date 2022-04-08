using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bow : MonoBehaviour
{
    public enum BowState
    {
        Ready,
        Charging,
        Attacking
    }

    private BowState state;
    [SerializeField] private GameObject projectile;
    private float damage;
    private float speed;
    [SerializeField] private float damageMin = 1f;
    [SerializeField] private float damageMax = 5f;
    [SerializeField] private float speedMin = 10f;
    [SerializeField] private float speedMax = 30f;
    private float chargeTime;
    [SerializeField] private float fullChargeTime = 2f;
    [SerializeField] private float reloadTime = 0.5f;
    [SerializeField] private float meleeDamage = 3f;
    [SerializeField] private float meleeTime = 0.5f;
    [SerializeField] private float meleeForce = 100f;
    [SerializeField] private Image crosshair;
    [SerializeField] private Sprite[] crosshairs;
    [SerializeField] private Animator bow;
    [SerializeField] private Animator hands;
    // private Rigidbody2D rb;

    private AudioManager audioManager;

    public Collider2D melee;
    public ContactFilter2D filter;

    void Start()
    {
        // rb = gameObject.GetComponent<Rigidbody2D>();
        state = BowState.Ready;
        damage = damageMin;
        speed = speedMin;
        chargeTime = 0f;

        audioManager = FindObjectOfType<AudioManager>();
        
    }

    private void Charge()
    {
        chargeTime = Mathf.Clamp(chargeTime + Time.deltaTime, 0, fullChargeTime);
        
        bow.SetFloat("charge", chargeTime / fullChargeTime);
        if (chargeTime / fullChargeTime >= 0.5)
        {
            if (chargeTime / fullChargeTime >= 1)
            {
                crosshair.sprite = crosshairs[2];
                bow.SetBool("charged", true);
            }
            else
                crosshair.sprite = crosshairs[1];
        }
        
        // Debug.Log("Charging " + chargeTime + "s");
    }

    private IEnumerator Fire()
    {
        //play audio
        string audioName = "bow" + Random.Range(0, 3);
        audioManager.Play(audioName);
        state = BowState.Attacking;
        bow.ResetTrigger("fire");
        bow.SetTrigger("fire");
        bow.SetBool("charged", false);
        damage += (damageMax - damageMin) * (chargeTime / fullChargeTime);
        speed += (speedMax - speedMin) * (chargeTime / fullChargeTime);
        // Debug.Log("Firing, charge time: " + chargeTime);
        crosshair.sprite = crosshairs[0];
        chargeTime = 0f;
        bow.SetFloat("charge", 0f);
        Vector2 lookDir = GetComponentInParent<Player>().lookDirection;
        Quaternion angle = Quaternion.Euler(0, 0, Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg + 45f);
        Projectile arrow = Instantiate(projectile, transform.position, angle).GetComponent<Projectile>();
        arrow.Launch(lookDir, speed, damage);
        //Add force
        yield return new WaitForSeconds(reloadTime);
        damage = damageMin;
        speed = speedMin;
        state = BowState.Ready;
        // Debug.Log("Ready To Attack");
    }

    IEnumerator Melee() 
    {
        //play audio
        string audioName = "slash" + Random.Range(0, 3);
        audioManager.Play(audioName);
        // Debug.Log("Slash");
        // rb.constraints = RigidbodyConstraints2D.FreezePosition;
        hands.ResetTrigger("attack");
        hands.SetTrigger("attack");
        List<Collider2D> enemies = new List<Collider2D>();
        melee.OverlapCollider(filter, enemies);
        foreach(Collider2D e in enemies)
        {
            Enemy enemy = e.GetComponentInParent<Enemy>();
            if(enemy != null)
            {
                Vector2 distance = enemy.transform.position - transform.position;
                Quaternion angle = Quaternion.Euler(0, 0, Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg);
                if(enemy.Knockbackable())
                    enemy.Knockback(angle, meleeForce);
                enemy.GetHit(meleeDamage);
                enemy.EmitBlood(angle);
            }
        }
        yield return new WaitForSeconds(meleeTime);
        // rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        state = BowState.Ready;
        // Debug.Log("Ready To Attack");
    }

    void Update()
    {
        if(state == BowState.Ready)
        {
            if(Input.GetButton("Fire"))
                state = BowState.Charging;

            if (Input.GetButtonDown("Melee"))
            {
                state = BowState.Attacking;
                StartCoroutine(Melee());
            }
        }

        if(state == BowState.Charging)
        {
            if(Input.GetButton("Fire"))
                Charge();

            if(Input.GetButtonUp("Fire"))
                StartCoroutine(Fire());
        }
    }
}

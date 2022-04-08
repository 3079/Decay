using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    private Vector2 movement;
    public Vector2 lookDirection;
    public Transform crosshair;
    public Transform melee;
    public Transform bow;
    [SerializeField] private Animator hands;

    private AudioManager audioManager;
    AudioSource BossSource;
    AudioSource OverworldSource;

    private int stepCD = 20;
    public bool outside = true;
    
    void Start()
    {
        Spawn();
        HealthBar.SetActive(true);
        rb = gameObject.GetComponent<Rigidbody2D>();
        audioManager = FindObjectOfType<AudioManager>();
        audioManager.Play("ambient");
        audioManager.Play("overworld");
        BossSource = audioManager.getSoundSource("boss");
        OverworldSource = audioManager.getSoundSource("overworld");
        OverworldSource.volume = 1;
        BossSource.volume = 0;
        BossSource.Play();
    }

    public override void GetHit(float amount)
    {
        if(amount > 0)
        {
            if (!invincible)
            {
                health -= amount;
                StartCoroutine(Invincibility());
            }
        }
        else 
            health -= amount;

        if(health <= 0f)
            OnDeath();

        HealthBarFill.transform.localScale = new Vector3(Mathf.Clamp(health / maxHealth, 0 , 1), 1f, 1f);
    }

    private IEnumerator Invincibility()
    {
        invincible = true;
        Color temp = gameObject.GetComponent<SpriteRenderer>().color;
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.15f);
        gameObject.GetComponent<SpriteRenderer>().color = temp;
        yield return new WaitForSeconds(0.15f);
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.15f);
        gameObject.GetComponent<SpriteRenderer>().color = temp;
        yield return new WaitForSeconds(0.15f);
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.15f);
        gameObject.GetComponent<SpriteRenderer>().color = temp;
        invincible = false;
    }

    public override void OnDeath()
    {
        GameManager.Lose();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        animator.SetFloat("moveDirX", movement.x);
        animator.SetFloat("moveDirY", movement.y);
        animator.SetFloat("lookDirX", lookDirection.x);
        animator.SetFloat("lookDirY", lookDirection.y);
        animator.SetFloat("speed", movement.magnitude);
        movement = movement.normalized;
        if((movement.x != 0 || movement.y != 0) && stepCD <= 0)
        {
            string audioName = "step" + Random.Range(0, 4);
            audioManager.Play(audioName);
            stepCD = 30;
        }
        stepCD--;
        // transform.position = position;
        lookDirection = (Camera.main.ScreenToWorldPoint(crosshair.position) - rb.transform.position);
        lookDirection = lookDirection.normalized;
        // Debug.Log(lookDirection.x + " " + lookDirection.y);
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        float angle1 = (angle + 22.5f) / 45 + 360f;
        
        melee.rotation = Quaternion.Euler(0, 0, (int)angle1 * 45);
        hands.SetFloat("lookDirX", lookDirection.x);
        hands.SetFloat("lookDirY", lookDirection.y);
        bow.rotation = Quaternion.Euler(0, 0, angle);

        if(outside)
        {
            OverworldSource.volume = Mathf.Min(1, OverworldSource.volume + 0.1f);
            BossSource.volume = Mathf.Max(0, BossSource.volume - 0.1f);
        }
        else
        {
            BossSource.volume = Mathf.Min(1, BossSource.volume + 0.1f);
            OverworldSource.volume = Mathf.Max(0, OverworldSource.volume - 0.1f);
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
    }
}

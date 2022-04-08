using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pentagram : MonoBehaviour
{
    [SerializeField] private ParticleSystem circle;
    [SerializeField] private GameObject timeBar;
    [SerializeField] private Image timeBarFill;
    private bool started = false;
    private bool decaying = false;
    private bool done = false;
    [SerializeField] private float time;
    private Animator animator;
    private float timer;
    private float defaultScale;
    private Player player;

    private void Start()
    {
        timer = time;
        defaultScale = transform.localScale.x;
        animator = gameObject.GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        Player player = other.GetComponent<Player>();

        if(player != null)
        {
            if(!done)
            player.outside = false;

            if (!started)
            {
                circle.gameObject.SetActive(true);
                started = true;
                timeBar.SetActive(true);
            }

            if(started && !done)
            {
                decaying = true;
                timeBar.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        Player player = other.GetComponent<Player>();

        if(player != null)
        {
            player.outside = true;

            if(started && !done)
            {
                decaying = false;
                timeBar.SetActive(false);
            }
        }
    }

    private void Update() 
    {
        if(started && !done)
        {
            if (decaying)
            {
                timer = Mathf.Clamp(timer - Time.deltaTime, 0f, time);
                timeBarFill.transform.localScale = new Vector3(defaultScale * (1f - timer / time), 1, 1);

                if (timer <= 0)
                {
                    done = true;
                    animator.ResetTrigger("burn");
                    animator.SetTrigger("burn");
                    GameManager.Pentagram();
                    circle.gameObject.SetActive(false);
                    timeBar.SetActive(false);
                    player.outside = true;
                    player.GetHit(-10f);
                }
            }
            else if(timer < time)
            timer = Mathf.Clamp(timer + Time.deltaTime, 0f, time);
        } 
        
    }
}

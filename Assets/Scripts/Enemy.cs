using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField] protected float damage;
    [SerializeField] protected float attackRate;
    [SerializeField] protected bool knockbackable;
    protected Transform player;

    public virtual void Decay(float frequency, float damage)
    {
    }

    public virtual IEnumerator DecayEnd()
    {
        yield return null;
    }

    public bool Knockbackable()
    {
        return knockbackable;
    }
}

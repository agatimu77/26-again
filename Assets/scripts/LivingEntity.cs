using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable {

    [SerializeField]
    protected float health;
    public float StartingHealth;
    bool dead;

    public event System.Action OnDeath;


	protected virtual void Start () {
        health = StartingHealth;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TakeHit(float damage, RaycastHit hit)
    {
        //algo con info del hit
        TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health<=0&&!dead)
        {
            Die();
        }
    }

    protected void Die()
    {
        dead = true;
        if (OnDeath!=null)
        {
            OnDeath();
        }
        GameObject.Destroy(gameObject);
    }
}

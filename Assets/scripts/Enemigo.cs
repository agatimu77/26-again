using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemigo : LivingEntity {

    public enum State { Idle, Chasing, Attacking};
    public State currentState;
    float attackDistance = 1.5f;
    public float damage = 1;

    float myCollisionRadius;
    float playerCollisionRadius;


    float timeBetweenAttacks = 1;
    float nextAttackTime;

    NavMeshAgent pathFinder;
    public Transform player;

    LivingEntity entityTarget;
    bool hasTarget;


	protected override void Start () {
        base.Start();

        if (GameObject.FindGameObjectWithTag("Player")!=null)
        {
            hasTarget = true;
            currentState = State.Chasing;
            pathFinder = GetComponent<NavMeshAgent>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
            entityTarget = player.GetComponent<LivingEntity>();
            entityTarget.OnDeath += OnTargetDeath;
            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            playerCollisionRadius = GetComponent<CapsuleCollider>().radius;
        }
        
    }

    public void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }
	
	
	void Update () {
        if (currentState==State.Chasing&&hasTarget)
        {
            StartCoroutine(Perseguir());
        }
        if (Time.time>nextAttackTime&&hasTarget)
        {
            float squareDistanceToTarget = (player.position - transform.position).sqrMagnitude;
            if (squareDistanceToTarget <= Mathf.Pow(attackDistance, 2))
            {
                currentState = State.Attacking;
                nextAttackTime = Time.time + timeBetweenAttacks;
                StartCoroutine(Attack());
            }
        }
        
	}

    IEnumerator Attack()
    {
        float percent = 0;
        float attackSpeed = 3;
        Vector3 originalPos = transform.position;
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        Vector3 targetPos = player.position - dirToPlayer * myCollisionRadius * .5f;
        bool hasAppliedDamage = false;
        while (percent <=1&&hasTarget)
        {
            percent += Time.deltaTime * attackSpeed;
            if (percent>=.5f&&!hasAppliedDamage)
            {
                entityTarget.TakeDamage(damage);
                hasAppliedDamage = true;
            }
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPos, targetPos, interpolation );
            yield return null;
        }
        currentState = State.Chasing;
    }

    IEnumerator Perseguir()
    {
        float tiempoDeActualización = .25f;
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        Vector3 pos = new Vector3(player.position.x, 0, player.position.z)-dirToPlayer*(myCollisionRadius+playerCollisionRadius);
        pathFinder.SetDestination(pos);
        yield return new WaitForSeconds(tiempoDeActualización);
    }
}

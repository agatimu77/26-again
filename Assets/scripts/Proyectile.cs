using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proyectile : MonoBehaviour {

    float speed = 10;
    public float damage = 1;
    float lifeTime=2;
    public LayerMask collisionMask;
    float skinWidth = .2f;
    Collider[] initialColliders;

    private void Start()
    {
        Destroy(this.gameObject, lifeTime);
        initialColliders = Physics.OverlapSphere(transform.position, .1f, collisionMask);
        if (initialColliders.Length > 0)
        {
            OnHit(initialColliders[0]);
        }
    }

    public void SetSpeed (float newSpeed)
    {
        speed = newSpeed;
    }

    void Update()
    {
        float distancePerFrame = speed * Time.deltaTime;
        CheckCollisions(distancePerFrame);
        transform.Translate(Vector3.forward * distancePerFrame);
     }

    void CheckCollisions(float distanceperFrame)
    {
        Ray ray = new Ray(transform.position, Vector3.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, distanceperFrame +skinWidth, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHit(hit);
        }
        
    }

    void OnHit(RaycastHit hit)
    {
        GameObject.Destroy (gameObject);
        IDamageable damageableObject = hit.collider.gameObject.GetComponent<IDamageable>();
        if (damageableObject!= null)
        {
            damageableObject.TakeHit(damage, hit);
        }
    }

    void OnHit(Collider c)
    {
        GameObject.Destroy(gameObject);
        IDamageable damageableObject = c.gameObject.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeDamage(damage);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    public Transform salidaBala;
    public Proyectile bala;
    public float balaSpeed = 35;
    public float msBetweenShots = 100;
    float nextShotTime;

    public void Shoot()
    {
        if (Time.time>nextShotTime)
        {
            nextShotTime = Time.time + msBetweenShots / 1000;
            Proyectile newBala = Instantiate(bala, salidaBala.position, salidaBala.rotation);
            newBala.SetSpeed(balaSpeed);
        }
    }
	
}

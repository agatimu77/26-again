using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

    Rigidbody rb;
    Vector3 velocity;

	void Start () {
        rb = GetComponent<Rigidbody>();
	}
    public void Move(Vector3 _velocity)
    {
        velocity = _velocity; 
    }

    public void LookAt(Vector3 point)
    {
        transform.LookAt(point + Vector3.up * transform.position.y);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }


}

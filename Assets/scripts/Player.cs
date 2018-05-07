using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GunController))]
[RequireComponent(typeof(PlayerController))]
public class Player : LivingEntity {

    public float speed = 8;
    PlayerController controller;
    GunController gunController;
    Camera viewCamera;

	protected override void Start () {
        base.Start();
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
	}
	
	
	void Update () {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 velocity = input.normalized * speed;
        controller.Move(velocity);

        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        float distance;
        Plane ground = new Plane(Vector3.up, Vector3.zero);
        if (ground.Raycast(ray, out distance))
        {
            Vector3 point = ray.GetPoint(distance);
            controller.LookAt(point);
            Debug.DrawLine(ray.origin, point, Color.red);
        }

        if (Input.GetMouseButton(0))
        {
            gunController.Shoot();
        }
	}
}

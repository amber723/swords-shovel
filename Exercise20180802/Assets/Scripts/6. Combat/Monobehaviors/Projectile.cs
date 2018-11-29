using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour 
{
    private GameObject caster;
    private float speed;
    private float range;
    private Vector3 travelDirection;

    private float distanceTraveled;

    //a delegate in C#. When this event is raised, we'll be sending along two
    //game objects, one for caster, the other for what we collide with.
    public event Action<GameObject, GameObject> ProjectileCollided;

    public void Fire(GameObject Caster, Vector3 Target, float Speed, float Range)
    {
        caster = Caster;
        speed = Speed;
        range = Range;

        //calculate travel direction
        travelDirection = Target - transform.position;
        travelDirection.y = 0f;
        travelDirection.Normalize();

        // initialize distance traveled
        distanceTraveled = 0f;
    }

	void Update () 
    {
        // move this projectile through space
        float distanceToTravel = speed * Time.deltaTime;

        transform.Translate(travelDirection * distanceToTravel);

        // check to see if we traveled too far, if so destroy this projectile
        distanceTraveled += distanceToTravel;
        if(distanceTraveled > range)
        {
            Destroy(gameObject);
        }
	}

    //OnTriggerEnter is a Unity function
    void OnTriggerEnter(Collider other)
    {
        // Raise a C# event, so other objects can listen to and respond to when
        //the event is raised.

        if(ProjectileCollided != null)
        {
            ProjectileCollided(caster, other.gameObject);
        }

        Destroy(gameObject);
    }
}

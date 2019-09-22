using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;

public class Bullet : MonoBehaviour
{
    public GameManager gm;
    
    public int numBounces;
    public float speed;

    private Rigidbody rb;
    private bool initialized = false;

    private Collider _collider;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    public void Initialize(GameManager gm, float angle, int numBounces, float speed)
    {
        if (initialized)
        {
            Debug.Log("Tried to initialize bullet a second time!");
        }

        this.gm = gm;
        this.numBounces = numBounces;
        this.speed = speed;
        
        if (rb == null)
            rb = GetComponent<Rigidbody>();
        rb.velocity = Quaternion.AngleAxis(angle, Vector3.up) * new Vector3(0, 0, speed);
        
        initialized = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        // Reflect among the normal vector so that it bounces off of the wall
        rb.velocity = Vector3.Reflect(rb.velocity, other.contacts[0].normal);
        numBounces--;
    }

    private void LateUpdate()
    {
        if(numBounces < 0)
            if(gameObject != null)
                gm.DestroyBullet(gameObject);
    }
}

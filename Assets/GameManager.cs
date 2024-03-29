﻿using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Bullet bulletPrefab;
    public TennisBall tennisBallPrefab;
    public TennisBall unyoyableTennisBallPrefab;
    
    private List<GameObject> _bullets;

    // Start is called before the first frame update
    void Start()
    {
        _bullets = new List<GameObject>();
    }

    public void SpawnBullet(Vector3 position, float angle, int numBounces, float speed)
    {
        Bullet bullet = Instantiate(bulletPrefab, position, Quaternion.identity);
        bullet.Initialize(this, angle, numBounces, speed);
        
        // bullets can't collide with each other
        SetBulletCollision(bullet.GetComponent<Collider>(), false);
        
        _bullets.Add(bullet.gameObject);
	}

	public void SpawnBullet(Vector3 position, float angle, int numBounces, float speed, bool _isHoming, Transform _rudo)
	{
		Bullet bullet = Instantiate(bulletPrefab, position, Quaternion.identity);
		bullet.Initialize(this, angle, numBounces, speed, _isHoming, _rudo);

		// bullets can't collide with each other
		SetBulletCollision(bullet.GetComponent<Collider>(), false);

		_bullets.Add(bullet.gameObject);
	}

    public GameObject SpawnTennisBall(Vector3 position, float angle, int numBounces, float speed, bool yoyoable=true, bool homing=false)
    {
	    TennisBall bullet;
	    if (yoyoable)
		    bullet = Instantiate(tennisBallPrefab, position, Quaternion.identity);
	    else
		    bullet = Instantiate(unyoyableTennisBallPrefab, position, Quaternion.identity);
	    bullet.Initialize(this, angle, numBounces, speed, homing);
        
	    // bullets can't collide with each other
	    SetBulletCollision(bullet.GetComponent<Collider>(), false);
        
	    _bullets.Add(bullet.gameObject);
	    return bullet.gameObject;
    }

    public void DestroyBullet(GameObject go)
    {
        _bullets.Remove(go);
        Destroy(go);
	}

	public void DestroyAllBullets()
	{
		if(_bullets.Count > 0)
		{
			foreach (GameObject go in _bullets)
			{
				Destroy(go);
			}
			_bullets.Clear();
		}
	}

	public void SetBulletCollision(Collider c, bool collide)
    {
        foreach(GameObject bullet in _bullets)
            Physics.IgnoreCollision(bullet.GetComponent<Collider>(), c, collide);
    }
}

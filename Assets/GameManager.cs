using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Bullet bulletPrefab;
    
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

    public void DestroyBullet(GameObject go)
    {
        _bullets.Remove(go);
        Destroy(go);
    }

    public void SetBulletCollision(Collider c, bool collide)
    {
        foreach(GameObject bullet in _bullets)
            Physics.IgnoreCollision(bullet.GetComponent<Collider>(), c, collide);
    }
}

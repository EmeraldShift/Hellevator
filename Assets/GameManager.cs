using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Rudo rudo;
    public Bullet bulletPrefab;
    
    private List<GameObject> _bullets;
    private Collider _rudoCollider;

    // Start is called before the first frame update
    void Start()
    {
        _bullets = new List<GameObject>();
        _rudoCollider = rudo.GetComponent<Collider>();
    }

    public void SpawnBullet(Vector3 position, float angle, int numBounces, float speed)
    {
        Bullet bullet = Instantiate(bulletPrefab, position, Quaternion.identity);
        bullet.Initialize(this, angle, numBounces, speed);
        
        // bullets can't collide with each other
        foreach(GameObject otherBullet in _bullets)
            Physics.IgnoreCollision(otherBullet.GetComponent<Collider>(), bullet.GetComponent<Collider>());
        
        _bullets.Add(bullet.gameObject);
    }

    public void DestroyBullet(GameObject go)
    {
        _bullets.Remove(go);
        Destroy(go);
    }

    IEnumerator ResetInvincibility()
    {
        yield return new WaitForSeconds(rudo.iSeconds);
        
        // re-enable all of the collisions for the bullets
        foreach(GameObject bullet in _bullets)
            Physics.IgnoreCollision(bullet.GetComponent<Collider>(), rudo.GetComponent<Collider>(), false); 
    }
    
    public void OnBecomeInvincible()
    {
        foreach(GameObject bullet in _bullets)
        {
            // disable collisions
            Physics.IgnoreCollision(bullet.GetComponent<Collider>(), _rudoCollider);
        }
        StartCoroutine(nameof(ResetInvincibility));
    }
}

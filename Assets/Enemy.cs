using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameManager gm;
    public float deltaAngle;
    public float period;
    private float angle;

    IEnumerator ShootCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(period);
            gm.SpawnBullet(transform.position, angle, 2, 0.4f);
            angle += deltaAngle;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        angle = 0;
        StartCoroutine(nameof(ShootCoroutine));
    }

    // Update is called once per frame
    void Update()
    {
        //todo
    }
}

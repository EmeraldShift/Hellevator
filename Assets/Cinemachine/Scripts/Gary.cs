using System.Collections;
using UnityEngine;

public class Gary : MonoBehaviour
{
    private static readonly System.Random kRandom = new System.Random();

    public GameManager gm;
    
    public float volleyTime;
    public float ricochetTime;
    public float bombTime;
    public float breakTime;
    
    private int _attack;
    private int _lastAttack;
    private float _time;
    private float _stateTime;
    private bool _firstTick;

    public void Update()
    {
        _time += Time.deltaTime;

        if (_time >= _stateTime + breakTime)
        {
            // Select new state
            while (_attack == _lastAttack)
                _attack = kRandom.Next(0, 4);
            _lastAttack = _attack;
            _stateTime = _time + (
                             _attack == 0 ? volleyTime :
                             _attack == 1 ? ricochetTime :
                                            bombTime);
            _firstTick = true;
        }

        // During break
        if (_time >= _stateTime)
            return;

        switch (_attack)
        {
            // Volley
            case 0:
                if (_firstTick) // If first tick
                {
                    // Fire volley
                    float down = 0.385f;
                    float back = 0.35f;
                    gm.SpawnTennisBall(transform.position + Vector3.down * down + Vector3.back * back, 160, 2, 0.4f);
                    gm.SpawnTennisBall(transform.position + Vector3.down * down + Vector3.back * back, 170, 2, 0.4f);
                    gm.SpawnTennisBall(transform.position + Vector3.down * down + Vector3.back * back, 180, 2, 0.4f);
                    gm.SpawnTennisBall(transform.position + Vector3.down * down + Vector3.back * back, 190, 2, 0.4f);
                    gm.SpawnTennisBall(transform.position + Vector3.down * down + Vector3.back * back, 200, 2, 0.4f);
                }
                break;
            case 1:
                if (_firstTick) // If first tick
                {
                    // Fire ricochet
                    float down = 0.385f;
                    float back = 0.35f;
                    if (kRandom.Next(0, 2) == 0)
                        gm.SpawnTennisBall(transform.position + Vector3.down * down + Vector3.back * back, 180 - 75, 6, 1.3f);
                    else
                        gm.SpawnTennisBall(transform.position + Vector3.down * down + Vector3.back * back, 180 + 75, 6, 1.3f);
                }
                break;
            case 2:
                if (_firstTick) // If first tick
                {
                    // Fire bomb
                    float down = 0.385f;
                    float back = 0.35f;
                    GameObject obj = gm.SpawnTennisBall(transform.position + Vector3.down * down + Vector3.back * back, 180, 0, 0.4f, false);
                    StartCoroutine(nameof(BombEffect), obj);
                }
                break;
            default:
                Debug.Log("Caught unexpected default case in switch.");
                break;
        }

        _firstTick = false;
    }

    private IEnumerator BombEffect(GameObject obj)
    {
        yield return new WaitForSeconds(1.25f);
        gm.DestroyBullet(obj);
        for (int i = 45; i < 360; i += 90)
            gm.SpawnTennisBall(obj.transform.position, i, 6, 0.8f);
        gm.DestroyBullet(obj);
    }
}

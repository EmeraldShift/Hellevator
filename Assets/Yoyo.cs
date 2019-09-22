using UnityEngine;

public class Yoyo : MonoBehaviour
{
    public GameManager gm;
    
    private Transform sender;
    private float time;
    private bool _retract;
    
    public void SetSender(Transform sender)
    {
        this.sender = sender;
    }
    
    public void FixedUpdate()
    {
        time += Time.deltaTime;
        
        if (_retract)
        {
            Vector3 diff = sender.position - transform.position;
            GetComponent<Rigidbody>().velocity = diff * 3f;
            GetComponent<Rigidbody>().velocity.Normalize();
        }

    }

    public void Retract(bool retract)
    {
        _retract = retract;
    }

    private void Kill()
    {
        Destroy(gameObject);
        sender.gameObject.SendMessage("YoyoDone");
    }
    
    private void OnCollisionEnter(Collision other)
    {
        GameObject go = other.gameObject;
        if (go.CompareTag("Bullet"))
            gm.DestroyBullet(other.gameObject);
    }

    public void OnCollisionStay(Collision other)
    {
        if (time > 0.1f && other.collider.GetComponent<Rudo>())
            Kill();
    }
}

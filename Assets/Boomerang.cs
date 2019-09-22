using UnityEngine;

public class Boomerang : MonoBehaviour
{
    private Transform sender;
    private float time;
    
    public void SetSender(Transform sender)
    {
        this.sender = sender;
    }
    
    public void FixedUpdate()
    {
        time += Time.deltaTime;

        Vector3 diff = sender.position - transform.position;
        GetComponent<Rigidbody>().velocity += diff / 10f;
        GetComponent<Rigidbody>().velocity.Normalize();
    
        transform.Rotate(0, 600 * Time.deltaTime, 0);

        if (time > 2)
            Kill();
    }

    private void Kill()
    {
        Destroy(gameObject);
        sender.gameObject.SendMessage("BoomerangDone");
    }

    public void OnCollisionStay(Collision other)
    {
        if (time > 0.1f && other.collider.GetComponent<Rudo>())
            Kill();
    }
}

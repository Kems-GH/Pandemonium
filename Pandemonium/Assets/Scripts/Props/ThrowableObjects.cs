using UnityEngine;

public class ThrowableObjects : GrabEvent
{
    private Rigidbody rb;
    private GameObject props_destruction;
    

    private void OnTriggerEnter(Collider other)
    {
        if(!IsServer) return;
        if(this.rb == null) this.rb = this.gameObject.GetComponent<Rigidbody>();
        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        // if(enemy == null) return;
        Debug.Log("Enemy hit : " + this.rb.velocity.magnitude );
        if(this.rb.velocity.magnitude > 5f)
        {
            enemy.TakeDamage(1);
            // props_destruction = GameObject.Find("Props_Destruction");
            GameObject particle = Instantiate<GameObject>(props_destruction);
            if(IsServer) particle.GetComponent<NetworkObject>().Spawn(true);
            Props_Destruction.transform.position = this.transform.position;
            Destroy(this.gameObject);
        }
    }
}

using UnityEngine;

public class EnemyLife
{
    private Enemy enemy;
    private int _currentHealth;
    private bool canTakeDamage = true;
    private float lastDamage = 0;

    public EnemyLife(Enemy enemy, int health)
    {
        this.enemy = enemy;
        _currentHealth = health;
    }

    public void TriggerDamage(Collider collider)
    {
        IGiveDamage damage = collider.gameObject.GetComponent<IGiveDamage>();
        Debug.Log("TriggerDamage : " + damage);
        if(damage == null) return;

        if(Time.time - this.lastDamage > 1f) this.canTakeDamage = true;

        this.TakeDamage(damage.GetAmountDamage());
    }

    /**
     * The Enemy take some damage
     */
    private void TakeDamage(int damage)
    {   
        if(!this.canTakeDamage) return;
        
        this.canTakeDamage = false;
        this._currentHealth -= damage;

        if(this._currentHealth <= 0)
        {
            this.enemy.Die();
        }

        this.lastDamage = Time.time;
    }

}

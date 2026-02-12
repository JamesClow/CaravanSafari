using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float healthMax = 10;
    public float healthCurrent = 0;

    public void TakeDamage(float dmg)
    {
        this.healthCurrent -= dmg;
        if (this.healthCurrent <= 0)
        {
            this.Die();
        }
    }

    private void Die()
    {
        // Notify death observers
        AudioManager.getInstance().Play("Chime");
        Destroy(this.gameObject);
    }

    void Start()
    {
        this.healthCurrent = healthMax;
    }
}

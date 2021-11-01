using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    // Parameters
    [SerializeField] public int health;

    // Cached

    // State

    // Start is called before the first frame update
    void Start()
    {
        if (health == 0) health = 100;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoseHealth(int healthToBeTaken)
    {
        health -= healthToBeTaken;

        if(health <= 0)
        {
            FindObjectOfType<SoundManager>().ChangePitch("UnitsDeath");
            FindObjectOfType<SoundManager>().PlaySound("UnitsDeath", transform.position);
            Destroy(this.gameObject);
        }
    }
}

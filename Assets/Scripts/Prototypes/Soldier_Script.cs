using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier_Script : MonoBehaviour
{
    private int health;

    // Start is called before the first frame update
    void Start()
    {
        health = 50;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SoldierLoseHealth(int lostHealth)
    {
        health -= lostHealth;

        if (health <= 0)
        {
            Object.Destroy(this.gameObject);
        }
    }
}
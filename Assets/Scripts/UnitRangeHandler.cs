using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRangeHandler : MonoBehaviour
{
    // Parameters
    [SerializeField] public CombatSearches combatSearches;

    // Cached
    private SoldierCombat soldierCombatComponent;
    private SkeletonCombat skeletonCombatComponent;

    // States
    public enum CombatSearches
    {
        Skeletons,
        Humans
    }


    // Start is called before the first frame update
    void Start()
    {
        if(combatSearches == CombatSearches.Skeletons)
        {
            soldierCombatComponent = GetComponentInParent<SoldierCombat>();
        }
        else
        {
            skeletonCombatComponent = GetComponentInParent<SkeletonCombat>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (combatSearches == CombatSearches.Skeletons)
        {
            if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "EnemyBuilding")
            {
                // Debug.Log(string.Format("{0} has {1} in range", this.name, other.name));
                soldierCombatComponent.OnEnemyInRange(other.gameObject);
            }
        }
        else
        {
            if(other.gameObject.tag == "PlayerPeon" || other.gameObject.tag == "SoldierUnit" || other.gameObject.tag == "PlayerBarracks" || other.gameObject.tag == "PlayerTC" || other.gameObject.tag == "Resource" || other.gameObject.tag == "TowerDefense")
            {
                skeletonCombatComponent.OnEnemyInRange(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (combatSearches == CombatSearches.Skeletons)
        {
            if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "EnemyBuilding")
            {
                soldierCombatComponent.OnEnemyEscapeRange(other.gameObject);
            }
        }
        else
        {
            if (other.gameObject.tag == "PlayerPeon" || other.gameObject.tag == "SoldierUnit" || other.gameObject.tag == "PlayerBarracks" || other.gameObject.tag == "PlayerTC" || other.gameObject.tag == "Resource" || other.gameObject.tag == "TowerDefense")
            {
                skeletonCombatComponent.OnEnemyEscapeRange(other.gameObject);
            }
        }
            
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    // Parameters
    [SerializeField] public int unitsToSpawn = 3;
    public GameObject[] unitToSpawn;
    public GameObject[] enemyMines;
    private GameObject enemyParent;
    private GameObject enemyUnit;
    private GameObject playerUnitsObject;
    private int timer;

    private int wave = 1;

    // Start is called before the first frame update
    void Start()
    {
        enemyParent = GameObject.FindGameObjectWithTag("EnemyUnitsHolder");
        enemyMines = GameObject.FindGameObjectsWithTag("EnemyBuilding");
        // InvokeRepeating("Spawn", 5.0f, 60.0f);
        InvokeRepeating("Spawn", 120f, 120f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //public void Spawn()
    //{
    //    int chooseMine = Random.Range(0, enemyMines.Length);
    //    int minRandom = 5;
    //    int maxRandom = 12;
    //    int enemysToSpawn = Random.Range(minRandom, maxRandom + 1);

    //    for(int i = 0; i< enemysToSpawn; i++)
    //    {
    //        Vector3 pos = Random.insideUnitCircle * 3;
    //        Vector3 lastPosition = new Vector3(pos.x, 0, pos.y) + enemyMines[chooseMine].transform.position;
    //        enemyUnit = Instantiate(unitToSpawn[0], lastPosition, Quaternion.identity);
    //        enemyUnit.transform.parent = enemyParent.transform;
    //    }
    //}

    private void Spawn()
    {
        int chooseMine = Random.Range(0, enemyMines.Length);
        int enemysToSpawn = unitsToSpawn + wave;

        for (int i = 0; i < enemysToSpawn; i++)
        {
            Vector3 pos = Random.insideUnitCircle * 3;
            Vector3 lastPosition = new Vector3(pos.x, 0, pos.y) + enemyMines[chooseMine].transform.position;
            enemyUnit = Instantiate(unitToSpawn[0], lastPosition, Quaternion.identity);
            enemyUnit.transform.parent = enemyParent.transform;
        }
        wave++;
    }

    //private int SigmoidUnitSpawnQuantity(int wave)
    //{
    //    return (int)((10.0 / 1.0 + Mathf.Pow(-0.2f, wave)) - 4);
    //}
}
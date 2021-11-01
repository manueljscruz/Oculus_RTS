using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BuildingSpawnUnit : MonoBehaviour
{
    // Parameters
    public GameObject unitToSpawn;
    public GameObject spawnAreaObject;
    [SerializeField] private float unitSpawnTimeInterval = 10f;

    // Cached
    private GameObject playerUnitsObject;
    private PlayerResources playerResourcesComponent;
    private Queue<GameObject> queueSpawn = new Queue<GameObject>();
    private float timer;
    private List<float> timesOfNextUnitSpawn = new List<float>();
    private TMP_Text unitQueueQtyText;

    // State

    // Start is called before the first frame update
    void Start()
    {
        playerUnitsObject = GameObject.FindGameObjectWithTag("PlayerUnitsHolder");
        playerResourcesComponent = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerResources>();
        unitQueueQtyText = FindObjectsWithTag(transform, "UnitQueueQtyUI").FirstOrDefault().GetComponent<TMP_Text>();


    }

    // Update is called once per frame
    void Update()
    {
        timer = Time.time;
        // Debug.Log(timer);
        if (queueSpawn.Count != 0)
        {
            CheckQueueForSpawns();
        }
    }

    private void Spawn()                                     // ResourceClass[] unitCosts
    {
        Vector3 vec3 = spawnAreaObject.transform.position;

        GameObject unitGameObject = Instantiate(unitToSpawn, vec3, Quaternion.identity) as GameObject;
        unitGameObject.transform.parent = playerUnitsObject.transform;
        UpdateQueueQtyUI();
        // playerResourcesComponent.DrainResources(unitCosts);
    }

    #region Add To Queue

    public void AddToQueue(ResourceClass[] unitCosts)
    {
        queueSpawn.Enqueue(unitToSpawn);
        float nextSpawnTime = GetLastQueueTimeValue() + unitSpawnTimeInterval;
        timesOfNextUnitSpawn.Add(nextSpawnTime);
        playerResourcesComponent.DrainResources(unitCosts);
        UpdateQueueQtyUI();
        // Debug.Log(string.Format("New Queue to spawn at {0}. Current Queue is {1}", nextSpawnTime, timesOfNextUnitSpawn.Count));
        // Debug.Log(string.Format("{0} : Spawns at {1}", unitToSpawn.name, nextSpawnTime));
    }

    #endregion

    #region Execute Queue

    private void ExecuteQueue(int index)
    {
        queueSpawn.Dequeue();
        Spawn();
        timesOfNextUnitSpawn.RemoveAt(index);
        // Debug.Log(string.Format("Queue Length is now : {0}",timesOfNextUnitSpawn.Count));
    }

    #endregion

    #region Check Queue For Spawns

    private void CheckQueueForSpawns()
    {
        int index = 0;
        foreach (float timeOfSpawn in timesOfNextUnitSpawn.ToList())
        {

            if (timeOfSpawn != 0 && timer >= timeOfSpawn)
            {
                ExecuteQueue(index);
            }
            else index++;
        }


    }

    #endregion

    private List<GameObject> FindObjectsWithTag(Transform parent, string tag)
    {
        List<GameObject> taggedGameObjects = new List<GameObject>();

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.tag == tag)
            {
                taggedGameObjects.Add(child.gameObject);
            }
            if (child.childCount > 0)
            {
                taggedGameObjects.AddRange(FindObjectsWithTag(child, tag));
            }
        }
        return taggedGameObjects;
    }

    private void UpdateQueueQtyUI()
    {
        unitQueueQtyText.SetText(queueSpawn.Count().ToString());
    }

    private float GetLastQueueTimeValue()
    {
        float lastQueueTime = 0;

        if (timesOfNextUnitSpawn.Count != 0)
            lastQueueTime = timesOfNextUnitSpawn.Max();

        else lastQueueTime = timer;
        return lastQueueTime;
    }
}

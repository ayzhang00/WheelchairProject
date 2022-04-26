using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInstantiator : MonoBehaviour
{
    public GameObject NPCPrefab;
    public int NPCcount;
    public Transform SpawnpointGroup;
    Transform[] Spawnpoints;
    int cur_spawnpoint;
    // Start is called before the first frame update
    void Start()
    {

        Spawnpoints = SpawnpointGroup.GetComponentsInChildren<Transform>();
        cur_spawnpoint= Random.Range(0, Spawnpoints.Length - 1);

        SpawnNPCS();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnNPCS()
    {
        for(int i = 0; i <= NPCcount; i++)
        {
        }
    }
}

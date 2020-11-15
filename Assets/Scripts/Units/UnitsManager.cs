using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitsManager : MonoBehaviour
{
    [Header("Units")]
    public GameObject worker;
    public GameObject explorer;

    [Space]
    [Header("Spawn Spots")]
    public Transform workerBaseSpawnSpot;
    public Transform explorerBaseSpawnSpot;

    Action OnWorkerSpawned;
    Action OnExplorerSpawned;

    public void SpawnWorker()
    {
        Instantiate(worker, workerBaseSpawnSpot.position, workerBaseSpawnSpot.rotation);
    }
    public void SpawnExplorer()
    {
        Instantiate(explorer, explorerBaseSpawnSpot.position, explorerBaseSpawnSpot.rotation);
    }
}

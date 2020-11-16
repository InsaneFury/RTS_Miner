using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitsManager : Singleton<UnitsManager>
{
    [Header("Units")]
    public GameObject worker;
    public GameObject explorer;
    public List<GameObject> workersReadyToWork;
    public List<GameObject> minesMarked;

    [Space]
    [Header("Spawn Spots")]
    public Transform workerBaseSpawnSpot;
    public Transform explorerBaseSpawnSpot;

    public override void Awake()
    {
        base.Awake();
        workersReadyToWork = new List<GameObject>();
        minesMarked = new List<GameObject>();
    }

    public void AddReadyWorker(GameObject worker)
    {
        Debug.Log("Added worker " + worker.name);
        if(minesMarked.Count <= 0)
        workersReadyToWork.Add(worker);
        else
        {
            foreach(GameObject mine in minesMarked)
            {
                if (mine.GetComponent<Mine>().isMarked)
                {
                    worker.GetComponent<Worker>().GoToMine(mine);
                    break;
                }
            }
        }
        
    }

    public void AddMarkedMine(GameObject mine)
    {
        Debug.Log("Added mine " + mine.name);
        minesMarked.Add(mine);
        if (mine.GetComponent<Mine>().isMarked)
        {
            foreach (GameObject worker in workersReadyToWork)
            {
                if (!worker.GetComponent<Worker>().IsBusy())
                {

                    worker.GetComponent<Worker>().GoToMine(mine);
                    break;

                }
            }
        }

    }

    public void SpawnWorker()
    {
        Instantiate(worker, workerBaseSpawnSpot.position, workerBaseSpawnSpot.rotation);
    }
    public void SpawnExplorer()
    {
        Instantiate(explorer, explorerBaseSpawnSpot.position, explorerBaseSpawnSpot.rotation);
    }


}

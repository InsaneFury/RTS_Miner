using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitsManager : Singleton<UnitsManager>
{
    [Header("Units")]
    public GameObject worker;
    public GameObject explorer;
    public Queue<GameObject> workersReadyToWork;
    public Queue<GameObject> minesMarked;

    [Space]
    [Header("Spawn Spots")]
    public Transform workerBaseSpawnSpot;
    public Transform explorerBaseSpawnSpot;

    public override void Awake()
    {
        base.Awake();
        workersReadyToWork = new Queue<GameObject>();
        minesMarked = new Queue<GameObject>();
    }

    public void AddReadyWorker(GameObject worker)
    {
        
        if(minesMarked.Count <= 0)
        {
            Debug.Log("Added worker to Queue" + worker.name);
            workersReadyToWork.Enqueue(worker);
        }
        else
        {
            Mine mine = minesMarked.Dequeue().GetComponent<Mine>();
            if (mine.isMarked)
            {
                worker.GetComponent<Worker>().GoToMine(mine.gameObject);
            }
        } 
    }

    public void AddMarkedMine(GameObject mine)
    {
        if (workersReadyToWork.Count <= 0)
        {
            minesMarked.Enqueue(mine);
            Debug.Log("Added mine to Queue" + mine.name);
        }
        else
        {
            Worker worker = workersReadyToWork.Dequeue().GetComponent<Worker>();
            worker.GoToMine(mine);
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

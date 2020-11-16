using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Explorer : MonoBehaviour
{
    [Header("Explorer Settings")]
    public Vector3 minPosition;
    public Vector3 maxPosition;
    public float timeToReachNewTarget = 2f;

    
    private FieldOfView fow;
    private Vector3 randomPositionToExplore;
    private bool isExploring = false;
    private Unit unit;

    enum ExplorerState
    {
        Idle,
        Patrol,
        Marking
    }
    ExplorerState state;
    Action OnExplorerFoundMine;

    private void Awake()
    {
        fow = GetComponent<FieldOfView>();
        unit = GetComponent<Unit>();
    }

    void Start()
    {
        unit.OnTargetReached += ResetTarget;
        randomPositionToExplore = transform.position;
        state = ExplorerState.Idle;
    }

    private void OnDestroy()
    {
        unit.OnTargetReached -= ResetTarget;
    }
    void Update()
    {
        switch (state)
        {
            case ExplorerState.Idle:
                // At instantiate the explorer needs to turn on the engine and prepare the vehicle correctly
                StartCoroutine(GoToPatrol());
                break;
            case ExplorerState.Patrol:
                // Explorer is exploring the map to find a mine
                if (!isExploring)
                {
                    StartCoroutine("ReachNewTarget");
                    isExploring = true;
                }
                // Explorer has found a mine to mark
                FindMine();
                break;
            case ExplorerState.Marking:
                // Explorer launch an event mine is ready to mine
                // Unit Manager listen the event and assign the first ready worker from queue of workers
                OnExplorerFoundMine?.Invoke();
                SetState(ExplorerState.Patrol);
                break;
            default:
                break;
        }
    }

    IEnumerator GoToPatrol()
    {
        yield return new WaitForSeconds(3.0f);
        SetState(ExplorerState.Patrol);
    }

    void FindMine()
    {
        if (fow.visibleTargets.Count <= 0) return;
        foreach (Transform visibleTarget in fow.visibleTargets)
        {
            // Mark Mine
            Mine mine = visibleTarget.GetComponent<Mine>();
            if (!mine.GetMark())
            {
                mine.SetMark(true);
                Debug.Log($"{visibleTarget.gameObject.name} was marked!");
            }
        }
        fow.visibleTargets.Clear();
        SetState(ExplorerState.Marking);
    }

    void SetState(ExplorerState newState)
    {
        state = newState;
    }

    IEnumerator ReachNewTarget()
    {
        yield return new WaitForSeconds(timeToReachNewTarget);
        randomPositionToExplore = GetRandomPositionToExplore();
        unit.GoTo(randomPositionToExplore);
    }

    public void ResetTarget()
    {
        isExploring = false;
    }

    Vector3 GetRandomPositionToExplore()
    {
        return new Vector3(
            UnityEngine.Random.Range(minPosition.x, maxPosition.x), 0f, 
            UnityEngine.Random.Range(minPosition.z, maxPosition.z));
    }


}

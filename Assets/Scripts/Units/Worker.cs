using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour
{
    enum WorkerState
    {
        Idle,
        Patrol,
        Mining,
        Returning
    }
    WorkerState state;

    [Header("Worker Settings")]
    public Vector3 minPosition;
    public Vector3 maxPosition;
    public float timeToReachNewTarget = 2f;

    private bool isBusy;
    public GameObject workingMine;
    private FieldOfView fow;
    private Vector3 randomPositionToExplore;
    private bool isExploring = false;
    private Unit unit;

    private void Awake()
    {
        fow = GetComponent<FieldOfView>();
        unit = GetComponent<Unit>();
    }

    void Start()
    {
        isBusy = false;
        unit.OnTargetReached += ResetTarget;
        randomPositionToExplore = transform.position;
        state = WorkerState.Idle;
        UnitsManager.Get().AddReadyWorker(gameObject);
    }

    private void Update()
    {
        switch (state)
        {
            case WorkerState.Idle:
                StartCoroutine("GoToPatrol");
                break;
            case WorkerState.Patrol:
                if (!isExploring)
                {
                    StartCoroutine("ReachNewTarget");
                    isExploring = true;
                }
                break;
            case WorkerState.Mining:

                if (workingMine && !isBusy)
                {
                    Debug.Log($"Going to {workingMine.name} with pos {workingMine.gameObject.transform.position}");
                    Debug.Log("MINING");
                    isBusy = true;
                }
                break;
            case WorkerState.Returning:
                break;
            default:
                break;
        }
    }

    IEnumerator GoToPatrol()
    {
        yield return new WaitForSeconds(5.0f);
        SetState(WorkerState.Patrol);
    }

    void SetState(WorkerState newState)
    {
        state = newState;
    }

    Vector3 GetRandomPositionToExplore()
    {
        return new Vector3(
            UnityEngine.Random.Range(minPosition.x, maxPosition.x), 0f,
            UnityEngine.Random.Range(minPosition.z, maxPosition.z));
    }

    IEnumerator ReachNewTarget()
    {     
        yield return new WaitForSeconds(timeToReachNewTarget);
        if (!isBusy)
        {
            randomPositionToExplore = GetRandomPositionToExplore();
            unit.GoTo(randomPositionToExplore);
        }
    }

    public void ResetTarget()
    {
        if(!isBusy)
        isExploring = false;
    }

    public bool IsBusy() => isBusy;

    public void GoToMine(GameObject mine)
    {
        workingMine = mine;
        Vector3 toGo = new Vector3(workingMine.transform.position.x, transform.position.y, workingMine.transform.position.z);
        unit.GoTo(toGo);
        SetState(WorkerState.Mining);
    }
}

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
    WorkersBase workersBase;

    [Header("Worker Settings")]
    public Vector3 minPosition;
    public Vector3 maxPosition;
    public float timeToReachNewTarget = 2f;
    public int goldExtractPerSecond = 1;

    private bool isBusy;
    [SerializeField]
    private GameObject workingMine;
    private Mine extractingMine;
    private FieldOfView fow;
    private Vector3 randomPositionToExplore;
    private bool isExploring = false;
    private Unit unit;

    public float timeToExtractGold = 2.0f;
    public int goldLimit = 50;
    private float toExtractTime;
    private bool canExtract = false;
    [SerializeField]
    private int unitGold = 0;
    private Vector3 depositPoint = new Vector3(1.4f, 0f, 15.8f);
    private void Awake()
    {
        fow = GetComponent<FieldOfView>();
        unit = GetComponent<Unit>();
    }

    void Start()
    {
        workersBase = WorkersBase.Get();
        toExtractTime = timeToExtractGold;
        isBusy = false;
        unit.OnTargetReached += ResetTarget;
        randomPositionToExplore = transform.position;
        state = WorkerState.Idle;
        UnitsManager.Get().AddReadyWorker(gameObject);
    }

    private void Update()
    {
        toExtractTime -= Time.deltaTime;
       
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
                if (workingMine)
                    SetState(WorkerState.Mining);
                break;
            case WorkerState.Mining:
                if (!workingMine) SetState(WorkerState.Patrol);
                if (unitGold >= goldLimit)
                {
                    SetState(WorkerState.Returning);
                    unit.GoTo(depositPoint);
                }
                else if(workingMine)
                {
                    if (Vector3.Distance(transform.position, workingMine.transform.position) < 2)
                        ExtractGoldFrom(workingMine);
                    else
                    {
                        if(workingMine)
                        GoToMine(workingMine);
                        Debug.Log($"{this.name} going to {workingMine.name}");
                        Debug.Log($"Worker {this.name} need to be near to extract");
                    }
                        
                }
                break;
            case WorkerState.Returning:
                // Go to base to deposit gold
                Debug.Log($"Worker {this.name} returning");
                
                if (unitGold > 0 && Vector3.Distance(transform.position, depositPoint) < 3)
                {
                    workersBase.DepositGold(unitGold);
                    unitGold = 0;
                }
                if(unitGold == 0 && workingMine)
                {
                    GoToMine(workingMine);
                    SetState(WorkerState.Mining);
                }
                else if(unitGold == 0 && !workingMine)
                {
                    isBusy = false;
                    isExploring = false;
                    SetState(WorkerState.Patrol);
                }
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
        if (workingMine)
        {

        }
        if (!isBusy)
        {
            randomPositionToExplore = GetRandomPositionToExplore();
            unit.GoTo(randomPositionToExplore);
        }
        else if(isBusy && unitGold < goldLimit)
        {
            SetState(WorkerState.Mining);
        }
        else if(unitGold > goldLimit)
        {
            SetState(WorkerState.Returning);
        }
    }

    public void ResetTarget()
    {
        if(!isBusy)
        isExploring = false;
    }

    public bool IsBusy() => isBusy;
    public void ExtractGoldFrom(GameObject mine)
    {
        if (unitGold < goldLimit)
        {
            if (toExtractTime <= 0 && extractingMine.currentGoldCapacity > 0)
            {
                extractingMine.StealGold(goldExtractPerSecond);
                toExtractTime = timeToExtractGold;
                unitGold += goldExtractPerSecond;
                Debug.Log($"Total Gold in worker {this.name}: {unitGold} " +
                    $"| Gold Stealed: -{goldExtractPerSecond} " +
                    $"Gold In Mine: {extractingMine.currentGoldCapacity}");
            }
        }
        if (!workingMine.GetComponent<Mine>().hasGold)
        {
            Destroy(workingMine);
            workingMine = null;
            UnitsManager.Get().AddReadyWorker(gameObject);
        }
    }
    public void GoToMine(GameObject mine)
    {
        workingMine = mine;
        extractingMine = mine.GetComponent<Mine>();
        Vector3 toGo = new Vector3(workingMine.transform.position.x, transform.position.y, workingMine.transform.position.z);
        unit.GoTo(toGo);
        Debug.Log($"{this.name} going to {workingMine.name}");
        SetState(WorkerState.Mining);
    }
}

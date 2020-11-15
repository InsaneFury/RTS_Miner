using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Explorer : MonoBehaviour
{
    public Vector3 minPosition;
    public Vector3 maxPosition;
    public float timeToReachNewTarget = 2f;
    private Vector3 randomPositionToExplore;
    private bool isExploring = false;
    private Unit unit;

    // Align to ground normal
    private RaycastHit hit;
    public Transform raycastPoint;
    private float hoverHeight = 1.0f;
    private float terrainHeight;
    private Vector3 pos;

    Action OnExplorerFoundMine;

    void Start()
    {
        unit = GetComponent<Unit>();
        unit.OnTargetReached += ResetTarget;
        randomPositionToExplore = new Vector3();
    }

    private void OnDestroy()
    {
        unit.OnTargetReached -= ResetTarget;
    }
    void Update()
    {
        //AlignWithGroundNormal();
        if (!isExploring)
        {
            StartCoroutine("ReachNewTarget");
            isExploring = true;
        }
        
    }

    IEnumerator ReachNewTarget()
    {
        yield return new WaitForSeconds(timeToReachNewTarget);
        randomPositionToExplore = GetRandomPositionToExplore();
        unit.CheckPathTo(randomPositionToExplore);
    }

    public void ResetTarget()
    {
        Debug.Log("Target Success");
        isExploring = false;
    }

    void AlignWithGroundNormal()
    {
        // Keep at specific height above terrain
        pos = transform.position;
        float terrainHeight = Terrain.activeTerrain.SampleHeight(pos);
        transform.position = new Vector3(pos.x,
                                         terrainHeight + hoverHeight,
                                         pos.z);

        // Rotate to align with terrain
        Physics.Raycast(raycastPoint.position, Vector3.down, out hit);
        transform.up -= (transform.up - hit.normal) * 0.1f;

    }

    Vector3 GetRandomPositionToExplore()
    {
        return new Vector3(
            UnityEngine.Random.Range(minPosition.x, maxPosition.x), 0f, 
            UnityEngine.Random.Range(minPosition.z, maxPosition.z));
    }
}

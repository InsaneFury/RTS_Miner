using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Unit : MonoBehaviour
{
    const float minPathUpdateTime = .2f;
    const float pathUpdateMoveThreshold = .5f;

    private Vector3 target;

    [Header("On Select")]
    public GameObject particleVFX;

    [Header("Unit Settings")]
    public float speed = 20.0f;
    public float turnDistance = 5.0f;
    public float turnSpeed = 3.0f;
    public float stoppingDistance = 10;

    public event Action OnTargetReached; 

    Path path;
    private void Start()
    {
        StartCoroutine(UpdatePath());
    }
    public void GoTo(Vector3 _target)
    {
        target = _target;
    }

    public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = new Path(waypoints,transform.position,turnDistance,stoppingDistance);
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
        else
        {
            OnTargetReached?.Invoke();
        }
    }

    IEnumerator UpdatePath()
    {
        if (Time.timeSinceLevelLoad < .3f)
        {
            yield return new WaitForSeconds(.3f);
        }
        PathRequestManager.RequestPath(new PathRequest(transform.position, target, OnPathFound));

        float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetPosOld = target;

        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);
            if((target - targetPosOld).sqrMagnitude > sqrMoveThreshold)
            {
                PathRequestManager.RequestPath(new PathRequest(transform.position, target, OnPathFound));
                targetPosOld = target;
            } 
           
        }
    }

    IEnumerator FollowPath()
    {
        bool followingPath = true;
        int pathIndex = 0;

        Vector3 relativePos = path.lookPoints[0] - transform.position;
        Quaternion toRotation = Quaternion.LookRotation(relativePos);
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 1 * Time.deltaTime);

        float speedPercent = 1;

        while (followingPath)
        {
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
            while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
            {
                if(pathIndex == path.finishLineIndex)
                {
                    followingPath = false;
                    OnTargetReached?.Invoke();
                    break;
                }
                else
                {
                    pathIndex++;
                }
            }

            if (followingPath)
            {
                if(pathIndex >= path.slowDownIndex && stoppingDistance > 0)
                {
                    speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos2D) / stoppingDistance);
                    if(speedPercent < 0.01f)
                    {
                        followingPath = false;
                    }
                }

                Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                transform.Translate(Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);
            }

            yield return null;
        }
    }

    public void OnDrawGizmos()
    {
        if(path != null)
        {
            path.DrawWithGizmos();
        }
    }

    public void OnSelect()
    {
        if(particleVFX)
            particleVFX.SetActive(true);
    }
    public void OnDeselect()
    {
        if (particleVFX)
            particleVFX.SetActive(false);
    }
}

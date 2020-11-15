using UnityEngine;
using System.Collections;
using RTS_Cam;

[RequireComponent(typeof(RTS_Camera))]
public class TargetSelector : MonoBehaviour 
{
    private RTS_Camera cam;
    private new Camera camera;
    public string targetsTag;

    private Unit lastTarget;

    private void Start()
    {
        cam = gameObject.GetComponent<RTS_Camera>();
        camera = gameObject.GetComponent<Camera>();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                if (hit.transform.CompareTag(targetsTag))
                {
                    cam.SetTarget(hit.transform);
                    lastTarget = hit.transform.GetComponent<Unit>();
                    if (lastTarget)
                        lastTarget.OnSelect();
                }
                else
                {
                    if (lastTarget)
                    {
                        lastTarget.OnDeselect();
                    }
                        
                    cam.ResetTarget();
                }
                    
            }
        }
    }
}

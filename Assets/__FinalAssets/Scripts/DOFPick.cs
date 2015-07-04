using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;

//[RequireComponent(typeof(DepthOfField))]
public class DOFPick : MonoBehaviour 
{
    public float minDistance = 5;
    public float maxDistance = 33;
    float distanceSpread;

    public float minSize= 0.35f;
    public float maxSize = 1.5f;
    float sizeSpread;

    public DepthOfField dof;
    Camera[] cameras = new Camera[2];

    Vector3 lastMousePos;

    void Awake()
    {
       dof = GetComponent<DepthOfField>();
       cameras = GetComponentsInChildren<Camera>();
       distanceSpread = maxDistance - minDistance;
       sizeSpread = maxSize - minSize;
    }

    void Update () 
    {
        if (lastMousePos == Input.mousePosition) return;
        lastMousePos = Input.mousePosition;

        if (mousePick(Camera.main)) return;
        foreach(var c in cameras)
            if (mousePick(c)) break;
    }

    bool mousePick(Camera cam)
    {
        var hit = new RaycastHit();
        
        var ray = cam.ScreenPointToRay(lastMousePos);
        
        if (Physics.Raycast(ray, out hit, 100.0f, 1 << LayerMask.NameToLayer("Terrain")))
        {
            //Debug.Log(hit.distance);
            var offset = Mathf.Clamp(hit.distance + cam.nearClipPlane, minDistance, maxDistance);
            dof.focalLength = offset;
            dof.focalSize = sizeSpread * ((offset - minDistance) / distanceSpread) + minSize;
            return true;
        }
        return false;
    }
}

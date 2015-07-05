using UnityEngine;
using System.Collections;

public class DOFFocalPoint : MonoBehaviour 
{
    public Camera MainCamera, PortalCamera;
    public Transform PortalFocalPoint;
    public LayerMask MainMask, PortalMask;

    public float moveTime = 1.0f;
    float moveDistance, portalMoveDistance;

    float m_BackgroundClickPlaneDistance;
    MeshRenderer m_MainRenderer, m_PortalRenderer;

    Vector3 lastMousePos;
    Vector3 mainTargetPos, portalTargetPos;

    void Awake()
    {
        m_MainRenderer = GetComponent<MeshRenderer>();
        m_PortalRenderer = PortalFocalPoint.GetComponent<MeshRenderer>();

        m_BackgroundClickPlaneDistance = 250.0f;
        //var bcp = GameObject.Find("Background Click Plane").transform;
        //m_BackgroundClickPlaneDistance = Mathf.Sqrt(Mathf.Pow((bcp.position - MainCamera.transform.position).z,2) + Mathf.Pow(bcp.localScale.x * 0.5f, 2));
        //Debug.Log(m_BackgroundClickPlaneDistance);
    }

    void Start()
    {
        lastMousePos = Input.mousePosition;
    }

	void Update () 
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            m_MainRenderer.enabled = m_PortalRenderer.enabled = !m_MainRenderer.enabled;     
        }

        if(lastMousePos != Input.mousePosition)
        {
            lastMousePos = Input.mousePosition;
            CastRay();
        }

        var moveSpeed = moveDistance / moveTime * Time.deltaTime;
        var portalMoveSpeed = portalMoveDistance / moveTime * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, mainTargetPos, moveSpeed);
        PortalFocalPoint.position = Vector3.MoveTowards(PortalFocalPoint.position, portalTargetPos, portalMoveSpeed);
	}

    void CastRay()
    {
        var hit = new RaycastHit();

        if (Physics.Raycast(MainCamera.ScreenPointToRay(lastMousePos), out hit, m_BackgroundClickPlaneDistance, MainMask))
        {
            mainTargetPos = hit.point;
            moveDistance = Vector3.Distance(mainTargetPos, transform.position);
            //transform.position = hit.point;

            if (hit.collider.tag == "Portal")
            {
                //var portalHit = new RaycastHit();
                if (Physics.Raycast(PortalCamera.ScreenPointToRay(lastMousePos), out hit, m_BackgroundClickPlaneDistance, PortalMask))
                {
                    portalTargetPos = hit.point;
                    //PortalFocalPoint.position = hit.point;
                }
            }
            else
            {
                portalTargetPos = hit.point + (PortalCamera.transform.position - MainCamera.transform.position);
                //PortalFocalPoint.position = hit.point + (PortalCamera.transform.position - MainCamera.transform.position);
            }

            portalMoveDistance = Vector3.Distance(portalTargetPos, PortalCamera.transform.position);
        }
    }
}

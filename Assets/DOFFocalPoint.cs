using UnityEngine;
using System.Collections;

public class DOFFocalPoint : MonoBehaviour 
{
    public Camera MainCamera, PortalCamera;
    public Transform PortalFocalPoint;
    public LayerMask MainMask, PortalMask;

    float m_BackgroundClickPlaneDistance;
    MeshRenderer m_MainRenderer, m_PortalRenderer;

    void Awake()
    {
        m_MainRenderer = GetComponent<MeshRenderer>();
        m_PortalRenderer = PortalFocalPoint.GetComponent<MeshRenderer>();

        m_BackgroundClickPlaneDistance = 250.0f;
        //var bcp = GameObject.Find("Background Click Plane").transform;
        //m_BackgroundClickPlaneDistance = Mathf.Sqrt(Mathf.Pow((bcp.position - MainCamera.transform.position).z,2) + Mathf.Pow(bcp.localScale.x * 0.5f, 2));
        //Debug.Log(m_BackgroundClickPlaneDistance);
    }
	void Update () 
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            m_MainRenderer.enabled = m_PortalRenderer.enabled = !m_MainRenderer.enabled;     
        }

        var hit = new RaycastHit();

        if (Physics.Raycast(MainCamera.ScreenPointToRay(Input.mousePosition), out hit, m_BackgroundClickPlaneDistance, MainMask))
        {
            transform.position = hit.point;

            if (hit.collider.tag == "Portal")
            {
                //var portalHit = new RaycastHit();
                if (Physics.Raycast(PortalCamera.ScreenPointToRay(Input.mousePosition), out hit, m_BackgroundClickPlaneDistance, PortalMask))
                {
                    PortalFocalPoint.position = hit.point;
                }
            }
            else
            {
                PortalFocalPoint.position = hit.point + (PortalCamera.transform.position - MainCamera.transform.position);
            }
        }
	
	}
}

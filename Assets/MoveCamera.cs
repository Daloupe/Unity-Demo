using UnityEngine;
using System.Collections;

public class MoveCamera : MonoBehaviour 
{
    public GameObject PortalMesh;

    float m_moveSpeed = 0.1f;
    float m_rotateSpeed = 1.0f;
    
    void Start()
    {
        transform.LookAt(PortalMesh.transform);
    }

	void LateUpdate () 
    {
        // Move Forward and Backward
	    if(Input.GetKey(KeyCode.W))
        {
            transform.position = transform.position + (transform.forward * m_moveSpeed);
        }
        else if(Input.GetKey(KeyCode.S))
        {
            transform.position = transform.position + (-transform.forward * m_moveSpeed);
        }


        // Strafe Left and Right
        if (Input.GetKey(KeyCode.A))
        {
            transform.position = transform.position + (-transform.right * m_moveSpeed);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.position = transform.position + (transform.right * m_moveSpeed);
        }


        // Rotate Left and Right
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(Vector3.up, -m_rotateSpeed);//position = transform.position + (-transform.right * speed);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(Vector3.up, m_rotateSpeed); //transform.position = transform.position + (transform.right * speed);
        }

        // Quit
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
	}
}

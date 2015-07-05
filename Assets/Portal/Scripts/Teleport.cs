// Copyright (C) Stanislaw Adaszewski, 2013
// http://algoholic.eu

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class Teleport : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    float m_moveSpeed = 0.1f;
    public Transform CameraMover;
    public Light BackgroundGlow;

    public GameObject Parent;
    public Transform OtherEnd;

    HashSet<Collider> colliding = new HashSet<Collider>();

    bool m_Walking;
    float m_CurrentIntensity = 0.0f;
    public float MaxIntensity = 1.0f;
    public float FadeSpeed = 1.0f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (m_Walking)
        {
            CameraMover.LookAt(transform);
            CameraMover.position = CameraMover.position + (CameraMover.forward * m_moveSpeed);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered");

        if (!colliding.Contains(other))
        {
            m_Walking = false;
            //Quaternion q1 = Quaternion.FromToRotation(transform.up, OtherEnd.up);
            //Quaternion q2 = Quaternion.FromToRotation(-transform.up, OtherEnd.up);

            //Vector3 newPos = OtherEnd.position + q2 * (other.transform.position - transform.position);// + OtherEnd.transform.up * 2;;

            ////if (other.GetComponent<Rigidbody>() != null) {
            ////    GameObject o = (GameObject) GameObject.Instantiate(other.gameObject, newPos, other.transform.localRotation);
            ////    o.GetComponent<Rigidbody>().velocity = q2 * other.GetComponent<Rigidbody>().velocity;
            ////    o.GetComponent<Rigidbody>().angularVelocity = other.GetComponent<Rigidbody>().angularVelocity;
            ////    other.gameObject.SetActive(false);
            ////    Destroy(other.gameObject);
            ////    other = o.GetComponent<Collider>();
            ////}

            //OtherEnd.GetComponent<Teleport>().colliding.Add(other);

            other.transform.position = OtherEnd.position;// newPos;

            //Vector3 fwd = other.transform.forward;

            //if (other.GetComponent<Rigidbody>() == null)
            //{
            //    other.transform.LookAt(other.transform.position + q2 * fwd, OtherEnd.transform.forward);
            //}

            Parent.GetComponent<PlayMakerFSM>().Fsm.Event("HidePortal");//.SetActive(false);


        }
    }

    void OnTriggerExit(Collider other)
    {
        colliding.Remove(other);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        m_Walking = true;
        StartCoroutine(DecGlow());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (m_Walking) return;
        StopAllCoroutines();
        StartCoroutine(IncGlow());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (m_Walking) return;
        StopAllCoroutines();
        StartCoroutine(DecGlow());
    }

    IEnumerator IncGlow()
    {
        float elapsedTime = 0;
        float time = FadeSpeed;// *(m_CurrentIntensity / MaxIntensity);
        float startIntensity = m_CurrentIntensity;

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            m_CurrentIntensity = Mathf.Lerp(startIntensity, MaxIntensity, (elapsedTime / time));
            BackgroundGlow.intensity = m_CurrentIntensity;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator DecGlow()
    {
        float elapsedTime = 0;
        float time = FadeSpeed;// *(m_CurrentIntensity / MaxIntensity);
        float startIntensity = m_CurrentIntensity;

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            m_CurrentIntensity = Mathf.Lerp(startIntensity, 0.0f, (elapsedTime / time));
            BackgroundGlow.intensity = m_CurrentIntensity;
            yield return new WaitForEndOfFrame();
        }
    }
}

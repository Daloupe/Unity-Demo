using System;
using UnityEngine;


public class PortalParticles : MonoBehaviour
{
    public float maxPower = 20;
    public float minPower = 5;
    public float changeSpeed = 5;
    public ParticleSystem[] hoseWaterSystems;
    public Renderer systemRenderer;

    public bool emit;
    //public bool shutOff;

    private float m_Power;

    void Start()
    {
        foreach (var system in hoseWaterSystems)
        {
            //system.startSpeed = m_Power;
            system.enableEmission = false;
        }
    }
    // Update is called once per frame
    private void Update()
    {
        m_Power = Mathf.Lerp(m_Power, emit ? maxPower : minPower, Time.deltaTime * changeSpeed);

        foreach (var system in hoseWaterSystems)
        {
            system.startSpeed = m_Power;
            system.enableEmission = (m_Power > minPower * 1.1f);
        }
    }

    public void InstantShutOff()
    {
        m_Power = minPower;
        emit = false;

        foreach (var system in hoseWaterSystems)
        {
            system.enableEmission = false;
        }
    }
}
using System;
using UnityEngine;


namespace UnityStandardAssets.Effects
{
    public class Hose : MonoBehaviour
    {
        public float maxPower = 20;
        public float minPower = 5;
        public float changeSpeed = 5;
        public ParticleSystem[] hoseWaterSystems;
        public Renderer systemRenderer;

        //public bool emit;
        //public bool shutOff;

        private float m_Power;


        // Update is called once per frame
        private void Update()
        {
            //if (shutOff)
            //{

            //    emit = false;
            //    m_Power = 0;
            //    shutOff = false;

            //    foreach (var system in hoseWaterSystems)
            //    {
            //        system.startSpeed = 0;
            //        system.enableEmission = false;
            //    }
            //}
            //else
            //{
            m_Power = Mathf.Lerp(m_Power, Input.GetMouseButtonDown(0) ? maxPower : minPower, Time.deltaTime * changeSpeed);

            foreach (var system in hoseWaterSystems)
            {
                system.startSpeed = m_Power;
                system.enableEmission = (m_Power > minPower * 1.1f);
            }
            //}
            //if (Input.GetKeyDown(KeyCode.Alpha1))
            //{
            //    systemRenderer.enabled = !systemRenderer.enabled;
            //}


        }

        //public void InstantShutOff()
        //{
        //    m_Power = 0;
        //    emit = false;
        //}
    }
}

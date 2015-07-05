using UnityEngine;
using System.Collections;

public class PortalGroupScript : MonoBehaviour 
{
    public PortalParticles particleScript;

    public void ShutOffParticles()
    {
        particleScript.InstantShutOff();
    }
}

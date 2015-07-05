using UnityEngine;
using System.Collections;

public class ZDepthChange : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
        GetComponent<Renderer>().materials[0].SetInt("_ZWrite", 1);
        GetComponent<Renderer>().materials[1].SetInt("_ZWrite", 1);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

﻿using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
	void Update () 
    {
        // Quit
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
	}
}
﻿using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)){
			Application.Quit();
		}
	}
}

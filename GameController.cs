﻿using UnityEngine;
using System.Collections;

/**
 * Classe permettant de gérer la fermeture du jeu
 */
public class GameController : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)){
			Application.Quit();
		}
	}
}

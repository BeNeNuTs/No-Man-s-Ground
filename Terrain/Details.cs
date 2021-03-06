﻿using UnityEngine;
using System.Collections;

/**
 * Classe définissant les attributs d'un détail sur le Terrain.
 */
[System.Serializable]
public class Details {

	public static int DETAIL_RESOLUTION = 256;		//Recommended 128 to 512
	public static int DETAIL_PER_PATCH = 16;		//Recommended 16
	
	public Texture2D[] grassTextures;
	public GameObject[] bushsMeshes;
	[Range(0,1f)]
	public float grassProbability;
	[Range(0,3f)]
	public float minGrass;
	[Range(0,5f)]
	public float maxGrass;

	public float minWidth = 1;
	public float maxWidth = 2;
	public float minHeight = 1;
	public float maxHeight = 2;
	[Range(0,1)]
	public float noiseSpread = 0.1f;
	public Color dryColor = new Color(0.26f,0.98f,0.14f);
	public Color healthyColor = new Color(0.8f,0.73f,0.1f);
}

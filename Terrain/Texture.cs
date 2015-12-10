using UnityEngine;
using System.Collections;

/**
 * Classe définissant les attributs des textures posées sur le Terrain.
 */
[System.Serializable]
public class Texture {

	//Types des textures.
	public static int SAND = 0, GRASSHILL = 1, GRASSROCKY = 2, MUDROCKY = 3;

	public Texture2D texture;
	public Texture2D normalMap;
	[Range(0,100)]
	public int minHeight;
	[Range(0,100)]
	public int maxHeight;
}

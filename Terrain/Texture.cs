using UnityEngine;
using System.Collections;

[System.Serializable]
public class Texture {

	public static int SAND = 0, GRASSHILL = 1, GRASSROCKY = 2, MUDROCKY = 3;

	public Texture2D texture;
	public Texture2D normalMap;
	[Range(0,100)]
	public int minHeight;
	[Range(0,100)]
	public int maxHeight;
}

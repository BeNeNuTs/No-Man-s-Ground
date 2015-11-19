using UnityEngine;
using System.Collections;

[System.Serializable]
public class Texture {

	public Texture2D texture;
	public Texture2D normalMap;
	[Range(0,100)]
	public int minHeight;
	[Range(0,100)]
	public int maxHeight;
}

using UnityEngine;
using System.Collections;

[System.Serializable]
public class Tree {
	
	public GameObject[] treesGamobject;
	[Range(0,100)]
	public int minTree;
	[Range(0,100)]
	public int maxTree;
}

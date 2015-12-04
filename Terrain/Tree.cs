using UnityEngine;
using System.Collections;

[System.Serializable]
public class Tree {
	
	public GameObject[] treesGamobject;
	[Range(0,1f)]
	public float treeProbability;
	[Range(0,0.1f)]
	public float minTree;
	[Range(0,0.5f)]
	public float maxTree;
}

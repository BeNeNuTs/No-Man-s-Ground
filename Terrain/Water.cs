using UnityEngine;
using System.Collections;

[System.Serializable]
public class Water {
	
	public GameObject waterGameobject;

	[Range(0,1f)]
	public float waterProbability = 0.1f;
	public float minPosY = -1;

	[Range(0,50)]
	public float offsetY = 0.5f;
}

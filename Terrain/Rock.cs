using UnityEngine;
using System.Collections;

[System.Serializable]
public class Rock {

	public GameObject[] rocksPrefab;
	public float rockProbability = 0.1f;
	[Range(0,0.1f)]
	public float minRock;
	[Range(0,0.5f)]
	public float maxRock;

	public float offsetY = 1f;

	private GameObject[] rocks = new GameObject[0];

	public void Init(){
		for(int i = 0 ; i < rocks.Length ; i++){
			GameObject.Destroy(rocks[i]);
		}
	}

	public GameObject[] Generate(int numberOfRocks){
		rocks = new GameObject[numberOfRocks];
		for(int i = 0 ; i < numberOfRocks ; i++){
			int rand = Random.Range(0, rocksPrefab.Length-1);
			rocks[i] = GameObject.Instantiate(rocksPrefab[rand], Vector3.zero, rocksPrefab[rand].transform.rotation) as GameObject; 
		}

		return rocks;
	}

	public GameObject[] GetRocks{
		get{
			return rocks;
		}
	}
}

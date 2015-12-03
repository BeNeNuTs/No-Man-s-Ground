using UnityEngine;
using System.Collections;

public class Trigger : MonoBehaviour {

	void OnTriggerEnter(Collider other){
		if(other.CompareTag("Player")){
			other.transform.position = new Vector3(127, 60, 127);
		}else{
			Destroy(other);
		}
	}
}

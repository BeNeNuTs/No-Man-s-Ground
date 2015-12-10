using UnityEngine;
using System.Collections;

/**
 * Classe permettant de prévenir la chute du joueur du Terrain.
 * Ou bien de certains objet qui pourrait sortir de la carte et 
 * ainsi tomber à l'infini.
 */
public class Trigger : MonoBehaviour {

	void OnTriggerEnter(Collider other){
		if(other.CompareTag("Player")){
			other.transform.position = new Vector3(127, 60, 127);
		}else{
			Destroy(other.gameObject);
		}
	}
}

using UnityEngine;
using System.Collections;

/**
 * Classe permettant de suivre le joueur.
 */
public class FollowPlayer : MonoBehaviour {
	
	public float offsetY;

	Transform player;

	void Start(){
		player = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	void FixedUpdate () {
		transform.position = new Vector3(player.position.x, player.position.y + offsetY, player.position.z);
	}
}

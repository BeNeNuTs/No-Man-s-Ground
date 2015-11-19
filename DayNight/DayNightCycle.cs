using UnityEngine;
using System.Collections;

public class DayNightCycle : MonoBehaviour {

	public Light directionalLight;
	public float speed;

	// Update is called once per frame
	void Update () {
		directionalLight.transform.Rotate(new Vector3(-speed * Time.deltaTime, 0, 0));
	}
}

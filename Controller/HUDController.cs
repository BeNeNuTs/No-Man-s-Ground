using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HUDController : MonoBehaviour {

	public TerrainGenerator tGenerator;
	public Light[] flashLights;

	public Sprite[] Sheightmaps;
	public Texture2D[] Theightmaps;
	private int index;

	private Canvas canvas;
	private Image img;

	private bool isShow;

	public GameObject HeightMapSelection;
	public GameObject HUD;
	public List<GameObject> HUDs;

	int indexHUD;


	void Start(){

		indexHUD = 1;

		index = 0;
		isShow = false;

		canvas = GetComponent<Canvas>();
		img = HeightMapSelection.GetComponentInChildren<Image>();

		UpdateImg();
	}

	void Update(){
		if (Input.GetKeyDown (KeyCode.A)) 
		{
			StartCoroutine(RotateHUD(false,0.5f));
		}

		if (Input.GetKeyDown (KeyCode.E)) 
		{
			StartCoroutine(RotateHUD(true,0.5f));
		}

		if(Input.GetKeyDown(KeyCode.H)){
			ToggleHUD();
		}else if(isShow && Input.GetKeyDown(KeyCode.LeftArrow)){
			index--;
			if(index < 0){
				index = Sheightmaps.Length - 1;
			}


			UpdateImg();
		}else if(isShow && Input.GetKeyDown(KeyCode.RightArrow)){
			index++;
			if(index > Sheightmaps.Length - 1){
				index = 0;
			}

			UpdateImg();
		}else if(Input.GetKeyDown(KeyCode.B)){
			tGenerator.Generate();
		}else if(Input.GetKeyDown(KeyCode.L)){
			flashLights[0].enabled = !flashLights[0].enabled;
			flashLights[1].enabled = !flashLights[1].enabled;
		}
	}

	IEnumerator Coroutine_ToggleHUD(){
		float newFill = 0f;
		if(!isShow){
			newFill = 1f;
		}
		float oldFill = img.fillAmount;
		
		float fill = img.fillAmount;
		
		float time = 1f;
		float elapsedTime = 0f;
		while (elapsedTime < time) {
			fill = Mathf.Lerp(oldFill, newFill, elapsedTime / time);
			img.fillAmount = fill;
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		img.fillAmount = newFill;

		ToggleHUD();
	}

	IEnumerator RotateHUD(bool direction, float time)
	{

		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		Vector3 toCurrentHud = HUDs[indexHUD].transform.position - player.transform.position;

		int nextIndex = 0;
		if (direction)
			nextIndex = 1;
		else
			nextIndex = -1;

		if (indexHUD + nextIndex > 2 || indexHUD + nextIndex < 0) {
			yield break;
		} 
		else 
		{
			indexHUD += nextIndex;
		}

		Vector3 toNextHud = HUDs[indexHUD].transform.position - player.transform.position;

		float angle = Vector3.Angle (toCurrentHud, toNextHud);

		float progression = 0;
		while (progression < 1.0f) 
		{
			yield return new WaitForSeconds(Time.deltaTime);
			progression += Time.deltaTime/time;
			float currentAngle = angle*Time.deltaTime/time;
			HUD.transform.RotateAround(player.transform.position, Camera.main.transform.up, currentAngle*nextIndex*-1);
		}

	}

	void ToggleHUD(){
		HeightMapSelection.SetActive(!isShow);

		isShow = !isShow;
	}

	void UpdateImg(){
		img.sprite = Sheightmaps[index];
		tGenerator.hMap = Theightmaps[index];
	}
}

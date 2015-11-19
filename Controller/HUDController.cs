using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDController : MonoBehaviour {

	public TerrainGenerator tGenerator;
	public Light[] flashLights;

	public Sprite[] Sheightmaps;
	public Texture2D[] Theightmaps;
	private int index;

	private Canvas canvas;
	private Image img;

	private bool isShow;

	void Start(){
		index = 0;
		isShow = false;

		canvas = GetComponent<Canvas>();
		img = GetComponentInChildren<Image>();

		UpdateImg();
	}

	void Update(){
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

	void ToggleHUD(){
		canvas.enabled = !isShow;
		isShow = !isShow;
	}

	void UpdateImg(){
		img.sprite = Sheightmaps[index];
		tGenerator.hMap = Theightmaps[index];
	}
}

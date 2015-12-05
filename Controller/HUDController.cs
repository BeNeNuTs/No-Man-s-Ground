using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HUDController : MonoBehaviour {

	public enum State { SEASON = 0, HEIGHTMAP = 1, WEATHER = 2 }

	[System.Serializable]
	public class DayNightCycle{
		public DayNightController dayNightScript;
		public Text timeMultiplierText;
	}

	[System.Serializable]
	public class Weather{
		public string name;
		public GameObject season;
		public GameObject[] weathers;
	}

	public TerrainGenerator tGenerator;
	public Light[] flashLights;

	public Sprite[] Sheightmaps;
	public Texture2D[] Theightmaps;

	public GameObject SeasonSelection;
	public GameObject HeightMapSelection;
	public GameObject WeatherSelection;

	public Image img;

	public GameObject[] seasons;
	int current_season = 0;

	public GameObject HUD;
	public List<GameObject> HUDs;

	public DayNightCycle dayNight;
	
	public Weather[] weathers;
	int current_weather = 0;

	private int index;

	private int indexHUD;

	private float time = 0f;
	private float cooldown = 0.3f;
	
	private float rotateHUDtime = 0f;
	private float rotateHUDCooldown = 0.5f;

	private float hourTime = 0f;
	private float hourCooldown = 0.3f;

	private State state = State.HEIGHTMAP;

	void Start(){

		indexHUD = 1;

		index = 0;

		UpdateImg();
	}

	void Update(){
		time += Time.deltaTime;
		rotateHUDtime += Time.deltaTime;
		hourTime += Time.deltaTime;

		// CHANGE TIME ////////////
		if (Input.GetAxis("ArrowsV") == 1 && hourTime > hourCooldown) 
		{
			dayNight.dayNightScript.timeMultiplier += 0.5f;
			if(dayNight.dayNightScript.timeMultiplier > 2f){
				dayNight.dayNightScript.timeMultiplier = 2f;
			}
			UpdateTimeMultiplier();
		}else if (Input.GetAxis("ArrowsV") == -1 && hourTime > hourCooldown) 
		{
			dayNight.dayNightScript.timeMultiplier -= 0.5f;
			if(dayNight.dayNightScript.timeMultiplier < 0f){
				dayNight.dayNightScript.timeMultiplier = 0f;
			}
			UpdateTimeMultiplier();
		}
		//////////////////////////

		// CHANGE HUD ////////////
		if (Input.GetButtonDown("LB") && rotateHUDtime > rotateHUDCooldown + 0.1f) 
		{
			StartCoroutine(RotateHUD(false,rotateHUDCooldown));
			rotateHUDtime = 0f;
		}else if (Input.GetButtonDown("RB") && rotateHUDtime > rotateHUDCooldown + 0.1f) 
		{
			StartCoroutine(RotateHUD(true,rotateHUDCooldown));
			rotateHUDtime = 0f;
		}
		////////////////////////// 

		// UPDATE HUD ////////////
		if(Input.GetButtonDown("HUD")){
			ToggleHUD();
		}else if(Input.GetAxis("ArrowsH") == -1 && time > cooldown){
			if(state == State.SEASON && SeasonSelection.activeSelf){
				PreviousSeason();
			}else if(state == State.HEIGHTMAP && HeightMapSelection.activeSelf){
				index--;
				if(index < 0){
					index = Sheightmaps.Length - 1;
				}
				
				UpdateImg();
			}else if(state == State.WEATHER && WeatherSelection.activeSelf){
				PreviousWeather();
			}

			time = 0f;
		}else if(Input.GetAxis("ArrowsH") == 1 && time > cooldown){
			if(state == State.SEASON && SeasonSelection.activeSelf){
				NextSeason();
			}else if(state == State.HEIGHTMAP && HeightMapSelection.activeSelf){
				index++;
				if(index > Sheightmaps.Length - 1){
					index = 0;
				}
				
				UpdateImg();

			}else if(state == State.WEATHER && WeatherSelection.activeSelf){
				NextWeather();
			}

			time = 0f;
		}
		//////////////////////////

		// GENERATE & LIGHT //////
		if(Input.GetButtonDown("Generate")){
			if(state == State.SEASON && SeasonSelection.activeSelf){
				tGenerator.SetSeason(current_season);
				InitWeatherIcon();
			}else if(state == State.HEIGHTMAP && HeightMapSelection.activeSelf){
				tGenerator.Generate();
				ToggleHUD();
			}else if(state == State.WEATHER && WeatherSelection.activeSelf){
				tGenerator.SetWeather(current_weather);
			}
		}else if(Input.GetButtonDown("Light")){
			flashLights[0].enabled = !flashLights[0].enabled;
			flashLights[1].enabled = !flashLights[1].enabled;
		}
		//////////////////////////
	}

	IEnumerator Coroutine_ToggleHUD(){
		/*float newFill = 0f;
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

		ToggleHUD();*/
		yield break;
	}

	IEnumerator RotateHUD(bool direction, float time)
	{
		if(state == State.WEATHER && direction){
			yield break;
		}else if(state == State.SEASON && !direction){
			yield break;
		}

		//Vector3 toCurrentHud = HUDs[indexHUD].transform.position - player.transform.position;

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

		//Vector3 toNextHud = HUDs[indexHUD].transform.position - player.transform.position;

		//float angle = Vector3.Angle (toCurrentHud, toNextHud);
		float angle = 52f;

		float progression = 0;
		while (progression < 1.0f) 
		{
			progression += Time.deltaTime/time;
			float currentAngle = angle*Time.deltaTime/time;
			HUD.transform.Rotate(new Vector3(0,currentAngle*nextIndex*-1,0));
			yield return new WaitForSeconds(Time.deltaTime);
		}

		if(direction){
			NextState();
		}else{
			PreviousState();
		}

		if(state == State.SEASON){
			HUD.transform.localEulerAngles = new Vector3(0,angle,0);
		}else if(state == State.HEIGHTMAP){
			HUD.transform.localEulerAngles = Vector3.zero;
		}else{
			HUD.transform.localEulerAngles = new Vector3(0,-angle,0);
		}

		ToggleHUD();
	}

	void ToggleHUD(){
		if(state == State.SEASON){
			SeasonSelection.SetActive(!SeasonSelection.activeSelf);
		}else if(state == State.HEIGHTMAP){
			HeightMapSelection.SetActive(!HeightMapSelection.activeSelf);
		}else if(state == State.WEATHER){
			WeatherSelection.SetActive(!WeatherSelection.activeSelf);
		}
	}

	void UpdateImg(){
		img.sprite = Sheightmaps[index];
		tGenerator.hMap = Theightmaps[index];
	}

	void UpdateTimeMultiplier(){
		if(dayNight.dayNightScript.timeMultiplier > 0f){
			dayNight.timeMultiplierText.enabled = true;

			dayNight.timeMultiplierText.text = "+"+dayNight.dayNightScript.timeMultiplier.ToString();
		}else{
			dayNight.timeMultiplierText.enabled = false;
		}
		hourTime = 0f;
	}

	void NextState(){
		if(state == State.SEASON){
			SeasonSelection.SetActive(false);
			state = State.HEIGHTMAP;
		}else if(state == State.HEIGHTMAP){
			HeightMapSelection.SetActive(false);
			state = State.WEATHER;
			current_weather = (int)tGenerator.season.seasons[tGenerator.season.CurrentSeason].particle.strenghParticle;
			UpdateWeatherIcon(current_weather);
		}
	}

	void PreviousState(){
		if(state == State.WEATHER){
			WeatherSelection.SetActive(false);
			state = State.HEIGHTMAP;
		}else if(state == State.HEIGHTMAP){
			HeightMapSelection.SetActive(false);
			state = State.SEASON;
			current_season = tGenerator.season.CurrentSeason;
			UpdateSeasonIcon(current_season);
		}
	}

	void NextSeason(){
		current_season++;
		if(current_season > (seasons.Length - 1)){
			current_season = 0;
		}

		UpdateSeasonIcon(current_season);
	}
	
	void PreviousSeason(){
		current_season--;
		if(current_season < 0){
			current_season = (seasons.Length - 1);
		}

		UpdateSeasonIcon(current_season);
	}

	void UpdateSeasonIcon(int current){
		seasons[current].SetActive(true);

		for(int i = 0 ; i < seasons.Length ; i++){
			if(i != current){
				seasons[i].SetActive(false);
			}
		}
	}

	void NextWeather(){
		current_weather++;
		if(current_weather > (weathers[tGenerator.season.CurrentSeason].weathers.Length - 1)){
			current_weather = 0;
		}
		
		UpdateWeatherIcon(current_weather);
	}
	
	void PreviousWeather(){
		current_weather--;
		if(current_weather < 0){
			current_weather = (weathers[tGenerator.season.CurrentSeason].weathers.Length - 1);
		}
		
		UpdateWeatherIcon(current_weather);
	}

	void InitWeatherIcon(){
		current_weather = 0;
		
		for(int i = 0 ; i < weathers.Length ; i++){
			weathers[i].season.SetActive(false);
			for(int j = 0 ; j < weathers[i].weathers.Length ; j++){
				weathers[i].weathers[j].SetActive(false);
			}
			weathers[i].weathers[0].SetActive(true);
		}
	}
	
	void UpdateWeatherIcon(int current){
		weathers[tGenerator.season.CurrentSeason].season.SetActive(true);
		weathers[tGenerator.season.CurrentSeason].weathers[current].SetActive(true);
		
		for(int i = 0 ; i < weathers[tGenerator.season.CurrentSeason].weathers.Length ; i++){
			if(i != current){
				weathers[tGenerator.season.CurrentSeason].weathers[i].SetActive(false);
			}
		}
	}
}

using UnityEngine;
using System.Collections;

/**
 * Classe définissant les différentes saisons.
 */
public class Seasons : MonoBehaviour {

	[System.Serializable]
	public class Season {
		public string seasonName;

		public Texture[] textures;
		public Tree trees;
		public Details details;
		public Particles particle;
		
		public Texture2D baseTexture;
		public Texture2D baseNormalMap;
	}

	public Material skybox;
	public Wind wind;
	public Water water;
	public Rock rock;
	public Season[] seasons;

	int current_season;

	/**
	 * Initialise la saison courante à été.
	 */
	void Awake(){
		current_season = 0;
	}

	/**
	 * Permet d'accéder à la saison courante.
	 */
	public int CurrentSeason {
		get{
			return current_season;
		}
	}

	/**
	 * Change la saison courante par la saison suivante suivant l'ordre.
	 */
	public void NextSeason() {
		current_season++;
		if(current_season > (seasons.Length - 1)){
			current_season = 0;
		}

		if(seasons[current_season].seasonName == "Autumn"){
			StartCoroutine(ChangeSkyboxColor(new Color(0.35f,0.35f,0.35f, 1f)));
		}else if(seasons[current_season].seasonName == "Winter"){
			StartCoroutine(ChangeSkyboxColor(new Color(0f,0f,0f, 1f)));
		}else{
			StartCoroutine(ChangeSkyboxColor(new Color(0.5f,0.5f,0.5f, 1f)));
		}
	}

	/**
	 * Change la saison courante par celle passée en paramètre.
	 */
	public void SetSeason(int current) {
		current_season = current;
		
		if(seasons[current_season].seasonName == "Autumn"){
			StartCoroutine(ChangeSkyboxColor(new Color(0.35f,0.35f,0.35f, 1f)));
		}else if(seasons[current_season].seasonName == "Winter"){
			StartCoroutine(ChangeSkyboxColor(new Color(0f,0f,0f, 1f)));
		}else{
			StartCoroutine(ChangeSkyboxColor(new Color(0.5f,0.5f,0.5f, 1f)));
		}
	}

	/**
	 * Change la couleur du ciel en fonction de la saison.
	 */
	IEnumerator ChangeSkyboxColor(Color color){
		Color oldColor = skybox.GetColor("_SkyTint");
		
		Color c;
		float time = 5f;
		float elapsedTime = 0f;
		while (elapsedTime < time) {
			c = Color.Lerp(oldColor, color, elapsedTime / time);
			skybox.SetColor("_SkyTint", c);
			elapsedTime += Time.deltaTime;
			yield return new WaitForSeconds(Time.deltaTime);
		}
		
		skybox.SetColor("_SkyTint", color);
	}
}

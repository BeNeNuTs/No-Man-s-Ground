using UnityEngine;
using System.Collections;

public class Seasons : MonoBehaviour {

	[System.Serializable]
	public class Season {
		public string seasonName;

		public Texture[] textures;
		public Tree trees;
		public Details details;
		public Wind wind;
		public Water water;
		
		public Texture2D baseTexture;
		public Texture2D baseNormalMap;
	}
	public Season[] seasons;

	int current_season;

	void Awake(){
		current_season = 0;
	}

	public int CurrentSeason {
		get{
			return current_season;
		}
	}

	public void NextSeason() {
		current_season++;
		if(current_season > (seasons.Length - 1)){
			current_season = 0;
		}
	}
}

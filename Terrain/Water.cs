using UnityEngine;
using System.Collections;

/**
 * Classe définissant les attributs de l'eau sur le Terrain.
 */
[System.Serializable]
public class Water {

	public static float lowAmplitude = 0.225f, mediumAmplitude = 0.4f, highAmplitude = 0.6f;
	public static float lowFrequency = 0.6f, mediumFrequency = 0.7f, highFrequency = 0.8f;
	public static float lowSpeed = 3f, mediumSpeed = 4f, highSpeed = 5f;
	

	public GameObject waterGameobject;
	public Material waterMaterial;

	[Range(0,1f)]
	public float waterProbability = 0.1f;
	public float minPosY = -1;

	[Range(0,5)]
	public float offsetY = 0.5f;

	/**
	 * Initialise les variables de la force des vagues
	 */
	public void Init(){
		waterMaterial.SetVector("_GAmplitude", new Vector4(0.14f,0.76f,0.175f,lowAmplitude));
		waterMaterial.SetVector("_GFrequency", new Vector4(0.5f,0.38f,0.59f,lowFrequency));
		waterMaterial.SetVector("_GSpeed", new Vector4(-3f,2f,1f,lowSpeed));
	}

	/**
	 * Met à jour les variables de la force des vagues en fonction
	 * de la force des éléments météorologiques.
	 */
	public void UpdateWater(Particles.Strengh strengh){
		if(strengh == Particles.Strengh.LOW){
			Init();
		}else if(strengh == Particles.Strengh.MEDIUM){
			waterMaterial.SetVector("_GAmplitude", new Vector4(0.14f,0.76f,0.175f,mediumAmplitude));
			waterMaterial.SetVector("_GFrequency", new Vector4(0.5f,0.38f,0.59f,mediumFrequency));
			waterMaterial.SetVector("_GSpeed", new Vector4(-3f,2f,1f,mediumSpeed));
		}else if(strengh == Particles.Strengh.HIGH){
			waterMaterial.SetVector("_GAmplitude", new Vector4(0.14f,0.76f,0.175f,highAmplitude));
			waterMaterial.SetVector("_GFrequency", new Vector4(0.5f,0.38f,0.59f,highFrequency));
			waterMaterial.SetVector("_GSpeed", new Vector4(-3f,2f,1f,highSpeed));
		} 
	}
}

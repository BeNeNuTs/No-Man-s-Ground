using UnityEngine;
using System.Collections;

/**
 * Classe définissant les attributs du vent sur le Terrain.
 */
[System.Serializable]
public class Wind {

	public static float lowStrengh = 0f, mediumStrengh = 0.3f, highStrengh = 0.6f;
	
	public WindZone windZone;
	public TerrainGenerator tGenerator;

	/**
	 * Initialise les variables de la force du vent.
	 */
	public void Init(){
		windZone.windMain = lowStrengh;
		windZone.windPulseMagnitude = lowStrengh;
		windZone.windTurbulence = lowStrengh;
		
		tGenerator.tData.wavingGrassStrength = lowStrengh;
	}

	/**
	 * Met à jour les variables de la force du vent en fonction
	 * de la force des éléments météorologiques.
	 */
	public void UpdateWind(Particles.Strengh strengh){

		if(strengh == Particles.Strengh.LOW){
			Init ();
		}else if(strengh == Particles.Strengh.MEDIUM){
			windZone.windMain = mediumStrengh;
			windZone.windPulseMagnitude = mediumStrengh;
			windZone.windTurbulence = mediumStrengh;

			tGenerator.tData.wavingGrassStrength = mediumStrengh;
		}else if(strengh == Particles.Strengh.HIGH){
			windZone.windMain = highStrengh;
			windZone.windPulseMagnitude = highStrengh;
			windZone.windTurbulence = highStrengh;

			tGenerator.tData.wavingGrassStrength = highStrengh;
		} 
	}
}

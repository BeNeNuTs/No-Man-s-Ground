using UnityEngine;
using System.Collections;

/**
 * Classe permettant de gérer l'ambience sonore.
 */
public class SoundController : MonoBehaviour {

	FMOD.Studio.EventInstance ambience; 
	FMOD.Studio.ParameterInstance ambienceWind;
	FMOD.Studio.ParameterInstance ambienceRain;

	float timeTween = 1f;

	// Use this for initialization
	void Start () {
		ambience = FMOD_StudioSystem.instance.GetEvent("event:/Ambience/Forest");
		ambience.start(); 
		ambience.getParameter("Wind", out ambienceWind);
		ambience.getParameter("Rain", out ambienceRain);
	}

	/**
	 * Met à jour les paramètre de l'ambience sonore.
	 */
	public void UpdateParameter(int season, Particles.Strengh strengh){
		//Summer || Winter || Spring
		if(season == 0 || season == 2 || season == 3){
			float wind = GetWind(strengh);
			float rain = 0f;

			StopCoroutine("TweenParameter");
			StartCoroutine(TweenParameter(wind, rain));
		//Autumn
		}else if(season == 1){
			float wind = GetWind(strengh);
			float rain = GetRain(strengh);

			StopCoroutine("TweenParameter");
			StartCoroutine(TweenParameter(wind, rain));
		}
	}

	/**
	 * Récupère la puissance du vent en fonction de la force
	 * des éléments météorologiques.
	 */
	float GetWind(Particles.Strengh strengh){
		if(strengh == Particles.Strengh.LOW){
			return 0;
		}else if(strengh == Particles.Strengh.MEDIUM){
			return 6.66f;
		}if(strengh == Particles.Strengh.HIGH){
			return 10f;
		}

		return 0;
	}

	/**
	 * Récupère la puissance de la pluie en fonction de la force
	 * des éléments météorologiques.
	 */
	float GetRain(Particles.Strengh strengh){
		if(strengh == Particles.Strengh.LOW){
			return 3.33f;
		}else if(strengh == Particles.Strengh.MEDIUM){
			return 6.66f;
		}if(strengh == Particles.Strengh.HIGH){
			return 10f;
		}

		return 0;
	}

	/**
	 * Permet de changer les paramètres wind et rain progressivement.
	 */
	IEnumerator TweenParameter(float newWind, float newRain){
		StopCoroutine("TweenParameter");

		float oldWind, oldRain;
		ambienceWind.getValue(out oldWind);
		ambienceRain.getValue(out oldRain);

		float wind, rain;
		
		float elapsedTime = 0f;
		while (elapsedTime < timeTween) {
			wind = Mathf.Lerp(oldWind, newWind, elapsedTime / timeTween);
			rain = Mathf.Lerp(oldRain, newRain, elapsedTime / timeTween);

			ambienceWind.setValue(wind);
			ambienceRain.setValue(rain);

			elapsedTime += Time.deltaTime;
			yield return null;
		}
		
		ambienceWind.setValue(newWind);
		ambienceRain.setValue(newRain);
	}
}

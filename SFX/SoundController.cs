using UnityEngine;
using System.Collections;

public class SoundController : MonoBehaviour {

	FMOD.Studio.EventInstance ambience; 
	FMOD.Studio.ParameterInstance ambienceWind;
	FMOD.Studio.ParameterInstance ambienceRain;

	//FMOD.Studio.EventDescription 

	float timeTween = 1f;

	// Use this for initialization
	void Start () {
		ambience = FMOD_StudioSystem.instance.GetEvent("event:/Ambience/Forest");
		ambience.start(); 
		ambience.getParameter("Wind", out ambienceWind);
		ambience.getParameter("Rain", out ambienceRain);
	}
	
	public void UpdateParameter(int season, Particles.Strengh strengh){
		//Summer || Winter || Spring
		if(season == 0 || season == 2 || season == 3){
			ambienceWind.setValue(GetWind(strengh));
			ambienceRain.setValue(0);

		//Autumn
		}else if(season == 1){
			ambienceWind.setValue(GetWind(strengh));
			ambienceRain.setValue(GetRain(strengh));
		}
	}

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

	//TODO
	IEnumerator TweenParameter(float wind, float rain){
		yield break;
	}
}

using UnityEngine;
using System.Collections;

[System.Serializable]
public class Particles {
	
	public enum Strengh { LOW = 0, MEDIUM = 1, HIGH = 2 };
	public Strengh strenghParticle = Strengh.LOW;

	[System.Serializable]
	public class Particle{
		public Strengh strengh;
		public GameObject[] particles;
	}
	
	public Particle[] particles;

	public void Init(){
		foreach(Particle p in particles){
			foreach(GameObject go in p.particles){
				go.SetActive(false);
			}
		}
	}

	public void SetStrengh(Strengh s){
		strenghParticle = s;

		Init();

		foreach(Particle p in particles){
			if(p.strengh == strenghParticle){
				foreach(GameObject go in p.particles){
					go.SetActive(true);
				}
			}
		}
	}

	public void NextStrengh(){
		if(strenghParticle == Strengh.LOW){
			SetStrengh(Strengh.MEDIUM);
		}else if(strenghParticle == Strengh.MEDIUM){
			SetStrengh(Strengh.HIGH);
		}else if(strenghParticle == Strengh.HIGH){
			SetStrengh(Strengh.LOW);
		}
	}
}

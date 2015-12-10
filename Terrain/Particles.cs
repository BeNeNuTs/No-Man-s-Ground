using UnityEngine;
using System.Collections;

/**
 * Classe définissant les attributs des éléments météorologiques
 * du Terrain pour chaque saison.
 */
[System.Serializable]
public class Particles {

	/**
	 * Définit les puissances que peuvent avoir les éléments météorologiques
	 */
	public enum Strengh { LOW = 0, MEDIUM = 1, HIGH = 2 };
	public Strengh strenghParticle = Strengh.LOW;

	[System.Serializable]
	public class Particle{
		public Strengh strengh;
		public GameObject[] particles;
	}
	
	public Particle[] particles;

	/**
	 * Permet d'initialiser toutes les particles.
	 */
	public void Init(){
		foreach(Particle p in particles){
			foreach(GameObject go in p.particles){
				go.SetActive(false);
			}
		}
	}

	/**
	 * Change la force des éléments météorologiques courants
	 */
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

	/**
	 * Augmente la force des éléments météorologiques courants
	 */
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

using UnityEngine;
using System.Collections;

public class PodController : Controller {
	
	public override IEnumerator UpdatePosition(float newYPos){
		StartCoroutine(base.UpdatePosition(newYPos));
		yield return null;
	}
}

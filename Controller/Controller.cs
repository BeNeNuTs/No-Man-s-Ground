using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {

	public float offsetY = 1f;

	private const float gap = 3f;

	public virtual IEnumerator UpdatePosition(float newYPos){
		float newYPostion = newYPos + offsetY;
		float oldYPosition = transform.position.y;
		
		float yPosition = transform.position.y;
		
		float time = Time.fixedDeltaTime * Mathf.FloorToInt(transform.position.x) * gap;
		
		float elapsedTime = 0f;
		while (elapsedTime < time) {
			yPosition = Mathf.Lerp(oldYPosition, newYPostion, elapsedTime / time);
			transform.position = new Vector3(transform.position.x, yPosition, transform.position.z);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		
		transform.position = new Vector3(transform.position.x, newYPostion, transform.position.z);
	}
}

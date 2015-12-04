using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DayNightCycle : MonoBehaviour {

	public Light directionalLight;
	public float speed;

	public int hour = 12;
	public int minute = 0;
	public Text hourText;

	public float secondsPerMinute;

	void Start()
	{
		StartCoroutine (UpdateHour ());
	}

	// Update is called once per frame
	void Update () {
		

		directionalLight.transform.Rotate(new Vector3(-speed * Time.deltaTime, 0, 0));
	}

	IEnumerator UpdateHour()
	{
	
		while (true) 
		{
			yield return new WaitForSeconds(secondsPerMinute);

			++minute;

			if (minute > 59) 
			{
				minute = 0;
				hour++;
			}
			
			if (hour > 23) 
			{
				hour = 0;
			}
			
			string hourString = hour.ToString ();
			string minuteString = minute.ToString ();
			
			if (hour < 10) 
			{
				hourString = "0" + hourString;
			}
			
			if (minute < 10) 
			{
				minuteString = "0" + minuteString;
			}
			
			hourText.text = hourString + " : " + minuteString;

			yield return null;
		}
	}
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Clock : MonoBehaviour {

	DayNightController controller;
	public Text hourText;

	void Awake() {
		controller = GetComponent<DayNightController>();
	}
	
	void Update() {
		float currentHour = 24 * controller.currentTimeOfDay;
		float currentMinute = 60 * (currentHour - Mathf.Floor(currentHour));
		

		
		string hourString = Mathf.Floor(currentHour).ToString(); 
		string minuteString = RoundValue(currentMinute,1).ToString();
		
		if (currentHour < 10) 
		{
			hourString = "0" + hourString;
		}
		
		if (currentMinute < 10) 
		{
			minuteString = "0" + minuteString;
		}
		
		hourText.text = hourString + " : " + minuteString;
	}

	/** Arrondi un float avec <precision> chiffre après la virgule */
	public static float RoundValue(float num, float precision)
	{
		return Mathf.Floor(num * precision + 0.5f) / precision;
	}

}
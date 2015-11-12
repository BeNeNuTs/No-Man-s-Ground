using UnityEngine;
using System.Collections;

using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof (FirstPersonController))]
public class PlayerController : Controller {

	private FirstPersonController FPSController;
	private float walkSpeed;
	private float runSpeed;
	private float gravity;

	// Use this for initialization
	void Start () {
		FPSController = GetComponent<FirstPersonController>();

		walkSpeed = FPSController.m_WalkSpeed;
		runSpeed = FPSController.m_RunSpeed;
		gravity = FPSController.m_GravityMultiplier;
	}

	void FreezePlayer(){
		FPSController.m_WalkSpeed = FPSController.m_RunSpeed = FPSController.m_GravityMultiplier = 0f;
	}

	public void UnfreezePlayer(){
		FPSController.m_WalkSpeed = walkSpeed;
		FPSController.m_RunSpeed = runSpeed;
		FPSController.m_GravityMultiplier = gravity;
	}

	public override IEnumerator UpdatePosition(float newYPos){
		FreezePlayer();
		StartCoroutine(base.UpdatePosition(newYPos));

		yield return null;
	}
}

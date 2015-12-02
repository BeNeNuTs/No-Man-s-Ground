using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;

using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof (FirstPersonController))]
public class PlayerController : Controller {

	public TerrainGenerator tGenerator;

	private FirstPersonController FPSController;
	private float walkSpeed;
	private float runSpeed;
	private float gravity;

	private bool freeze = false;
	private bool jetpack = false;

	private Vector2 m_Input;
	private Vector3 m_MoveDir = Vector3.zero;

	private int maxY = 75;

	// Use this for initialization
	void Start () {
		FPSController = GetComponent<FirstPersonController>();

		walkSpeed = FPSController.m_WalkSpeed;
		runSpeed = FPSController.m_RunSpeed;
		gravity = FPSController.m_GravityMultiplier;
	}

	void Update(){
		if(Input.GetKeyDown(KeyCode.J)){
			if(!jetpack && !tGenerator.inCreation){
				jetpack = true;
				FPSController.enabled = false;
			}else{
				jetpack = false;
				FPSController.enabled = true;
			}
		}

		if (!FPSController.m_Jump)
		{
			FPSController.m_Jump = CrossPlatformInputManager.GetButton("Jump");
		}
	}

	void FixedUpdate(){
		if(jetpack && !freeze){
			FPSController.RotateView();
			
			float speed;
			FPSController.GetInput(out speed);
			speed *= 2f;
			// always move along the camera forward as it is the direction that it being aimed at
			Vector3 desiredMove = transform.forward*FPSController.m_Input.y + transform.right*FPSController.m_Input.x;
			m_MoveDir.x = desiredMove.x*speed;
			m_MoveDir.z = desiredMove.z*speed;
			
			if (FPSController.m_Jump)
			{
				m_MoveDir.y = FPSController.m_JumpSpeed;
				FPSController.m_Jump = false;
				FPSController.m_Jumping = true;
			}else{
				m_MoveDir.y = -2f;
			}
			
			FPSController.m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);

			if(transform.position.y > maxY){
				transform.position = new Vector3(transform.position.x, maxY, transform.position.z);
			}
		}	
	}

	void FreezePlayer(){
		freeze = true;
		FPSController.m_WalkSpeed = FPSController.m_RunSpeed = FPSController.m_GravityMultiplier = 0f;
	}

	public void UnfreezePlayer(){
		freeze = false;

		FPSController.m_WalkSpeed = walkSpeed;
		FPSController.m_RunSpeed = runSpeed;
		FPSController.m_GravityMultiplier = gravity;
	}

	public override IEnumerator UpdatePosition(float newYPos){
		FreezePlayer();
		StartCoroutine(base.UpdatePosition(newYPos));

		yield return null;
	}

	public bool isGrounded {
		get{
			return FPSController.m_CharacterController.isGrounded;
		}
	}
}

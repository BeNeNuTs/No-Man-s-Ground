using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;

using UnityStandardAssets.Characters.FirstPerson;

/**
 * Classe permettant de déplacer le joueur 
 * et d'activer le jetpack.
 */
[RequireComponent(typeof (FirstPersonController))]
public class PlayerController : MonoBehaviour {

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

	private const float offsetY = 2f;
	private const float gap = 3f;

	// Use this for initialization
	void Start () {
		FPSController = GetComponent<FirstPersonController>();

		walkSpeed = FPSController.m_WalkSpeed;
		runSpeed = FPSController.m_RunSpeed;
		gravity = FPSController.m_GravityMultiplier;
	}

	void Update(){
		if(Input.GetAxis("LT") == 1 && !jetpack && !freeze){
			jetpack = true;
			FPSController.enabled = false;
		}else if(Input.GetAxis("LT") == 0 && jetpack && !freeze){
			jetpack = false;
			FPSController.enabled = true;
		}

		if (!FPSController.m_Jump && jetpack && !freeze)
		{
			FPSController.m_Jump = CrossPlatformInputManager.GetButton("Jump");
		}
	}

	void FixedUpdate(){
		if(jetpack){
			FPSController.RotateView();
		}

		if(jetpack && !freeze){
			
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

	/**
	 * Gèle le joueur pour qu'il ne puisse plus
	 * bouger lors de la génération du Terrain.
	 */
	void FreezePlayer(){
		freeze = true;
		FPSController.m_WalkSpeed = FPSController.m_RunSpeed = FPSController.m_GravityMultiplier = 0f;
	}

	/**
	 * Dégèle le joueur.
	 */
	public void UnfreezePlayer(){
		freeze = false;

		FPSController.m_WalkSpeed = walkSpeed;
		FPSController.m_RunSpeed = runSpeed;
		FPSController.m_GravityMultiplier = gravity;
	}

	/**
	 * Met à jour la position du joueur
	 * pour qu'il arrive à l'endroit où le Terrain
	 * se générera sous ces pieds.
	 */
	public IEnumerator UpdatePosition(float newYPos){
		FreezePlayer();

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

	/**
	 * Permet de savoir si le joueur touche le sol.
	 */
	public bool isGrounded {
		get{
			return FPSController.m_CharacterController.isGrounded;
		}
	}
}

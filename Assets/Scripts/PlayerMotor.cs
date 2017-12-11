using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Camera))]
public class PlayerMotor : MonoBehaviour {

	[SerializeField]
	private Camera cam;

	private Vector3 velocity = Vector3.zero;
	private Vector3 rotation = Vector3.zero;
	private float cameraRotationX = 0f;
	private float currentCameraRotationX = 0f;
	private Vector3 thrustForce = Vector3.zero;

	[SerializeField]
	private float cameraRotationLimit = 85f;

	private Rigidbody rb;

	void Start(){
		rb = GetComponent<Rigidbody> ();
	}

	//run every physics iteration
	void FixedUpdate(){
		PerformMovement ();
		PerformRotation ();
	}

	//gets a movement vector
	public void Move(Vector3 velocity){
		this.velocity = velocity;
	}

	//gets a rotation vector
	public void Rotate(Vector3 rotation){
		this.rotation = rotation;
	}

	//gets a rotation vector
	public void RotateCamera(float cameraRotationX){
		this.cameraRotationX = cameraRotationX;
	}

	//gets a thrust force vector
	public void ApplyThruster(Vector3 thrustForce){
		this.thrustForce = thrustForce;
	}

	//perform movement based on velocity and thrust
	void PerformMovement(){
		if (velocity != Vector3.zero) {
			//moves rigidbody to the position of the player + the new vector
			rb.MovePosition (rb.position + velocity * Time.fixedDeltaTime);
		}
		if (thrustForce != Vector3.zero) {
			rb.AddForce (thrustForce * Time.fixedDeltaTime, ForceMode.Acceleration);
		}
	}

	//perform rotation based on rotational vectors
	void PerformRotation(){
		//apply y rotation
		rb.MoveRotation (rb.rotation * Quaternion.Euler(rotation));

		//set and clamp x rotation
		currentCameraRotationX -= cameraRotationX;
		currentCameraRotationX = Mathf.Clamp (currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

		//apply camera x rotation
		cam.transform.localEulerAngles = new Vector3 (currentCameraRotationX, 0f, 0f);
	}
}
	

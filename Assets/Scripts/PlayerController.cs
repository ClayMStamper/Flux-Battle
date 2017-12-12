using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour {

	[SerializeField]
	private float speed = 5f;
	[SerializeField]
	private float lookSensitivity = 5f;

	[SerializeField]
	private float thrusterForce = 1000f;

   // private float blendValue = 0f;

	[Header("Spring Settings")]
//	[SerializeField]
	//private JointDriveMode jointMode = JointDriveMode.Position;
	[SerializeField]
	private float jointSpring = 20f;
	[SerializeField]
	private float jointMaxForce = 40f;

    // component caching
    private Animator animator;
	private PlayerMotor motor;
	private ConfigurableJoint joint;

	void Start(){
		motor = GetComponent<PlayerMotor> ();
		joint = GetComponent<ConfigurableJoint> ();
        animator = GetComponent<Animator>();

		SetJointSettings (jointSpring);
	}

	void Update(){
		//calculate movement velocity as a 3d vector
		float xMov = Input.GetAxis("Horizontal");
		float zMov = Input.GetAxis("Vertical");

		Vector3 movHorizontal = transform.right * xMov; // (1,0,0)
		Vector3 movVertical = transform.forward * zMov; // (0,0,1)

		// final movement vector
		Vector3 velocity = (movHorizontal + movVertical) * speed; 

		//apply movement
		motor.Move(velocity);

        //animate movement
        //blendValue = Mathf.Clamp(blendValue + zMov / 5, -1, 1);
        animator.SetFloat("ForwardVelocity",  zMov);

		//calculate rotation as a 3D vector
		float yRot = Input.GetAxisRaw("Mouse X");

		Vector3 rotation = new Vector3 (0, yRot, 0) * lookSensitivity;

		//apply rotation
		motor.Rotate(rotation);

		//calculate camera rotation as a 3D vector
		float xRot = Input.GetAxisRaw("Mouse Y");

		float cameraRotationX = xRot * lookSensitivity;

		//apply camera rotation
		motor.RotateCamera(cameraRotationX);

		//calculate thrust force
		Vector3 newThrustForce = Vector3.zero;
		if (Input.GetButton ("Jump")) {
			newThrustForce = Vector3.up * thrusterForce;
			SetJointSettings (0f); //to keep spring and thrust from applying together
		} else {
			SetJointSettings (jointSpring);
		}

		//apply thruster force
		motor.ApplyThruster(newThrustForce);
	}

	private void SetJointSettings(float newJointSpring){
		joint.yDrive = new JointDrive {
		//	mode = jointMode,
			positionSpring = newJointSpring,
			maximumForce = jointMaxForce
		};
	}
}

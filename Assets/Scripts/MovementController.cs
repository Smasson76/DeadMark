using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour {
    
    [SerializeField]
    float speed = 2f;

    public float lookSensitivity = 10f;

    //Jumping variables
    float jumpForce = 2.0f;
    bool isGrounded;
    public Vector3 jump;

    [SerializeField]
    GameObject fpsCamera;

    Vector3 velocity = Vector3.zero;
    Vector3 rotation = Vector3.zero;
    float cameraUpAndDownRotation = 0f;
    float currentCamRotation = 0;

    Rigidbody body;
    Animator anim;

    void Start() {
        body = GetComponent<Rigidbody>();
        jump = new Vector3(0.0f, 1.5f, 0.0f);
        anim = GetComponent<Animator>();
    }

    void Update() {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 horizontalMovement = transform.right * x;
        Vector3 verticalMovement = transform.forward * z;

        //Setting walk animations
        anim.SetFloat("Horizontal", x);
		anim.SetFloat("Vertical", z);

        //Sprinting
        anim.SetBool("isRunning", Input.GetKey(KeyCode.LeftShift) || Input.GetButton("Sprint"));
        if (anim.GetBool("isRunning")) {
             speed = 8f;
        }
        else {
            speed = 3f;
        }

        //Final movement velocity vector
        Vector3 movementVelocity = (horizontalMovement + verticalMovement).normalized * speed;

        //Apply movement
        Move(movementVelocity);

        //Calculate rotation as a 3D Vector for turning around
        float yRot = Input.GetAxis("Mouse X");
        Vector3 rotVector = new Vector3(0, yRot, 0) * lookSensitivity;

        //Apply rotation
        Rotate(rotVector);

        //Calculate look up and down camera rotation
        float _camUpDownRotation = Input.GetAxis("Mouse Y") * lookSensitivity;

        //Apply rotation
        RotateCamera(_camUpDownRotation);

        //Jumping
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump") && isGrounded){

            body.AddForce(jump * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    //Runs per physics interation
    void FixedUpdate() {
        if (velocity != Vector3.zero) {
            body.MovePosition(body.position + velocity * Time.fixedDeltaTime);
        }

        body.MoveRotation(body.rotation * Quaternion.Euler(rotation));

        if (fpsCamera != null) {
            currentCamRotation -= cameraUpAndDownRotation;
            currentCamRotation = Mathf.Clamp(currentCamRotation, -85, 85);
            fpsCamera.transform.localEulerAngles = new Vector3(currentCamRotation, 0, 0);
        }
    }

    void Move(Vector3 movementVelocity) {
        velocity = movementVelocity;
    }

    void Rotate(Vector3 rotationVector) {
        rotation = rotationVector;
    }

    void RotateCamera(float camUpAndDownRotation) {
        cameraUpAndDownRotation = camUpAndDownRotation;
    }

    //Checking if were on the ground
    void OnCollisionStay(){
        isGrounded = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using FMODUnity;

public class XRCharacterController_old : MonoBehaviour
{

    public WheelCollider leftWheelW, rightWheelW;
    public Transform leftWheelT, rightWheelT;
    public float motorForce = 50;

    // References
    // public Transform mesh = null;
    private InputDevice deviceL;
    private InputDevice deviceR;
    private float m_horizontalInput;
    private float m_verticalInput;


    // Components
    private CharacterController character = null;

    // Values
    private Vector3 currentDirection = new Vector3(0.0f, 0.0f, 1.0f);
    private float localZAxis_R;
    private float localZAxis_L;
    private float moveR;
    private float moveL;

    //Character speed
    private float maxSpeed = 1.0f;

    //Fmod reference (for audio)
    FMOD.Studio.EventInstance wheels;
    [FMODUnity.EventRef]
    FMOD.Studio.PARAMETER_ID wheelSpeedID;

  // Called at the very beginning, before start (not necessary but useful)
    private void Awake()
    {
        // collect components
        character = GetComponent<CharacterController>();
    }


    // Called at the start, when everything is loaded from awake
    private void Start()
    {
        // initialize left devices
        List<InputDevice> devicesL = new List<InputDevice>();
        InputDeviceCharacteristics leftControllerC = InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(leftControllerC, devicesL);

        // initialize right devices
        List<InputDevice> devicesR = new List<InputDevice>();
        InputDeviceCharacteristics rightControllerC = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(rightControllerC, devicesR);

        // grab first device from each list
        if (devicesL.Count > 0 && devicesR.Count > 0) {
            deviceL = devicesL[0];
            deviceR = devicesR[0];
        }

        // set local z axis
        if (deviceR.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 posR))
        {
            localZAxis_R = posR.z;
        }
        if (deviceL.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 posL))
        {
            localZAxis_L = posL.z;
        }

        // Create FMOD instance & attach to game object
        wheels = FMODUnity.RuntimeManager.CreateInstance("event:/Wheels");

        FMOD.Studio.EventDescription wheelsEventDescription;
        wheels.getDescription(out wheelsEventDescription);
        FMOD.Studio.PARAMETER_DESCRIPTION wheelsParameterDescription;
        wheelsEventDescription.getParameterDescriptionByName("WheelSpeed", out wheelsParameterDescription);
        wheelSpeedID = wheelsParameterDescription.id;

        wheels.start();
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(wheels, GetComponent<Transform>(), GetComponent<Rigidbody>());
 

    }


    // Update is called once per frame
    private void Update()
    {
        // GetInput();
        // Steer angle should never change

        if (deviceL != null && deviceR != null) {
           CheckForMovement();
           Move();
        }

        UpdateWheelPoses();

        //updates character speed every frame
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 velocity = rb.velocity;

        rb.velocity = velocity.normalized * maxSpeed;
        Debug.Log(rb.velocity.magnitude);

        //update wheelspeed for fmod
        wheels.setParameterByID(wheelSpeedID, rb.velocity.magnitude);
    }

  // checks for movement, sets direction, and applies movement
  private void CheckForMovement()
  {
      //Resets At beginning to ensure PlayerMovement
      moveR = 0;
      moveL = 0;

      // check for value
      if (deviceR.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 newPosR) &&
          deviceR.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out Vector3 rotR))
      {
          // calculate move speed
          // rotRX is angular rotation along x axis
          float rotRX = rotR.x;
          // z coordinate in relation to the local z axis

          // if z coordinate is on the negative side of local axis
          // reverse angular velocity
          if (Mathf.Abs(newPosR.z) < Mathf.Abs(localZAxis_R)) {
              rotRX *= -1.0f;
          }
          moveR = rotRX;
      }

      if (deviceL.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 newPosL) &&
          deviceL.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out Vector3 rotL))
      {
          // calculate move speed
          // rotRX is angular rotation along x axis
          float rotLX = rotL.x;
          // z coordinate in relation to the local z axis

          // if z coordinate is on the negative side of local axis
          // reverse angular velocity
          if (Mathf.Abs(newPosL.z) < Mathf.Abs(localZAxis_L)) {
              rotLX *= -1.0f;
          }
          moveL = rotLX;
      }

      //Update moveL and moveR, which are variables for speed. 
  }

    private void UpdateWheelPoses()
    {
        UpdateWheelPose(leftWheelW, leftWheelT);
        UpdateWheelPose(rightWheelW, rightWheelT);
    }

    private void UpdateWheelPose(WheelCollider _collider, Transform _transform)
    {
        Vector3 _pos = _transform.position;
        Quaternion _quat = transform.rotation;

        _collider.GetWorldPose(out _pos, out _quat);

        // _transform.position = _pos;
        _transform.rotation = _quat;
    }



    private void Move()
    {
        rightWheelW.motorTorque = moveR * motorForce;
        leftWheelW.motorTorque = moveL * motorForce;
    }
}

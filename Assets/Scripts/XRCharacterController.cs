using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using FMODUnity;

public class XRCharacterController : MonoBehaviour
{

    public bool debugMode = false;

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
    private Rigidbody wheelchairRigid = null;

    // Values
    private Vector3 currentDirection = new Vector3(0.0f, 0.0f, 1.0f);
    private float localZAxis_R;
    private float localZAxis_L;
    private float moveR;
    private float moveL;



    //Debug Values
    private float R_accellCounter = 0;
    private float L_accellCounter = 0;

    //For Speed
    private float WheelchairSpeed;

    //Fmod reference (for audio)
    FMOD.Studio.EventInstance wheels;
    [FMODUnity.EventRef]
    FMOD.Studio.PARAMETER_ID wheelSpeedID;

    // Called at the very beginning, before start (not necessary but useful)
    private void Awake()
    {
        // collect components
        character = GetComponent<CharacterController>();
        wheelchairRigid = GetComponent<Rigidbody>();
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

        if(debugMode){
          debug_KeyMovement();
          debug_Move();

        }
        else if (deviceL != null && deviceR != null) {
           CheckForMovement();
           Move();
        }

        UpdateWheelPoses();
        UpdateSpeed();

        //update wheelspeed for fmod
        wheels.setParameterByID(wheelSpeedID, WheelchairSpeed);


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
        Debug.Log("here1" + moveR);
        Debug.Log(rightWheelW.brakeTorque);
        if (Mathf.Abs(moveR) <= Mathf.Epsilon) rightWheelW.brakeTorque = 1000;
        else {
            rightWheelW.brakeTorque = 0;
            Debug.Log("here2" + moveR);
            rightWheelW.motorTorque = moveR * motorForce;
            Debug.Log("movement:" + rightWheelW.motorTorque);
        }
        if (Mathf.Abs(moveL) <= Mathf.Epsilon) leftWheelW.brakeTorque = 1000;
        else {
            leftWheelW.brakeTorque = 0;
            leftWheelW.motorTorque = moveL * motorForce;
        }



        // rightWheelW.brakeTorque = 0;
        // leftWheelW.brakeTorque = 0;
        // rightWheelW.motorTorque = moveR * motorForce;
        // leftWheelW.motorTorque = moveL * motorForce;
    }


    private void UpdateSpeed(){
        WheelchairSpeed = wheelchairRigid.velocity.magnitude;
    }





    ////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////
    //////////////////Functions for Debugging///////////////////////
    ////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////

    private void debug_Move()
    {
        float factor = 0.2f;

        moveR = R_accellCounter * factor;
        moveL = L_accellCounter * factor;

        if (Mathf.Abs(moveR) <= 0.05f) rightWheelW.brakeTorque = 100;
        else {
            rightWheelW.brakeTorque = 0;
            rightWheelW.motorTorque = moveR;
        }
        if (Mathf.Abs(moveL) <= 0.05f) leftWheelW.brakeTorque = 100;
        else {
            leftWheelW.brakeTorque = 0;
            leftWheelW.motorTorque = moveL;
        }
    }

    private void debug_KeyMovement(){
      if (Input.GetKey(KeyCode.RightArrow)) {
        R_accellCounter++;
      } else {
        if (R_accellCounter > -1){
          R_accellCounter--;

        }
        else{
          R_accellCounter = 0;
        }
      }

      if (Input.GetKey(KeyCode.LeftArrow))
      {
        L_accellCounter++;
      }
      else
      {
        if (L_accellCounter > 0){
          L_accellCounter--;
        }
      }

    }



}

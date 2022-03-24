using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class XRCharacterController : MonoBehaviour
{
  // public values
  public float speed = 1.0f;
  public float wheelRadius = 0.25f;
  public float seatWidth = 0.7f;
  public float rotationSpeed = 30;


    // References
    // public Transform mesh = null;
    private InputDevice deviceL;
  private InputDevice deviceR;

  // Components
  private CharacterController character = null;

  // Values
  private Vector3 currentDirection = new Vector3(0.0f, 0.0f, 1.0f);
  private float localZAxis_R;
  private float localZAxis_L;
  private float moveR;
  private float moveL;


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
  }

  // Update is called once per frame
  private void Update()
  {
      if (deviceL != null && deviceR != null) {
          CheckForMovement();
      }
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
      Vector3 MoveDirection = CalculateMovement();
      Move(MoveDirection);

  }

  private void Move(Vector3 moveDirection)
  {
        // get normalized and act as arc length
        // for now only do the r movement

        //Depend on later requirement, can remove speed for the most accurate
        //real world result
        //moveDirection.Normalize();
        //transform.Translate(moveDirection,Space.World);

        character.SimpleMove(moveDirection);

        if(moveDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

  }


  // use two wheels
  private Vector3 CalculateMovement()
  {

    float velocityZ = (float)(0.5f*wheelRadius*(moveL+moveR)*Mathf.Cos((wheelRadius/seatWidth)*(moveL-moveR)));
    float velocityX = (float)(0.5f*wheelRadius*(moveL+moveR)*Mathf.Sin((wheelRadius/seatWidth)*(moveL-moveR)));

    Vector3 MoveDirection = new Vector3(velocityX, 0.0f , velocityZ);

    return MoveDirection;
  }
  // can definitely add animation/mesh later
}

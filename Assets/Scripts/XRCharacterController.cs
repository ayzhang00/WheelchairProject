using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class XRCharacterController : MonoBehaviour
{
    // public values
    public float speed = 1.0f;

    // References
    // public Transform mesh = null;
    private InputDevice deviceL;
    private InputDevice deviceR;

    // Components
    private CharacterController character = null;

    // Values
    private Vector3 currentDirection = new Vector3(0.0f, 0.0f, 1.0f);
    private float localZAxis;
    private float moveR;

    private void Awake()
    {
        // collect components
        character = GetComponent<CharacterController>();
    }
    private void Start()
    {
        // initialize devices
        List<InputDevice> devicesL = new List<InputDevice>();
        List<InputDevice> devicesR = new List<InputDevice>();

        InputDeviceCharacteristics leftControllerC = InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        InputDeviceCharacteristics rightControllerC = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;

        InputDevices.GetDevicesWithCharacteristics(leftControllerC, devicesL);
        InputDevices.GetDevicesWithCharacteristics(rightControllerC, devicesR);
        

        if (devicesL.Count > 0 && devicesR.Count > 0) {
            deviceL = devicesL[0];
            deviceR = devicesR[0];
        }

        // set last position and local z axis
        if (deviceR.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 posR))
        {
            localZAxis = posR.z;
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
        // check for value
        if (deviceR.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 newPosR) &&
            deviceR.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out Vector3 rotR))
        {   
            // calculate move speed
            // rotRX is angular rotation along x axis
            float rotRX = rotR.x;
            // z coordinate in relation to the local z axis
            if (Mathf.Abs(newPosR.z) < Mathf.Abs(localZAxis)) {
                rotRX *= -1.0f;
            }
            moveR = rotRX;
            // Calculate Dir
            CalculateDirection();
            Move();
        }
    }

    private void Move()
    {
        // get normalized and act as arc length 
        // for now only do the r movement
        Vector3 movement = currentDirection * (moveR * speed);
        character.SimpleMove(movement);
    }

    // get two 
    private void CalculateDirection()
    {

    }
}
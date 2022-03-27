using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class CurbDialogue : MonoBehaviour
{
    public bool canMove = true;
    public GameObject canvas;
    public GameObject warning;
    private InputDevice deviceR;
    // Start is called before the first frame update
    void Start()
    {
        // deviceR = GetComponent<XRCharacterController>().deviceR;
    }

    void Update()
    {
        if (!canMove) {
            canvas.SetActive(true);
        }
    }

    public void Continue(){
        canvas.SetActive(false);
        canMove = true;
        warning.SetActive(true);
    }

    public void NoContinue(){
        canvas.SetActive(false);
        canMove = true;
    }

    // Update is called once per frame
    void OnCollisionEnter(Collision collision) {
        if (collision.collider.tag == "Curb") {
            canMove = false;
        }
    }
}

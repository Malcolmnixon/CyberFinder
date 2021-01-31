using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLightHands : MonoBehaviour
{
    public GameObject leftHandFlashLight;
    public GameObject rightHandFlashLight;

    public bool startOn;
    public bool startLeft;

    private bool _onLeft;
    private bool _onRight;

    // Start is called before the first frame update
    void Start()
    {
        _onLeft = startOn && startLeft;
        _onRight = startOn && !startLeft;
        leftHandFlashLight.SetActive(_onLeft);
        rightHandFlashLight.SetActive(_onRight);
    }

    // Update is called once per frame
    void Update()
    {
        var change = false;

        // Handle left button press
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch))
        {
            change = true;
            if (_onRight)
            {
                // Switch to left
                _onRight = false;
                _onLeft = true;
            }
            else
            {
                // Toggle left
                _onLeft = !_onLeft;
            }
        }

        // Handle right button press
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            change = true;
            if (_onLeft)
            {
                // Switch to right
                _onLeft = false;
                _onRight = true;
            }
            else
            {
                // Toggle right
                _onRight = !_onRight;
            }
        }

        // Update active on change
        if (change)
        {
            leftHandFlashLight.SetActive(_onLeft);
            rightHandFlashLight.SetActive(_onRight);
        }
    }
}

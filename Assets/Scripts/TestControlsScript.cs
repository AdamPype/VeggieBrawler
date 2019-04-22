using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestControlsScript : MonoBehaviour
{
    [SerializeField] private string _button;
    [SerializeField] private string _axis;

    // Update is called once per frame
    void Update()
    {
        Debug.ClearDeveloperConsole();

        if (Input.GetButtonDown(_button))
            Debug.Log("Button press: " + _button);

        Debug.Log("Axis: " + Input.GetAxis(_axis));
    }
}

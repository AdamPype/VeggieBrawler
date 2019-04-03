using UnityEngine;

public class InputController 
{
    private static InputController _instance;

    public static InputController Instance()
    {
        if (_instance == null)
        {
            _instance = new InputController();
        }

        return _instance;
    }

    private InputController() { }
    
    public Vector3 GetRightJoystickFromPlayer(int controller)
    {
        float h = Input.GetAxis("C" + controller + "_RightJoystickX");
        float v = Input.GetAxis("C" + controller + "_RightJoystickY");
        
        return new Vector3(h,0,v);
    }

    public float GetLeftJoystickHorizontal(int controller)
    {
        return Input.GetAxis("C" + controller + "_LeftJoystickX");
    }

    public Vector3 GetLeftJoystickFromPlayer(int controller)
    {
        float h = GetLeftJoystickHorizontal(controller);
        float v = Input.GetAxis("C" + controller + "_LeftJoystickY");

        return new Vector3(h, 0, v);
    }

    public bool IsAButtonPressed(int controller)
    {
        return Input.GetButtonDown("C" + controller + "_AButton");
    }
    
    public bool IsBButtonPressed(int controller)
    {
        return Input.GetButtonDown("C" + controller + "_BButton");
    }

    public bool IsXButtonPressed(int controller)
    {
        return Input.GetButtonDown("C" + controller + "_XButton");
    }

    public bool IsYButtonPressed(int controller)
    {
        return Input.GetButtonDown("C" + controller + "_YButton");
    }

    public bool IsYButtonHeldDown(int controller)
    {
        return Input.GetButton("C" + controller + "_YButton");
    }

    public float GetLeftTriggerFromPlayer(int controller)
    {
        return Input.GetAxis("C" + controller + "_LeftTrigger");
    }

    public float GetRightTriggerFromPlayer(int controller)
    {
        return Input.GetAxis("C" + controller + "_RightTrigger");
    }

    public bool IsStartButtonPressed(int controller)
    {
        return Input.GetButtonDown("C" + controller + "_StartButton");
    }
}



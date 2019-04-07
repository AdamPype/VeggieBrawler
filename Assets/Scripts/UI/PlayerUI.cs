using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerUI : MonoBehaviour
{
    public bool AllowMouseMovement = true;

    public int PlayerNumber;
    public RectTransform Mouse;
    public Transform VisSpot;
    public RectTransform Border;
    public Text NameSpace;

    public GameObject ChosenCharacter;

    private InputController _input = InputController.Instance();

    private Vector2 _originalBorderPos;

    private List<RaycastResult> _hitElements = new List<RaycastResult>();

    private CharacterSelectManager _csm;
    void Start()
    {
        _originalBorderPos = Border.anchoredPosition; //Save original UI Pos to Unlerp it if Necessary
        _csm = CharacterSelectManager.Instance;
    }
    // Update is called once per frame
    void Update()
    {
        if (AllowMouseMovement)
        {
            MoveMouse();
            HideUIElements();
        }

        if(_input.IsAButtonPressed(PlayerNumber))
            ChooseCharacter();

        if (ChosenCharacter != null)
        {
            if (_input.IsBButtonPressed(PlayerNumber))
                UnLoadCharacter();
            ShowUIElements();
        }
    }

    private void MoveMouse()
    {
        Mouse.anchoredPosition += new Vector2(_input.GetLeftJoystickFromPlayer(PlayerNumber).x, _input.GetLeftJoystickFromPlayer(PlayerNumber).z) * _csm.MouseMoveSpeed;
    }

    //Method to check what's under the players mouse
    private GameObject GetElementUnderMouse()
    {
        PointerEventData point = new PointerEventData(EventSystem.current);

        Vector2 pixelPos = Camera.main.WorldToScreenPoint(Mouse.transform.position);
        point.position = pixelPos;

        EventSystem.current.RaycastAll(point, _hitElements);

        if (_hitElements.Count <= 1)
            return null;

        return _hitElements[1].gameObject;
    }


    private void ChooseCharacter()
    {
        GameObject Element = GetElementUnderMouse();

        if (Element != null && Element.tag == "CharacterUI")
        {
            GameObject chosenChar = Element.GetComponent<CharacterTemplate>().Character;    //Get Gameobject

            chosenChar.GetComponent<PlayerScript>().enabled = false;    //Disable Playerscript since We only want to visualize model

            NameSpace.text = Element.GetComponent<CharacterTemplate>().CharacterName;   //Show character name on screen

            LoadCharacter(chosenChar);  //Actually visualize the model - can be commented out if you dont want player to be visualized at all
        }
    }

    //Visualizes character on Left/Right Side Of Scene
    private void LoadCharacter(GameObject chosenChar)
    {
        ChosenCharacter = Instantiate(chosenChar, VisSpot);

        //Code below puts gameobject in specific layer.
        //This allows the renderTexture to see it.
        Transform[] charChildren = ChosenCharacter.GetComponentsInChildren<Transform>();

        foreach (Transform var in charChildren)
        {
            var.gameObject.layer = PlayerNumber+9;
        }

        AllowMouseMovement = false;
    }

    private void ShowUIElements()
    {
        LerpUI(Border,-10f);
        NameSpace.enabled = true;
    }

    private void HideUIElements()
    {
        LerpUI(Border,_originalBorderPos.y);
        NameSpace.enabled = false;
    }

    private void LerpUI(RectTransform lerpThis,float endPosition)
    {
        lerpThis.anchoredPosition = Vector2.Lerp(Border.anchoredPosition, new Vector2(lerpThis.anchoredPosition.x, endPosition), Time.deltaTime * _csm.CharacterSelectBorderMoveSpeed);
    }

    private void UnLoadCharacter()
    {
        Destroy(ChosenCharacter);
        ChosenCharacter = null;
        AllowMouseMovement = true;

        HideUIElements();
    }
}

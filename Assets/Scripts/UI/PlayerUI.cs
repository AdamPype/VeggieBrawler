﻿using System.Collections;
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

    private Vector2 _originalBorderPos;

    private List<RaycastResult> _hitElements = new List<RaycastResult>();

    private CharacterSelectManager _csm;

    private GameObject _instantiatedCharacter;

    public SoundManager UISoundManager;

    //juice
    private float _maxAmplitude = 30;
    private float _amplitude;
    private float _freq = 0.4f;
    private float _frameCount;
    private Squishy _squish;

    void Start()
    {
        _originalBorderPos = Border.anchoredPosition; //Save original UI Pos to Unlerp it if Necessary
        _csm = CharacterSelectManager.Instance;
        _squish = Mouse.GetComponent<Squishy>();
    }
    // Update is called once per frame
    void Update()
    {
        if (AllowMouseMovement)
        {
            MoveMouse();
            HideUIElements();
        }

        if(InputController.IsAButtonPressed(PlayerNumber))
            ChooseCharacter();

        if (ChosenCharacter != null)
        {
            if (InputController.IsBButtonPressed(PlayerNumber))
                UnLoadCharacter();
            ShowUIElements();
        }
    }

    private void FixedUpdate()
        {
        _frameCount++;
        }

    private void MoveMouse()
    {
        Mouse.anchoredPosition += new Vector2(InputController.GetLeftJoystickFromPlayer(PlayerNumber).x, InputController.GetLeftJoystickFromPlayer(PlayerNumber).z) * _csm.MouseMoveSpeed;

        //juice
        float x = InputController.GetRawLeftJoystickFromPlayer(PlayerNumber).x;
        if (x != 0)
            {
            _amplitude = _maxAmplitude * -x;
            _frameCount = 1/_freq;
            }
        float sin = _amplitude * Mathf.Sin(_frameCount * _freq);
        Mouse.localRotation = Quaternion.Lerp(Mouse.localRotation, Quaternion.Euler(0, 0, sin), 10 * Time.deltaTime);
        _amplitude = Mathf.Lerp(_amplitude, 0, Time.deltaTime * 3);

        if (InputController.IsAButtonPressed(PlayerNumber))
            _squish.Squish();
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

        if (Element != null && Element.tag == "CharacterUI" && ChosenCharacter==null)
        {
            UISoundManager.PlaySimple("Select");

            ChosenCharacter = Element.GetComponent<CharacterTemplate>().Character;    //Get Gameobject

            DisableCharacterScripts(ChosenCharacter);    //Disable certain script since we just want to visualize the character 

            NameSpace.text = Element.GetComponent<CharacterTemplate>().CharacterName;   //Show character name on screen

            LoadCharacter(ChosenCharacter);  //Actually visualize the model - can be commented out if you dont want player to be visualized at all
        }
    }

    //Visualizes character on Left/Right Side Of Scene
    private void LoadCharacter(GameObject chosenChar)
    {
        _instantiatedCharacter = Instantiate(chosenChar, VisSpot);

        //Code below puts gameobject in specific layer.
        //This allows the renderTexture to see it.
        Transform[] charChildren = _instantiatedCharacter.GetComponentsInChildren<Transform>();

        foreach (Transform var in charChildren)
        {
            var.gameObject.layer = PlayerNumber+9;
        }

        AllowMouseMovement = false;
    }

    private void ShowUIElements()
    {
        UILerper.LerpUI(Border,new Vector2(Border.anchoredPosition.x,-10f),_csm.CharacterSelectBorderMoveSpeed);
        NameSpace.enabled = true;
    }

    private void HideUIElements()
    {
        UILerper.LerpUI(Border, _originalBorderPos, _csm.CharacterSelectBorderMoveSpeed);
        NameSpace.enabled = false;
    }

    private void UnLoadCharacter()
    {
        UISoundManager.PlaySimple("Cancel");

        Destroy(_instantiatedCharacter);
        ChosenCharacter = null;
        AllowMouseMovement = true;

        HideUIElements();
    }

    private void DisableCharacterScripts(GameObject chosenChar)
    {
        chosenChar.GetComponent<PlayerScript>().enabled = false;
        chosenChar.GetComponent<PhysicsController>().enabled = false;
    }
}

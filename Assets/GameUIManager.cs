﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public bool DebugMode;

    public static GameUIManager Instance;
    public Text TimeText;

    public Image P1Health;
    public Image P2Health;

    public GameObject Player1 { get; set; }
    public GameObject Player2 { get; set; }

    private float _originalP1HealthWidth;
    private float _originalP2HealthWidth;

    void Awake()
    {
        Instance = this;


        #region old
        /*if (!Debug)
        {
            Player1 = FindObjectOfType<ChosenCharactersSaver>().ChosenCharacters[0];
            Player2 = FindObjectOfType<ChosenCharactersSaver>().ChosenCharacters[1];
            //Destroy(FindObjectOfType<ChosenCharactersSaver>());
        }*/
        #endregion
    }

    void Start()
    {
        _originalP1HealthWidth = P1Health.rectTransform.localScale.x;
        _originalP2HealthWidth = P2Health.rectTransform.localScale.x;

        Player1 = GameControllerScript.Instance.SpawnedPlayer1;
        Debug.Log(Player1);
        Player2 = GameControllerScript.Instance.SpawnedPlayer2;
    }

    void Update()
    {
        TimeText.text = GameControllerScript.Instance.TimeText;

        //DebugWidthTest();
        if (!DebugMode)
        {
            SetHealthImageWidth(_originalP1HealthWidth,P1Health.rectTransform,Player1);
            SetHealthImageWidth(_originalP2HealthWidth,P2Health.rectTransform,Player2);
        }
    }

    //Method that links image scale to player health remaining - can change image color/img around too
    private void SetHealthImageWidth(float originalWidth,RectTransform health,GameObject player)
    {
        if (health.localScale.x > 0)
        {
            float newWidth = (originalWidth / player.GetComponent<PlayerScript>().MaxHealth) * player.GetComponent<PlayerScript>().Health;
            health.localScale = new Vector3(newWidth,health.localScale.y,health.localScale.z);
            if (health.localScale.x < 0)
                health.localScale = Vector3.Scale(health.localScale, new Vector3(0, 1, 1));
        }
    }

    //Debug test to see how it works
    private void DebugWidthTest()
    {
        if (P1Health.rectTransform.localScale.x > 0f)
            P1Health.rectTransform.localScale += Vector3.right * -Time.deltaTime * 0.1f;

    }
}

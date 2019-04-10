using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;
    public float TimeRemaining;
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
    }

    void Start()
    {
        Player1 = FindObjectOfType<ChosenCharactersSaver>().ChosenCharacters[0];
        Player2 = FindObjectOfType<ChosenCharactersSaver>().ChosenCharacters[1];
        Destroy(FindObjectOfType<ChosenCharactersSaver>());

        _originalP1HealthWidth = P1Health.rectTransform.localScale.x;
        //_originalP2HealthWidth = P2Health.rectTransform.localScale.x;
    }

    void Update()
    {
        CalculateRemainingTime();

        DebugWidthTest();

        //SetImageWidth(_originalP1HealthWidth,P1Health.rectTransform,Player1);
        //SetImageWidth(_originalP2HealthWidth,P2Health.rectTransform,Player2);
    }

    private void CalculateRemainingTime()
    {
        TimeRemaining -= Time.deltaTime;
        string minutes = Mathf.Floor(TimeRemaining / 60).ToString("00");
        string seconds = Mathf.Floor(TimeRemaining % 60).ToString("00");

        TimeText.text = minutes + ":" + seconds;
    }
    //Method that links image scale to player health remaining - can change image color/img around too
    private void SetImageWidth(float originalWidth,RectTransform health,GameObject player)
    {
        if (health.localScale.x > 0)
        {
            float newWidth = (originalWidth / player.GetComponent<PlayerScript>().MaxHealth) * player.GetComponent<PlayerScript>().Health;
            health.localScale = new Vector3(newWidth,health.localScale.y,health.localScale.z);
        }
    }

    //Debug test to see how it works
    private void DebugWidthTest()
    {
        if (P1Health.rectTransform.localScale.x > 0f)
            P1Health.rectTransform.localScale += Vector3.right * -Time.deltaTime * 0.1f;

    }
}

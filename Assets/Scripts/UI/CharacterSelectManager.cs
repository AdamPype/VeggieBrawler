using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour
{
    public static CharacterSelectManager Instance;

    public float MouseMoveSpeed;
    public float CharacterSelectBorderMoveSpeed;
    public float StartGameMoveSpeed;

    public RectTransform StartGameUI;
    private PlayerUI[] AreCharactersChosen = new PlayerUI[2];

    private Vector2 _originalUIPos;
    private bool _allowStartGame;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        AreCharactersChosen = FindObjectsOfType<PlayerUI>();
        _originalUIPos = StartGameUI.anchoredPosition; //Save original UI Pos to Unlerp it if Necessary
    }

    void Update()
    {
        if (!AreCharactersChosen[0].AllowMouseMovement && !AreCharactersChosen[1].AllowMouseMovement) //If both players have chosen a character
        {
            ShowStartGameUI();
            _allowStartGame = true;
        }
        else
        {
            HideStartGameUI();
            _allowStartGame = false;
        }

        if(_allowStartGame && Input.GetButtonDown("StartGame"))
            StartGame();
    }

    private void StartGame()
    {
        //Save Chosen Characters And Go To Game Scene 
        for (int i = 0; i < AreCharactersChosen.Length; i++)
        {
            ChosenCharactersSaver.Instance.ChosenCharacters[i] = AreCharactersChosen[i].ChosenCharacter;
        }

        SceneManager.LoadScene(2);
    }

    private void ShowStartGameUI()
    {
       LerpUI(StartGameUI,Vector2.zero);
    }

    private void HideStartGameUI()
    {
        LerpUI(StartGameUI,_originalUIPos);
    }

    private void LerpUI(RectTransform lerpThis,Vector2 targetPos)
    {
        lerpThis.anchoredPosition = Vector2.Lerp(lerpThis.anchoredPosition, new Vector2(targetPos.x, StartGameUI.anchoredPosition.y), Time.deltaTime * StartGameMoveSpeed);
    }
}

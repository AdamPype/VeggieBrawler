using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControllerScript : MonoBehaviour
{
    public static GameControllerScript Instance { get; private set; }

    public bool DebugMode;
    public GameObject BasePlayerPrefab; //Used when starting in GameScene instead of coming from CharacterSelect
    public float TimeRemaining;
    public float TimeUntilGameStart;

    public int Winner { get; set; }
    public string TimeText { get; set; }

    public Transform Player1SpawnPoint;
    public Transform Player2SpawnPoint;

    public GameObject Player1;
    public GameObject Player2;
    public GameObject SpawnedPlayer1;
    public GameObject SpawnedPlayer2;
    [SerializeField] private int _winScreenBuildIndex;

    private int player = 0;
    // Start is called before the first frame update
    void Awake()
    {
        Winner = -1;

        CreateInstance(); //create GameControllerScript Instance

        CreatePlayer(out Player1, out SpawnedPlayer1, Player1SpawnPoint); //Create Player 1
        CreatePlayer(out Player2, out SpawnedPlayer2, Player2SpawnPoint); //Create Player 2

        StartCoroutine(WaitToStartGame());
    }

    private void Update()
    {
        WinnerCheck();
        CalculateRemainingTime();
    }

    //Waits "TimeUntilGameStart" Seconds to start the game
    private IEnumerator WaitToStartGame()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(TimeUntilGameStart);
        StartGame();
    }

    private void CreatePlayer(out GameObject player,out GameObject spawnedPlayer,Transform spawnPoint)
    {
        player = DebugMode ? FindObjectOfType<ChosenCharactersSaver>().ChosenCharacters[this.player-1] : BasePlayerPrefab;
        this.player += 1;

        spawnedPlayer = SpawnPlayer(player, spawnPoint); //Spawn Player

        CameraScript temp = GameObject.Find("Main Camera").GetComponent<CameraScript>();
        temp.ObjectsToTrack.Add(spawnedPlayer.transform);

        EnableCharacterScripts(spawnedPlayer); //Enable necessary Scripts
    }

    private void StartGame()
    {
        StopAllCoroutines();
        Time.timeScale = 1;
    }

    private GameObject SpawnPlayer(GameObject player, Transform spawnPoint)
    {
         return Instantiate(player, spawnPoint);
    }

    private void WinnerCheck()
    {
        if (Winner != -1)
        {
            SceneManager.LoadScene(_winScreenBuildIndex);
        }
    }

    private void CalculateRemainingTime()
    {
        TimeRemaining -= Time.deltaTime;
        string minutes = Mathf.Floor(TimeRemaining / 60).ToString("00");
        string seconds = Mathf.Floor(TimeRemaining % 60).ToString("00");

        TimeText = minutes + ":" + seconds;
    }

    private void EnableCharacterScripts(GameObject character)
    {
        character.GetComponent<PlayerScript>().enabled = true;
        character.GetComponent<PhysicsController>().enabled = true;
    }

    private void CreateInstance()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
    }
}

    
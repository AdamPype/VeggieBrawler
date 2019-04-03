using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControllerScript : MonoBehaviour
{
    public static GameControllerScript Instance { get; private set; }
    public int Winner { get; set; }

    [SerializeField] private int _winScreenBuildIndex;

    // Start is called before the first frame update
    void Awake()
        {
        Winner = -1;

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

    private void Update()
        {
        if (Winner != -1)
            {
            SceneManager.LoadScene(_winScreenBuildIndex);
            }
        }
    }

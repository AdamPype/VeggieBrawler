using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RedirecterScript : MonoBehaviour
{
    [SerializeField] private int _buildIndex;
    
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene(_buildIndex);
    }
}

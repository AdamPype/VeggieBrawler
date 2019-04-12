using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowCooldown : MonoBehaviour
{
    public bool Debug;
    [Range(1, 2)] public int Player = 1;

    public GameObject BasicAttackUI;
    public GameObject SpecialAttackUI;

    private PlayerScript _player;

    private float basicAttackCooldown;
    private float SpecialAttackCooldown;

    private bool _basicUp = true;
    private bool _specialUp = true;
    // Start is called before the first frame update
    void Start()
    {
        if (!Debug)
        {
            if (Player == 1)
                _player = GameControllerScript.Instance.Player1.GetComponent<PlayerScript>();
            else
                _player = GameControllerScript.Instance.Player2.GetComponent<PlayerScript>();
        }     
    }

    // Update is called once per frame
    void Update()
    {
        DebugHeightCheck();
        if (!Debug)
        {
            UpdateCoolDown(BasicAttackUI,_player.AttackDuration-_player.AttackCooldownTimer,_player.AttackDuration,_basicUp); //Cooldown is MaxCooldown - AttackCD Timer | (Because AttackCDTimer counts abilityUp)  
            UpdateCoolDown(SpecialAttackUI,_player.SpecialAttackDuration-_player.SpecialAttackCooldownTimer,_player.SpecialAttackDuration,_specialUp);
        }
    }

    private void UpdateCoolDown(GameObject uiElement, float cooldown,float maxCooldown,bool abilityUp)
    {
        if (cooldown > 0)
        {
            if (abilityUp)
            {
                uiElement.transform.GetChild(0).GetComponent<RectTransform>().localScale = Vector3.one; //If ability just got on cooldown, reset image Scale
            }


            uiElement.transform.GetChild(1).GetComponent<Text>().text = cooldown.ToString(); //Set text to cooldown timer
        
            float newHeight = uiElement.transform.GetChild(0).GetComponent<RectTransform>().localScale.x / maxCooldown * cooldown; //calculate new height according to CD

            uiElement.transform.GetChild(0).GetComponent<RectTransform>().localScale -= Vector3.up * newHeight; //set height

        }
        else //if cooldown < 0
        {
            uiElement.transform.GetChild(1).GetComponent<Text>().text = ""; //Leave text empty
            abilityUp = true; //Notify that ability is up
        }
    }

    private void DebugHeightCheck()
    {
        BasicAttackUI.transform.GetChild(0).GetComponent<RectTransform>().localScale -= Vector3.up * Time.deltaTime * 0.1f;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Blackhole_HotKey_Controller : MonoBehaviour
{

    private SpriteRenderer sr;
    private KeyCode myHotkey;
    private TextMeshProUGUI myText;
    private Transform myEnemy;
    private Blackhole_Skill_Controller blackHole;

    public void SetHotKey(KeyCode _myNewHotKey,Transform _myEnemy, Blackhole_Skill_Controller _myBlackhole )
    {
        sr  = GetComponent<SpriteRenderer>();
        myText = GetComponentInChildren<TextMeshProUGUI>();

        myEnemy = _myEnemy.transform;
        blackHole = _myBlackhole;

        myHotkey = _myNewHotKey;
        myText.text = _myNewHotKey.ToString();
    }

    private void Update()
    {

        if(Input.GetKeyDown(myHotkey))
        {
            blackHole.AddEnemyToList(myEnemy);

            myText.color = Color.clear;
            sr.color = Color.clear;
        }
    }
}

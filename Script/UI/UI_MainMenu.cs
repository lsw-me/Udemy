using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private string sceneName = "MainScene";
    [SerializeField] private GameObject continueButton;
    [SerializeField] private UI_FadeScreen fadeScreen;


    private void Start()
    {
        if(SaveManager.instance.HasSaveData() == false)
            continueButton.SetActive(false);
    }
    public void ContinueGame()
    {
        StartCoroutine(LoadScenWithFadeEffect(1.5f));
    }

    public void NewGame()
    {

        SaveManager.instance.DeleteSavedData();  //这样你每次开新游戏存档就删掉了
        StartCoroutine(LoadScenWithFadeEffect(1.5f));
      
    }

    public void ExitGame()
    {

        Debug.Log("Exit game");
        //Application.Quit();
    }

    IEnumerator LoadScenWithFadeEffect(float _delay)  //fade in/out 之后延迟进入 
    {
        fadeScreen.FadeOut();
        yield return  new WaitForSeconds(_delay);

        SceneManager.LoadScene(sceneName);
    }

}

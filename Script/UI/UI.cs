using System.Collections;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    [Header("End Screen")]
    [SerializeField] private UI_FadeScreen fadeScreen;
    [SerializeField] private GameObject endText;
    [SerializeField] private GameObject restartButton;
    [Space]
    [SerializeField] private GameObject characterUI;
    [SerializeField] private GameObject skillTreeUI;
    [SerializeField] private GameObject craftUI;
    [SerializeField] private GameObject optionUI;
    [SerializeField] private GameObject inGameUI;

    public UI_ItemToolTip itemToolTip;
    public UI_StatToolTip statToolTip;
    public UI_CraftWindow craftWindow;
    public UI_SkillToolTip skillToolTip;


    private void Awake()
    {
        SwitchTo(skillTreeUI);// 这里解决bug 顺序问题，关闭skilltree的情况下 UI_SkillTreeSlot 中awake 不会立刻执行，只有在开启时候执行，但是这是执行已经发生在技能分配事件之后了
                              // 比如 clone技能中 我们分配button 是在start函数中，导致顺序问题，解锁技能但是无法使用  感觉这个肯定写法有问题，这是找补的解决方法，需要思考如何避免这种情况

        fadeScreen.gameObject.SetActive(true);
    }
    void Start()
    {
        SwitchTo(inGameUI);
        itemToolTip.gameObject.SetActive(false);
        statToolTip.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))   //切换ui
            SwitchWithKeyTo(characterUI);
        if (Input.GetKeyDown(KeyCode.B))
            SwitchWithKeyTo(craftUI);
        if (Input.GetKeyDown(KeyCode.K))
            SwitchWithKeyTo(skillTreeUI);
        if (Input.GetKeyDown(KeyCode.O))
            SwitchWithKeyTo(optionUI);
    }

    //切换UI菜单
    public void SwitchTo(GameObject _menu)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            bool fadeScreen = transform.GetChild(i).GetComponent<UI_FadeScreen>() != null;  //如果有UI_FadeScreen 这个组件 返回真  利用这个保持 fade screen object active

            if (fadeScreen == false)
                transform.GetChild(i).gameObject.SetActive(false); //不是fadeScreen就设为false
        }

        if (_menu != null)
            _menu.SetActive(true);


        if (GameManager.instance != null)
        {
            if (_menu == inGameUI)
                GameManager.instance.PauseGame(false);
            else
                GameManager.instance.PauseGame(true);
        }


    }


    public void SwitchWithKeyTo(GameObject _menu)
    {


        if (_menu != null && _menu.activeSelf)
        {
            _menu.SetActive(false);
            CheckForInGameUI();
            return;
        }
        SwitchTo(_menu);
    }

    private void CheckForInGameUI()  //这里想法是 切换到技能树 状态这些窗口时候不显示ingame ui 当关闭任意其他非ingameUI的窗口时候，都能保持ui自动打开
    {
        for (int i = 0; i < transform.childCount; i++) //当这个画布上所有gameobject关闭 切换ui in game
        {
            if (transform.GetChild(i).gameObject.activeSelf && transform.GetChild(i).GetComponent<UI_FadeScreen>() == null)  //因为fade in/out 的问题所以 他会执行到return 也就不会在转换  解决：加个判断
                return;
        }

        SwitchTo(inGameUI);
    }

    public void SwitchOnEndScreen()
    {
        fadeScreen.FadeOut();

        StartCoroutine(EndScreenCoroutine());

    }

    IEnumerator EndScreenCoroutine()
    {
        yield return new WaitForSeconds(1);
        endText.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        restartButton.SetActive(true);
    }

    public void RestartGameButton() => GameManager.instance.ResartScene();

}

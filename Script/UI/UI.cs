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
        SwitchTo(skillTreeUI);// ������bug ˳�����⣬�ر�skilltree������� UI_SkillTreeSlot ��awake ��������ִ�У�ֻ���ڿ���ʱ��ִ�У���������ִ���Ѿ������ڼ��ܷ����¼�֮����
                              // ���� clone������ ���Ƿ���button ����start�����У�����˳�����⣬�������ܵ����޷�ʹ��  �о�����϶�д�������⣬�����Ҳ��Ľ����������Ҫ˼����α����������

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
        if (Input.GetKeyDown(KeyCode.C))   //�л�ui
            SwitchWithKeyTo(characterUI);
        if (Input.GetKeyDown(KeyCode.B))
            SwitchWithKeyTo(craftUI);
        if (Input.GetKeyDown(KeyCode.K))
            SwitchWithKeyTo(skillTreeUI);
        if (Input.GetKeyDown(KeyCode.O))
            SwitchWithKeyTo(optionUI);
    }

    //�л�UI�˵�
    public void SwitchTo(GameObject _menu)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            bool fadeScreen = transform.GetChild(i).GetComponent<UI_FadeScreen>() != null;  //�����UI_FadeScreen ������ ������  ����������� fade screen object active

            if (fadeScreen == false)
                transform.GetChild(i).gameObject.SetActive(false); //����fadeScreen����Ϊfalse
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

    private void CheckForInGameUI()  //�����뷨�� �л��������� ״̬��Щ����ʱ����ʾingame ui ���ر�����������ingameUI�Ĵ���ʱ�򣬶��ܱ���ui�Զ���
    {
        for (int i = 0; i < transform.childCount; i++) //���������������gameobject�ر� �л�ui in game
        {
            if (transform.GetChild(i).gameObject.activeSelf && transform.GetChild(i).GetComponent<UI_FadeScreen>() == null)  //��Ϊfade in/out ���������� ����ִ�е�return Ҳ�Ͳ�����ת��  ������Ӹ��ж�
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveManager
{
    public static GameManager instance;
    private Transform player;

    [SerializeField] private CheckPoint[] checkPoints;
    [SerializeField] private string closestCheckPointId;

    [Header("Lost currency")]
    [SerializeField] private GameObject lostCurrencyPrefab;
    public int lostCurrencyAmount;
    [SerializeField] private float lostCurrenctX;
    [SerializeField] private float lostCurrenctY; //������Ҫ����

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;

        checkPoints = FindObjectsOfType<CheckPoint>();
    }

    private void Start()
    {
        player = PlayerManager.instance.player.transform;
    }


    public void ResartScene()
    {
        SaveManager.instance.SaveGame();

        Scene scene = SceneManager.GetActiveScene();//��ǰ����

        SceneManager.LoadScene(scene.name);  //����������ǰ����

    }

    public void LoadData(GameData _data) => StartCoroutine(LoadWithDely(_data));

    //PlacePlayerAtClosestCheckPoint();  //����̳�ʹ��Invoke���� Ȼ�����0.1����ӳ٣���������Ӧ����ִ��˳������⣬��������û�� ������Կ�151����������������������Կ����ӳٴ���
    //�� ������ô�ţ� ��߶����ӳ� Ȼ��ͺ���

    public void SaveData(ref GameData _data)
    {

        _data.lostCurrencyAmount = lostCurrencyAmount;
        _data.lostCurrencyX = player.position.x;
        _data.lostCurrencyY = player.position.y;


        if(FindClosestCheckPoint()!=null)
            _data.closestCheckPointId = FindClosestCheckPoint().Id;

        _data.checkPoints.Clear();

        foreach (CheckPoint checkPoint in checkPoints)
        {
            _data.checkPoints.Add(checkPoint.Id, checkPoint.activationStatus);
        }
    }

    private void LoadCheckPoints(GameData _data)
    {
        foreach (KeyValuePair<string, bool> pair in _data.checkPoints)
        {
            foreach (CheckPoint checkPoint in checkPoints)
            {
                if (checkPoint.Id == pair.Key && pair.Value == true)
                    checkPoint.ActivateCheckpoint();
            }
        }
    }

    private void LoadLostCurrency(GameData _data)
    {
        lostCurrencyAmount = _data.lostCurrencyAmount;
        lostCurrenctX = _data.lostCurrencyX;
        lostCurrenctY = _data.lostCurrencyY;

        if(lostCurrencyAmount > 0)
        {
            Debug.Log("enter create prefab");
            GameObject newLostCurrency = Instantiate(lostCurrencyPrefab, new Vector3(lostCurrenctX, lostCurrenctY), Quaternion.identity);
            newLostCurrency.GetComponent<LostCurrencyController>().currency = lostCurrencyAmount;
        }

        lostCurrencyAmount = 0; //�ٴ�����û���Ͷ��� -�� ȥ��playerstats ����״̬
    }

    private IEnumerator LoadWithDely(GameData _data)  //�ӳٴ�������
    {
        yield return new WaitForSeconds(.1f);
        LoadCheckPoints(_data);
        LoadLostCurrency(_data);
        LoadClosestCheckPoint(_data);
    }

    private void LoadClosestCheckPoint(GameData _data)
    {

        if (_data.closestCheckPointId == null)
            return;

        closestCheckPointId = _data.closestCheckPointId;
        foreach (CheckPoint checkPoint in checkPoints)
        {
            if (closestCheckPointId == checkPoint.Id)
            {
                player.position = checkPoint.transform.position;
            }
        }
    }
    private CheckPoint FindClosestCheckPoint()
    {
        float closestDistance = Mathf.Infinity;
        CheckPoint closestCheckPoint = null;

        foreach (CheckPoint checkPoint in checkPoints)
        {
            float distanceToCheckPoint = Vector2.Distance(player.transform.position, checkPoint.transform.position);
            if (distanceToCheckPoint < closestDistance && checkPoint.activationStatus == true)
            {
                closestDistance = distanceToCheckPoint;
                closestCheckPoint = checkPoint;
            }

        }
        return closestCheckPoint;
    }

    public void PauseGame(bool _pause) //��ͣ��Ϸ
    {
        if (_pause)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
}

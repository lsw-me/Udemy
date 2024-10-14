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
    [SerializeField] private float lostCurrenctY; //这三个要保存

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

        Scene scene = SceneManager.GetActiveScene();//当前场景

        SceneManager.LoadScene(scene.name);  //用来重启当前场景

    }

    public void LoadData(GameData _data) => StartCoroutine(LoadWithDely(_data));

    //PlacePlayerAtClosestCheckPoint();  //这里教程使用Invoke（） 然后给了0.1秒的延迟，他的问题应该是执行顺序的问题，但我这里没有 详情可以看151，我如果按照它的来，明显看到延迟传送
    //嘿 您猜怎么着？ 后边都改延迟 然后就好了

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

        lostCurrencyAmount = 0; //再次死亡没捡魂就丢了 -》 去看playerstats 死亡状态
    }

    private IEnumerator LoadWithDely(GameData _data)  //延迟触发加载
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

    public void PauseGame(bool _pause) //暂停游戏
    {
        if (_pause)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
}

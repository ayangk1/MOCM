using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private void Awake()
    {
        instance = this;
    }

    public AudioClip bgm;
    public Goods currGoods;

    public GameObject[] door;
    public GameObject[] goodsShow;
    public GameObject[] goods;
    public Sprite[] murals;

    public GameObject introductionPanel;
    public GameObject goodIntroPanel;
    public GameObject settingPanel;

    public bool isTest;
    public bool isOpenTeleportion;
    public bool isOpenInteraction;

    public Text promptGlobal;

    public GameObject player;

    
    void Start()
    {
        if (isTest) return;
        settingPanel.SetActive(false);
        isOpenTeleportion = false;
    }

    void Update()
    {
        if (isTest) return;
        if (player.transform.position.z < 28)
        {
            door[1].transform.rotation = quaternion.identity;
            door[0].transform.rotation = quaternion.identity;
        }
        OpenDoor();
    }

    public void StartButton()
    {
        isOpendooor = true;
        isOpenTeleportion = true;
        introductionPanel.SetActive(false);
    }
    public void MusicButton()
    {
        settingPanel.SetActive(true);
    }

    

    bool isOpendooor;
    void OpenDoor()
    {
        if (isOpendooor)
        {
            door[1].transform.rotation = Quaternion.Lerp(door[1].transform.rotation, Quaternion.Euler(0, 90, 0), 0.05f);
            door[0].transform.rotation = Quaternion.Lerp(door[0].transform.rotation, Quaternion.Euler(0, -90, 0), 0.05f);
        }
        if (door[0].transform.rotation == Quaternion.Euler(0,-90,0))
        {
            isOpendooor = false;
        }
    }

    public void ShowPromptGlobal(string mText)
    {
        promptGlobal.gameObject.SetActive(true);
        promptGlobal.text = mText;

    }
    public void GetShowObj(Goods mgoods)
    {
        currGoods = mgoods;
        ActiveShowGoods(mgoods);
    }

    public int GetGoodsIndex(Goods mgoods)
    {
        switch (mgoods)
        {
            case Goods.niujiao:
                return 0;
            case Goods.fourCup:
                return 1;
            case Goods.yaoNianzi:
                return 2;
            case Goods.tianPing:
                return 3;
            case Goods.yaoguanzi:
                return 4;
            case Goods.tongren:
                return 5;
            case Goods.fourzhen:
                return 6;
        }
        return -1;
    }

    public void ActiveShowGoods(Goods mgoods)
    {
        int index = 0;
        switch (mgoods)
        {
            case Goods.niujiao:
                index = 0;
                break;
            case Goods.fourCup:
                index = 1;
                break;
            case Goods.yaoNianzi:
                index = 2;
                break;
            case Goods.tianPing:
                index = 3;
                break;
            case Goods.yaoguanzi:
                index = 4;
                break;
            case Goods.tongren:
                index = 5;
                break;
            case Goods.fourzhen:
                index = 6;
                break;
        }
        for (int i = 0; i < goodsShow.Length; i++)
        {
            if (i == index)
            {
                goodsShow[i].SetActive(true);
            }
            else
            {
                goodsShow[i].SetActive(false);
            }
        }
    }

    public void ButtonTest()
    {
        Debug.Log("ButtonTest");
    }
}

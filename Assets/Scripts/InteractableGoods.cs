using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum Goods
{
    niujiao,//0
    fourCup,
    yaoNianzi,
    tianPing,
    yaoguanzi,//1
    tongren,//2
    fourzhen,
}
public class InteractableGoods : MonoBehaviour
{
    public Goods goods;
    public GameObject player;

    public GameObject canGrabPre;
    
    
    void Start()
    {
        player = GameObject.Find("XR Origin");
    }
    void Update()
    {

        

    }
    public void Select()
    {
        if (goods == Goods.niujiao)
        {
            canGrabPre.SetActive(true);
        }
        else
        {
            canGrabPre.SetActive(false);
        }
        UIManager.instance.SwitchBGImg(goods);
        GameManager.instance.goodIntroPanel.transform.GetChild(0).gameObject.SetActive(true);
        if (transform.position.x > player.transform.position.x)
        {
            GameManager.instance.goodIntroPanel.transform.position
            = transform.position + new Vector3(-1, 0.5f, 0);
            GameManager.instance.goodIntroPanel.transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        else
        {
            GameManager.instance.goodIntroPanel.transform.position
            = transform.position + new Vector3(1, 0.5f, 0);
            GameManager.instance.goodIntroPanel.transform.rotation = Quaternion.Euler(0, -90, 0);
        }
        
        GameManager.instance.GetShowObj(goods);
    }

    public void RayEnter()
    {

        switch (goods)
        {
            case Goods.niujiao:
                break;
        }

    }
    public void RayExit()
    {
        switch (goods)
        {
            case Goods.niujiao:
                break;
        }
    }
}

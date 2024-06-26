using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public enum YaoGuanZiState
{
    idle,
    grabed,
    grind,
}
public class YaoGuanZI : XRGrabInteractable
{
    private bool isHaveRenShen;
    public YaoGuanZiState state;
    public GameObject yaoChuZi;
    public Transform grindPos;
    private int grindCount;
    void Start()
    {
        state = YaoGuanZiState.idle;
    }
    void Update()
    {
        if (interactorsSelecting.Count == 0)
        {
            state = YaoGuanZiState.idle;
        }

        if (grindCount > 2)
        {
            GameManager.instance.ShowPromptGlobal("已研磨成功");
            grindCount = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("YaoChuZi") && isHaveRenShen)
        {
            grindCount++;
            GameManager.instance.ShowPromptGlobal("已研磨" + grindCount + "次");
        }
        if (other.CompareTag("YaoChuZi") && !isHaveRenShen)
        {
            GameManager.instance.ShowPromptGlobal("请先放入药材");
        }
        if(other.CompareTag("RenShen"))
        {
            GameManager.instance.ShowPromptGlobal("已放入药材请研磨");
            Destroy(other.gameObject);
            isHaveRenShen = true;
        }
    }
}

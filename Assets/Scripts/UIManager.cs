using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    private void Awake()
    {
        instance = this;
    }
    
    public enum TypeVolume
    {
        global,//0
        people,//1
        music,//2
    }

    public TypeVolume typeVolume;
    public GameObject goodsIntroPanel;
    public GameObject introductionPanel;
    public GameObject volumeSettingPanel;
    public GameObject OperationIntroPanel;
    public Button startButton;

    public Button gameSettingButton;
    
    public Button operationIntroButton;
    public Button exitButton;
    
    public Button handExitButton;

    public Sprite[] optionEnterTextures;
    public Sprite[] optionExitTextures;
    public Sprite[] volumeTextures;
    public Image[] volumeNumImgs;
    public int[] volumeNums;
    public GameObject[] root;
    public Sprite[] showBG;
    public Image bg;
    
    
    private void Start()
    {
        OperationIntroPanel.SetActive(false);
        introductionPanel.SetActive(true);
        volumeNums = new int[3] { 5,5,5};
        typeVolume = TypeVolume.global;
        goodsIntroPanel.transform.GetChild(0).gameObject.SetActive(false);
        volumeSettingPanel.SetActive(false);
        for (int i = 0; i < volumeNumImgs.Length; i++)
        {
            volumeNumImgs[i].sprite = volumeTextures[5];
        }
    }

    public void SwitchBGImg(Goods goods)
    {
        switch (goods)
        {
            case Goods.niujiao:
                bg.sprite = showBG[0];
                break;
            case Goods.fourCup:
                break;
            case Goods.yaoNianzi:
                break;
            case Goods.tianPing:
                break;
            case Goods.yaoguanzi:
                bg.sprite = showBG[1];
                break;
            case Goods.tongren:
                bg.sprite = showBG[2];
                break;
            case Goods.fourzhen:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(goods), goods, null);
        }
    }
    
    public void OperationIntroButton()
    {
        OperationIntroPanel.SetActive(true);
    }
    public void OperationIntroBackButton()
    {
        OperationIntroPanel.SetActive(false);
    }
    public void VolumeSettingButton()
    {
        volumeSettingPanel.SetActive(true);
        
    }
    public void VolumeSettingBackButton()
    {
        volumeSettingPanel.SetActive(false);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void IncreaseVolumeButton()
    {
        //控制音量
        switch (typeVolume)
        {
            case TypeVolume.global:
                if (volumeNums[0] < 9) volumeNums[0]++;

                volumeNums[1] = volumeNums[0];
                volumeNums[2] = volumeNums[0];
                
                AudioManager.instance.audioSource.volume = (float)volumeNums[0] / 10;
                volumeNumImgs[0].sprite = volumeTextures[volumeNums[0]];
                volumeNumImgs[1].sprite = volumeTextures[volumeNums[1]];
                volumeNumImgs[2].sprite = volumeTextures[volumeNums[2]];
                break;
            case TypeVolume.people:
                if (volumeNums[1] < 9) volumeNums[1]++;
                volumeNumImgs[1].sprite = volumeTextures[volumeNums[1]];
                break;
            case TypeVolume.music:
                if (volumeNums[2] < 9) volumeNums[2]++;
                
                AudioManager.instance.audioSource.volume = (float)volumeNums[2] / 10;
                volumeNumImgs[2].sprite = volumeTextures[volumeNums[2]];
                break;
        }
    }

    public void DecreaseVolumeButton()
    {
        //控制音量
        switch (typeVolume)
        {
            case TypeVolume.global:
                if (volumeNums[0] > 0) volumeNums[0]--;
                
                volumeNums[1] = volumeNums[0];
                volumeNums[2] = volumeNums[0];
                
                AudioManager.instance.audioSource.volume = (float)volumeNums[0] / 10;
                volumeNumImgs[0].sprite = volumeTextures[volumeNums[0]];
                volumeNumImgs[1].sprite = volumeTextures[volumeNums[1]];
                volumeNumImgs[2].sprite = volumeTextures[volumeNums[2]];
                break;
            case TypeVolume.people:
                if (volumeNums[1] > 0) volumeNums[1]--;
                volumeNumImgs[1].sprite = volumeTextures[volumeNums[1]];
                break;
            case TypeVolume.music:
                if (volumeNums[2] > 0) volumeNums[2]--;
                
                AudioManager.instance.audioSource.volume = (float)volumeNums[2] / 10;
                volumeNumImgs[2].sprite = volumeTextures[volumeNums[2]];
                break;
        }
    }
    
    public void GlobalVolumeButton()
    {
        typeVolume = TypeVolume.global;
    }
    public void PeopleVolumeButton()
    {
        typeVolume = TypeVolume.people;
    }
    public void MusicVolumeButton()
    {
        typeVolume = TypeVolume.music;
    }
    
    public void StartButtonEnter()
    {
        startButton.image.sprite = optionEnterTextures[0];
    }
    public void StartButtonExit()
    {
        startButton.image.sprite = optionExitTextures[0];
    }
    public void GameSettingButtonEnter()
    {
        gameSettingButton.image.sprite = optionEnterTextures[1];
    }
    public void GameSettingButtonExit()
    {
        gameSettingButton.image.sprite = optionExitTextures[1];
    }
    
    public void OperationIntroButtonEnter()
    {
        operationIntroButton.image.sprite = optionEnterTextures[2];
    }
    public void OperationIntroButtonExit()
    {
        operationIntroButton.image.sprite = optionExitTextures[2];
    }
    
    
    public void ExitButtonEnter()
    {
        exitButton.image.sprite = optionEnterTextures[3];
    }
    public void ExitButtonExit()
    {
        exitButton.image.sprite = optionExitTextures[3];
    }
    
    public void HandExitButtonEnter()
    {
        handExitButton.image.sprite = optionEnterTextures[3];
    }
    public void HandExitButtonExit()
    {
        handExitButton.image.sprite = optionExitTextures[3];
    }

    public GameObject niujiao;
    public void GoodsIntroPanelClose()
    {
        goodsIntroPanel.transform.GetChild(0).gameObject.SetActive(false);
        niujiao.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum TypeMural
{
    xiuzhi,
    shuizhi,
    huozhi,
    shuihuogongzhi,
}
public class MuralRead : MonoBehaviour
{
    public GameObject readUI;
    public Button button;
    public GameObject pagePanel;

    public Sprite mural;
    public Image image;


    private void Start()
    {
        readUI.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            readUI.transform.position = transform.position + new Vector3(1,-0.3f,0); 
            readUI.SetActive(true);
            image.sprite = mural;
            button.GetComponent<Button>().onClick.AddListener
            (delegate ()
            {
                
            });
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        readUI.SetActive(false);
        pagePanel.SetActive(false);
    }

    public void PlayAudio(TypeMural typeMural)
    {
        switch (typeMural)
        {
            case TypeMural.xiuzhi:
                break;
            case TypeMural.shuizhi:
                break;
            case TypeMural.huozhi:
                break;
            case TypeMural.shuihuogongzhi:
                break;
            default:
                break;
        }
    }
}

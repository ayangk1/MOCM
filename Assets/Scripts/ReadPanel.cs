using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadPanel : MonoBehaviour
{
    public GameObject pagePanel;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        pagePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Read()
    {
        pagePanel.SetActive(true);
    }
    public void ClosePagePanel()
    {
        pagePanel.SetActive(false);
    }
}

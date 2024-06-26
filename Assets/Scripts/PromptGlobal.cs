using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromptGlobal : MonoBehaviour
{
    Text text;
    private void OnEnable()
    {
        Invoke("SetState", 2);
    }
    void SetState()
    {
        text = GetComponent<Text>();
        text.text = "";
        transform.gameObject.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

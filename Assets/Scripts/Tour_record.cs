using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class Tour_record : MonoBehaviour
{
    public float penDis;
    private InputDevice rightHandController;
    public GameObject vrRay;
    public GameObject pen;
    public GameObject eraser;
    public GameObject switchTool;
    public bool isRay;
    private bool triggerValue;
    public bool isBrush;
    public Texture2D main;
    public Texture2D initMain;
    private Color32[] initColorArray;
    public int m_Width;
    public int penWidth;
    public int eraserWidth;
    private Texture2D brushColorTex;
    private Texture2D eraserColorTex;
    private Texture2D switchColorTex;
    private Color switchColor;
    private Color eraserColor;
    private Color brushColor;
    private Vector2 preVec = Vector2.zero;
    private Vector2 currVec = Vector2.zero;//当前位置
    private Vector2 newVec = Vector2.zero;
    public Vector2 NewVec
    {
        set
        {
            if (value != newVec)
            {
                newVec = value;
            }
        }
        get => newVec;
    }
    private Color32[] currentColorArray;
    // Start is called before the first frame update
    void Start()
    {
        HidTool();
        m_Width = penWidth;
        brushColor = new Color(0.02f,0.75f,0.37f,1);
        eraserColor = new Color(0, 0, 0, 0);
        switchColor = brushColor;
        InitMap();
        currentColorArray = main.GetPixels32();
        brushColorTex = new Texture2D(10, 10);
        for (int i = 0; i < brushColorTex.width; i++)
        {
            for (int j = 0; j < brushColorTex.height; j++)
            {
                brushColorTex.SetPixel(i, j, brushColor);
            }
        }
        eraserColorTex = new Texture2D(20, 20);
        for (int i = 0; i < eraserColorTex.width; i++)
        {
            for (int j = 0; j < eraserColorTex.height; j++)
            {
                eraserColorTex.SetPixel(i, j, eraserColor);
            }
        }
        switchTool = pen;
        switchColorTex = brushColorTex;
    }

    public void ShowTool()
    {
        pen.GetComponent<MeshRenderer>().enabled = true;
        eraser.GetComponent<MeshRenderer>().enabled = true;
    }

    public void HidTool()
    {
        pen.GetComponent<MeshRenderer>().enabled = false;
        eraser.GetComponent<MeshRenderer>().enabled = false;
    }
    // Update is called once per frame
    void Update()
    {
        VRDraw();
    }
    public void InitMap()
    {
        initColorArray = initMain.GetPixels32();
        main.SetPixels32(initColorArray);
        main.Apply();
    }

    public void SwitchTool()
    {
        if (isBrush)
        {
            switchTool = pen;
            m_Width = penWidth;
            switchColorTex = brushColorTex;
            switchColor = brushColor;
        }
        else
        {
            m_Width = eraserWidth;
            switchTool = eraser;
            switchColorTex = eraserColorTex;
            switchColor = eraserColor;
        }
    }
    void VRDraw()
    {
        rightHandController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        RaycastHit hit;
        Ray ray = new Ray(switchTool.transform.position, switchTool.transform.forward);
        if (Physics.Raycast(ray, out hit,penDis) && isRay)
        {
            Vector2Int uv = new Vector2Int((int)(hit.textureCoord.x * main.width), (int)(hit.textureCoord.y * main.height));
            BrushMap(uv, switchColorTex, false);
            NewVec = uv;//生成下一个点
            if (preVec == Vector2.zero)
                preVec = NewVec;
            float diss = Vector2.Distance(preVec, NewVec);
            float step = 0.1f / diss;
            for (float i = 0; i <= 1; i += step)
            {
                currVec = Vector2.Lerp(preVec, NewVec, i);
                PenDraw(currVec);
            }
            main.SetPixels32(currentColorArray);
            preVec = NewVec;
        }
        else
        {
            preVec = Vector2.zero;
        }
    }
    public void BrushMap(Vector2Int pos,Texture2D tex,bool isLock)
    {
        for (int i = 0; i < tex.width; i++)
        {
            for (int j = 0; j < tex.height; j++)
            {
                Color color = tex.GetPixel(i, j);
                if (isLock) color = color * new Color(0.5f, 0.5f, 0.5f);

                if (color.a != 0)
                {
                    main.SetPixel(i + pos.x - tex.width / 2, j + pos.y - tex.height / 2, color);
                }
                else if (color.a == 0)
                {
                    main.SetPixel(i + pos.x - tex.width / 2, j + pos.y - tex.height / 2, color);
                }
            }
        }
        main.Apply();
    }
    void PenDraw(Vector2 centerPixel)
    {
        MarkPixelsToColour(centerPixel, m_Width);
    }
    // 在颜色数组中找到点击的像素的位置，并更改颜色
    private void MarkPixelsToColour(Vector2 centerPixel, int penWidth)
    {
        //中心位置X
        int centerX = (int)centerPixel.x;//363
        //中心位置Y
        int centerY = (int)centerPixel.y;//396
        //X = 360 X<=366
        for (int x = centerX - penWidth; x <= centerX + penWidth; x++)
        {
            // y 393->399
            for (int y = centerY - penWidth; y <= centerY + penWidth; y++)
            {
                //边界外不画
                if (x >= main.width || x < 0 ||
                    y >= main.height || y < 0)
                    continue;
                //数组的标示
                int arrayPos = y * main.width + x;
                currentColorArray[arrayPos] = switchColor;
            }
        }
    }
    public void IsRay()
    {
        isRay = true;
    }
    public void NoRay()
    {
        isRay = false;
    }
}

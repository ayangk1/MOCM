using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

public class CustomDirectGrab : XRDirectInteractor
{
    public enum Hand
    {
        right,
        left,
    }
    public Hand hand;
    public InputActionAsset InputActionAsset;
    //public GameObject leftHand;
    //public GameObject rightHand;
    private InputAction rightTrigger;
    private InputAction rightOpen;
    private InputAction leftGrab;
    private InputAction leftOpen;
    private InputAction leftOpenDown;
    private bool isCheckBook;
    private Animator bookAnimator;
    private Book book;
    public GameObject bookDup;
    public XRDirectInteractor interactor;
    public GameObject niujiao;
    public bool handPanelIsOpen;
    public GameObject handPanel;
    public GameObject checkPanel;
    public bool checkPanelIsOpen;

    // Start is called before the first frame update
    protected override void Start()
    {
        var rightHandInteraction = InputActionAsset.FindActionMap("XRI RightHand Interaction");
        var rightHandLocomotion = InputActionAsset.FindActionMap("XRI RightHand Locomotion");
        var leftHandInteraction = InputActionAsset.FindActionMap("XRI LeftHand Interaction");
        var leftHandLocomotion = InputActionAsset.FindActionMap("XRI LeftHand Locomotion");
        var mamap = InputActionAsset.FindActionMap("MyMap");
        
        rightTrigger = rightHandInteraction.FindAction("Activate");
        rightOpen = rightHandLocomotion.FindAction("Teleport Mode Activate");
        rightTrigger.performed += OpenBook;
        
        leftGrab = leftHandInteraction.FindAction("Select");
        leftOpen = leftHandLocomotion.FindAction("Teleport Mode Activate");
        leftOpenDown = mamap.FindAction("Open");
        
        leftOpen.performed += OpenHandPanel;
        leftGrab.canceled += CloseBook;
        
        if (hand == Hand.right)
        {
            
            rightOpen.performed += WWW;
        }
        
        if (hand == Hand.left)
        {
            handPanel.SetActive(false);
            MeshRenderer mr = checkPanel.transform.GetChild(0).transform.GetComponent<MeshRenderer>();
            mr.enabled = false;
            
            
            leftOpenDown.performed += OpenCheckPanel;
        }
        interactor = GetComponent<XRDirectInteractor>();
        interactor.selectEntered.AddListener(SwitchTool);
        interactor.selectExited.AddListener(SwitchToolExit);
        interactor.selectEntered.AddListener(GetNiujiao);
        interactor.selectExited.AddListener(PushNiujiao);
    }

    public Tour_record tr;
    public void SwitchTool(SelectEnterEventArgs hover)
    {
        if (hover.interactableObject is  XRGrabInteractable)
        {
            GameObject interactor = hover.interactableObject.transform.gameObject;
            if (interactor.CompareTag("Pen"))
            {
                tr.isBrush = true;
                tr.SwitchTool();
            }
            else if (interactor.CompareTag("eraser"))
            {
                tr.isBrush = false;
                tr.SwitchTool();
            }
        }
    }
    public void SwitchToolExit(SelectExitEventArgs hover)
    {
        if (hover.interactableObject is  XRGrabInteractable)
        {
            GameObject interactor = hover.interactableObject.transform.gameObject;
            if (interactor.CompareTag("Pen"))
            {
            }
            else if (interactor.CompareTag("eraser"))
            {
            }
        }
    }
    public void GetNiujiao(SelectEnterEventArgs hover)
    {
        if (hover.interactableObject is  XRGrabInteractable)
        {
            GameObject interactor = (GameObject)hover.interactableObject.transform.gameObject;
            if (interactor.CompareTag("Niujiao"))
            {
                niujiao.gameObject.SetActive(false);
            }
        }
    }
    public void PushNiujiao(SelectExitEventArgs hover)
    {
        if (hover.interactableObject is  XRGrabInteractable)
        {
            GameObject interactor = (GameObject)hover.interactableObject.transform.gameObject;
            if (interactor.CompareTag("Niujiao"))
            {
                niujiao.gameObject.SetActive(true);
            }
        }
    }

    void OpenBook(InputAction.CallbackContext context)
    {
        if (!isCheckBook) return;
        if (!book.isOpen)
        {
            bookAnimator.Play("BookOpen");
            book.isOpen = true;
        }
        else if (book.isOpen)
        {
            bookAnimator.Play("BookClose");
            book.isOpen = false;
        }
    }
    
    void HideLeftHand(InputAction.CallbackContext context)
    {
        //leftHand.SetActive(false);
    }
    void HideRightHand(InputAction.CallbackContext context)
    {
        //rightHand.SetActive(false);
    }
    void ExposeLeftHand(InputAction.CallbackContext context)
    {
        //leftHand.SetActive(true);
    }
    void ExposeRightHand(InputAction.CallbackContext context)
    {
        //rightHand.SetActive(true);
    }

    void CloseBook(InputAction.CallbackContext context)
    {
        if (!isCheckBook) return;
       // leftHand.SetActive(true);
        bookDup.SetActive(true);
        book.transform.gameObject.SetActive(false);
    }

    void OpenHandPanel(InputAction.CallbackContext context)
    {
        if (hand == Hand.left && !handPanelIsOpen && !checkPanelIsOpen)
        {
            handPanel.SetActive(true);
            handPanelIsOpen = true;
            
            leftOpen.performed += CloseHandPanel;
            leftOpen.performed -= OpenHandPanel;
            
        }
        
    }
    void CloseHandPanel(InputAction.CallbackContext context)
    {
        if (hand == Hand.left && handPanelIsOpen)
        {
            handPanel.SetActive(false);
            handPanelIsOpen = false;
            
            leftOpen.performed -= CloseHandPanel;
            leftOpen.performed += OpenHandPanel;
        }
    }

    public GameObject pen;
    void OpenCheckPanel(InputAction.CallbackContext context)
    {
        if (context.action != leftOpenDown)
        {
            
            return;
        }
        if (hand == Hand.left && !checkPanelIsOpen && !handPanelIsOpen)
        {
            MeshRenderer mr = checkPanel.transform.GetChild(0).transform.GetComponent<MeshRenderer>();
            mr.enabled = true;
            checkPanelIsOpen = true;
            
            leftOpenDown.performed += CloseCheckPanel;
            leftOpenDown.performed -= OpenCheckPanel;

            HandPanel ex = checkPanel.GetComponent<HandPanel>();
            ex.isFollow = false;
            
            tr.ShowTool();
            
            
            // checkPanel.transform.position = cam.transform.position + cam.transform.forward * matchPosOffset;
            // pen.transform.position = checkPanel.transform.position + new Vector3(-0.7f, 0.2f, 0);
            // checkPanel.transform.LookAt(cam.transform);

        }
        
    }
    void WWW(InputAction.CallbackContext context)
    {
        if (hand == Hand.right)
        {
            HandPanel ex = checkPanel.GetComponent<HandPanel>();
            ex.isFollow = false;
        }
    }
    void CloseCheckPanel(InputAction.CallbackContext context)
    {
        if (context.action != leftOpenDown)
        {
            return;
        }
        
        if (hand == Hand.left && checkPanelIsOpen)
        {
            MeshRenderer mr = checkPanel.transform.GetChild(0).transform.GetComponent<MeshRenderer>();
            mr.enabled = false;
            checkPanelIsOpen = false;
            
            leftOpenDown.performed -= CloseCheckPanel;
            leftOpenDown.performed += OpenCheckPanel;
            
            HandPanel ex = checkPanel.GetComponent<HandPanel>();
            ex.isFollow = true;
            
            tr.HidTool();
        }
    }

    public Transform cam;
    public float matchPosOffset = 0.5f;
    // Update is called once per frame
    void Update()
    {
        if (interactablesSelected.Count != 0)
        {
            foreach (var item in interactablesSelected)
            {
                if (item.transform.CompareTag("book"))
                {
                    bookAnimator = item.transform.GetComponent<Animator>();
                    book = item.transform.GetComponent<Book>();
                    isCheckBook = true;
                   // leftHand.SetActive(false);
                }
            }
        }
        else
        {
            isCheckBook = false;
        }
    }
}

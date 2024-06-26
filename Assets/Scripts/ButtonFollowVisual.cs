using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ButtonFollowVisual : MonoBehaviour
{
    public Transform visualTarget;
    public Vector3 localAxis;
    private Vector3 offset;
    private Transform pokeAttachTransform;
    public float resetSpeed = 5;
    private bool freeze = false;
    private Vector3 initialLocalPos;
    
    private XRBaseInteractable interactable;
    private bool isFollowing = false;
    private void Start()
    {
        initialLocalPos = visualTarget.localPosition;
        
        interactable = GetComponent<XRBaseInteractable>();
        interactable.hoverEntered.AddListener(Follow);
        interactable.hoverExited.AddListener(Reset);
        interactable.selectEntered.AddListener(Freeze);
    }

    public void Follow(BaseInteractionEventArgs hover)
    {
        if (hover.interactorObject is XRPokeInteractor)
        {
            XRPokeInteractor interactor = (XRPokeInteractor)hover.interactorObject;
            
            isFollowing = true;
            freeze = false;
            
            pokeAttachTransform = interactor.attachTransform;
            offset = visualTarget.position - pokeAttachTransform.position;
            
            Debug.Log("Follow");
        }
    }

    public void Reset(BaseInteractionEventArgs hover)
    {
        if (hover.interactorObject is  XRPokeInteractor)
        {
            isFollowing = false;
            freeze = false;
            
            Debug.Log("Reset");
        }
    }

    public void Freeze(BaseInteractionEventArgs hover)
    {
        if (hover.interactorObject is XRPokeInteractor)
        {
            freeze = true;
            Debug.Log("Freeze");
        }
    }

    private void Update()
    {
        if (freeze) return;
        
        if (isFollowing)
        {
            //Vector3 localTargetPosition = visualTarget.InverseTransformPoint(pokeAttachTransform.position + offset);
            //Vector3 constrainedLocalTargetPosition = Vector3.Project(localTargetPosition, localAxis);
            //visualTarget.position = visualTarget.TransformPoint(constrainedLocalTargetPosition);
            visualTarget.position = new Vector3(visualTarget.position.x,pokeAttachTransform.position.y + offset.y,visualTarget.position.z ) ;
        }
        else
        {
            visualTarget.localPosition = Vector3.Lerp(visualTarget.localPosition,initialLocalPos,Time.deltaTime * resetSpeed);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportationController : MonoBehaviour
{
    public InputActionAsset inputActionAsset;
    public InputActionProperty m_teleportModeActivate;
    public InputActionProperty m_teleportModeCancel;
    private InputAction teleportModeActivate;
    private InputAction teleportModeCancel;
    public XRRayInteractor teleportInteractor;

    void Start()
    {
        var rightHandLocomotion = inputActionAsset.FindActionMap("XRI RightHand Locomotion");
        teleportModeActivate = m_teleportModeActivate.action;
        teleportModeCancel = m_teleportModeCancel.action;
        SetTeleportController(false);
        EnableAction();
    }
    private void OnDestroy()
    {
        DisableAction();
    }
    void Update()
    {
        if (!GameManager.instance.isOpenTeleportion)
            return;

        if (CanEnterTeleport())
        {
            SetTeleportController(true);
            return;
        }
        if (CanExitTeleport())
        {
            SetTeleportController(false);
            return;
        }
    }
    private void SetTeleportController(bool isEnable)
    {
        if (teleportInteractor != null)
        {
            teleportInteractor.gameObject.SetActive(isEnable);
        }

    }
    private bool CanEnterTeleport()
    {
        bool isTriggerTeleport = teleportModeActivate != null && teleportModeActivate.triggered;
        bool isCancelTeleport = teleportModeCancel != null && teleportModeCancel.triggered;
        return isTriggerTeleport && !isCancelTeleport; //判断是否触发传送且没有按下取消传送的键
    }
    private bool CanExitTeleport()
    {
        bool isCancelTeleport = teleportModeCancel != null && teleportModeCancel.triggered;
        bool isReleaseTeleport = teleportModeActivate != null && teleportModeActivate.phase == InputActionPhase.Waiting;
        return isCancelTeleport || isReleaseTeleport; //判断是否按下取消传送的键或者释放了之前推动的摇杆
    }
    private void EnableAction()
    {
        if (teleportModeActivate != null && teleportModeActivate.enabled)
        {
            teleportModeActivate.Enable();
        }
    }
    private void DisableAction()
    {
        if (teleportModeActivate != null && teleportModeActivate.enabled)
        {
            teleportModeActivate.Disable();
        }
    }
}



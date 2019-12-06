using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;

public class ViveControllerLookModule : PointerInputModule
{
    public Camera ControllerCamera;
    public SteamVR_TrackedObject RightController;
    public GameObject reticle;
    public Transform laserTransform;
    private Transform rightControllerTransform;
    public float reticleSizeMultiplier = 0.02f;
    private PointerEventData pointerEventData;
    private RaycastResult currentRaycast;
    private GameObject currentLookAtHandler;

    public override void Process()
    {
        HandleLook();
        HandleSelection();
    }

    void Awake()
    {
        rightControllerTransform = RightController.transform;    
    }

    void HandleLook()
    {
        if (pointerEventData == null)
        {
            pointerEventData = new PointerEventData(eventSystem);
        }
        pointerEventData.position = ControllerCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        eventSystem.RaycastAll(pointerEventData, raycastResults);
        currentRaycast = pointerEventData.pointerCurrentRaycast = FindFirstRaycast(raycastResults);
        reticle.transform.position = rightControllerTransform.position
            + (rightControllerTransform.forward * currentRaycast.distance);
        laserTransform.position = Vector3.Lerp(rightControllerTransform.position,
                                                reticle.transform.position, 0.5f);
        laserTransform.LookAt(reticle.transform);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, laserTransform.localScale.z);
        float reticleSize = currentRaycast.distance * reticleSizeMultiplier;
        reticle.transform.localScale = new Vector3(reticleSize, reticleSize, reticleSize);
        ProcessMove(pointerEventData);
      
    }
    private bool IsTriggerPressed()
    {
        return SteamVR_Controller.Input((int)RightController.index).GetPressDown(EVRButtonId.k_EButton_SteamVR_Trigger);
    }

    void HandleSelection()
    {
        if(pointerEventData.pointerEnter != null)
        {
            currentLookAtHandler = ExecuteEvents.GetEventHandler
                <IPointerClickHandler>(pointerEventData.pointerEnter);
            if(currentLookAtHandler != null && IsTriggerPressed())
            {
                ExecuteEvents.ExecuteHierarchy(currentLookAtHandler, pointerEventData, ExecuteEvents.pointerClickHandler);
            }
        }
        else
        {
            currentLookAtHandler = null;
        }
    }


}

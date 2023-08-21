using System.Collections;
using System.Collections.Generic;
using Obi;
using UnityEngine;

public class ToMazeTrigger : MonoBehaviour
{
    public GameObject mazeManager;
    public ThirdPersonCam thirdPersonCam;
    public SoftbodyMovement softbodyMovement;
    public ObiSoftbody sb;
    public void triggerAction()
    {
       
        //Debug.Log("trigger enter: to maze");
        
        thirdPersonCam.SwitchCameraStyle(ThirdPersonCam.CameraStyle.Fixed);
        softbodyMovement.enabled = false;
        mazeManager.SetActive(true);
        //Invoke(nameof(mazeGen), 0.5f);
    }

}

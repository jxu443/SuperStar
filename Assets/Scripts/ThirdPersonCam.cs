using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("References")]
    public Transform  orientation;
    public Transform player;
    public Transform playerObj;
    
    public float rotationSpeed;

    //public Transform combatLookAt;

    public GameObject thirdPersonCam;
    //public GameObject combatCam;
    public GameObject topDownCam;
    public GameObject fixedCam;
    public GameObject fixedCamJug;

    public CameraStyle currentStyle = CameraStyle.Basic;

    public enum CameraStyle
    {
        Basic,
        Topdown,
        Fixed,
        FixedJug,
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // switch styles
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchCameraStyle(CameraStyle.Basic);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchCameraStyle(CameraStyle.Topdown);

        // player pos - camera pos w/o y axis
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        // roate player object
        if(currentStyle == CameraStyle.Basic || currentStyle == CameraStyle.Topdown)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

            if (inputDir != Vector3.zero)
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }
        // else if(currentStyle == CameraStyle.Combat)
        // {
        //     Vector3 dirToCombatLookAt = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
        //     orientation.forward = dirToCombatLookAt.normalized;
        //
        //     playerObj.forward = dirToCombatLookAt.normalized;
        // }
    }

    public void SwitchCameraStyle(CameraStyle newStyle)
    {
        if (newStyle == currentStyle) return;
        
        fixedCamJug.SetActive(false);
        fixedCam.SetActive(false);
        thirdPersonCam.SetActive(false);
        topDownCam.SetActive(false);

        if (newStyle == CameraStyle.Basic) thirdPersonCam.SetActive(true);
        if (newStyle == CameraStyle.Fixed) fixedCam.SetActive(true);
        if (newStyle == CameraStyle.Topdown) topDownCam.SetActive(true);
        if (newStyle == CameraStyle.FixedJug) fixedCamJug.SetActive(true);

        currentStyle = newStyle;
    }
    
    
}

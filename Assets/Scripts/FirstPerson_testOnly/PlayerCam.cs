// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class PlayerCam : MonoBehaviour
// {
//
//     public float sensX;
//     public float sensY;
//     
//     public Transform orientation;
//     
//     float xRotation;
//     float yRotation;
//     
//     void Start()
//     {
//         // mouse is locked and invisible
//         Cursor.lockState = CursorLockMode.Locked;
//         Cursor.visible = false;
//     }
//     
//     void Update()
//     {
//         
//         // GetAxisRaw : value changes instantly when the input device is moved, instead of lerping towards it like GetAxis
//         float mouseX = Input.GetAxisRaw("Mouse X") * sensX * Time.deltaTime;
//         float mouseY = Input.GetAxisRaw("Mouse Y") * sensY * Time.deltaTime;
//         
//         xRotation -= mouseY;    
//         yRotation += mouseX;
//         
//         xRotation = Mathf.Clamp(xRotation, -90f, 90f);
//         
//         transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
//         orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
//     }
// }

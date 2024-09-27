using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleCamera : MonoBehaviour
{
    [SerializeField] public GameObject cam1;
    [SerializeField] public GameObject cam2;

    // Public method to toggle cameras, which can be called from other scripts
    public void ToggleCameras()
    {
        // Toggle the active state of both cameras
        bool isCam1Active = cam1.activeSelf;

        cam1.SetActive(!isCam1Active);
        cam2.SetActive(isCam1Active);
    }
}

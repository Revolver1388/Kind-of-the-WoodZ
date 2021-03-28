using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{ 
    Camera main;
    HeroControls player;
    Vector3 smoothingVelocity;
    [SerializeField] float distFromPlayer = 8, c_Lerp = 0.5f, cameraOffsetX = 0.2f;
    public float cameraOffsetY;
    void Start()
    {
       
        if(!main) main = Camera.main;
        if (!player) player = FindObjectOfType<HeroControls>();
    }

    private void LateUpdate()
    {
        CamMovement();
    }
    void CamMovement()
    {
        transform.position = Vector3.SmoothDamp(transform.position, player.transform.position - (transform.forward - (transform.right * cameraOffsetX) - (transform.up * cameraOffsetY)) * distFromPlayer, ref smoothingVelocity, c_Lerp); 
    }
}
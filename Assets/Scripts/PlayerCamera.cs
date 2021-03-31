using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    Camera main;
    Transform player;
    Vector3 smoothingVelocity;
    [SerializeField] float distFromPlayer = 8, c_Lerp = 0.5f, cameraOffsetX = 0.2f;
    public float cameraOffsetY;
    private bool isPause;
    void Start()
    {

        if (!main) main = Camera.main;
        player = GameManager.Instance.GetPlayerTransform();
        //if (!player) player = FindObjectOfType<HeroControls>();
    }

    private void LateUpdate()
    {
        if(!isPause)
            CamMovement();
        //else
            ///do shit here
    }
    void CamMovement()
    {
        transform.position = Vector3.SmoothDamp(transform.position, player.position - (transform.forward - (transform.right * cameraOffsetX) - (transform.up * cameraOffsetY)) * distFromPlayer, ref smoothingVelocity, c_Lerp);
    }

public void togglePause()
    {
        isPause = !isPause;
    }
}
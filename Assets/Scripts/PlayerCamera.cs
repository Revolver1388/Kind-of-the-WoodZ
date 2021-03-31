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
    public bool isPause = false;
    public Transform _holdPos;
    void Start()
    {

        if (!main) main = Camera.main;
        player = GameManager.Instance.GetPlayerTransform();
        //if (!player) player = FindObjectOfType<HeroControls>();
    }

    private void LateUpdate()
    {
        CamMovement();
    }
    void CamMovement()
    {
        if (!isPause)
            transform.position = Vector3.SmoothDamp(transform.position, player.position - (transform.forward - (transform.right * cameraOffsetX) - (transform.up * cameraOffsetY)) * distFromPlayer, ref smoothingVelocity, c_Lerp);
        else
            transform.position = Vector3.SmoothDamp(transform.position, _holdPos.position - (transform.forward - (transform.up * cameraOffsetY)) * distFromPlayer, ref smoothingVelocity, c_Lerp);
    }

    public void togglePause()
    {
        isPause = !isPause;
    }
}
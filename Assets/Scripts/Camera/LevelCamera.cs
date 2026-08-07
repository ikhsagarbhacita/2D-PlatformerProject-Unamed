using NUnit.Framework;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class LevelCamera : MonoBehaviour
{
    public List<Player> playerList {  get; private set; }
    private CinemachineCamera cinemachine;

    [SerializeField] private Collider2D[] limits; 
    [SerializeField] private Transform cameraCenterPoint;
    public float minCameraSize;
    public float maxCameraSize;
    private float targetLensSize;

    private bool cameraActived;
    private bool cameraFocusedOnCenter;
    private Transform player1;
    private Transform player2;

    private void Awake()
    {
        cinemachine = GetComponentInChildren<CinemachineCamera>(true);
        //targetLensSize = cinemachine.Lens.OrthographicSize;
        targetLensSize = minCameraSize; // Adds

        EnableLimits(false);
        InvokeRepeating(nameof(UpdateCenterPointPosition), 0, 0.1f);
        //EnableCamera(false);
    }

    private void Update()
    {
        if (cameraActived == false)
            return;
       
        UpdateLenseSizeIfNeeded();
    }

    // 
    private void UpdateCameraStatus()
    {
        playerList = PlayerManager.instance.GetPlayerList();

        if (playerList.Count > 1)
        {
            SetNewTarget(cameraCenterPoint);

            player1 = playerList[0].transform;
            player2 = playerList[1].transform;
            cameraFocusedOnCenter = true;
        }
        else
        {
            cameraFocusedOnCenter = false;
            SetNewTarget(playerList[0].transform);
        }
    }

    // 
    public void UpdateCenterPointPosition()
    {
        if (cameraActived == false)
            return;

        if (cameraCenterPoint == false)
            return;

        Vector3 midPoint = (player1.position + player2.position) / 2;
        cameraCenterPoint.position = midPoint;
    }

    // 
    public void ChangeCameraLenseSize(float size)
    {
        targetLensSize = size;
    }

    // 
    private void UpdateLenseSizeIfNeeded()
    {
        float currentLenseSize = cinemachine.Lens.OrthographicSize;

        if (Mathf.Abs(currentLenseSize - targetLensSize) > 0.5f)
        {
            float newLensSize = Mathf.Lerp(currentLenseSize, targetLensSize, 0.5f * Time.deltaTime);
            cinemachine.Lens.OrthographicSize = newLensSize;
        }
        else
            cinemachine.Lens.OrthographicSize = targetLensSize;
    }

    public void EnableCamera(bool enable)
    {
        cameraActived = enable;
        cinemachine.gameObject.SetActive(enable);
    }

    // 
    public void EnableLimits(bool enable)
    {
        foreach (var collider in limits)
        {
            collider.enabled = enable;
        }
    }

    public void SetNewTarget(Transform newTarget)
    {
        cinemachine.Follow = newTarget; // 
    }

    private void OnEnable()
    {
        PlayerManager.OnPlayerRespawn += UpdateCameraStatus;
        PlayerManager.OnPlayerDeath += UpdateCameraStatus;
    }

    private void OnDisable()
    {
        PlayerManager.OnPlayerRespawn -= UpdateCameraStatus;
        PlayerManager.OnPlayerDeath -= UpdateCameraStatus;
    }
}

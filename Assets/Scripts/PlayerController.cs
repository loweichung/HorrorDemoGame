using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using UnityStandardAssets.Utility;

public class PlayerController : MonoBehaviour
{
    private PlayerControls controls;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float cameraRotationX = 0f;
    private float distanceTravelled;

    [SerializeField]
    private CurveControlledBob headBob = new CurveControlledBob();
    [SerializeField]
    private CharacterController controller;
    [SerializeField]
    private Camera playerCamera;
    [SerializeField]
    private float moveSpeed = 5f;
    [SerializeField]
    private float mouseSensitivity = 0.1f;
    [SerializeField]
    private AudioSource footstepSource;
    [SerializeField]
    private AudioClip[] footstepClip;
    [SerializeField]
    private float stepDistance = 2.0f;

    void Awake()
    {
        controls = new PlayerControls();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        headBob.Setup(playerCamera, 1.0f);
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Update()
    {
        // 讀取數值 (輪詢方式，適合移動)
        moveInput = controls.Player.Move.ReadValue<Vector2>();
        lookInput = controls.Player.Look.ReadValue<Vector2>();

        Vector3 handBob = headBob.DoHeadBob(0.8f, moveInput.magnitude > 0.1f);
        playerCamera.transform.localPosition = handBob;

        UpdateFootsteps();
        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    void HandleRotation()
    {
        // 左右旋轉身體
        transform.Rotate(Vector3.up * lookInput.x * mouseSensitivity);

        // 上下旋轉鏡頭
        cameraRotationX -= lookInput.y * mouseSensitivity;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(cameraRotationX, 0, 0);
    }

    void UpdateFootsteps()
    {
        if (moveInput.magnitude > 0.1f)
        {
            Vector3 horizontalVelcity = new Vector3(controller.velocity.x, 0, controller.velocity.z);
            distanceTravelled += horizontalVelcity.magnitude * Time.deltaTime;
            if (distanceTravelled >= stepDistance)
            {
                PlayFootstepAudio();
                distanceTravelled = 0;
            }
        }
    }
    void PlayFootstepAudio()
    {
        if (footstepClip.Length > 0)
        {
            int index = Random.Range(0, footstepClip.Length);

            footstepSource.pitch = Random.Range(0.9f, 1.1f);

            footstepSource.PlayOneShot(footstepClip[index]);
        }
    }
}
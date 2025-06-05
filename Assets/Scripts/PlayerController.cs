using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace SCAD
{
    public class PlayerController : Singleton<PlayerController>
    {
        [SerializeField]
        GameObject playerCamera;

        [SerializeField]
        float normalSpeed = 3;

        [SerializeField]
        float stealthSpeed = 1.5f;

        [SerializeField]
        float runSpeed = 5;

        [SerializeField]
        float turnSpeed = 90;

        [SerializeField]
        float acceleration = 20;

        [SerializeField]
        float deceleration = 50;

        [SerializeField]
        float normalNoiseRange = 3;

        [SerializeField]
        float stealthNoiseRange = 1.5f;

        [SerializeField]
        float runNoiseRange = 6;

      

        float maxSpeed = 3;

        float maxNoiseRange;

        float noiseRange;
        public float NoiseRange
        {
            get { return noiseRange; }
        }

        Vector3 currentVelocity;
        public float Speed
        {
            get{ return currentVelocity.magnitude; }
        }

        CharacterController characterController;

        Vector2 moveInput;
        Vector2 aimInput;

        float aimSpeed = 90;

        [SerializeField]
        [Range(1, 10)]
        float mouseSensitivity = 5.5f;

        float yaw = 0;
        float pitch = 0;

        float maxPitch = 80;
        float minPitch = -80;

        bool inversePitch = false;


        protected override void Awake()
        {
            base.Awake();
            characterController = GetComponent<CharacterController>();
            yaw = transform.eulerAngles.y;
            pitch = 0;


            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            CheckInput();

            UpdateRotation();

            UpdatePosition();
        }

        private void UpdatePosition()
        {
            Vector3 targetDirection;
            float targetSpeed;


            if (moveInput.magnitude < Mathf.Epsilon)
            {
                noiseRange = 0;
                targetDirection = transform.forward; // Keep forward direction
                targetSpeed = Mathf.MoveTowards(currentVelocity.magnitude, 0f, deceleration * Time.deltaTime);
            }
            else
            {
                noiseRange = maxNoiseRange;
                targetDirection = transform.TransformDirection(new Vector3(moveInput.x, 0, moveInput.y)).normalized;
                targetSpeed = Mathf.MoveTowards(currentVelocity.magnitude, maxSpeed, acceleration * Time.deltaTime);
            }

            if (currentVelocity.magnitude > Mathf.Epsilon)
                targetDirection = Vector3.RotateTowards(currentVelocity.normalized, targetDirection, turnSpeed * Time.deltaTime, 0f).normalized;


            currentVelocity = targetDirection * targetSpeed;

            characterController.Move(currentVelocity * Time.deltaTime);

        }

        private void UpdateRotation()
        {
            yaw += aimSpeed * mouseSensitivity * aimInput.x * Time.deltaTime;
            yaw %= 360;

            characterController.transform.eulerAngles = Vector3.up * yaw;

            pitch += aimSpeed * mouseSensitivity * aimInput.y * Time.deltaTime * (inversePitch ? 1 : -1);

            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            playerCamera.transform.localEulerAngles = Vector3.right * pitch;

        }

        void CheckInput()
        {
            moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            aimInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            maxSpeed = normalSpeed;
            maxNoiseRange = normalNoiseRange;

            if (Input.GetKey(KeyBinding.StealthKey))
            {
                maxSpeed = stealthSpeed;
                maxNoiseRange = stealthNoiseRange;
            }
            else if (Input.GetKey(KeyBinding.SprintKey))
            {
                maxSpeed = runSpeed;
                maxNoiseRange = runNoiseRange;
            }



        }
        

    }
    
}

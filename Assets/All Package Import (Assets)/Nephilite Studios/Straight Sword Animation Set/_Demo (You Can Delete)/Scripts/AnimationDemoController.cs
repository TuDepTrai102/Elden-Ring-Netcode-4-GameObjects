using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class AnimationDemoController : MonoBehaviour
    {
        //CHARACTER CONTROLLER
        CharacterController characterController;

        //ANIMATOR
        [Header("Animator")]
        Animator animator;
        public bool isUsingRootMotion;
        public bool allowMovement = true;
        public bool allowRotation = true;

        //INPUT VARIABLES
        [Header("INPUTS")]
        [SerializeField] float verticalMovement;
        [SerializeField] float horizontalMovement;
        [SerializeField] float mouseX;
        [SerializeField] float mouseY;

        //PLAYER VARIABLES
        [Header("Player")]
        [SerializeField] bool isDualWielding;
        [SerializeField] bool isBlocking;
        [SerializeField] bool isStrafing;
        [SerializeField] bool isSprinting;
        [SerializeField] bool isRunning;
        [SerializeField] bool isWalking;
        [SerializeField] bool isTwoHandingWeapon;
        [SerializeField] bool isJumping;
        [SerializeField] bool isGrounded;
        [SerializeField] bool isChargingAttack;
        [SerializeField] float rotationSpeed;
        [SerializeField] float sprintSpeed;
        [SerializeField] float runningSpeed;
        [SerializeField] float walkingSpeed;
        [SerializeField] float jumpHeight = 5;

        [Header("Gravity")]
        [SerializeField] Vector3 yVelocity;
        [SerializeField] float groundedYVelocity = -20;
        [SerializeField] float fallStartYVelocity = -5;
        [SerializeField] float graivityForce = -9.81f;
        [SerializeField] float groundCheckSphereRadius = 1;
        [SerializeField] LayerMask groundLayer;
        private bool fallingVelocitySet = false;
        private float inAirTimer = 0;

        [Header("Player Debug")]
        [SerializeField] float moveAmount;
        [SerializeField] bool isPerformingAction;
        [SerializeField] bool isPerformingBackStep;
        private Vector3 moveDirection;
        private Vector3 jumpDirection;

        //CAMERA VARIABLES
        [Header("Camera")]
        [SerializeField] float leftAndRightLookSpeed;
        [SerializeField] float upAndDownLookSpeed;
        [SerializeField] float minimumPivot;
        [SerializeField] float maximumPivot;
        [SerializeField] GameObject playerCamera;
        [SerializeField] GameObject playerCameraPivot;
        [SerializeField] Camera cameraObject;
        [SerializeField] ParticleSystem lockOnTransform;

        [Header("Camera Debug")]
        [SerializeField] float leftandRightLookAngle;
        [SerializeField] float upAndDownLookAngle;
        private Vector3 cameraFollowVelocity = Vector3.zero;

        //ATTACK VARIABLES
        [SerializeField] string attackLastPerformed;
        [SerializeField] bool lightAttackInput;
        [SerializeField] bool chargeAttackInput;
        [SerializeField] bool isLockedOn;

        [Header("Main Hand Weapon")]
        [SerializeField] ParticleSystem mainWeaponFX;
        [SerializeField] ParticleSystem mainWeaponChargeFX;

        [Header("Off Hand Weapon")]
        [SerializeField] GameObject offHandWeapon;
        [SerializeField] ParticleSystem offWeaponFX;
        [SerializeField] ParticleSystem offWeaponChargeFX;

        string main_Light_Attack_01 = "Main_Light_Attack_01";
        string main_Light_Attack_02 = "Main_Light_Attack_02";

        string th_Light_Attack_01 = "TH_Light_Attack_01";
        string th_Light_Attack_02 = "TH_Light_Attack_02";

        string dw_Light_Attack_01 = "DW_Light_Attack_01";
        string dw_Light_Attack_02 = "DW_Light_Attack_02";

        string main_Charge_Attack_01 = "Main_Charge_Attack_01_Wind_Up";
        string main_Charge_Attack_02 = "Main_Charge_Attack_02_Wind_Up";

        string th_Charge_Attack_01 = "TH_Charge_Attack_01_Wind_Up";
        string th_Charge_Attack_02 = "TH_Charge_Attack_02_Wind_Up";

        string dw_Charge_Attack_01 = "DW_Charge_Attack_01_Wind_Up";
        string dw_Charge_Attack_02 = "DW_Charge_Attack_02_Wind_Up";

        private void Awake()
        {
            animator = GetComponent<Animator>();
            characterController = GetComponent<CharacterController>();
            cameraObject = Camera.main;
        }

        #region UPDATE

        private void Update()
        {
            HandleInputs();
            UpdateAnimatorParameters();

            if (!isPerformingAction)
            {
                HandleAllPlayerLocomotion();
            }

            isPerformingAction = animator.GetBool("isPerformingAction");
            isPerformingBackStep = animator.GetBool("isPerformingBackStep");
            animator.SetBool("isGrounded", isGrounded);
            animator.SetBool("isBlocking", isBlocking);
            animator.SetBool("isChargingAttack", isChargingAttack);
        }

        private void FixedUpdate()
        {

        }

        private void LateUpdate()
        {
            HandleCameraActions();
        }

        #endregion

        #region INPUTS

        private void HandleInputs()
        {
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");

            if (Input.GetKeyDown(KeyCode.C))
            {
                isChargingAttack = true;
            }
            if (Input.GetKeyUp(KeyCode.C))
            {
                isChargingAttack = false;
            }

            HandleLightAttackCombo();
            HandleChargeAttackCombo();
            HandleBackStepAttack();
            HandleBlockToggle();
            HandleBackStep();
            HandleRoll();
            HandleSprint();
            HandleWalkOrRun();
            HandleLockOn();
            HandleTwoHand();
            HandleLightAttack();
            HandleChargeAttack();
            HandleJumpAttack();
            HandleToggleOffHandWeapon();

            if (Input.GetKey(KeyCode.W))
            {
                verticalMovement = 1;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                verticalMovement = -1;
            }
            else
            {
                verticalMovement = 0;
            }

            if (Input.GetKey(KeyCode.D))
            {
                horizontalMovement = 1;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                horizontalMovement = -1;
            }
            else
            {
                horizontalMovement = 0;
            }

            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalMovement) + Mathf.Abs(verticalMovement));
        }

        #endregion

        #region LOCOMOTION

        private void HandleAllPlayerLocomotion()
        {
            HandleGroundCheck();
            HandlePlayerRotation();
            HandlePlayerMovement();
            HandlePlayerFreeFallMovement();
            HandleJump();
            ApplyForwardJumpForceOverTime();
        }

        private void HandleWalkOrRun()
        {
            if (isSprinting || isPerformingAction)
                return;

            if (Input.GetKeyUp(KeyCode.V))
            {
                if (isWalking || !isRunning)
                {
                    isWalking = false;
                    isRunning = true;
                }
                else if (!isWalking || isRunning)
                {
                    isWalking = true;
                    isRunning = false;
                }
            }
        }

        private void HandlePlayerRotation()
        {
            if (!allowRotation)
                return;

            if (isStrafing)
            {
                Vector3 rotationDirection = moveDirection;
                rotationDirection = cameraObject.transform.forward;
                rotationDirection.y = 0;
                rotationDirection.Normalize();
                Quaternion tr = Quaternion.LookRotation(rotationDirection);
                Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);
                transform.rotation = targetRotation;
            }
            else
            {
                Vector3 targetDirection = Vector3.zero;
                targetDirection = cameraObject.transform.forward * verticalMovement;
                targetDirection = targetDirection + cameraObject.transform.right * horizontalMovement;
                targetDirection.Normalize();
                targetDirection.y = 0;

                if (targetDirection == Vector3.zero)
                {
                    targetDirection = transform.forward;
                }

                Quaternion turnRotation = Quaternion.LookRotation(targetDirection);
                Quaternion targetRotation = Quaternion.Slerp(transform.rotation, turnRotation, rotationSpeed * Time.deltaTime);
                transform.rotation = targetRotation;
            }
        }

        private void HandlePlayerMovement()
        {
            if (!allowMovement)
                return;

            if (!isGrounded)
                return;

            moveDirection = cameraObject.transform.forward * verticalMovement;
            moveDirection = moveDirection + cameraObject.transform.right * horizontalMovement;
            moveDirection.Normalize();
            moveDirection.y = 0;

            if (isSprinting)
            {
                moveDirection = moveDirection * sprintSpeed * Time.deltaTime;
                characterController.Move(moveDirection);
                return;
            }
            else if (isRunning)
            {
                moveDirection = moveDirection * runningSpeed * Time.deltaTime;
                characterController.Move(moveDirection);
                return;
            }
            else if (isWalking)
            {
                moveDirection = moveDirection * walkingSpeed * Time.deltaTime;
                characterController.Move(moveDirection);
                return;
            }
        }

        private void HandlePlayerFreeFallMovement()
        {
            if (!allowMovement)
                return;

            if (isGrounded)
                return;

            moveDirection = cameraObject.transform.forward * verticalMovement;
            moveDirection = moveDirection + cameraObject.transform.right * horizontalMovement;
            moveDirection.Normalize();
            moveDirection.y = 0;

            if (isSprinting)
            {
                moveDirection = moveDirection * sprintSpeed * Time.deltaTime;
                characterController.Move(moveDirection / 3);
                return;
            }
            else if (isRunning)
            {
                moveDirection = moveDirection * runningSpeed * Time.deltaTime;
                characterController.Move(moveDirection / 3);
                return;
            }
            else if (isWalking)
            {
                moveDirection = moveDirection * walkingSpeed * Time.deltaTime;
                characterController.Move(moveDirection / 3);
                return;
            }
        }

        private void HandleLockOn()
        {
            if (Input.GetKeyUp(KeyCode.L))
            {
                isLockedOn = !isLockedOn;

                if (isLockedOn)
                {
                    lockOnTransform.Play();
                    isStrafing = true;
                }
                else
                {
                    lockOnTransform.Stop();
                    isStrafing = false;
                }
            }
        }

        private void HandleSprint()
        {
            if (isPerformingAction)
                return;

            if (!isGrounded)
                return;

            if (isLockedOn && !isSprinting)
            {
                isStrafing = true;
            }

            if (Input.GetKey(KeyCode.LeftShift) && moveAmount > 0)
            {
                isSprinting = true;
                isWalking = false;
                isRunning = true;
            }
            else
            {
                isSprinting = false;
            }
        }

        private void HandleJump()
        {
            if (isPerformingAction)
                return;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isTwoHandingWeapon)
                {
                    PlayActionAnimation("TH_Jump_01", false, false, true, false);
                }
                else
                {
                    PlayActionAnimation("Main_Jump_01", false, false, true, false);
                }

                Vector3 moveDirection;
                isJumping = true;


                moveDirection = cameraObject.transform.forward * verticalMovement;
                moveDirection += cameraObject.transform.right * horizontalMovement;
                moveDirection.y = 0;

                if (moveDirection != Vector3.zero)
                {
                    Quaternion jumpRotation = Quaternion.LookRotation(moveDirection);
                    transform.rotation = jumpRotation;

                    if (isSprinting)
                    {
                        jumpDirection = moveDirection;
                        return;
                    }
                    else if (moveAmount > 0.5)
                    {
                        jumpDirection = moveDirection * 0.5f;
                        return;
                    }
                    else if (moveAmount <= 0.5)
                    {
                        jumpDirection = moveDirection * 0.25f;
                    }
                }
                else
                {
                    jumpDirection = Vector3.zero;
                }
            }
        }

        private void HandleGroundCheck()
        {
            isGrounded = Physics.CheckSphere(transform.position, groundCheckSphereRadius, groundLayer);

            if (isGrounded)
            {
                if (yVelocity.y < 0)
                {
                    inAirTimer = 0;
                    fallingVelocitySet = false;
                    yVelocity.y = groundedYVelocity;
                }
            }
            else
            {
                if (!isJumping && !fallingVelocitySet)
                {
                    fallingVelocitySet = true;
                    yVelocity.y = fallStartYVelocity;
                }

                inAirTimer = inAirTimer + Time.deltaTime;
                yVelocity.y += graivityForce * Time.deltaTime;
            }

            characterController.Move(yVelocity * Time.deltaTime);
        }

        public void ApplyJumpingVelocity()
        {
            yVelocity.y = Mathf.Sqrt(jumpHeight * -2 * graivityForce);
        }

        private void ApplyForwardJumpForceOverTime()
        {
            if (isJumping)
            {
                characterController.Move(jumpDirection * runningSpeed * Time.deltaTime);
            }
        }

        public void ResetIsJumping()
        {
            isJumping = false;
        }

        #endregion

        #region ACTIONS

        public void HandleManualAnimation(string animationName, bool playManualOffHandWeaponFX, bool playManualMainHandWeaponFX)
        {
            PlayActionAnimation(animationName, true, true);

            if (playManualOffHandWeaponFX)
            {
                offWeaponFX.Stop();
                offWeaponFX.Play();
            }
            if (playManualMainHandWeaponFX)
            {
                mainWeaponFX.Stop();
                mainWeaponFX.Play();
            }
        }

        private void HandleToggleOffHandWeapon()
        {
            if (isPerformingAction)
                return;

            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                isBlocking = false;

                if (offHandWeapon.activeInHierarchy)
                {
                    offHandWeapon.SetActive(false);
                }
                else
                {
                    if (isTwoHandingWeapon)
                    {
                        isTwoHandingWeapon = false;
                        animator.SetBool("isTwoHandingWeapon", isTwoHandingWeapon);
                    }

                    offHandWeapon.SetActive(true);
                }

                isDualWielding = offHandWeapon.activeInHierarchy;
            }
        }

        private void HandleTwoHand()
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                isBlocking = false;
                isTwoHandingWeapon = !isTwoHandingWeapon;
                animator.SetBool("isTwoHandingWeapon", isTwoHandingWeapon);

                if (isTwoHandingWeapon)
                {
                    isDualWielding = false;
                    offHandWeapon.SetActive(false);
                }
            }
        }

        private void HandleRoll()
        {
            if (isPerformingAction)
                return;

            if (Input.GetKeyUp(KeyCode.R))
            {
                animator.SetBool("isPerformingBackStep", true);

                if (isTwoHandingWeapon)
                {
                    PlayActionAnimation("TH_Roll_01", true);
                }
                else
                {
                    PlayActionAnimation("Main_Roll_01", true);
                }
            }
        }

        private void HandleLightAttack()
        {
            if (isPerformingAction)
                return;

            if (!isGrounded)
                return;

            if (Input.GetKeyDown(KeyCode.E))
            {
                lightAttackInput = true;
                mainWeaponFX.Stop();
                mainWeaponFX.Play();

                if (isDualWielding)
                {
                    offWeaponFX.Stop();
                    offWeaponFX.Play();
                }
            }

            if (lightAttackInput)
            {
                lightAttackInput = false;

                if (isDualWielding)
                {
                    if (isSprinting)
                    {
                        PlayActionAnimation("DW_Running_Attack_01", true);
                        return;
                    }

                    PlayActionAnimation("DW_Light_Attack_01", true);
                    attackLastPerformed = dw_Light_Attack_01;
                    return;
                }

                if (isTwoHandingWeapon)
                {
                    if (isSprinting)
                    {
                        PlayActionAnimation("TH_Running_Attack_01", true);
                        return;
                    }

                    PlayActionAnimation("TH_Light_Attack_01", true);
                    attackLastPerformed = th_Light_Attack_01;
                }
                else
                {
                    if (isSprinting)
                    {
                        PlayActionAnimation("Main_Running_Attack_01", true);
                        return;
                    }

                    PlayActionAnimation("Main_Light_Attack_01", true);
                    attackLastPerformed = main_Light_Attack_01;
                }
            }
        }

        private void HandleLightAttackCombo()
        {
            if (!isGrounded)
                return;

            if (isPerformingAction)
            {
                if (attackLastPerformed == main_Light_Attack_01)
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        PlayActionAnimation("Main_Light_Attack_02", true);
                        mainWeaponFX.Stop();
                        mainWeaponFX.Play();
                        attackLastPerformed = main_Light_Attack_02;
                        return;
                    }
                }
                else if (attackLastPerformed == main_Light_Attack_02)
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        PlayActionAnimation("Main_Light_Attack_01", true);
                        mainWeaponFX.Stop();
                        mainWeaponFX.Play();
                        attackLastPerformed = main_Light_Attack_01;
                    }
                }
                else if (attackLastPerformed == th_Light_Attack_01)
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        PlayActionAnimation("TH_Light_Attack_02", true);
                        mainWeaponFX.Stop();
                        mainWeaponFX.Play();
                        attackLastPerformed = th_Light_Attack_02;
                    }
                }
                else if (attackLastPerformed == th_Light_Attack_02)
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        PlayActionAnimation("TH_Light_Attack_01", true);
                        mainWeaponFX.Stop();
                        mainWeaponFX.Play();
                        attackLastPerformed = th_Light_Attack_01;
                    }
                }
                else if (attackLastPerformed == dw_Light_Attack_01)
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        PlayActionAnimation("DW_Light_Attack_02", true);
                        mainWeaponFX.Stop();
                        mainWeaponFX.Play();
                        offWeaponFX.Stop();
                        offWeaponFX.Play();
                        attackLastPerformed = dw_Light_Attack_02;
                    }
                }
                else if (attackLastPerformed == dw_Light_Attack_02)
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        PlayActionAnimation("DW_Light_Attack_01", true);
                        mainWeaponFX.Stop();
                        mainWeaponFX.Play();
                        offWeaponFX.Stop();
                        offWeaponFX.Play();
                        attackLastPerformed = dw_Light_Attack_01;
                    }
                }
            }
        }

        private void HandleChargeAttack()
        {
            if (!isGrounded)
                return;

            if (Input.GetKeyDown(KeyCode.C))
            {
                chargeAttackInput = true;
            }

            if (isPerformingAction)
                return;

            if (chargeAttackInput)
            {
                chargeAttackInput = false;

                if (isDualWielding)
                {
                    PlayActionAnimation("DW_Charge_Attack_01", true);
                    mainWeaponChargeFX.Stop();
                    mainWeaponChargeFX.Play();
                    offWeaponChargeFX.Stop();
                    offWeaponChargeFX.Play();
                    attackLastPerformed = dw_Charge_Attack_01;
                    return;
                }

                if (isTwoHandingWeapon)
                {
                    PlayActionAnimation("TH_Charge_Attack_01", true);
                    mainWeaponChargeFX.Stop();
                    mainWeaponChargeFX.Play();
                    attackLastPerformed = th_Charge_Attack_01;
                }
                else
                {
                    PlayActionAnimation("Main_Charge_Attack_01", true);
                    mainWeaponChargeFX.Stop();
                    mainWeaponChargeFX.Play();
                    attackLastPerformed = main_Charge_Attack_01;
                }
            }
        }

        private void HandleChargeAttackCombo()
        {
            if (!isGrounded)
                return;

            if (isPerformingAction)
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    chargeAttackInput = true;
                }

                if (chargeAttackInput)
                {
                    chargeAttackInput = false;

                    if (attackLastPerformed == main_Charge_Attack_01)
                    {
                        if (Input.GetKeyDown(KeyCode.C))
                        {
                            PlayActionAnimation("Main_Charge_Attack_02", true);
                            mainWeaponChargeFX.Stop();
                            mainWeaponChargeFX.Play();
                            attackLastPerformed = main_Charge_Attack_02;
                        }
                    }
                    else if (attackLastPerformed == main_Charge_Attack_02)
                    {
                        if (Input.GetKeyDown(KeyCode.C))
                        {
                            PlayActionAnimation("Main_Charge_Attack_01", true);
                            mainWeaponChargeFX.Stop();
                            mainWeaponChargeFX.Play();
                            attackLastPerformed = main_Charge_Attack_01;
                        }
                    }
                    else if (attackLastPerformed == th_Charge_Attack_01)
                    {
                        if (Input.GetKeyDown(KeyCode.C))
                        {
                            PlayActionAnimation("TH_Charge_Attack_02", true);
                            mainWeaponChargeFX.Stop();
                            mainWeaponChargeFX.Play();
                            attackLastPerformed = th_Charge_Attack_02;
                        }
                    }
                    else if (attackLastPerformed == th_Charge_Attack_02)
                    {
                        if (Input.GetKeyDown(KeyCode.C))
                        {
                            PlayActionAnimation("TH_Charge_Attack_01", true);
                            mainWeaponChargeFX.Stop();
                            mainWeaponChargeFX.Play();
                            attackLastPerformed = th_Charge_Attack_01;
                        }
                    }
                    else if (attackLastPerformed == dw_Charge_Attack_01)
                    {
                        if (Input.GetKeyDown(KeyCode.C))
                        {
                            PlayActionAnimation("DW_Charge_Attack_02", true);
                            mainWeaponChargeFX.Stop();
                            mainWeaponChargeFX.Play();
                            offWeaponChargeFX.Stop();
                            offWeaponChargeFX.Play();
                            attackLastPerformed = dw_Charge_Attack_02;
                        }
                    }
                    else if (attackLastPerformed == dw_Charge_Attack_02)
                    {
                        if (Input.GetKeyDown(KeyCode.C))
                        {
                            PlayActionAnimation("DW_Charge_Attack_01", true);
                            mainWeaponChargeFX.Stop();
                            mainWeaponChargeFX.Play();
                            offWeaponChargeFX.Stop();
                            offWeaponChargeFX.Play();
                            attackLastPerformed = dw_Charge_Attack_01;
                        }
                    }
                }
            }
        }

        private void HandleJumpAttack()
        {
            if (isGrounded)
                return;

            if (isPerformingAction)
                return;

            if (Input.GetKeyUp(KeyCode.C))
            {
                if (isDualWielding)
                {
                    PlayActionAnimation("DW_Jumping_Heavy_Attack_01", false, false);
                    mainWeaponChargeFX.Stop();
                    mainWeaponChargeFX.Play();
                    offWeaponChargeFX.Stop();
                    offWeaponChargeFX.Play();
                    attackLastPerformed = dw_Charge_Attack_01;
                    return;
                }

                if (isTwoHandingWeapon)
                {
                    PlayActionAnimation("TH_Jumping_Heavy_Attack_01", false, false);
                    mainWeaponChargeFX.Stop();
                    mainWeaponChargeFX.Play();
                    attackLastPerformed = th_Charge_Attack_01;
                }
                else
                {
                    PlayActionAnimation("Main_Jumping_Heavy_Attack_01", false, false);
                    mainWeaponChargeFX.Stop();
                    mainWeaponChargeFX.Play();
                    attackLastPerformed = main_Charge_Attack_01;
                }
            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                if (isDualWielding)
                {
                    PlayActionAnimation("DW_Jumping_Light_Attack_01", false, false);
                    mainWeaponChargeFX.Stop();
                    mainWeaponChargeFX.Play();
                    offWeaponChargeFX.Stop();
                    offWeaponChargeFX.Play();
                    attackLastPerformed = dw_Charge_Attack_01;
                    return;
                }

                if (isTwoHandingWeapon)
                {
                    PlayActionAnimation("TH_Jumping_Light_Attack_01", false, false);
                    mainWeaponChargeFX.Stop();
                    mainWeaponChargeFX.Play();
                    attackLastPerformed = th_Charge_Attack_01;
                }
                else
                {
                    PlayActionAnimation("Main_Jumping_Light_Attack_01", false, false);
                    mainWeaponChargeFX.Stop();
                    mainWeaponChargeFX.Play();
                    attackLastPerformed = main_Charge_Attack_01;
                }
            }
        }

        private void HandleBackStep()
        {
            if (!isGrounded)
                return;

            if (isPerformingAction)
                return;

            if (Input.GetKeyUp(KeyCode.Tab))
            {
                attackLastPerformed = null;

                animator.SetBool("isPerformingBackStep", true);

                if (isTwoHandingWeapon)
                {
                    PlayActionAnimation("TH_Backstep_01", true);
                }
                else
                {
                    PlayActionAnimation("Main_Backstep_01", true);
                }
            }
        }

        private void HandleBackStepAttack()
        {
            if (!isGrounded)
                return;

            if (isPerformingBackStep)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    animator.SetBool("isPerformingBackStep", false);

                    if (isDualWielding)
                    {
                        PlayActionAnimation("DW_Backstep_Attack_01", true);
                        mainWeaponFX.Stop();
                        mainWeaponFX.Play();
                        offWeaponFX.Stop();
                        offWeaponFX.Play();
                        return;
                    }

                    if (isTwoHandingWeapon)
                    {
                        PlayActionAnimation("TH_Backstep_Attack_01", true);
                        mainWeaponFX.Stop();
                        mainWeaponFX.Play();
                    }
                    else
                    {
                        PlayActionAnimation("Main_Backstep_Attack_01", true);
                        mainWeaponFX.Stop();
                        mainWeaponFX.Play();
                    }
                }
            }
        }

        private void HandleBlockToggle()
        {
            if (!isGrounded)
                return;

            if (isPerformingAction)
                return;

            if (Input.GetKeyDown(KeyCode.B))
            {
                isBlocking = !isBlocking;

                if (!isTwoHandingWeapon && isBlocking)
                {
                    offHandWeapon.SetActive(true);
                }
                else
                {
                    offHandWeapon.SetActive(false);
                }
            }
        }

        #endregion

        #region CAMERA

        private void HandleCameraActions()
        {
            HandleCameraFollowPlayer();
            HandleCameraRotate();
        }

        private void HandleCameraFollowPlayer()
        {
            Vector3 targetPosition = Vector3.SmoothDamp(playerCamera.transform.position, transform.position, ref cameraFollowVelocity, 0.1f);
            playerCamera.transform.position = targetPosition;
        }

        private void HandleCameraRotate()
        {
            Vector3 cameraRotation;
            Quaternion targetCameraRotation;

            if (isLockedOn)
            {
                cameraRotation = lockOnTransform.transform.position - playerCamera.transform.position;
                cameraRotation.Normalize();
                cameraRotation.y = 0;

                targetCameraRotation = Quaternion.LookRotation(cameraRotation);
                playerCamera.transform.rotation = Quaternion.Slerp(playerCamera.transform.rotation, targetCameraRotation, Time.deltaTime * 250);

                cameraRotation = lockOnTransform.transform.position - playerCameraPivot.transform.position;
                cameraRotation.Normalize();

                targetCameraRotation = Quaternion.LookRotation(cameraRotation);
                Vector3 eulerAngle = targetCameraRotation.eulerAngles;
                eulerAngle.y = 0;
                playerCameraPivot.transform.localEulerAngles = eulerAngle;

                leftandRightLookAngle = playerCamera.transform.eulerAngles.y;
                upAndDownLookAngle = playerCameraPivot.transform.localRotation.x;
                return;
            }
            else
            {
                leftandRightLookAngle += (mouseX * leftAndRightLookSpeed) * Time.deltaTime;
                upAndDownLookAngle -= (mouseY * upAndDownLookSpeed) * Time.deltaTime;
                upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

                cameraRotation = Vector3.zero;
                cameraRotation.y = leftandRightLookAngle;
                targetCameraRotation = Quaternion.Euler(cameraRotation);
                playerCamera.transform.rotation = targetCameraRotation;

                cameraRotation = Vector3.zero;
                cameraRotation.x = upAndDownLookAngle;
                targetCameraRotation = Quaternion.Euler(cameraRotation);
                playerCameraPivot.transform.localRotation = targetCameraRotation;
            }
        }

        #endregion

        #region ANIMATOR

        private void PlayActionAnimation(string animation, bool isPerformingAction, bool isUsingRootMotion = true, bool allowMovement = false, bool allowRotation = false)
        {
            this.isUsingRootMotion = isUsingRootMotion;
            this.allowMovement = allowMovement;
            this.allowRotation = allowRotation;
            animator.SetBool("isPerformingAction", isPerformingAction);
            animator.CrossFade(animation, 0.2f);
        }

        private void UpdateAnimatorParameters()
        {
            float snappedVertical;
            float snappedHorizontal;

            #region Horizontal
            //This if chain will round the horizontal movement to -1, -0.5, 0, 0.5 or 1

            if (horizontalMovement > 0 && horizontalMovement <= 0.5f)
            {
                snappedHorizontal = 0.5f;
            }
            else if (horizontalMovement > 0.5f)
            {
                snappedHorizontal = 1;
            }
            else if (horizontalMovement < 0 && horizontalMovement >= -0.5f)
            {
                snappedHorizontal = -0.5f;
            }
            else if (horizontalMovement < -0.5f)
            {
                snappedHorizontal = -1;
            }
            else
            {
                snappedHorizontal = 0;
            }

            #endregion

            #region Vertical
            //This if chain will round the vertical movement to -1, -0.5, 0, 0.5 or 1

            if (verticalMovement > 0 && verticalMovement <= 0.5f)
            {
                snappedVertical = 0.5f;
            }
            else if (verticalMovement > 0.5f)
            {
                snappedVertical = 1;
            }
            else if (verticalMovement < 0 && verticalMovement >= -0.5f)
            {
                snappedVertical = -0.5f;
            }
            else if (verticalMovement < -0.5f)
            {
                snappedVertical = -1;
            }
            else
            {
                snappedVertical = 0;
            }

            #endregion

            if (isSprinting)
            {
                isStrafing = false;
                animator.SetFloat("Horizontal", 0, 0.2f, Time.deltaTime);
                animator.SetFloat("Vertical", 2, 0.2f, Time.deltaTime);
            }
            else
            {
                if (isStrafing)
                {
                    if (isWalking)
                    {
                        animator.SetFloat("Vertical", snappedVertical / 2, 0.2f, Time.deltaTime);
                        animator.SetFloat("Horizontal", snappedHorizontal / 2, 0.2f, Time.deltaTime);
                    }
                    else
                    {
                        animator.SetFloat("Vertical", snappedVertical, 0.2f, Time.deltaTime);
                        animator.SetFloat("Horizontal", snappedHorizontal, 0.2f, Time.deltaTime);
                    }
                }
                else
                {
                    if (isWalking)
                    {
                        animator.SetFloat("Vertical", moveAmount / 2, 0.2f, Time.deltaTime);
                        animator.SetFloat("Horizontal", 0, 0.2f, Time.deltaTime);
                    }
                    else
                    {
                        animator.SetFloat("Vertical", moveAmount, 0.2f, Time.deltaTime);
                        animator.SetFloat("Horizontal", 0, 0.2f, Time.deltaTime);
                    }
                }
            }

            if (moveAmount == 0)
            {
                animator.SetFloat("Vertical", 0, 0.2f, Time.deltaTime);
                animator.SetFloat("Horizontal", 0, 0.2f, Time.deltaTime);
            }
        }

        private void OnAnimatorMove()
        {
            if (isUsingRootMotion)
            {
                Vector3 velocity = animator.deltaPosition;
                characterController.Move(velocity);
                transform.rotation *= animator.deltaRotation;
            }
        }

        #endregion
    }
}

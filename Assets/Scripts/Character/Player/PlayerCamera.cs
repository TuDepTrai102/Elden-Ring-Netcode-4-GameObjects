using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EldenRing.NT
{
    public class PlayerCamera : MonoBehaviour
    {
        public static PlayerCamera instance;
        public PlayerManager player;
        public Camera cameraObject;
        [SerializeField] Transform cameraPivotTransform;

        //  CHANGE THESE TWEAK CAMERA PERFORMANCE
        [Header("CAMERA SETTINGS")]
        [SerializeField] private float cameraSmoothSpeed = 1;    //  THE BIGGER THIS NUMBER, THE LONGER FOR THE CAMERA TO REACH IT'S POSITION DURING MOVEMENT
        [SerializeField] float leftAndRightRotationSpeed = 220;
        [SerializeField] float upAndDownRotationSpeed = 220;
        [SerializeField] float minimumPivot = -30;  //  THE LOWEST POINT YOU ARE ABLE TO LOOK DOWN
        [SerializeField] float maximumPivot = 60;   //  THE HIGHEST POINT YOU ARE ABLE TO LOOK UP
        [SerializeField] float cameraCollisionRadius = 0.2f;
        [SerializeField] LayerMask colliderWithLayers;

        [Header("CAMERA VALUES")]
        [SerializeField] private Vector3 cameraVelocity;
        [SerializeField] private Vector3 cameraObjectPosition;  //  USED FOR CAMERA COLLISIONS (MOVES THE CAMERA OBJECT TO THIS POSITION UPON COLLIDING)
        [SerializeField] float leftAndRightLookAngle;
        [SerializeField] float upAndDownLookAngle;
        private float cameraZPosition;    //  VALUES USED FOR CAMERA COLLISIONS
        private float targetCameraZPosition;     //  VALUES USED FOR CAMERA COLLISIONS

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            cameraZPosition = cameraObject.transform.localPosition.z;
        }

        public void HandleAllCameraActions()
        {
            if (player !=  null)
            {
                HandleFollowTarget();
                HandleRotations();
                HandleCollisions();
            }
        }

        private void HandleFollowTarget()
        {
            Vector3 targetCameraPosition = Vector3.SmoothDamp
                (transform.position,
                player.transform.position,
                ref cameraVelocity,
                cameraSmoothSpeed * Time.deltaTime);
            transform.position = targetCameraPosition;
        }

        private void HandleRotations()
        {
            //  OF LOCKED ON, FORCE ROTATION TOWARDS TARGET
            //  ELSE ROTATE REGULARLY

            //  ROTATE LEFT AND RIGHT BASED ON HORIZONTAL MOVEMENT ON THE MOUSE (RIGHT JOYSTICK)
            leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontal_Input * leftAndRightRotationSpeed) * Time.deltaTime;
            //  ROTATE UP AND DOWN BASED ON VERTICAL MOVEMENT ON THE MOUSE (RIGHT JOYSTICK)
            upAndDownLookAngle -= (PlayerInputManager.instance.cameraVertical_Input * upAndDownRotationSpeed) * Time.deltaTime;
            //  CLAMP THE UP AND DOWN LOOK ANGLE BETWEEN A MIN AND MAX VALUE (PIVOT)
            upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

            Vector3 cameraRotation = Vector3.zero;
            Quaternion targetRotation;

            //  ROTATE THIS GAME-OBJECT LEFT AND RIGHT
            cameraRotation.y = leftAndRightLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            transform.rotation = targetRotation;

            //  ROTATE THE PIVOT GAME-OBJECT UP AND DOWN
            cameraRotation = Vector3.zero;
            cameraRotation.x = upAndDownLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            cameraPivotTransform.localRotation = targetRotation;
        }

        private void HandleCollisions()
        {
            targetCameraZPosition = cameraZPosition;

            RaycastHit hit;
            //  DIRECTION FOR COLLISION CHECK
            Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
            direction.Normalize();

            //  WE CHECK IF THERE IS AN OBJECT IN FRONT IF OUR DESIRED DIRECTION ^ (SEE ABOVE)
            if (Physics.SphereCast
                (cameraPivotTransform.position, 
                cameraCollisionRadius, direction, out hit, 
                Mathf.Abs(targetCameraZPosition), 
                colliderWithLayers))
            {
                //  IF THIS IS, WE GET OUR DISTANCE FROM IT
                float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
                //  WE THEN EQUATE OUR TARGET Z POSITION TO THE FOLLOWING
                targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
            }

            //  IF OUR TARGET POSITION IS LESS THAN OUR COLLISION RADIUS, WE SUBTRACT OUR COLLISION RADIUS (MAKING IT SNAP BACK)
            if (Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
            {
                targetCameraZPosition = -cameraCollisionRadius;
            }

            //  WE THEN APPLY OUR FINAL POSITION USING A LERP OVER A TIME OF 0.2F
            cameraObjectPosition.z = Mathf.Lerp
                (cameraObject.transform.localPosition.z, 
                targetCameraZPosition, 
                0.2f);
            cameraObject.transform.localPosition = cameraObjectPosition;
        }
    }
}
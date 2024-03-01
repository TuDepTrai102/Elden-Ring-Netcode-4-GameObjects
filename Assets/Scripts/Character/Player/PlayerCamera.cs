using System.Collections;
using System.Collections.Generic;
using TreeEditor;
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

        [Header("LOCK ON")]
        [SerializeField] float lockOnRadius = 20;
        [SerializeField] float minimumViewableAngle = -50;
        [SerializeField] float maximumViewableAngle = 50;
        [SerializeField] float lockOnTargetFollowSpeed = 0.2f;
        [SerializeField] float setCameraHeightSpeed = 1;
        [SerializeField] float unlockedCameraHeight = 1.65f;
        [SerializeField] float lockedCameraHeight = 2.00f;
        private Coroutine cameraLockOnHeightCoroutine;
        private List<CharacterManager> availableTargets = new List<CharacterManager>();
        public CharacterManager nearestLockOnTarget;
        public CharacterManager leftLockOnTarget;
        public CharacterManager rightLockOnTarget;

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
            if (player.playerNetworkManager.isLockedOn.Value)
            {
                //  MAIN PLAYER CAMERA OBJECT THIS ROTATES THIS GAMEOBJECT
                Vector3 rotationDirection = player.playerCombatManager.currentTarget.characterCombatManager.lockOnTransform.position - transform.position;
                rotationDirection.Normalize();
                rotationDirection.y = 0;

                Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lockOnTargetFollowSpeed);

                //  THIS ROTATES THE PIVOT OBJECT
                rotationDirection = player.playerCombatManager.currentTarget.characterCombatManager.lockOnTransform.position - cameraPivotTransform.position;
                rotationDirection.Normalize();

                targetRotation = Quaternion.LookRotation(rotationDirection);
                cameraPivotTransform.transform.rotation = Quaternion.Slerp(cameraPivotTransform.rotation, targetRotation, lockOnTargetFollowSpeed);

                //  SAVE OUR ROTATIONS TO OUR LOOK ANGLES, SO WHEN WE UNLOCK IT DOESN'T SNAP TOO FAR AWAY
                leftAndRightLookAngle = transform.eulerAngles.y;
                upAndDownLookAngle = transform.eulerAngles.x;
            }
            //  ELSE ROTATE REGULARLY
            else
            {
                //  ROTATE LEFT AND RIGHT BASED ON HORIZONTAL MOVEMENT ON THE MOUSE (RIGHT JOYSTICK)
                leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontal_Input * leftAndRightRotationSpeed) * Time.deltaTime;
                //  ROTATE UP AND DOWN BASED ON VERTICAL MOVEMENT ON THE MOUSE (RIGHT JOYSTICK)
                upAndDownLookAngle -= (PlayerInputManager.instance.cameraVertical_Input * upAndDownRotationSpeed) * Time.deltaTime;
                //  CLAMP THE UP AND DOWN LOOK ANGLE BETWEEN A MIN AND MAX VALUE (PIVOT)
                upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

                Vector3 cameraRotation = Vector3.zero;
                Quaternion targetRotation;

                //  ROTATE THIS GAMEOBJECT LEFT AND RIGHT
                cameraRotation.y = leftAndRightLookAngle;
                targetRotation = Quaternion.Euler(cameraRotation);
                transform.rotation = targetRotation;

                //  ROTATE THE PIVOT GAMEOBJECT UP AND DOWN
                cameraRotation = Vector3.zero;
                cameraRotation.x = upAndDownLookAngle;
                targetRotation = Quaternion.Euler(cameraRotation);
                cameraPivotTransform.localRotation = targetRotation;
            }
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

        public void HandleLocatingLockOnTargets()
        {
            float shortestDistance = Mathf.Infinity;               //  WILL BE USED TO DETERMINE THE TARGET CLOSET TO US
            float shortestDistanceOfRightTarget = Mathf.Infinity;  //  WILL BE USED TO DETERMINE SHORTEST DISTANCE ON ONE AXIS TO THE RIGHT OF CURRENT TARGET (+)
            float shortestDistanceOfLeftTarget = -Mathf.Infinity;  //  WILL BE USED TO DETERMINE SHORTEST DISTANCE ON ONE AXIS TO THE LEFT OF CURRENT TARGET (-)

            //  TO DO USE A LAYERMASK
            Collider[] colliders = Physics.OverlapSphere
                (player.transform.position, lockOnRadius, WorldUtilityManager.instance.GetCharacterLayers());

            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager lockOnTarget = colliders[i].GetComponent<CharacterManager>();

                if (lockOnTarget != null)
                {
                    //  CHECK IF THEY ARE WITHIN OUT FIELD OF VIEW
                    Vector3 lockOnTargetsDirection = lockOnTarget.transform.position - player.transform.position;
                    float distanceFromTarget = Vector3.Distance(player.transform.position, lockOnTarget.transform.position);
                    float viewableAngle = Vector3.Angle(lockOnTargetsDirection, cameraObject.transform.forward);

                    //  IF TARGET IS DEAD, CHECK THE NEXT POTENTIAL TARGET
                    if (lockOnTarget.isDead.Value)
                        continue;

                    //  IF TARGET IS US, CHECK THE NEXT POTENTIAL TARGET
                    if (lockOnTarget.transform.root == player.transform.root)
                        continue;

                    //  LASTLY IF THE TARGET IS OUTSIDE FIELD OF VIEW OR IS BLOCKED BY ENVIRO, CHECK NEXT POTENTIAL TARGET
                    if (viewableAngle > minimumViewableAngle && viewableAngle < maximumViewableAngle)
                    {
                        RaycastHit hit;

                        //  TODO ADD LAYER MASK FOR ENVIRONMENT LAYERS ONLY
                        if (Physics.Linecast(player.playerCombatManager.lockOnTransform.position, 
                            lockOnTarget.characterCombatManager.lockOnTransform.position, 
                            out hit, WorldUtilityManager.instance.GetEnviroLayers()))
                        {
                            //  WE HIT SOMETHING, WE CANNOT SEE OUR LOCK ON TARGET
                            continue;
                        }
                        else
                        {
                            //  OTHERWISE, ADD THEM TO POTENTIAL TARGET LIST
                            availableTargets.Add(lockOnTarget);
                        }
                    }
                }
            }

            //  WE NOW SORT THROUGH OUR POTENTIAL TARGETS TO SEE WHICH ONE WE LOCK ONTO FIRST
            for (int k = 0; k < availableTargets.Count; k++)
            {
                if (availableTargets[k] != null)
                {
                    float distanceFromTarget = Vector3.Distance(player.transform.position, availableTargets[k].transform.position);
                    Vector3 lockTargetsDirection = availableTargets[k].transform.position - player.transform.position;

                    if (distanceFromTarget < shortestDistance)
                    {
                        shortestDistance = distanceFromTarget;
                        nearestLockOnTarget = availableTargets[k];
                    }

                    //  IF WE ARE ALREADY LOCKED ON WHEN SEARCHING FOR TARGETS, SEARCHING FOR OUR NEAREST LEFT/RIGHT TARGETS
                    if (player.playerNetworkManager.isLockedOn.Value)
                    {
                        Vector3 relativeEnemyPosition = player.transform.InverseTransformPoint(availableTargets[k].transform.position);

                        var distanceFromLeftTarget = relativeEnemyPosition.x;
                        var distanceFromRightTarget = relativeEnemyPosition.x;

                        if (availableTargets[k] == player.playerCombatManager.currentTarget)
                            continue;

                        //  CHECK THE LEFT SIDE FOR TARGETS
                        if (relativeEnemyPosition.x < 0.00 && 
                            distanceFromLeftTarget > shortestDistanceOfLeftTarget)
                        {
                            shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                            leftLockOnTarget = availableTargets[k];
                        }
                        //  CHECK THE RIGHT SIDE FOR TARGETS
                        else if (relativeEnemyPosition.x >= 0.00 && 
                            distanceFromRightTarget < shortestDistanceOfRightTarget)
                        {
                            shortestDistanceOfRightTarget = distanceFromRightTarget;
                            rightLockOnTarget = availableTargets[k];
                        }
                    }
                }
                else
                {
                    ClearLockOnTargets();
                    player.playerNetworkManager.isLockedOn.Value = false;
                }
            }
        }

        public void SetLockCameraHeight()
        {
            if (cameraLockOnHeightCoroutine != null)
            {
                StopCoroutine(cameraLockOnHeightCoroutine);
            }

            cameraLockOnHeightCoroutine = StartCoroutine(SetCameraHeight());
        }

        public void ClearLockOnTargets()
        {
            nearestLockOnTarget = null;
            leftLockOnTarget = null;
            rightLockOnTarget = null;
            availableTargets.Clear();
        }

        public IEnumerator WaitThenFindNewTarget()
        {
            while (player.isPerformingAction)
            {
                yield return null;
            }

            ClearLockOnTargets();
            HandleLocatingLockOnTargets();

            if (nearestLockOnTarget != null)
            {
                player.playerCombatManager.SetTarget(nearestLockOnTarget);
                player.playerNetworkManager.isLockedOn.Value = true;
            }

            yield return null;
        }

        private IEnumerator SetCameraHeight()
        {
            float duration = 1;
            float timer = 0;

            Vector3 velocity = Vector3.zero;
            Vector3 newLockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, lockedCameraHeight);
            Vector3 newUnlockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, unlockedCameraHeight);

            while (timer < duration)
            {
                timer += Time.deltaTime;

                if (player != null)
                {
                    if (player.playerCombatManager.currentTarget != null)
                    {
                        cameraPivotTransform.transform.localPosition = 
                            Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newLockedCameraHeight, ref velocity, setCameraHeightSpeed);
                        
                        cameraPivotTransform.transform.localRotation =
                            Quaternion.Slerp(cameraPivotTransform.transform.localRotation, Quaternion.Euler(0, 0, 0), lockOnTargetFollowSpeed);
                    }
                    else
                    {
                        cameraPivotTransform.transform.localPosition = 
                            Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newUnlockedCameraHeight, ref velocity, setCameraHeightSpeed);
                    }
                }

                yield return null;
            }

            if (player != null)
            {
                if (player.playerCombatManager.currentTarget != null)
                {
                    cameraPivotTransform.transform.localPosition = newLockedCameraHeight;
                    cameraPivotTransform.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
                else
                {
                    cameraPivotTransform.transform.localPosition = newUnlockedCameraHeight;
                }
            }

            yield return null;
        }
    }
}
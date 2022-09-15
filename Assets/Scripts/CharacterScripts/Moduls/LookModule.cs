using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//����� �������� �� ������� ����������� ������ ���������
public class LookModule : CharacterBase
{
    [SerializeField] private Transform rotateAIMyCenter;

    [SerializeField] private Transform rotateCameraCenter;
    [SerializeField] private float verticalRotateSpeed = 1;
    [SerializeField] private float horizontalRotateSpeed = 0.5f;

    private float factVerticalRotateSpeed = 1;
    private float factHorizontalRotateSpeed = 0.5f;

    [Header("���� ����. �������")]
    [SerializeField] private float minVerticalAngle = -45;
    [SerializeField] private float maxVerticalAngle = 60;

    [Header("���� ����. ��������")]
    [SerializeField] private float minHorizontAngle = -90;
    [SerializeField] private float maxHorizontAngle = 65;

    private float yRotate = 0;
    private float yRotateCamera = 0;

    [Header("Anim Trigger Names")]
    [SerializeField] private string rightStepTrigger = "StepRight";
    [SerializeField] private string leftStepTrigger = "StepLeft";
    [SerializeField] private string RunRotateFloat = "RunRotate";

    [Header("AIM Position")]
    [SerializeField] private Camera _personCamera;
    [SerializeField] private float maxDistance = 150.0f;
    [SerializeField] private float minDistance = 10.0f;
    [SerializeField] private Transform TargetPoint;
    [SerializeField] private float guidanceSpeed = 50;

    private float startPos;
    private float inCar = 1;
    [SerializeField] private float minRayDistance = 0.1f;

    [SerializeField] private Transform raycastPoint;
    [SerializeField] private float rayDistance = 2;
    [SerializeField] private float moveCamSpeed = 50;

    private void Start() {
        startPos = mainCamera.transform.localPosition.z;
        inCar = startPos * 3;

        targetPos = startPos;

        factHorizontalRotateSpeed = horizontalRotateSpeed;
        factVerticalRotateSpeed = verticalRotateSpeed;

        initWorldPosition = mainCamera.transform.position; //////////////////////
    }

    //������� ������������ ������ ��������� (��� ������� "C")
    public void CameraRotate(Vector2 mousePos, bool moveForward = false) {
        yRotate = yRotateCamera;

        float xAxis = mousePos.x * factHorizontalRotateSpeed * Time.deltaTime;
        float yAxis = mousePos.y * factVerticalRotateSpeed * Time.deltaTime;

        yRotate -= yAxis;
        yRotate = Mathf.Clamp(yRotate, minVerticalAngle, maxVerticalAngle);

        rotateCameraCenter.transform.localRotation = Quaternion.Euler(yRotate, rotateCameraCenter.transform.localEulerAngles.y + xAxis, rotateCameraCenter.transform.localEulerAngles.z);

        if (moveForward) {
            visual.transform.rotation = Quaternion.Euler(visual.transform.eulerAngles.x, rotateCameraCenter.transform.eulerAngles.y, visual.transform.eulerAngles.z);
            rotateCameraCenter.transform.localRotation = Quaternion.Euler(yRotate, 0, 0);

            //���� �������� �� ����� ���� ������������ ������, �� ������� ��������� ��� � ������� ���������
            playerAnim.SetFloat(RunRotateFloat, Mathf.LerpUnclamped(playerAnim.GetFloat(RunRotateFloat), xAxis, 2.5f * Time.deltaTime));
        }

        //�� ������ ������������ ���� �������� ����� ������������ � ����� �������� ������
        rotateAIMyCenter.rotation = Quaternion.Euler(rotateCameraCenter.transform.eulerAngles.x,
                        rotateCameraCenter.transform.eulerAngles.y, rotateCameraCenter.transform.eulerAngles.z);

        yRotateCamera = yRotate;
    }

    //������� ������������� � ����� � ������� �������
    public void CameraAIMRotate(Vector2 mousePos, bool moveForward = false)
    {
        Ray ray = _personCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance) &&
            (Vector3.Distance(_personCamera.transform.position, hit.point) > minDistance && Vector3.Distance(_personCamera.transform.position, hit.point) < maxDistance))
            TargetPoint.position = Vector3.LerpUnclamped(TargetPoint.position, hit.point, guidanceSpeed * Time.deltaTime);
        else
        {
            Vector3 newPos = new Vector3(_personCamera.transform.localPosition.x, _personCamera.transform.localPosition.y, _personCamera.transform.localPosition.z + maxDistance);
            TargetPoint.localPosition = Vector3.LerpUnclamped(TargetPoint.localPosition, newPos, guidanceSpeed * Time.deltaTime);
        }



        yRotate = yRotateCamera;

        float xAxis = mousePos.x * factHorizontalRotateSpeed * Time.deltaTime;
        float yAxis = mousePos.y * factVerticalRotateSpeed * Time.deltaTime;

        yRotate -= yAxis;
        yRotate = Mathf.Clamp(yRotate, minVerticalAngle, maxVerticalAngle);

        rotateAIMyCenter.transform.localRotation = Quaternion.Euler(yRotate,
            rotateAIMyCenter.transform.localEulerAngles.y + xAxis, rotateAIMyCenter.transform.localEulerAngles.z);

        if (!moveForward)//���� ��������� (WASD)
        {
            if (rotateAIMyCenter.transform.localEulerAngles.y >= (360 + minHorizontAngle) || rotateAIMyCenter.transform.localEulerAngles.y <= maxHorizontAngle)
            {
                rotateCameraCenter.rotation = Quaternion.LerpUnclamped(rotateCameraCenter.transform.rotation,
                                                                         rotateAIMyCenter.transform.rotation, 0.1f * horizontalRotateSpeed * Time.deltaTime);

                playerAnim.SetFloat("RunRotate", 0);
                playerAnim.SetBool(leftStepTrigger, false);
                playerAnim.SetBool(rightStepTrigger, false);
            }
            else
            {
                visual.transform.localRotation = Quaternion.Euler(visual.transform.localEulerAngles.x,
                    visual.transform.localEulerAngles.y + (xAxis > 0 ? xAxis + 90 : xAxis - 90), visual.transform.localEulerAngles.z);

                rotateAIMyCenter.transform.localRotation = Quaternion.Euler(yRotate,
                    rotateAIMyCenter.transform.localEulerAngles.y - (xAxis > 0 ? xAxis + 90 : xAxis - 90), rotateAIMyCenter.transform.localEulerAngles.z);
                rotateCameraCenter.localRotation = rotateAIMyCenter.transform.localRotation;

                if (xAxis < 0)
                { 
                    playerAnim.SetBool(leftStepTrigger, true);
                    playerAnim.SetFloat("RunRotate", -1);
                }
                else
                {
                    playerAnim.SetBool(rightStepTrigger, true);
                    playerAnim.SetFloat("RunRotate", 1);
                }
            }
        }
        else
        {
            visual.transform.rotation = Quaternion.Euler(visual.transform.localEulerAngles.x,
                    rotateAIMyCenter.transform.eulerAngles.y, visual.transform.localEulerAngles.z);
            rotateAIMyCenter.transform.localRotation = Quaternion.Euler(yRotate,
                    0, 0);

            rotateCameraCenter.rotation = Quaternion.Euler(rotateAIMyCenter.transform.eulerAngles.x,
                    rotateAIMyCenter.transform.eulerAngles.y, rotateAIMyCenter.transform.eulerAngles.z);
        }

        yRotateCamera = yRotate;
    }

    private float targetPos;
    public void SetInCar() {
        targetPos = inCar;
    }

    public void ExitCar() {
        targetPos = startPos;
    }


    //solo
   // [SerializeField]
    //private Transform pointCameraTalking;
    private Transform parentCamera;
    private Vector3 lastPositionCamera;
    private Quaternion quaternionCamera;
    private float fieldOfView;
    //solo

    //solo
    public void TalkingNPC(Transform NPCTransform, Transform pointCameraTalking = null)
    {
        parentCamera = mainCamera.transform.parent;
        lastPositionCamera = mainCamera.transform.position;
        quaternionCamera = mainCamera.transform.rotation;

        Quaternion quaternionNPC = NPCTransform.rotation;
        Transform parentPointCameraTalking = pointCameraTalking.transform.parent;
        NPCTransform.LookAt(this.transform);
        if (pointCameraTalking)
        {
            pointCameraTalking.SetParent(this.transform);
        }
        else
        {
            print(" Not pointCameraTalking");
        }
        NPCTransform.rotation = quaternionNPC;

        float offSet = 0.0f; //objectTransform.position.y - _player.transform.position.y;

        mainCamera.transform.SetParent(pointCameraTalking);
        mainCamera.transform.localPosition = new Vector3(0, offSet, mainCamera.transform.localPosition.z);
        targetPos = 0.0f;
        mainCamera.transform.localRotation = new Quaternion(0, 0, 0, 0);
        fieldOfView = mainCamera.GetComponent<Camera>().fieldOfView;
        mainCamera.GetComponent<Camera>().fieldOfView = 30.0f;
        //print("TalkingNPC");
    }
    //solo

    //solo
    public void EndTalkingNPC()
    {
        mainCamera.transform.SetParent(parentCamera);
        mainCamera.transform.position = lastPositionCamera;
        mainCamera.transform.rotation = quaternionCamera;
        mainCamera.GetComponent<Camera>().fieldOfView = fieldOfView;
        targetPos = startPos;
        //print("EndTalkingNPC");
    }
    //solo



    public void Aim(bool aiming) {
        if (aiming) {
            factHorizontalRotateSpeed = horizontalRotateSpeed / 4;
            factVerticalRotateSpeed = verticalRotateSpeed / 4;
        }
        else {
            factHorizontalRotateSpeed = horizontalRotateSpeed;
            factVerticalRotateSpeed = verticalRotateSpeed;
        }
    }

    private void FixedUpdate() {
        RaycastHit hit;
        float target;
        Physics.Raycast(raycastPoint.position, raycastPoint.forward * -1, out hit, rayDistance);

        if (hit.collider != null) {
            target = (Vector3.Distance(raycastPoint.position, hit.point) > minRayDistance) ? (raycastPoint.localPosition.z - (Vector3.Distance(raycastPoint.position, hit.point) - 0.05f))
                : (raycastPoint.localPosition.z - minRayDistance);
        } 
        else
            target = targetPos;

        mainCamera.transform.localPosition = new Vector3(mainCamera.transform.localPosition.x, mainCamera.transform.localPosition.y, Mathf.LerpUnclamped(mainCamera.transform.localPosition.z, target, moveCamSpeed * Time.deltaTime));
    }


    #region Followcamera 
    [Header("---------Followcamera---------")]
    //[SerializeField]
    //private string targetTag = "Player";
    [SerializeField]
    private Transform targetTransform;
    //public Transform TargetTrasfrom
    //{
    //    get => targetTransform;
    //    set => targetTransform = value;
    //}
    [Header("Offset settings")]
    [SerializeField]
    private Vector3 offsetPosition;

    [Header("Following in world settings")]
    [SerializeField]
    private bool followWorldX = true;
    [SerializeField]
    private bool followWorldY = true;
    [SerializeField]
    private bool followWorldZ = true;

    [Header("Smooth settings")]
    [SerializeField]
    private bool smoothUpdate = true;
    [SerializeField]
    private float speedUpdate = 1f;

    private Vector3 initWorldPosition;


    private void Awake()
    {
        // initWorldPosition = transform.position;
       // initWorldPosition = mainCamera.transform.position;
        //targetTransform = foundGO.transform;
    }

    //private IEnumerator Start()
    //{
    //    if (targetTransform)
    //        yield break;

    //    while (true)
    //    {
    //        GameObject foundGO = GameObject.FindGameObjectWithTag(targetTag);

    //        if (foundGO)
    //        {
    //            targetTransform = foundGO.transform;
    //            break;
    //        }

    //        yield return null;
    //    }
    //}
    private void LateUpdate()
    {
        if (!targetTransform)
            return;

        UpdatePosition();
    }

    private void UpdatePosition()
    {
        Vector3 newPosition = targetTransform.position + offsetPosition;

        if (!followWorldX)
            newPosition.x = initWorldPosition.x;

        if (!followWorldY)
            newPosition.y = initWorldPosition.y;

        if (!followWorldZ)
            newPosition.z = initWorldPosition.z;

        if (smoothUpdate)
            newPosition = Vector3.Lerp(mainCamera.transform.position, newPosition, speedUpdate * Time.deltaTime);

        mainCamera.transform.position = newPosition;
    }

    #endregion TargetFollowerPosition

}

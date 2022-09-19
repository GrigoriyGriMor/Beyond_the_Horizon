using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Класс отвечает за функцию осмотреться вокруг персонажа
public class LookModule : CharacterBase
{
    [SerializeField] private Transform rotateAIMyCenter;
    [SerializeField] private Transform rotateCameraCenter;
    [SerializeField] private float verticalRotateSpeed = 1;
    [SerializeField] private float horizontalRotateSpeed = 0.5f;

    private float factVerticalRotateSpeed = 1;
    private float factHorizontalRotateSpeed = 0.5f;

    [Header("Угол макс. наклона")]
    [SerializeField] private float minVerticalAngle = -45;
    [SerializeField] private float maxVerticalAngle = 60;

    [Header("Угол макс. поворота")]
    [SerializeField] private float minHorizontAngle = -90;
    [SerializeField] private float maxHorizontAngle = 65;

    private float yRotate = 0;
    private float yRotateCamera = 0;

    [Header("Anim Trigger Names")]
    [SerializeField] private string rightStepTrigger = "StepRight";
    [SerializeField] private string leftStepTrigger = "StepLeft";
    [SerializeField] private string RunRotateFloat = "RunRotate";

    [Header("AIM Position")]
    //[SerializeField] private Camera _personCamera;
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

    [Header("need correct")]
    [SerializeField] private GameObject[] visualGO = new GameObject[2];

    private void Start()
    {

        //startPos = mainCamera.transform.localPosition.z;
       // inCar = startPos * 3;
        lastTransform = pointFollowCamera.localPosition;


        //solo
        //targetPosZ = startPos;
        //mainCameraX = mainCamera.transform.localPosition.x;
       // mainCameraY = mainCamera.transform.localPosition.y;
        //solo

        factHorizontalRotateSpeed = horizontalRotateSpeed;
        factVerticalRotateSpeed = verticalRotateSpeed;
    }

    //функция осматривания вокруг персонажа (при нажатой "C")
    public void CameraRotate(Vector2 mousePos, bool moveForward = false)
    {
        yRotate = yRotateCamera;

        float xAxis = mousePos.x * factHorizontalRotateSpeed * Time.deltaTime;
        float yAxis = mousePos.y * factVerticalRotateSpeed * Time.deltaTime;

        yRotate -= yAxis;
        yRotate = Mathf.Clamp(yRotate, minVerticalAngle, maxVerticalAngle);

        rotateCameraCenter.transform.localRotation = Quaternion.Euler(yRotate, rotateCameraCenter.transform.localEulerAngles.y + xAxis, rotateCameraCenter.transform.localEulerAngles.z);

        if (moveForward)
        {
            visual.transform.rotation = Quaternion.Euler(visual.transform.eulerAngles.x, rotateCameraCenter.transform.eulerAngles.y, visual.transform.eulerAngles.z);
            rotateCameraCenter.transform.localRotation = Quaternion.Euler(yRotate, 0, 0);

            //если персонаж во время бега поворачивает мышкой, то немного наклоняем его с помощью аниматора
            playerAnim.SetFloat(RunRotateFloat, Mathf.LerpUnclamped(playerAnim.GetFloat(RunRotateFloat), xAxis, 2.5f * Time.deltaTime));
        }

        //сл строка приравнивает угол поворота блока прицеливания к блоку поворота камеры
        rotateAIMyCenter.rotation = Quaternion.Euler(rotateCameraCenter.transform.eulerAngles.x,
                        rotateCameraCenter.transform.eulerAngles.y, rotateCameraCenter.transform.eulerAngles.z);

        yRotateCamera = yRotate;
    }

    //функция привцеливания в точку и поворот корпуса
    public void CameraAIMRotate(Vector2 mousePos, bool moveForward = false)
    {
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance) &&
            (Vector3.Distance(mainCamera.transform.position, hit.point) > minDistance && Vector3.Distance(mainCamera.transform.position, hit.point) < maxDistance))
            TargetPoint.position = Vector3.LerpUnclamped(TargetPoint.position, hit.point, guidanceSpeed * Time.deltaTime);
        else
        {
            Vector3 newPos = new Vector3(mainCamera.transform.localPosition.x, mainCamera.transform.localPosition.y, mainCamera.transform.localPosition.z + maxDistance);
            TargetPoint.localPosition = Vector3.LerpUnclamped(TargetPoint.localPosition, newPos, guidanceSpeed * Time.deltaTime);
        }

        yRotate = yRotateCamera;

        float xAxis = mousePos.x * factHorizontalRotateSpeed * Time.deltaTime;
        float yAxis = mousePos.y * factVerticalRotateSpeed * Time.deltaTime;

        yRotate -= yAxis;
        yRotate = Mathf.Clamp(yRotate, minVerticalAngle, maxVerticalAngle);

        rotateAIMyCenter.transform.localRotation = Quaternion.Euler(yRotate,
            rotateAIMyCenter.transform.localEulerAngles.y + xAxis, rotateAIMyCenter.transform.localEulerAngles.z);

        if (!moveForward)//если двигаемся (WASD)
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

    private float targetPosZ;

    private Vector3 lastTransform;
    public void SetInCar()
    {
        lastTransform = pointFollowCamera.localPosition;
        pointFollowCamera.localPosition = pointCameraInCar.localPosition;
        //startPos = pointFollowCamera.position.z;
        //targetPosZ = inCar;
        // pointFollowCamera.position = new Vector3(pointFollowCamera.position.x, pointFollowCamera.position.y, pointFollowCamera.position.z - targetPosZ);

    }

    public void ExitCar()
    {
        pointFollowCamera.localPosition = lastTransform;
        //targetPosZ = startPos;
        //pointFollowCamera.position = new Vector3(pointFollowCamera.position.x, pointFollowCamera.position.y, startPos);
    }

    //solo
    [SerializeField]
    private float delayFlyCamera = 0.1f;
    [SerializeField]
    private Transform pointFollowCamera;
    [SerializeField]
    private Transform pointLookCamera;
    [SerializeField]
    private Transform pointCameraInCar;
    private GameObject backPlane;
    private bool isTalking;
    private Vector3 lastPositionFollowCamera;
    private Transform lastTransformCamera;
    //solo

    //solo
    public void TalkingNPC(Transform pointCameraTalking = null, GameObject backPlane = null, Transform pointLookCamera = null)
    {
        if ((pointCameraTalking == null || backPlane == null) || pointLookCamera == null) return;

        this.backPlane = backPlane;
        this.pointLookCamera = pointLookCamera;
        for (int i = 0; i < visualGO.Length; i++)
            visualGO[i].SetActive(false);
        isTalking = true;
        lastTransformCamera = pointFollowCamera;
        lastPositionFollowCamera = pointFollowCamera.position;
        StartCoroutine(DelayFlyCamera(pointCameraTalking));
    }
    //solo

    private IEnumerator DelayFlyCamera(Transform pointCameraTalking)
    {
        yield return new WaitForSeconds(delayFlyCamera);

        if (pointCameraTalking)
        {
            pointFollowCamera = pointCameraTalking;
            backPlane.SetActive(true);
        }
        else
        {
            Debug.LogError("Not pointCameraTalking");
        }
    }

    //solo
    public void EndTalkingNPC()
    {
        pointFollowCamera = lastTransformCamera;
        pointFollowCamera.position = lastPositionFollowCamera;
        for (int i = 0; i < visualGO.Length; i++)
            visualGO[i].SetActive(true);
        backPlane.SetActive(false);
        isTalking = false;
    }
    //solo

    public void Aim(bool aiming)
    {
        if (aiming)
        {
            factHorizontalRotateSpeed = horizontalRotateSpeed / 4;
            factVerticalRotateSpeed = verticalRotateSpeed / 4;
        }
        else
        {
            factHorizontalRotateSpeed = horizontalRotateSpeed;
            factVerticalRotateSpeed = verticalRotateSpeed;
        }
    }

    private void FixedUpdate()
    {

        RaycastHit hit;
        float target;

        Physics.Raycast(raycastPoint.position, raycastPoint.forward * -1, out hit, rayDistance);

        if (hit.collider != null)
        {
            target = (Vector3.Distance(raycastPoint.position, hit.point) > minRayDistance) ? (raycastPoint.localPosition.z - (Vector3.Distance(raycastPoint.position, hit.point) - 0.05f))
                : (raycastPoint.localPosition.z - minRayDistance);
        }
        else
        {
            target = targetPosZ;
        }


        if (isTalking)
        {
            mainCamera.transform.LookAt(pointLookCamera.transform);
        }
        else
        {
            mainCamera.transform.LookAt(TargetPoint);
        }

        UpdatePosititon();
    }

    [SerializeField]
    private float moveCameraFollow = 1.0f;
    private void UpdatePosititon()
    {
        mainCamera.transform.position = new Vector3(Mathf.LerpUnclamped(mainCamera.transform.position.x, pointFollowCamera.position.x, moveCameraFollow * Time.deltaTime),
           Mathf.LerpUnclamped(mainCamera.transform.position.y, pointFollowCamera.position.y, moveCameraFollow * Time.deltaTime),
              Mathf.LerpUnclamped(mainCamera.transform.position.z, pointFollowCamera.position.z, moveCameraFollow * Time.deltaTime));

    }

}

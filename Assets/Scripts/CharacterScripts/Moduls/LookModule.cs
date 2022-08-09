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
    }

    private void Update() {
        if (state == SupportClass.gameState.client || state == SupportClass.gameState.clone) {
            Quaternion camEuler = Quaternion.Euler(_rotateCamEuler);
            rotateCameraCenter.rotation = Quaternion.Lerp(rotateCameraCenter.rotation, camEuler, factHorizontalRotateSpeed * Time.deltaTime);

            Quaternion aimEuler = Quaternion.Euler(_rotateAIMEuler);
            rotateAIMyCenter.rotation = Quaternion.Lerp(rotateAIMyCenter.rotation, aimEuler, factHorizontalRotateSpeed * Time.deltaTime);
            return;
        }
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
           /* Quaternion _euler = Quaternion.Euler(visual.transform.localEulerAngles.x,
                rotateCameraCenter.transform.eulerAngles.y, visual.transform.localEulerAngles.z);
            Quaternion _euler1 = Quaternion.Euler(rotateCameraCenter.transform.eulerAngles.x,
                visual.transform.localEulerAngles.y, rotateCameraCenter.transform.eulerAngles.z);

            if (Mathf.Abs(_euler1.y - _euler.y) * 100 > 50) {*/
                visual.transform.rotation = Quaternion.Euler(visual.transform.eulerAngles.x, rotateCameraCenter.transform.eulerAngles.y, visual.transform.eulerAngles.z);
                rotateCameraCenter.transform.localRotation = Quaternion.Euler(yRotate, 0, 0);
           /* }
            else {
                visual.transform.rotation = Quaternion.Lerp(visual.transform.rotation, _euler, 30 * Time.deltaTime);
                //после того как мы поворачиваем визуал, камера поворачивается вместе с ним и ее нужно вернуть обратно
                rotateCameraCenter.transform.rotation = Quaternion.Lerp(rotateCameraCenter.transform.rotation, _euler1, 20 * Time.deltaTime);//Quaternion.Euler(yRotate, 0, 0);
            }*/

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
        if (state == SupportClass.gameState.client || state == SupportClass.gameState.clone) {

            Quaternion camEuler = Quaternion.Euler(_rotateCamEuler);
            rotateCameraCenter.rotation = Quaternion.LerpUnclamped(rotateCameraCenter.rotation, camEuler, factHorizontalRotateSpeed * Time.deltaTime);

            Quaternion aimEuler = Quaternion.Euler(_rotateAIMEuler);
            rotateAIMyCenter.rotation = Quaternion.LerpUnclamped(rotateAIMyCenter.rotation, aimEuler, factHorizontalRotateSpeed * Time.deltaTime);

            Ray ray1 = _personCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

            if (Physics.Raycast(ray1, out RaycastHit hit2, maxDistance) &&
                (Vector3.Distance(_personCamera.transform.position, hit2.point) > minDistance && Vector3.Distance(_personCamera.transform.position, hit2.point) < maxDistance))
                TargetPoint.position = Vector3.LerpUnclamped(TargetPoint.position, hit2.point, guidanceSpeed * Time.deltaTime);
            else {
                Vector3 newPos = new Vector3(_personCamera.transform.localPosition.x, _personCamera.transform.localPosition.y, _personCamera.transform.localPosition.z + maxDistance);
                TargetPoint.localPosition = Vector3.LerpUnclamped(TargetPoint.localPosition, newPos, guidanceSpeed * Time.deltaTime);
            }

            float xAxis1 = mousePos.x * horizontalRotateSpeed * Time.deltaTime;

            if (!moveForward)//если двигаемся (WASD)
            {
                if (rotateAIMyCenter.transform.localEulerAngles.y >= (360 + minHorizontAngle) || rotateAIMyCenter.transform.localEulerAngles.y <= maxHorizontAngle) {
                    playerAnim.SetFloat("RunRotate", 0);
                    playerAnim.SetBool(leftStepTrigger, false);
                    playerAnim.SetBool(rightStepTrigger, false);
                }
                else {
                    if (xAxis1 < 0) {
                        playerAnim.SetBool(leftStepTrigger, true);
                        playerAnim.SetFloat("RunRotate", -1);

                        rotateCameraCenter.eulerAngles = _rotateCamEuler;
                        rotateAIMyCenter.eulerAngles = _rotateAIMEuler;
                    }
                    else {
                        playerAnim.SetBool(rightStepTrigger, true);
                        playerAnim.SetFloat("RunRotate", 1);

                        rotateCameraCenter.eulerAngles = _rotateCamEuler;
                        rotateAIMyCenter.eulerAngles = _rotateAIMEuler;
                    }
                }
            }

            return;
        }

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

    public Vector3 GetCameraRotate() {
        return rotateCameraCenter.eulerAngles;
    }

    public Vector3 GetAimRotate() {
        return rotateAIMyCenter.eulerAngles;
    }

    private Vector3 _rotateCamEuler;
    private Vector3 _rotateAIMEuler;
    public void SetCameraRotate(Vector3 camRotate) {
        //  rotateCameraCenter.eulerAngles = camRotate;
        _rotateCamEuler = camRotate;
    }

    public void SetAimRotate(Vector3 aimRotate) {
        //  rotateAIMyCenter.eulerAngles = aimRotate;
        _rotateAIMEuler = aimRotate;
    }

    private float targetPos;
    public void SetInCar() {
        targetPos = inCar;
    }

    public void ExitCar() {
        targetPos = startPos;
    }

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
}

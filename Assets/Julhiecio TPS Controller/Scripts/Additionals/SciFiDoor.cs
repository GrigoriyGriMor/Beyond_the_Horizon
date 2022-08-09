using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("JU TPS/Utilities/Sci-fi Door")]
public class SciFiDoor : MonoBehaviour
{

    [Header("Sci-fi Door")]
    [Range(1, 15f)]
    public float PlayerDistanceToOpen = 5f;

    [Range(0.5f, 2f)]
    public float DoorMovementLenght = 2;

    [Range(1, 15)]
    public float DoorSpeed = 8;

    public Transform LeftDoor, RightDoor;
    private Vector3 RightDoorStartPosition, LeftDoorStartPosition;

    Vector3 rightDoorOpenedPosition;
    Vector3 leftDoorOpenedPosition;

    private ThirdPersonController player;
    void Start()
    {
        RightDoorStartPosition = RightDoor.position;
        LeftDoorStartPosition = LeftDoor.position;
        player = FindObjectOfType<ThirdPersonController>();

        rightDoorOpenedPosition = RightDoor.position + RightDoor.right * DoorMovementLenght;
        leftDoorOpenedPosition = LeftDoor.position - LeftDoor.right * DoorMovementLenght;
    }

    void Update()
    {
        var dist = Vector3.Distance(player.transform.position, transform.position);
        if (dist < PlayerDistanceToOpen)
        {
            LeftDoor.position = Vector3.Lerp(LeftDoor.position, leftDoorOpenedPosition, DoorSpeed * Time.deltaTime);
            RightDoor.position = Vector3.Lerp(RightDoor.position, rightDoorOpenedPosition, DoorSpeed * Time.deltaTime);
        }
        else if (LeftDoor.position != RightDoorStartPosition && RightDoor.position != LeftDoorStartPosition)
        {
            LeftDoor.position = Vector3.Lerp(LeftDoor.position, LeftDoorStartPosition, DoorSpeed * Time.deltaTime);
            RightDoor.position = Vector3.Lerp(RightDoor.position, RightDoorStartPosition, DoorSpeed * Time.deltaTime);
        }
    }
    private void OnDrawGizmos()
    {
        if (LeftDoor != null && RightDoor != null)
        {
            Gizmos.DrawLine(LeftDoor.position + Vector3.up * 2f, LeftDoor.position + Vector3.up * 2f + LeftDoor.right * DoorMovementLenght);
            Gizmos.DrawLine(RightDoor.position + Vector3.up * 2f, RightDoor.position + Vector3.up * 2f - RightDoor.right * DoorMovementLenght);

            rightDoorOpenedPosition = LeftDoor.position + LeftDoor.right * DoorMovementLenght;
            leftDoorOpenedPosition = RightDoor.position - RightDoor.right * DoorMovementLenght;

            Gizmos.DrawWireCube(leftDoorOpenedPosition + Vector3.up * 2f, new Vector3(0.05f, 0.05f, 0.05f));
            Gizmos.DrawWireCube(rightDoorOpenedPosition + Vector3.up * 2f, new Vector3(0.05f, 0.05f, 0.05f));

        }
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(PlayerDistanceToOpen * 2f, 0, PlayerDistanceToOpen * 2f));
    }
}

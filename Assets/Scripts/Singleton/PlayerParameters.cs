using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParameters : MonoBehaviour {
    private static PlayerParameters instance;

    public static PlayerParameters Instance => instance;

    private PlayerController playerController;

    private void Awake() {
        instance = this;
    }


    public void SetPlayerController(PlayerController playerController) {
        this.playerController = playerController;
    }

    public PlayerController GetPlayerController() {
        if (playerController == null)
            return null;
        else
            return playerController;
    }
}

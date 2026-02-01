using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using InputDataSystem;

public class PlayerMouseCameraPos : MonoBehaviour
{
    PlayerInputHandler playerInputHandler;
    [SerializeField] float proportionFromPlayer; // 0 being directly on the player, 1 being directly on the mouse, 0.5 being halfway between

    void Start()
    {
        playerInputHandler = GameObject.FindFirstObjectByType<PlayerInputHandler>();
    }

    void Update()
    {
        if (playerInputHandler == null) return;

        Vector2 playerPosition = playerInputHandler.GetCurrentInputData().GetPosition();
        Vector2 worldMousePosition = playerInputHandler.GetCurrentMouseWorldPosition();
        Vector2 targetPosition = Vector2.Lerp(playerPosition, worldMousePosition, proportionFromPlayer);

        transform.position = targetPosition;
    }
}

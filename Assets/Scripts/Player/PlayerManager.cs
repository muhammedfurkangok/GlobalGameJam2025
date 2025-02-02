using System;
using Runtime.Extensions;
using UnityEngine;

public class PlayerManager : SingletonMonoBehaviour<PlayerManager>
{
    [Header("Manager References")]
    public PlayerHealthController playerHealthController;
    public PlayerUIController playerUIController;
    public PlayerTriggerController playerTriggerController;
    public PlayerController playerController;
    public PlayerController PlayerController;
    public PlayerCombatController playerCombatController;
    public Animator animator;
    public Rigidbody2D playerRigidbody2D;
    public SceneController sceneController;

    private void Start()
    {
        playerHealthController = GetComponent<PlayerHealthController>();
        playerUIController = GetComponent<PlayerUIController>();
        playerTriggerController = GetComponent<PlayerTriggerController>();
        playerController = GetComponent<PlayerController>();
        playerCombatController = GetComponent<PlayerCombatController>();
    }
}
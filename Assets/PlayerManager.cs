using Runtime.Extensions;
using UnityEngine;

public class PlayerManager : SingletonMonoBehaviour<PlayerManager>
{
    [Header("Manager References")]
    public PlayerHealthController playerHealthController;
    public PlayerUIController playerUIController;
    public PlayerTriggerController playerTriggerController;
    public PlayerController playerController;
}
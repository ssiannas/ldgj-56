using System.Collections;
using System.Collections.Generic;
using Menus.Characters;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerPersistentState", menuName = "ScriptableObjects/Player/PlayerPersistentState",
    order = 1)]
public class PlayerPersistentState : ScriptableObject
{
    [field: SerializeField] public CharacterChoiceSO CharacterChoice { get; set; }
    [field: SerializeField] public int HighScore { get; set; } = 0;
}
using UnityEngine;

namespace Menus.Characters
{
    [CreateAssetMenu(fileName = "CharacterChoice", menuName = "ScriptableObjects/UI/CharacterChoice", order = 1)]
    public class CharacterChoiceSO : ScriptableObject
    {
        [field: SerializeField] public string name { get; private set; }
        [field: SerializeField] public Sprite img { get; private set; }

        [field: SerializeField] public RuntimeAnimatorController animations { get; private set; }
        [field: SerializeField] public RuntimeAnimatorController mainMenuAnimation { get; private set; }
    }
}
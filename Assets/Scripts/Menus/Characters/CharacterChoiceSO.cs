using UnityEditor.Animations;
using UnityEngine;

namespace Menus.Characters
{
    [CreateAssetMenu(fileName = "CharacterChoice", menuName = "ScriptableObjects/UI/CharacterChoice", order = 1)]
    public class CharacterChoiceSO : ScriptableObject
    {
        [field: SerializeField] public string name { get; private set; }
        [field: SerializeField] public Sprite img { get; private set; }

        [field: SerializeField] public AnimatorController animations { get; private set; }
    }
}
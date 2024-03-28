using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Y
{
    [CreateAssetMenu(fileName = "Item", menuName = "Characters/ItemData")]
    public class ItemData : ScriptableObject
    {
        public string ItemName;
        public Sprite ItemImage;
        [TextArea] public string ItemDescription;
        public CharacterData Character;

        public bool CanUse(CharacterData currentCharacter) => Character == currentCharacter; // Objeyi kullanmaya �al��an kullan�c� do�ru kullan�c� m� kontrol et
    }
}

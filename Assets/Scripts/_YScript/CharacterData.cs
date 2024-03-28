using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Y
{
    [CreateAssetMenu(fileName = "Character", menuName = "Characters/CharacterData")]
    public class CharacterData : ScriptableObject
    {
        public string CharacterName;
        [TextArea] public string CharacterDestription;
        public List<ItemData> NeededItem;
        public List<ItemData> Inventory;

        public void SkillActive()
        {
            //?
        }
    }
}
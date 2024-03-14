using UnityEngine;

namespace Interact
{
    public class IntrectableObject : MonoBehaviour, IIntrectable
    {
        //[SerializeField] private ItemData collectableItem;
        [SerializeField] private GameObject UI;
        private void Start()
        {
            UI.gameObject.SetActive(false);
        }
        //public ItemData getInteractItemData()
        //{
        //    return collectableItem;
        //}

        public void Interact() // interacta ne yapacak
        {
            Debug.Log("interact!!!" + name);
        }

        public void UIActive(Transform lookAt, bool activity) //UI olaylarý
        {
            UI.SetActive(activity);
            UI.transform.LookAt(lookAt.position * -1);
        }
    }/**/
}
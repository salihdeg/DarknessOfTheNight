//using Item;
using UnityEngine;
namespace Interact
{
    public interface IIntrectable
    {
        void Interact();
        void UIActive(Transform t, bool activity);
        //ItemData getInteractItemData();
    }/**/
}
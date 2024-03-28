using System.Linq;
using UnityEngine;

namespace Interact
{
    public class PlayerInteract : MonoBehaviour
    {
        [SerializeField] private float interactRange = 2f;

        private void Update()
        {
            IteractingAction();
        }
        private void IteractingAction()
        {
            //Menzil kontrol� ile etraftaki colliderler� alg�lad�k.
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
            foreach (Collider collider in colliderArray)
            {
                if (collider.TryGetComponent(out IIntrectable interactableObject)) // colliderlar�n i�inde IIntrectable interfacesi var m� diye kontrol.
                {
                    interactableObject.UIActive(transform, true); // toplanacak objenin ui aktifle�tirme
                                                                  //Debug.Log("bi�iler alg�land�");
                    if (Input.GetKeyDown(KeyCode.E)) // inputtan sonra yap�lacak i�lemler
                    {
                        interactableObject.Interact();
                        //Debug.Log(interactableObject.getInteractItemData().ItemName);
                    }
                }
            }

            Collider[] colliderArrayLong = Physics.OverlapSphere(transform.position, interactRange * 1.5f);
            foreach (Collider collider in colliderArrayLong) // toplanacak objenin ui pasifle�tirme
            {
                if (!colliderArray.Contains(collider) && collider.TryGetComponent(out IIntrectable interactableObject))
                    interactableObject.UIActive(transform, false);
            }
        }
    }/**/
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Y
{
    public class Movement : MonoBehaviour
    {
        [SerializeField] float _speed = 10f;
        void Update()
        {
            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            transform.position += Vector3.Normalize(move) * _speed * Time.deltaTime;
        }
    }/**/
}
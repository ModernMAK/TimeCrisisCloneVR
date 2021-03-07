using UnityEngine;
using System.Collections;

namespace Valve.VR.InteractionSystem
{
    //-------------------------------------------------------------------------
    public class LinearLerpRaw : MonoBehaviour
    {
        public Transform startPosition;
        public Transform endPosition;
        public float value;


        private void Start()
        {
            transform.position = Vector3.Lerp(startPosition.position, endPosition.position, value);
            
        }
    }
}
using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Input
{
    [RequireComponent(typeof(Collider))]
    public class RayCastDispatcher : MonoBehaviour
    {
        public event Action DispatchRayCastAction;
    
        [ShowInInspector]
        public void DispatchRayCast()
        {
            DispatchRayCastAction?.Invoke();
        }
    }
}
using System;
using Unity.Netcode;
using UnityEngine;

namespace TankCode.Players
{
    public class PlayerAim : NetworkBehaviour
    {
        [SerializeField] private PlayerInputSO playerInput;
        [SerializeField] private Transform turretTrm;

        private void LateUpdate()
        {
            if(IsOwner == false) return;
            
            Vector3 direction = (playerInput.GetWorldMousePosition() - transform.position).normalized;
            turretTrm.up = direction;
        }
    }
}
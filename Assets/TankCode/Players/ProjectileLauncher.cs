﻿using System;
 using TankCode.Projectiles;
 using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace TankCode.Players
{
    public class ProjectileLauncher : NetworkBehaviour
    {
        [Header("references")]
        [SerializeField] private PlayerInputSO playerInput;
        [SerializeField] private Transform projectileSpawnTrm;
        [SerializeField] private ProjectileBase serverPrefab;
        [SerializeField] private ProjectileBase clientPrefab;

        [SerializeField] private Collider2D playerCollider;
        
        [Header("Settings values")] 
        [SerializeField] private float projectileSpeed;
        [SerializeField] private float fireCooldown;
        
        public UnityEvent OnFire;
        
        private bool _isFire;
        private float _prevFireTime;

        public override void OnNetworkSpawn()
        {
            if (IsOwner == false) return;
            playerInput.OnFire += HandleFireKey;
        }

        public override void OnNetworkDespawn()
        {
            if (IsOwner == false) return;
            playerInput.OnFire -= HandleFireKey;
        }

        private void HandleFireKey(bool isFire)
        {
            _isFire = isFire;
        }

        private void Update()
        {
            if (IsOwner == false) return;
            if (_isFire == false) return;

            if (Time.time < _prevFireTime + fireCooldown) return;

            SpawnDummyProjectile(projectileSpawnTrm.position, projectileSpawnTrm.up);
            SpawnServerProjectileServerRpc(projectileSpawnTrm.position, projectileSpawnTrm.up);
            _prevFireTime = Time.time;
        }

        //클라이언트가 서버에 요청을 해서 콜을 하는거고. 반드시 오너만 실행할 수 있어.
        [ServerRpc]
        private void SpawnServerProjectileServerRpc(Vector3 position, Vector3 direction)
        {
            ProjectileBase instance = Instantiate(serverPrefab, position, Quaternion.identity);
            instance.transform.up = direction; //회전설정
            //자기자신과는 충돌하지 않도록 설정한다.
            Physics2D.IgnoreCollision(playerCollider, instance.ColliderCompo);

            instance.FireProjectile(direction * projectileSpeed);

            SpawnProjectileClientRpc(position, direction);
        }

        //서버가 클라이언트에 있는 Rpc를 호출 할 때 쓴다.
        [ClientRpc]
        private void SpawnProjectileClientRpc(Vector3 position, Vector3 direction)
        {
            if (IsOwner) return; //소유자는 이미 더미프로젝타일을 쐈어.
            
            SpawnDummyProjectile(position, direction);
        }

        private void SpawnDummyProjectile(Vector3 position, Vector3 direction)
        {
            ProjectileBase instance = Instantiate(clientPrefab, position, Quaternion.identity);

            instance.transform.up = direction; //회전설정
            //자기자신과는 충돌하지 않도록 설정한다.
            Physics2D.IgnoreCollision(playerCollider, instance.ColliderCompo);

            instance.FireProjectile(direction * projectileSpeed);
            OnFire?.Invoke();
        }
    }
}
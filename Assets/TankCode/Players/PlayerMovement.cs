﻿using System;
using Unity.Netcode;
using UnityEngine;

namespace TankCode.Players
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : NetworkBehaviour
    {
        [Header("reference data")] 
        [SerializeField] private PlayerInputSO playerInput;
        [SerializeField] private Transform bodyTrm;

        [Header("Setting values")] 
        [SerializeField] private float moveSpeed = 4f; //이동속도
        [SerializeField] private float turningRate = 30f; //회전속도

        private new Rigidbody2D rigidbody;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (IsOwner == false) return;
            HandleRotate();
        }

        private void FixedUpdate()
        {
            if (IsOwner == false) return;
            HandleMovement();
        }

        private void HandleRotate()
        {
            float zRotation = playerInput.MovementKey.x * - turningRate * Time.deltaTime;
            bodyTrm.Rotate(0, 0, zRotation); //키보드의 좌우 입력값에 따라 몸통을 돌려준다.
        }
        
        private void HandleMovement()
        {
            rigidbody.linearVelocity = bodyTrm.up * (playerInput.MovementKey.y * moveSpeed);
        }
    }
}
﻿using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TankCode.Combat
{
    public class CoinSpawner : NetworkBehaviour
    {
        [SerializeField] private RespawnCoin coinPrefab;

        [Header("Setting values")] 
        [SerializeField] private int maxCoinCount = 30;
        [SerializeField] private int coinValue = 10; //코인당 10
        [SerializeField] private LayerMask whatIsObstacle;
        [SerializeField] private float spawningTerm = 30f; //30초마다 코인생성
        [SerializeField] private float spawnRadius = 8f; // 반지름8인 원안에서 랜덤으로 생성

        private bool _isSpawning = false; //현재 생성중인가?
        private float _spawnTime = 0;
        private int _spawnCountTime = 10; //10초 카운트하고 시작

        public List<Transform> spawnPoints;
        private float coinRadius;

        private Stack<RespawnCoin> _coinPool = new Stack<RespawnCoin>();
        private List<RespawnCoin> _activeCoinList = new List<RespawnCoin>();

        //이 매서드는 서버만 실행합니다.
        private RespawnCoin SpawnCoin()
        {
            RespawnCoin coinInstance = Instantiate(coinPrefab);
            coinInstance.SetCoinValue(coinValue);
            coinInstance.GetComponent<NetworkObject>().Spawn(); 
            //서버가 클라이언트들에게 스폰되었음을 알림(네트워크 오브젝트는 이렇게 해줘야 해)
            
            coinInstance.OnCollected.AddListener(HandleCoinCollected);
            return coinInstance;
        }

        private void HandleCoinCollected(RespawnCoin targetCoin)
        {
            _activeCoinList.Remove(targetCoin);
            targetCoin.SetVisible(false); //서버가 꺼준다.
            _coinPool.Push(targetCoin);
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer == false) return;

            coinRadius = coinPrefab.GetComponent<CircleCollider2D>().radius; //코인의 반지름을 알아낸다.

            for (int i = 0; i < maxCoinCount; i++)
            {
                RespawnCoin coin = SpawnCoin();
                coin.SetVisible(false); //처음 생성된 것들은 꺼준다.
                _coinPool.Push(coin);
            }
        }

        public override void OnNetworkDespawn()
        {
            if(IsServer)
                StopAllCoroutines();
        }

        private void Update()
        {
            if (IsServer == false) return; //서버가 아니면 업데이트 할게 없다.

            //현재 생성에 들어가지 않았고. 생성된 코인도 하나도 없다면 생성을 시작한다.
            if (_isSpawning == false && _activeCoinList.Count == 0)
            {
                _spawnTime += Time.deltaTime;
                if (_spawnTime >= spawningTerm)
                {
                    _spawnTime = 0;
                    StartCoroutine(SpawnCoroutine());
                }
            }
        }

        private IEnumerator SpawnCoroutine()
        {
            _isSpawning = true;
            int pointIdx = Random.Range(0, spawnPoints.Count);
            int coinCount = Random.Range( maxCoinCount / 2, maxCoinCount + 1);
            
            for(int i = _spawnCountTime; i > 0; i--)
            {
                //카운트 다운 시작
                CountDownClientRpc(i, pointIdx, coinCount);
                yield return new WaitForSeconds(1f); //서버가 1초마다 한번씩 클라이언트에게 RPC를 날려준다.
            }

            Vector2 center = spawnPoints[pointIdx].position;
            for (int i = 0; i < coinCount; i++)
            {
                Vector2 pos = Random.insideUnitCircle * spawnRadius + center;
                RespawnCoin coin = _coinPool.Pop();
                coin.transform.position = pos;
                coin.ResetCoin();
                
                _activeCoinList.Add(coin);
                yield return new WaitForSeconds(4f); //4초마다 한개씩 생성해준다.
            }

            _isSpawning = false; //스폰 작업이 모두 완료되었다면 flag를 꺼준다.
        }

        [ClientRpc]
        private void CountDownClientRpc(int sec, int pointIdx, int coinCount)
        {
            Debug.Log($"{pointIdx} 지점에서 {sec}초 후 {coinCount}개의 코인이 생성됩니다.");
        }
    }
}
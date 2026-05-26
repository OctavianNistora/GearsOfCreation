using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public class CustomSceneManager : MonoBehaviour
    {
        public static CustomSceneManager Instance;
        
        private Dictionary<string, SpawnData> _spawnPoints = new();
        private bool _useDefaultSpawnPoint = true;
        
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            SceneManager.sceneLoaded += OnSceneLoaded;
            CombatManager.Instance.OnBattleEnded += OnBattleEnded;
        }
        
        public void ChangeScene(string sceneName, bool useDefaultSpawnPoint = true)
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                _spawnPoints[SceneManager.GetActiveScene().name] = new SpawnData
                {
                    Position = player.transform.position,
                    Rotation = player.transform.rotation
                };
            }
            
            _useDefaultSpawnPoint = useDefaultSpawnPoint;
            
            SceneManager.LoadScene(sceneName);
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (_useDefaultSpawnPoint || !_spawnPoints.TryGetValue(scene.name, out var spawnPoint)) return;
            
            var player = GameObject.FindWithTag("Player");
            if (player)
            {
                player.transform.position = spawnPoint.Position;
                player.transform.rotation = spawnPoint.Rotation;
            }
            
            _useDefaultSpawnPoint = true;
        }
        
        private void OnBattleEnded(BattleEndStateEnum battleEndState)
        {
            if (battleEndState == BattleEndStateEnum.Victory)
            {
                StartCoroutine(ChangeSceneDelayed("PlatformerScene", 2f, false));
            }
            else
            {
                StartCoroutine(ChangeSceneDelayed("PlatformerScene", 2f, true));
            }
        }
        
        private IEnumerator ChangeSceneDelayed(string sceneName, float delay, bool useDefaultSpawnPoint = true)
        {
            yield return new WaitForSeconds(delay);
            
            ChangeScene(sceneName, useDefaultSpawnPoint);
        }

        struct SpawnData
        {
            public Vector3 Position;
            public Quaternion Rotation;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public Wave[] waves;
    public Enemigo enemy;

    Wave currentWave;
    int currentWaveIndex;

    int enemigosRemainingToSpawn;
    int enemigosRemainingAlive;
    float timeRemainingForNextSpawn;

    private void Start()
    {
        NextWave();
    }


    void NextWave()
    {
        currentWaveIndex++;
        if (currentWaveIndex-1<waves.Length)
        {
            currentWave = waves[currentWaveIndex - 1];
            enemigosRemainingToSpawn = currentWave.enemyCount;
            enemigosRemainingAlive = enemigosRemainingToSpawn;
            timeRemainingForNextSpawn = currentWave.timeBetweenSpawns;
        }
        
    }

    private void Update()
    {
        if (timeRemainingForNextSpawn<=0&&enemigosRemainingToSpawn>0)
        {
           
            enemigosRemainingToSpawn--;
            timeRemainingForNextSpawn = currentWave.timeBetweenSpawns;
            Enemigo nuevoEnemigo = Instantiate(enemy, Vector3.zero, Quaternion.identity) as Enemigo;
            nuevoEnemigo.OnDeath += OnEnemyDeath;

        }
        timeRemainingForNextSpawn -= Time.deltaTime;
    }

    public void OnEnemyDeath()
    {
        enemigosRemainingAlive--;
        if (enemigosRemainingAlive<=0)
        {
            Invoke ("NextWave",2);
        }
    }


    [System.Serializable]
    public class Wave
    {
        public int enemyCount;
        public float timeBetweenSpawns;

    }
}

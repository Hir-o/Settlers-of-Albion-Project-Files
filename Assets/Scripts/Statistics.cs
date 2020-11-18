using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statistics : MonoBehaviour
{
    private static Statistics _instance;
    public static  Statistics Instance => _instance;

    public int boarsKilled,
               wolfsKilled,
               werewolfsKilled,
               bearsKilled,
               slimesKilled,
               goblinsKilled,
               orcsKilled,
               frogmansKilled,
               trollsKilled,
               cyclopsKilled,
               yetisKilled;

    public int spawnedVillagers,
               spawnedPikemans,
               spawnedArchers,
               spawnedFootmans,
               spawnedHussars,
               spawnedKnights,
               spawnedArmouredArchers,
               spawnedMountedKnights,
               spawnedPaladins,
               spawnedMountedPaladins,
               spawnedKings,
               spawnedSettlements,
               spawnedStrongholds;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }
}
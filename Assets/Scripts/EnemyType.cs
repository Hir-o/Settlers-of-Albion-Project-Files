using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyType : MonoBehaviour
{
    public Type type = Type.None;

    public enum Type
    {
        None,
        Boar,
        Wolf,
        Werewolf,
        Bear,
        Slime,
        Goblin,
        Orc,
        Frogman,
        Troll,
        Cyclops,
        Yeti
    }

    private void OnDestroy()
    {
        switch (type)
        {
            case Type.Boar:
                Statistics.Instance.boarsKilled++;
                break;
            case Type.Wolf:
                Statistics.Instance.wolfsKilled++;
                break;
            case Type.Werewolf:
                Statistics.Instance.werewolfsKilled++;
                break;
            case Type.Bear:
                Statistics.Instance.bearsKilled++;
                break;
            case Type.Slime:
                Statistics.Instance.slimesKilled++;
                break;
            case Type.Goblin:
                Statistics.Instance.goblinsKilled++;
                break;
            case Type.Orc:
                Statistics.Instance.orcsKilled++;
                break;
            case Type.Frogman:
                Statistics.Instance.frogmansKilled++;
                break;
            case Type.Troll:
                Statistics.Instance.trollsKilled++;
                break;
            case Type.Cyclops:
                Statistics.Instance.cyclopsKilled++;
                break;
            case Type.Yeti:
                Statistics.Instance.yetisKilled++;
                break;
        }
        
        if (Quests.Instance != null) Quests.Instance.CheckOptionalQuests();
    }
}
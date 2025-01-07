using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritBehavior : EnemyBehavior
{
   // Inheriting from the EnemyBehavior class

    public override void AssignStats(int dungeonLevel)
    {
        EnemyStats baseStats = new EnemyStats(enemyType, HP: 10, SPD: 1, AGI: 2, STR: 1, AP: 0);

        enemyStats = ScaleEnemies(dungeonLevel, baseStats);
    }
}

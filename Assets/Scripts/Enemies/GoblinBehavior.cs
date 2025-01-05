using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinBehavior : EnemyBehavior
{
   // Inheriting from the EnemyBehavior class

    public override void AssignStats(int dungeonLevel)
    {
        EnemyStats baseStats = new EnemyStats(enemyType, HP: 12, SPD: 1, AGI: 1, STR: 2, AP: 1);

        enemyStats = ScaleEnemies(dungeonLevel, baseStats);
    }
}

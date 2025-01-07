using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinBehavior : EnemyBehavior
{
   // Inheriting from the EnemyBehavior class

    public override void AssignStats(int dungeonLevel)
    {
        EnemyStats baseStats = new EnemyStats(enemyType, HP: 7, SPD: 2, AGI: 2, STR: 1, AP: 0);

        enemyStats = ScaleEnemies(dungeonLevel, baseStats);
    }
}

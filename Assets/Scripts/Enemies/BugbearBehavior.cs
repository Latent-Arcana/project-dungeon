using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugbearBehavior : EnemyBehavior
{
   // Inheriting from the EnemyBehavior class

    public override void AssignStats(int dungeonlevel)
    {
        EnemyStats baseStats = new EnemyStats(enemyType, HP: 15, SPD: 1, AGI: 1, STR: 2, AP: 2);

        enemyStats = ScaleEnemies(dungeonlevel, baseStats);
    }
}

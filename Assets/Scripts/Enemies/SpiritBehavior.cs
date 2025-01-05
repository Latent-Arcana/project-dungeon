using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritBehavior : EnemyBehavior
{
   // Inheriting from the EnemyBehavior class

    public override void AssignStats()
    {
        enemyStats = new EnemyStats(enemyType, HP: 10, SPD: 1, AGI: 4, STR: 1, AP: 0);
    }
}

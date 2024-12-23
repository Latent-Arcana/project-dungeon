using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugbearBehavior : EnemyBehavior
{
   // Inheriting from the EnemyBehavior class

    public override void AssignStats()
    {
        enemyStats = new EnemyStats(Enums.EnemyType.Bugbear, HP: 15, SPD: 1, AGI: 1, STR: 2, AP: 2);
    }
}

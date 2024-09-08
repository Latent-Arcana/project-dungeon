using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PyroBehavior : EnemyBehavior
{
    // Inheriting from the EnemyBehavior class
    // Pyromancer is a TEST enemy. Test your overrides here!
    public override void AssignStats()
    {
        enemyStats = new EnemyStats("pyromancer", 14, 2, 1);
    }
}

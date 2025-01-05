using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KoboldBehavior : EnemyBehavior
{
    // Inheriting from the EnemyBehavior class
    // Pyromancer is a TEST enemy. Test your overrides here!
    public override void AssignStats(int dungeonLevel)
    {
        EnemyStats baseStats = new EnemyStats(enemyType, 3, 2, 2, 1, 0);

        enemyStats = ScaleEnemies(dungeonLevel, baseStats);
    }
}

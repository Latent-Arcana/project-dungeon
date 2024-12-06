using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KoboldBehavior : EnemyBehavior
{
    // Inheriting from the EnemyBehavior class
    // Pyromancer is a TEST enemy. Test your overrides here!
    public override void AssignStats()
    {
        enemyStats = new EnemyStats(Enums.EnemyType.Kobold, 3, 2, 2, 1, 0);
    }
}

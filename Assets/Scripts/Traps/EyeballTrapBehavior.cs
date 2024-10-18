using UnityEngine;


public class EyeballTrapBehavior : TrapBehavior
{

    public override void Input_OnPlayerMoved(object sender, PlayerMovement.MovementArgs e)
    {
        float spawnChance = UnityEngine.Random.value;
        int prefabChoice = UnityEngine.Random.Range(0, projectilePrefabs.Length);

        if (spawnChance <= 0.2f && playerInRoom)
        {
            SpawnProjectile(projectilePrefabs[prefabChoice]);
        }

        Physics2D.SyncTransforms();
    }


    // The ghosts spawn on top of the trap. They are invisible and they cannot collide
    // Then they navigate towards the player until they are within 4 tiles
    public override void SpawnProjectile(GameObject projectile)
    {

        Vector3 originPosition = transform.position;

        int randomSpawnChoice = UnityEngine.Random.Range(0, 8);

        Vector3 spawnPosition = originPosition;
        Vector3 direction = Vector3.zero;

        /*
            Spawn positions
                0 1 2
                3 X 4
                5 6 7
        */

        switch (randomSpawnChoice)
        {
            case 0:
                direction = Vector3.left + Vector3.up;
                spawnPosition = originPosition + direction;
                break;

            case 1:
                direction = Vector3.up;
                spawnPosition = originPosition + direction;
                break;

            case 2:
                direction = Vector3.right + Vector3.up;
                spawnPosition = originPosition + direction;
                break;

            case 3:
                direction = Vector3.left;
                spawnPosition = originPosition + direction;
                break;

            case 4:
                direction = Vector3.right;
                spawnPosition = originPosition + direction;
                break;

            case 5:
                direction = Vector3.down + Vector3.left;
                spawnPosition = originPosition + direction;
                break;

            case 6:
                direction = Vector3.down;
                spawnPosition = originPosition + direction;
                break;

            case 7:
                direction = Vector3.right + Vector3.down;
                spawnPosition = originPosition + direction;
                break;

            default:
                direction = Vector3.zero;
                spawnPosition = originPosition;
                break;
        }

        // Now lets's do the check to see if this spawn point is even valid
        Vector2 checkPosition = (Vector2)spawnPosition;
        LayerMask mask = LayerMask.GetMask("Default"); // we only care about colliding on default for now, but we should add in other layers here if needed
        Collider2D collision = Physics2D.OverlapCircle(checkPosition, 0.1f, mask);

        if (collision == null || collision.tag == "enemy")
        {
            GameObject proj = Instantiate(projectile, spawnPosition, Quaternion.identity);
            ProjectileBehavior projectileBehavior = proj.GetComponent<ProjectileBehavior>();
            projectileBehavior.directionOfTravel = direction;
            projectileBehavior.trap = gameObject;
            projectileBehavior.projectileId = 0;
            projectileBehavior.isAtSpawn = true;
            projectileBehavior.Player_Stats = Player_Stats;

            GhostProjectileBehavior ghostProjectile = projectileBehavior as GhostProjectileBehavior;
            ghostProjectile.moveCountMax = Random.Range(3, 11);
        }

        return;
    }
}
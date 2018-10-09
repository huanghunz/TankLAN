using UnityEngine;
using UnityEngine.Networking;

public class PowerUp : NetworkBehaviour
{

    public GameObject enemyPrefab;
    public int numberOfEnemies = 5;
    

    //public override void OnStartServer()
    public void SpawnPowerupItems()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            //var spawnPosition = new Vector3(
            //    Random.Range(-8.0f, 8.0f),
            //    0.0f,
            //    Random.Range(-8.0f, 8.0f));

            //var spawnRotation = Quaternion.Euler(
            //    0.0f,
            //    Random.Range(0, 180),
            //    0.0f);

            var item = (GameObject)Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity);//, spawnPosition, spawnRotation);
            Vector3 pos = GameController.GetUniqueSpawnPosition();
            item.transform.position = pos;// new Vector3(pos.x, 10f, pos.z);
            NetworkServer.Spawn(item);
        }
    }
}
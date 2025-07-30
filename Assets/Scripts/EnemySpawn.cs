using UnityEngine;

public class EnemySpawn : MonoBehaviour
{

    public GameObject[] PossileEnemys;

    private LevelPart part;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        part = transform.parent.gameObject.GetComponent<LevelPart>();
        int random = Random.Range(0, PossileEnemys.Length - 1);

        var clone = GameObject.Instantiate(PossileEnemys[random]);
        clone.transform.position = transform.position;
        clone.GetComponent<IEnemy>().playerController = part.player;
        clone.GetComponent<IEnemy>().gameHandler = part.gameHandler;

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(5, 3));
    }

    // Update is called once per frame
    void Update()
    {

    }
}

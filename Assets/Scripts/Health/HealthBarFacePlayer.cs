using UnityEngine;

public class HealthBarFacePlayer : MonoBehaviour
{
    private Transform player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(player)
        {
            Vector3 targetPosition = player.position;

            targetPosition.y = transform.position.y;

            transform.LookAt(targetPosition);
        }
    }
}

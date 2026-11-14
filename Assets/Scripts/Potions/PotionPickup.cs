using UnityEngine;

public class PotionPickup : MonoBehaviour
{
    [SerializeField]
    private StatHandler handler;

    private AllPotions potionOptions;

    void Start()
    {
        handler = GameObject.FindGameObjectWithTag("StatHandler").GetComponent<StatHandler>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int randomIndex = Random.Range(0, potionOptions.GetAllPotions().Count);
            handler.AddPotionToList(potionOptions.GetAllPotions()[randomIndex]);
        }
    }
}

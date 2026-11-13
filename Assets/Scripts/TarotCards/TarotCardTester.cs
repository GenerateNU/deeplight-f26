using UnityEngine;

/// <summary>
/// TEMPORARY: test class for OnKillTarotCard.
/// K = simulate an enemy kill, L = log current AttackDamage.
/// </summary>
public class TarotCardTester : MonoBehaviour
{
    [SerializeField] private OnKillTarotCard _card;
    [SerializeField] private StatHandler _statHandler;  // drag the Stat Handler object here
    [SerializeField] private GameObject _player;

    private TarotContext _context;

    void Start()
    {
        _context = new TarotContext { Player = _player, StatsHandler = _statHandler };
        _card.ApplyTarotCard(_context);
        Debug.Log("Applied " + _card.CardName);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            _card.HandleEnemyKilled();
            Debug.Log("Simulated kill");
        }


        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log($"AttackDamage = {_statHandler.GetStat(Stat.AttackDamage)} at {Time.time:F1}s");
        }
    }

    void OnDestroy()
    {
        _card.RemoveTarotCard();
    }
}

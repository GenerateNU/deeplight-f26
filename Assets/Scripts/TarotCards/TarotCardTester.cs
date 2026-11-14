using UnityEngine;

/// <summary>
/// TEMPORARY: test class for tarot cards.
/// Drag in only the card(s) you want to test; empty slots are skipped.
/// K = simulate an enemy kill (On Kill), L = log current stats.
/// </summary>
public class TarotCardTester : MonoBehaviour
{
    [Header("Cards to test (leave empty to skip)")]
    [SerializeField] private OnKillTarotCard _onKillCard;
    [SerializeField] private PierceEffectTarotCard _pierceCard;

    [Header("References")]
    [SerializeField] private StatHandler _statHandler;  // drag the Stat Handler object here
    [SerializeField] private GameObject _player;

    private TarotContext _context;

    void Start()
    {
        _context = new TarotContext { Player = _player, StatsHandler = _statHandler };

        if (_onKillCard != null)
        {
            _onKillCard.ApplyTarotCard(_context);
            Debug.Log("Applied " + _onKillCard.CardName);
        }

        if (_pierceCard != null)
        {
            _pierceCard.ApplyTarotCard(_context);
            Debug.Log("Applied " + _pierceCard.CardName);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (_onKillCard != null)
            {
                _onKillCard.HandleEnemyKilled();
                Debug.Log("Simulated kill");
            }
            else
            {
                Debug.Log("No On Kill card assigned, K does nothing.");
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log($"AttackDamage = {_statHandler.GetStat(Stat.AttackDamage)}, " +
                      $"ProjectilePierce = {_statHandler.GetStat(Stat.ProjectilePierce)} at {Time.time:F1}s");
        }
    }

    void OnDestroy()
    {
        if (_onKillCard != null)
        {
            _onKillCard.RemoveTarotCard();
        }

        if (_pierceCard != null)
        {
            _pierceCard.RemoveTarotCard();
        }
    }
}

using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class CardRewardController : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject[] cards;
    private bool isEnabled = false;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Deactivate();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && !isEnabled)
        {
            if (isEnabled)
            {
                Deactivate();
            }
            else
            {
                Activate();
            }
        }
    }

    public void Activate()
    {
        canvas.enabled = true;
        isEnabled = true;
    }

    public void Deactivate()
    {
        canvas.enabled = false;
        isEnabled = false;
    }
}

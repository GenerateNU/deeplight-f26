using UnityEngine;

public class DestroyObject : MonoBehaviour
{

    // Fields 
    public float duration = 3f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, duration); // destroy object after duration
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

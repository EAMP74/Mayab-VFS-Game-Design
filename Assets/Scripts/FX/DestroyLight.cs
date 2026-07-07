using UnityEngine;

public class DestroyLight : MonoBehaviour
{
    [SerializeField] private float _destroyDelay = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, _destroyDelay); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

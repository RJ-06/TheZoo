using UnityEngine;

public class RandomizeColor : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        Renderer rend = gameObject.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color =
                new Color(Random.value, Random.value, Random.value);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

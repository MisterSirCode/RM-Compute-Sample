using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Type
{
    Rectangle,
    Circle
}

public class Object : MonoBehaviour
{
    [SerializeField]
    Type type = new Type();

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Type
{
    Rectangle,
    Circle
}

public enum Operation
{
    Union,
    Subtraction,
    Intersection,
}

public class Object : MonoBehaviour
{
    public Color color = Color.white;
    public Type type = new Type();
    public Operation operation = new Operation();
    [HideInInspector]
    public int children = 0;

    public Vector2 Position {
        get {
            return transform.position;
        }
    }

    public Vector2 Scale {
        get {
            Vector2 parentScale = Vector2.one;
            if (transform.parent != null && transform.parent.GetComponent<Object>() != null) {
                parentScale = transform.parent.GetComponent<Object>().Scale;
            }
            return Vector3.Scale(transform.localScale, parentScale);
        }
    }
}

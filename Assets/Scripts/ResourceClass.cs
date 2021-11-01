using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    Food,
    Wood,
    Gold,
    Stone
} // , Metal

public class ResourceClass 
{

    public ResourceType Type;
    public int Quantity;

    public ResourceClass( ResourceType type, int quantity)
    {
        this.Type = type;
        this.Quantity = quantity;
    }
}
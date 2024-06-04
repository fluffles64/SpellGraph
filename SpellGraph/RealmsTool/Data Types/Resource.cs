using System;
using UnityEngine;

[CreateAssetMenu]
[Serializable]
public class Resource : ScriptableObject
{
    [SerializeField] private string name = "New Resource";
    [SerializeField] private Color color;
    [SerializeField] private bool isGlobal;
    [SerializeField] private float startValue;
    [SerializeField] private float baseValue;
    [SerializeField] private float regen;
    [SerializeField] private float regenRate = 5;

    public string Name { get { return name; } set { name = value; } }
    public Color Color { get { return color; } set { color = value; } }
    public bool IsGlobal { get { return isGlobal; } set { isGlobal = value; } }

    public float StartValue { get { return startValue; } set { startValue = value; } }
    public float BaseValue { get { return baseValue; } set { baseValue = value; } }
    public float Regen { get { return regen; } set { regen = value; } }
    public float RegenRate { get { return regenRate; } set { regenRate = value; } }
}
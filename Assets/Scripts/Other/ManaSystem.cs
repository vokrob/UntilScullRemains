using System;
using UnityEngine;

public class ManaSystem : MonoBehaviour
{
    public static ManaSystem Instance { get; private set; }

    public event EventHandler OnManaChanged;
    public event EventHandler OnManaRegenerated;

    [Header("Mana Settings")]
    [SerializeField] private float maxMana = 100f;
    [SerializeField] private float manaRegenRate = 10f;
    [SerializeField] private float manaRegenDelay = 2f;

    private float currentMana;
    private float lastManaChangeTime;
    private bool isRegenerating = false;

    private void Awake()
    {
        Instance = this;
        currentMana = maxMana;
    }

    private void Update()
    {
        HandleManaRegeneration();
    }

    private void HandleManaRegeneration()
    {
        if (Time.time >= lastManaChangeTime + manaRegenDelay && !isRegenerating)
        {
            isRegenerating = true;
        }

        if (isRegenerating && currentMana < maxMana)
        {
            currentMana += manaRegenRate * Time.deltaTime;
            currentMana = Mathf.Min(currentMana, maxMana);
            OnManaRegenerated?.Invoke(this, EventArgs.Empty);
            OnManaChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool TryUseMana(float amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            lastManaChangeTime = Time.time;
            isRegenerating = false;
            OnManaChanged?.Invoke(this, EventArgs.Empty);
            return true;
        }
        return false;
    }

    public float GetCurrentMana() => currentMana;
    public float GetMaxMana() => maxMana;
    public float GetManaPercentage() => currentMana / maxMana;

    public void AddMana(float amount)
    {
        currentMana += amount;
        currentMana = Mathf.Min(currentMana, maxMana);
        OnManaChanged?.Invoke(this, EventArgs.Empty);
    }
}
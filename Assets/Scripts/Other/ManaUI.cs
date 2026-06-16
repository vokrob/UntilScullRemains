using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ManaUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ManaSystem manaSystem;
    [SerializeField] private Image manaBarFill;
    [SerializeField] private Image manaBarBackground;

    [Header("Animation Settings")]
    [SerializeField] private float decreaseSpeed = 10f;
    [SerializeField] private float increaseSpeed = 5f;

    private float currentFillAmount;
    private float targetFillAmount;

    private void Start()
    {
        StartCoroutine(InitManaSystemRoutine());
    }
    private IEnumerator InitManaSystemRoutine()
    {
        yield return null; // ∆дем один кадр

        if (manaSystem == null)
            manaSystem = ManaSystem.Instance;

        if (manaSystem != null)
        {
            currentFillAmount = manaSystem.GetManaPercentage();
            targetFillAmount = currentFillAmount;

            manaSystem.OnManaChanged += OnManaChanged;
        }
    }

    private void Update()
    {
        targetFillAmount = manaSystem.GetManaPercentage();

        if (currentFillAmount > targetFillAmount)
        {
            currentFillAmount -= decreaseSpeed * Time.deltaTime;
            currentFillAmount = Mathf.Max(currentFillAmount, targetFillAmount);
        }
        else if (currentFillAmount < targetFillAmount)
        {
            currentFillAmount += increaseSpeed * Time.deltaTime;
            currentFillAmount = Mathf.Min(currentFillAmount, targetFillAmount);
        }

        if (manaBarFill != null)
        {
            manaBarFill.fillAmount = currentFillAmount;
        }
    }

    private void OnManaChanged(object sender, System.EventArgs e)
    {
        
    }

    private void OnDestroy()
    {
        if (manaSystem != null)
        {
            manaSystem.OnManaChanged -= OnManaChanged;
        }
    }
}
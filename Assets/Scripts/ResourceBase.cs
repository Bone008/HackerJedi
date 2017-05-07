using UnityEngine;
using UnityEngine.Events;

public abstract class ResourceBase : MonoBehaviour
{
    public float minValue;
    public float maxValue;

    [SerializeField]
    private float _currentValue;
    public float currentValue { get { return _currentValue; } private set { _currentValue = value; } }
    
    public float filledPercentage
    {
        get { return currentValue / maxValue; }
    }

    public UnityEvent onChange;

    public RectTransform resourcePanel;

    private void Awake()
    {
        // add listener to update the resource panel
        if (resourcePanel != null)
            onChange.AddListener(UpdateResourcePanel);
    }

    void Start()
    {
        // notify listeners
        if (onChange != null)
            onChange.Invoke();
    }

    /// <summary>
    /// Changes the value of the resource by amount. Clips between minValue and maxValue of resource.
    /// </summary>
    /// <param name="amount">the amount of the resource to add</param>
    /// <returns>true if the value has been affected by the change, false otherwise</returns>
    public virtual bool ChangeValue(float amount)
    {
        float valueBefore = currentValue;
        // change value
        currentValue = Mathf.Clamp(currentValue + amount, minValue, maxValue);

        bool hasChanged = (valueBefore != currentValue);

        // notify listeners
        if (hasChanged && onChange != null)
            onChange.Invoke();

        return hasChanged;
    }

    /// <summary>
    /// Changes the value of the resource by amount, if the full amount fits between minValue and maxValue.
    /// </summary>
    /// <param name="amount">the amount of the resource to fully add</param>
    /// <returns>true if the value was changed, false otherwise</returns>
    public virtual bool SafeChangeValue(float amount)
    {
        // do nothing if cannot change value by full amount
        if (!CanChangeValue(amount))
            return false;

        ChangeValue(amount);
        return true;
    }

    /// <summary>
    /// Checks if the value changed by amount fits between minValue and maxValue
    /// </summary>
    /// <param name="amount">the amount of the resource to theoretically add</param>
    /// <returns>true if the amount fits, false otherwise</returns>
    public virtual bool CanChangeValue(float amount)
    {
        float targetValue = currentValue + amount;
        return targetValue == Mathf.Clamp(targetValue, minValue, maxValue);
    }

    private void UpdateResourcePanel()
    {
        resourcePanel.transform.localScale = new Vector3(filledPercentage, 1, 1);
    }

}

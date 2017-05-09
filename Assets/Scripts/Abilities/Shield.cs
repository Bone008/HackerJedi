using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ShieldEnergyResource))]
public class Shield : AbstractAbility
{

    public float drainRate;
    public float cooldown;
    public GameObject shieldTransform;

    private ShieldEnergyResource energy;
    private Vector3 initialScale;
    private Image energyPanelImage;
    private Color initialPanelColor;

    private bool isActive = false;
    private bool isRecovering = false;
    private float lastUpdateTime;

    [Header("Level 2")]
    public float maxEnergyLevel2;

    private void Start()
    {
        energy = GetComponent<ShieldEnergyResource>();
        energy.onChange.AddListener(energy_Change);

        initialScale = shieldTransform.transform.localScale;
        shieldTransform.transform.localScale = Vector3.zero;

        energyPanelImage = energy.resourcePanel.GetComponent<Image>();
        initialPanelColor = energyPanelImage.color;

        lastUpdateTime = Time.time;
    }

    public override void ConfigureForLevel(int level)
    {
        if (level > 1)
        {
            var energy = GetComponent<ShieldEnergyResource>(); // Start has not been called yet
            energy.maxValue = maxEnergyLevel2;
            energy.ChangeValue(energy.maxValue);
        }
    }

    public void AbsorbDamage(float damage)
    {
        energy.ChangeValue(-damage);
    }


    private void energy_Change()
    {
        if (energy.currentValue <= 0)
        {
            isActive = false;
            isRecovering = true;
            this.AnimateVector(0.1f, initialScale, Vector3.zero, Util.EaseInOut01, v => shieldTransform.transform.localScale = v);
            energyPanelImage.color = Color.red;
        }
        else if (isRecovering && energy.currentValue >= energy.maxValue)
        {
            isRecovering = false;
            energyPanelImage.color = initialPanelColor;
        }
    }

    private void Update()
    {
        // this is used so the shield also recharges while disabled
        float actualDelta = Time.time - lastUpdateTime;
        lastUpdateTime = Time.time;

        if (isActive)
        {
            energy.ChangeValue(-drainRate * actualDelta);
        }
        else
        {
            float regenRate = energy.maxValue / cooldown;
            energy.ChangeValue(regenRate * actualDelta);
        }
    }

    protected override void OnTriggerDown()
    {
        if (!isRecovering)
        {
            this.AnimateVector(0.1f, Vector3.zero, initialScale, Util.EaseInOut01, v => shieldTransform.transform.localScale = v);
            isActive = true;
        }
    }

    protected override void OnTriggerUp()
    {
        if (isActive)
        {
            this.AnimateVector(0.1f, initialScale, Vector3.zero, Util.EaseInOut01, v => shieldTransform.transform.localScale = v);
            isActive = false;
        }
    }
}

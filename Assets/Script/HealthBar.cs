using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public TMP_Text healthBarText;

    Damageable playerDamageable;

	private void Awake()
	{
        GameObject player = GameObject.FindGameObjectWithTag("Player");

		if(player == null)
		{
			Debug.Log("No Player ...");
		}
        playerDamageable = player.GetComponent<Damageable>();
	}

	// Start is called before the first frame update
	void Start()
    {
        healthSlider.value = CalculateSliderPercentage(playerDamageable.Health, playerDamageable.MaxHealth);
		healthBarText.text = playerDamageable.Health + "/" + playerDamageable.MaxHealth;
	}

	private float CalculateSliderPercentage(float currentHealth, float maxHealth)
	{
		return currentHealth / maxHealth;
	}

	private void OnEnable()
	{
		playerDamageable.healthChanged.AddListener(OnPlayerHealthChanged);
	}
	private void OnDisable()
	{
		playerDamageable.healthChanged.RemoveListener(OnPlayerHealthChanged);
	}

	private void OnPlayerHealthChanged(int newHealth, int maxHealth)
	{
		healthSlider.value = CalculateSliderPercentage(newHealth, maxHealth);
		healthBarText.text = newHealth + "/" + maxHealth;
	}
}

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
	public Slider manaSlider;
	public TMP_Text manaBarText;

	public int MaxMana = 400;
	public int currentMana;
	private float manaRegenRate = 2f;
	private int manaRegenAmount = 100; // Mỗi lần hồi phục 10 mana

	private void Awake()
	{
		currentMana = MaxMana;
		manaSlider.maxValue = MaxMana;
		manaSlider.value = currentMana;
		UpdateManaText();
		StartCoroutine(RegenMana());
	}

	private void UpdateManaText()
	{
		manaBarText.text = currentMana + "/" + MaxMana;
	}

	private IEnumerator RegenMana()
	{
		while (true)
		{
			yield return new WaitForSeconds(manaRegenRate);
			RestoreMana(manaRegenAmount);
		}
	}

	public void UseMana(int amount)
	{
		if (currentMana >= amount)
		{
			currentMana -= amount;
			manaSlider.value = currentMana;
			UpdateManaText();
		}
	}

	private void RestoreMana(int amount)
	{
		currentMana = Mathf.Min(currentMana + amount, MaxMana);
		manaSlider.value = currentMana;
		UpdateManaText();
	}
	public void ActivateManaRegenBoost(float duration, float multiplier)
	{
		StartCoroutine(ManaRegenBoostCoroutine(duration, multiplier));
	}

	private IEnumerator ManaRegenBoostCoroutine(float duration, float multiplier)
	{
		float originalRegenRate = manaRegenRate;
		manaRegenRate *= multiplier;

		Debug.Log("Mana Regen Boost Activated!");

		yield return new WaitForSeconds(duration);

		manaRegenRate = originalRegenRate;

		Debug.Log("Mana Regen Boost Expired.");
	}
	public void ResetMana()
	{
		currentMana = MaxMana;
		manaSlider.value = currentMana;
		UpdateManaText();
	}

}

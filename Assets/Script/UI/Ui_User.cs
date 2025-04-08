using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Ui_User : MonoBehaviour
{
    public int SkillPoint_InRe;

    [SerializeField] private TextMeshProUGUI skillPointText;

	void Start()
	{
		UpdateSkillPointUI();
	}

	public void IncreaseSkillPoint(int amount)
	{
		int currentPoints = PlayerPrefs.GetInt("SkillPoints", 0);
		PlayerPrefs.SetInt("SkillPoints", currentPoints + amount);
		UpdateSkillPointUI();
	}

	private void UpdateSkillPointUI()
	{
		skillPointText.text = PlayerPrefs.GetInt("SkillPoints", 0).ToString("#,#");
	}

	private bool EnoughSkillPoint(int skillPoint)
    {
        int mySkillPoint = PlayerPrefs.GetInt("SkillPoints");

        if(mySkillPoint > SkillPoint_InRe)
        {
            int newAmountOfSkillPoint = mySkillPoint - skillPoint;
            PlayerPrefs.SetInt("SkillPoints", newAmountOfSkillPoint);
			skillPointText.text = PlayerPrefs.GetInt("SkillPoints").ToString("#,#");
			return true;
        }
        return false;
    }

}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Ui_InGame : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI skillPoint;

	private void Start()
	{
		InvokeRepeating("UpdateInfo", 0, .15f);
	}

	private void UpdateInfo()
	{
		skillPoint.text = GameManager.instance.skillPoints.ToString();
	}
}

﻿// Gắn vào chính object gốc
using UnityEngine;

public class KeepAlive : MonoBehaviour
{
	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}
}

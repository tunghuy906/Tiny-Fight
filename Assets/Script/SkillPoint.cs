using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPoint : MonoBehaviour
{
	public int skillPointValue = 1;
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.GetComponent<PlayerController>() != null)
		{
			if (collision.CompareTag("Player")) 
			{
				AudioManager.instance.PlaySfx(3);
				GameManager.instance.AddSkillPoint(skillPointValue);
				Destroy(gameObject); 
			}
		}
	}
}

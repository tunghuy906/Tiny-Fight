using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource[] sfx;
    [SerializeField] private AudioSource bgm;

	private void Start()
	{
        PlayBGM();
	}
	private void Awake() => instance = this;

    public void PlaySfx(int index)
    {
        if(index < sfx.Length)
        {
            sfx[index].Play();
        }
    }

    public void StopSfx(int index)
    {
        sfx[index].Stop();
    }

    public void PlayBGM()
    {
        bgm.Play();
    }
    public void StopBGM()
    {
        bgm.Stop();
    }
}

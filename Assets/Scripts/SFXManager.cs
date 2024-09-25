using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXManager : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        InteractionManager.instance.OnCombination.AddListener(OnCombination);
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCombination()
    {
        audioSource.PlayOneShot(audioSource.clip);
    }
}

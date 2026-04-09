using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AmbientMusic : MonoBehaviour
{
    [Header("=== MÚSICA ===")]
    public AudioClip musicClip;

    [Range(0f, 1f)]
    public float volume = 0.5f;

    [Tooltip("¿Reproducir al iniciar la escena?")]
    public bool playOnStart = true;

    [Tooltip("Fade in al iniciar (segundos). 0 = sin fade")]
    public float fadeInDuration = 2f;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (musicClip == null)
        {
            Debug.LogWarning("[AmbientMusic] No hay AudioClip asignado.");
            return;
        }

        audioSource.clip = musicClip;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;    // 2D global
        audioSource.volume = fadeInDuration > 0f ? 0f : volume;

        if (playOnStart)
        {
            audioSource.Play();
            if (fadeInDuration > 0f)
                StartCoroutine(FadeIn());
        }
    }

    System.Collections.IEnumerator FadeIn()
    {
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, volume, elapsed / fadeInDuration);
            yield return null;
        }
        audioSource.volume = volume;
    }

    public void StopMusic() { audioSource.Stop(); }
    public void PlayMusic() { if (!audioSource.isPlaying) audioSource.Play(); }
    public void SetVolume(float v) { volume = v; audioSource.volume = v; }
}
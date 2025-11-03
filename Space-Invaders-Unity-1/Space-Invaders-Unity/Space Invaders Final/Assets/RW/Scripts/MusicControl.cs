using UnityEngine;

public class MusicControl : MonoBehaviour
{
    private readonly float defaultTempo = 1.33f; // 4 beats in 3 seconds

    public AudioSource source;
    public int pitchChangeSteps = 5;
    public float maxPitch = 5.5f;

    private float pitchChange;

    public float Tempo { get; private set; }

    public void StopPlaying() => source?.Stop();

    public void IncreasePitch()
    {
        if (source == null) return;
        if (source.pitch >= maxPitch) return;

        // ⬇️ Augmentation plus douce du pitch
        source.pitch = Mathf.Clamp(source.pitch + pitchChange * 0.5f, 1, maxPitch);

        // ⬇️ Accélération réduite (moins brutale quand il reste peu d’invaders)
        Tempo *= Mathf.Pow(2, pitchChange * 0.25f);
    }

    private void Start()
    {
        if (source == null)
            Debug.LogWarning("MusicControl: no AudioSource assigned.");
        else
            source.pitch = 1f;

        Tempo = defaultTempo;
        pitchChange = (pitchChangeSteps > 0) ? maxPitch / pitchChangeSteps : 0f;
    }
}

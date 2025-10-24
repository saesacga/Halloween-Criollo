using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Singleton
    public static AudioManager Instance { get; private set; }
    
    private void Awake() 
    { 
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        
        Instance = this;
    }

    #endregion
    
    [TabGroup("🎵"), SerializeField]
    private AudioSource _musicAudioSource;
    [TabGroup("🎵"), SerializeField]
    private AudioClip _changeLevelMusicClip;
    public AudioClip ChangeLevelMusicClip => _changeLevelMusicClip;
    [TabGroup("🎵"), SerializeField]
    private AudioClip[] _levelMusicClips;
    public AudioClip[] LevelMusicClips => _levelMusicClips;
    
    public void FadeMusic(AudioClip newClip, float fadeDuration = 1f)
    {
        if (!_musicAudioSource.isPlaying)
        {
            _musicAudioSource.clip = newClip;
            _musicAudioSource.volume = 1f;
            _musicAudioSource.Play();
            return;
        }

        _musicAudioSource.DOFade(0f, fadeDuration).OnComplete(() =>
        {
            _musicAudioSource.clip = newClip;
            _musicAudioSource.Play();
            _musicAudioSource.DOFade(1f, fadeDuration);
        });
    }
}

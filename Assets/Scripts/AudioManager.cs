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
    
    [TabGroup("SFX", TextColor = "orange"), SerializeField]
    private AudioSource _sfxAudioSource;
    [TabGroup("SFX"), SerializeField]
    private AudioSource _sfxLoopAudioSource;
    [TabGroup("SFX"), SerializeField]
    private AudioClip[] _sfxClips;
    public AudioClip[] SfxClips => _sfxClips;
    
    public void FadeMusic(AudioClip newClip, float fadeDuration = 0.5f)
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

    public void PlaySfx(AudioClip clip, float volume = 1f)
    {
        if (_sfxAudioSource == null || clip == null) return;
        
        _sfxAudioSource.PlayOneShot(clip, volume);
    }
    
    public void PlayLoop(AudioClip clip, float volume = 1f)
    {
        _sfxLoopAudioSource.clip = clip;
        _sfxLoopAudioSource.volume = volume;
        _sfxLoopAudioSource.loop = true;
        _sfxLoopAudioSource.Play();
    }
    
    public void StopLoop()
    {
        if (_sfxLoopAudioSource == null) return;

        _sfxLoopAudioSource.Stop();
        _sfxLoopAudioSource.loop = false;
    }
}

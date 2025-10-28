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
        
        MusicVolume = _musicAudioSource.volume;
        SfxVolume = _sfxAudioSource.volume;
    }

    #endregion
    
    [TabGroup("🎵"), SerializeField]
    private AudioSource _musicAudioSource;
    private float _musicVolume;
    public float MusicVolume
    {
        get => _musicVolume;
        set
        {
            _musicVolume = Mathf.Clamp01(value);
            _musicAudioSource.volume = _musicVolume;
        }
    }

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
    private float _sfxVolume;
    public float SfxVolume
    {
        get => _sfxVolume;
        set
        {
            _sfxVolume = Mathf.Clamp01(value);
            _sfxAudioSource.volume = _sfxVolume;
            _sfxLoopAudioSource.volume = _sfxVolume;
        }
    }
    [TabGroup("SFX"), SerializeField]
    private AudioClip[] _sfxClips;
    public AudioClip[] SfxClips => _sfxClips;
    
    public void FadeMusic(AudioClip newClip, float fadeDuration = 0.5f)
    {
        if (!_musicAudioSource.isPlaying)
        {
            _musicAudioSource.clip = newClip;
            _musicAudioSource.Play();
            return;
        }

        _musicAudioSource.DOFade(0f, fadeDuration).OnComplete(() =>
        {
            _musicAudioSource.clip = newClip;
            _musicAudioSource.Play();
            _musicAudioSource.DOFade(MusicVolume, fadeDuration);
        });
    }

    public void PlaySfx(AudioClip clip)
    {
        if (_sfxAudioSource == null || clip == null) return;
        
        _sfxAudioSource.PlayOneShot(clip, SfxVolume);
    }
    
    public void PlayLoop(AudioClip clip)
    {
        _sfxLoopAudioSource.clip = clip;
        _sfxLoopAudioSource.loop = true;
        _sfxLoopAudioSource.Play();
    }
    
    public void StopLoop()
    {
        if (_sfxLoopAudioSource == null) return;

        _sfxLoopAudioSource.Stop();
        _sfxLoopAudioSource.loop = false;
    }
    
    private bool _musicWasPlaying;
    private bool _sfxWasPlaying;
    private bool _loopWasPlaying;
    
    public void PauseAllAudio()
    {
        if (_musicAudioSource.isPlaying)
        {
            _musicWasPlaying = true;
            _musicAudioSource.Pause();
        }
        else _musicWasPlaying = false;
        
        if (_sfxLoopAudioSource.isPlaying)
        {
            _loopWasPlaying = true;
            _sfxLoopAudioSource.Pause();
        }
        else _loopWasPlaying = false;
    }
    
    public void ResumeAllAudio()
    {
        if (_musicWasPlaying)
            _musicAudioSource.Play();

        if (_loopWasPlaying)
            _sfxLoopAudioSource.Play();
    }
}

using UnityEngine;
using System.Collections;

public class FadeAudio : MonoBehaviour
{
    public AudioSource other;

    public float maxVolume = 0.35f;
    public float fadeInSpeed = 0.01f;
    public float fadeOutSpeed = 0.045f;
    AudioSource m_Audio;
    float audio2Volume;

    void Awake()
    {
        m_Audio = GetComponent<AudioSource>();
        audio2Volume = m_Audio.volume;

    }

    void Start()
    {
        if (m_Audio.playOnAwake)
        {
            StartCoroutine(FadeIn());
            Invoke("FadeFile", m_Audio.clip.length - 5.0f);
        }
        else
            Invoke("PlayFile", other.clip.length - 10.0f);

    }

    void PlayFile()
    {
        //Debug.Log("Playing");
        m_Audio.Play();
        StartCoroutine(FadeIn());
    }

    void FadeFile()
    {
        //Debug.Log("Fading");
        StartCoroutine(FadeOut());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopAllCoroutines();
            StartCoroutine(FadeOut());
            Camera.main.GetComponent<PlayMakerFSM>().Fsm.Event("FadeCameraOut");
        }
    }

    IEnumerator FadeIn()
    {
        while (audio2Volume < maxVolume)
        {
            audio2Volume += fadeInSpeed * Time.deltaTime;
            m_Audio.volume = audio2Volume;
            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        while (audio2Volume > 0.1f)
        {
            audio2Volume -= fadeOutSpeed * Time.deltaTime;
            m_Audio.volume = audio2Volume;
            yield return null;
        }
        Application.Quit();
    }
}

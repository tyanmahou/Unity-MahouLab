
using UnityEngine;

//音楽再生class
public class Sound
{
    //hierarchyでまとめあげる親
    static GameObject m_parentObject;

    //AuioSource用のゲームオブジェクト
    GameObject m_soundObject;

    AudioClip m_clip;
    AudioSource m_source;

    public Sound(string path)
    {
        if (m_parentObject==null)
        {
            m_parentObject = new GameObject("Sound");
        }
        if (m_soundObject==null)
        {
            m_soundObject = new GameObject(path);
            m_soundObject.transform.parent = m_parentObject.transform;
            m_soundObject.AddComponent<AudioSource>();
        }
        m_source = m_soundObject.GetComponent<AudioSource>();

        m_clip = Resources.Load(path) as AudioClip;
        if (m_clip == null)
        {
            Debug.Log("LoadError:" + path);
            return;
        }
        m_source.clip = m_clip;
    }

    //再生
    public void Play()
    {
        if(!m_source.isPlaying)
        m_source.Play();
    }

    //停止
    public void Stop()
    {
        m_source.Stop();
    }

    //一時停止
    public void Pause()
    {
        m_source.Pause();
    }

    //複数再生
    public void PlayMulti(float volume=1.0f)
    {
        m_source.PlayOneShot(m_clip, volume);
    }
    //ループ設定
    public void SetLoop(bool loop=true)
    {
        m_source.loop = loop;
    }

    //bool演算子
    //ロード失敗していたら falseが返る
    public static explicit operator bool(Sound s)
    {
        return s.m_clip != null;
    }

    //ゲッター
    public AudioSource audioSource
    {
        get
        {
            return m_source;
        }
    }
    public AudioClip clip
    {
        get
        {
            return m_clip;
        }
    }


}

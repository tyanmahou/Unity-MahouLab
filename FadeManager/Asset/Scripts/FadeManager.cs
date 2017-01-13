using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    [SerializeField]
    Texture2D m_texture;

    [SerializeField]
    Material m_mat;

    float m_fadeTime=0;
    bool m_isFading = false;

    static FadeManager Instance;

    public void Awake()
    {
        if(Instance==null)
            Instance = this;

        if (this != Instance)
        {
            Destroy(this.gameObject);
            return;
        }


        m_mat.mainTexture = m_texture;
        DontDestroyOnLoad(this.gameObject);
    }

    void OnGUI()
    {
        if (m_isFading)
        {
            float value = m_fadeTime;
            if (value > 0.75)
            {
                value = 1;
            }

            value *= (1.0f / 0.75f);
            m_mat.SetFloat("_MyFloat", value);
            Graphics.Blit(m_texture, m_mat);
        }
    }

    private IEnumerator ChangeScene(string scene, float timeSec)
    {
        m_isFading = true;
        float fadeTime = timeSec / 2.0f;
        float time = 0;
        while (time <= fadeTime)
        {
            m_fadeTime = time /fadeTime;
            time += Time.deltaTime;
            yield return 0;
        }

        //シーン切替
        SceneManager.LoadScene(scene);

        //だんだん明るく
        time = 0;
        while (time <= fadeTime)
        {
            m_fadeTime = 1-(time / fadeTime);
            time += Time.deltaTime;
            yield return 0;
        }

        m_isFading = false;
    }

    void loadScene(string name, float timeSec)
    {
        if(!m_isFading)
        StartCoroutine(ChangeScene(name, timeSec));
    }

    //シーンのロード
    static public void LoadScene(string name,float timeSec=3.0f)
    {
        Instance.loadScene(name,timeSec);
    }

    static public void SetTexture(Texture2D texture)
    {
        Instance.m_texture = texture;
    }
}

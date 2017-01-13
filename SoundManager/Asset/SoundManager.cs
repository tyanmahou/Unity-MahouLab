using System.Collections.Generic;


public class SoundManager
{
    class SoundData
    {
        public string m_path;
        public Sound m_obg;

        public SoundData(string path)
        {
            m_path = path;
        }
        public void Load()
        {
            if(m_obg == null||m_obg.audioSource==null)
            {
                m_obg = new Sound(m_path);
            }
        }
    }
    static Dictionary<string, SoundData> m_factory = new Dictionary<string, SoundData>();

    //サウンドデータの登録
    public static bool Register(string name, string path)
    {
        if (m_factory.ContainsKey(name))
            return false;

        m_factory.Add(name, new SoundData(path));

        return true;
    }

    //サウンドデータの取得
    public static Sound Get(string name)
    {
        if (m_factory.ContainsKey(name) == false)
        {
            return null;
        }
        m_factory[name].Load();

        return m_factory[name].m_obg;
    }
}

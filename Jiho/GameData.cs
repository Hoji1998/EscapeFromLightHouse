using System;

[Serializable] // 직렬화

public class Data
{
    // 각 챕터의 잠금여부를 저장할 배열
    public bool[] isUnlock = new bool[40];
    public SceneLoadManager.TextLanguage currentLanguage;
    public int resolutionIndex = 2;
    public float sfxVolume = 1f;
    public float backgroundVolume = 1f;
    public void InitializationData()
    {
        for (int i = 0; i < 40; i++)
        {
            isUnlock[i] = false;
        }

        isUnlock[0] = true;
        isUnlock[5] = true;

        currentLanguage = SceneLoadManager.TextLanguage.English;
        sfxVolume = 1f;
        backgroundVolume = 1f;
        resolutionIndex = 2;
    }
}
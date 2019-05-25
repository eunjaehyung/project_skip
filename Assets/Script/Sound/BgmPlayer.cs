using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ゲーム起動時にBGM再生を始めるオブジェクトを生成する.
public class BgmPlayer : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod]
    static private void Play()
    {
        var bgmPlayer = new GameObject("BgmPlayer");
        DontDestroyOnLoad(bgmPlayer);

        var audioSource = bgmPlayer.AddComponent<AudioSource>() as AudioSource;
        audioSource.clip = Resources.Load<AudioClip>("Sound/Bg/bgm01") as AudioClip;
        Debug.Assert(audioSource.clip != null);

        audioSource.loop = true;
        audioSource.Play();
    }
}

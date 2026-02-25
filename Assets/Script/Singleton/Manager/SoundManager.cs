using UnityEngine;
using Audio;
//音を司るクラスとインスタンスです。
public class SoundManager : MonoBehaviour
{
    public static SoundManager Current;
    [Header("単発SEのメンバ")]
    [SerializeField]AudioClip _shot;
    [SerializeField]AudioClip _gunSwap;
    [SerializeField]AudioClip _comboConnect;
    [SerializeField]AudioClip _explosion;
    [SerializeField]AudioClip _downButton;
    [Header("audioSourceのメンバ")]
    [SerializeField]AudioSource _audioSourceForOneShot;
    [SerializeField]AudioSource _audioSourceForLoop;

    void Awake()
    {
        if(Current == null)
        {
            Current = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
    }
    //こっちはスクリプトから呼ぶ用の奴
    public void PlayOneShot2D_SE(OneShot oneShot,float volume)
    {
        switch (oneShot)
        {
            case OneShot.shot:
            _audioSourceForOneShot.PlayOneShot(_shot,volume);
            break;
            case OneShot.gunSwap:
            _audioSourceForOneShot.PlayOneShot(_gunSwap,volume);
            break;
            case OneShot.comboConnect:
            _audioSourceForOneShot.PlayOneShot(_comboConnect,volume);
            break;
            case OneShot.explosion:
            _audioSourceForOneShot.PlayOneShot(_explosion,volume);
            break;
            case OneShot.downButton:
            _audioSourceForOneShot.PlayOneShot(_downButton);
            break;
        }
    }
}
namespace Audio
{
    public enum OneShot
    {
        shot,
        gunSwap,
        comboConnect,
        explosion,
        downButton,
    }

}

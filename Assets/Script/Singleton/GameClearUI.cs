using UnityEngine;
using TMPro;
using System;
//StageUI_managerにより制御されるクラス∧ゲームクリアに関するUIを責務とするクラスです。
public class GameClearUI : MonoBehaviour
{
    [SerializeField] GameObject _indicatorToClear;
    [SerializeField] TextMeshPro _breakableCountTM;
    [SerializeField] TextMeshPro _perfectCountTM;
    [SerializeField] TextMeshPro _greatCountTM;
    [SerializeField] TextMeshPro _goodCountTM;
    [SerializeField] TextMeshPro _overlookCountTM;
    [SerializeField] TextMeshPro _totalScoreTM;
    [SerializeField] TextMeshPro _accidentalShoot;
    [SerializeField] Animator _animator;

    public void Show(double score, double accidentalShoot,int perfectCount,int greatCount,int goodCount,int overlookCount)
    {
        gameObject.SetActive(true);
        _indicatorToClear.SetActive(true);
        _animator.SetTrigger("Show");

        score = Math.Round(score,1);
        accidentalShoot = Math.Round(accidentalShoot,1);
        transform.RotateAround(Player.Current.transform.position, Vector3.up, Player.Current.transform.rotation.eulerAngles.y);
        _indicatorToClear.SetActive(true);

        _perfectCountTM.text = perfectCount.ToString();
        _greatCountTM.text = greatCount.ToString();
        _goodCountTM.text = goodCount.ToString();
        _overlookCountTM.text = overlookCount.ToString();
        _breakableCountTM.text = (overlookCount + goodCount + greatCount + perfectCount).ToString();

        _accidentalShoot.text = accidentalShoot.ToString();
        _totalScoreTM.text = (score - accidentalShoot).ToString();

    }
    public void Hide()
    {
        gameObject.SetActive(false);
        _indicatorToClear.SetActive(false);
    }
    
    public void RestartShootableButton()
    {
        GameManager.Current.ReloadCurrentScene();
    }
    public void GoBackTitleShootableButton()
    {
        GameManager.Current.LoadTitle();
    }
    public void LoadStageShootableButton(string stageName)
    {
        GameManager.Current.LoadScene(stageName);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

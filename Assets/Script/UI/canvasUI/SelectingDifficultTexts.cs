using UnityEngine;

public class SelectingDifficultTexts : MonoBehaviour
{
    public GameObject EasyTextObj;
    public GameObject NormalTextObj;
    public GameObject HardTextObj;

    void Start()
    {
        switch (GameManager.Current.CurrentDifficult)
        {
            case GameManager.Difficult.easy:
                EasyTextObj.SetActive(true);
                NormalTextObj.SetActive(false);
                HardTextObj.SetActive(false);
            break;
            case GameManager.Difficult.normal:
                EasyTextObj.SetActive(false);
                NormalTextObj.SetActive(true);
                HardTextObj.SetActive(false);
            break;
            case GameManager.Difficult.hard:
                EasyTextObj.SetActive(false);
                NormalTextObj.SetActive(false);
                HardTextObj.SetActive(true);
            break;
        }

        
    }

}

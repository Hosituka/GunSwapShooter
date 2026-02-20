using UnityEngine;
using TMPro;
using System.Collections;

public class AddScoreText : MonoBehaviour
{
    public float AddScore { private get; set; }
    [SerializeField]
    float AnimTime = 0.5f;
    TextMeshProUGUI _textMeshProUGUI;
    RectTransform _rectTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        _rectTransform = GetComponent<RectTransform>();
        _textMeshProUGUI.SetText("{0:1}＋",AddScore);
        StartCoroutine("FadeOut");
    }
    IEnumerator  FadeOut()
    {
        Color vertexColor = _textMeshProUGUI.color;
        for (float a = 1; a >= 0; a -= Time.deltaTime * (1/AnimTime))
        {
            vertexColor.a = a;
            _textMeshProUGUI.color = vertexColor;
            _rectTransform.position += Vector3.up * Time.deltaTime * 2 * 20;              
            yield return null;
        }
        vertexColor.a = 0;
        _textMeshProUGUI.color = vertexColor;
        Destroy(gameObject);
    }
}

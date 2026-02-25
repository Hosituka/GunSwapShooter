using TMPro;
using UnityEngine;
using AttitudeSensor = UnityEngine.InputSystem.AttitudeSensor;

public class DebugUI_manager : MonoBehaviour
{
    public static DebugUI_manager DebugUI_Manager;
    void Awake()
    {
        if (DebugUI_Manager == null)
        {
            DebugUI_Manager = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public TextMeshProUGUI InitialAttitudeSensorText;
    public TextMeshProUGUI AttitudeSensorValueText;
    public TextMeshProUGUI IsEnabledAttitudeSensor;
    public TextMeshProUGUI CameraDeviceLength;
    public TextMeshProUGUI IsDeviceVerticalText;
    public TextMeshProUGUI TestText;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        UpdateIsEnabledAttitudeSensor(AttitudeSensor.current?.enabled.ToString());
        UpdateIsDeviceVertical((Screen.height > Screen.width).ToString());
    }
    public void ChangeTestText(string text)
    {
        TestText.text = text;
    }
    public void UpdateInitialAttitudeValueText(string gyroValue)
    {
        InitialAttitudeSensorText.text = gyroValue;
    }
    public void UpdateAttitudeValueText(string value)
    {
        AttitudeSensorValueText.text = value;
    }
    public void UpdateIsEnabledAttitudeSensor(string value)
    {
        IsEnabledAttitudeSensor.text = value;
    }
    public void UpdateCameraDeviceLength(string value)
    {
        CameraDeviceLength.text = value + "：camera";
    }
    public void UpdateIsDeviceVertical(string value)
    {
        IsDeviceVerticalText.text = value + "：縦向き?";
    }
}

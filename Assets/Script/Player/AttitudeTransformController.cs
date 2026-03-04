using UnityEngine;



public class AttitudeTransformController : MonoBehaviour
{
    Quaternion _initialAttitudeValueOffset;
    Quaternion _currentAttitudeValue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Current.StopWebCam == true) return;
        _initialAttitudeValueOffset = GameManager.Current.InitialAttitudeValueOffset;
        _currentAttitudeValue = GameManager.Current.CurrentAttitudeValue;

        transform.rotation =  _initialAttitudeValueOffset * _currentAttitudeValue ;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
    }
}

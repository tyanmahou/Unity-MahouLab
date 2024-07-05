using Maho.UI.Common;
using UnityEngine;
using UnityEngine.UI;

public class BackGroundButtonTest : MonoBehaviour
{
    [SerializeField] BackgroundButton _bgButton1;
    [SerializeField] BackgroundButton _bgButton2;
    [SerializeField] Button _button1;
    [SerializeField] Button _button2;

    public void Awake()
    {
        _bgButton1.onClick.AddListener(() =>
        {
            Debug.Log("BackGround1");
        });
        _bgButton2.onClick.AddListener(() =>
        {
            Debug.Log("BackGround2");
        });
        _button1.onClick.AddListener(() =>
        {
            Debug.Log("Button1");
        });
        _button2.onClick.AddListener(() =>
        {
            Debug.Log("Button2");
        });
    }
}

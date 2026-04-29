using Maho.UI.Common;
using UnityEngine;
using UnityEngine.UI;

public class LightDismissTest : MonoBehaviour
{
    [SerializeField] InteractArea _interactArea;
    [SerializeField] Button _button1;
    [SerializeField] Button _button2;
    [SerializeField] Button _button3;

    public void Awake()
    {
        _button1.onClick.AddListener(() =>
        {
            Debug.Log("Button1");
        });
        _button2.onClick.AddListener(() =>
        {
            Debug.Log("Button2");
        });
        _button3.onClick.AddListener(() =>
        {
            _interactArea.gameObject.SetActive(true);
        });
        _interactArea.onClick.AddListener(() => {
            Debug.Log("Bubble");
            _interactArea.gameObject.SetActive(false);
        });
    }
}

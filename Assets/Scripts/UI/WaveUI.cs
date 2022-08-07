using UnityEngine.UI;
using UnityEngine;

public class WaveUI : MonoBehaviour
{
    Text waveText;


    private void Awake()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        waveText = GetComponentInChildren<Text>();
    }

    private void OnEnable()
    {
        waveText.text = "- WAVE " + EnemyManager.Instance.WaveNumber + " -";
    }

}

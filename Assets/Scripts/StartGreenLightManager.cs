using System;
using System.Collections;
using UnityEngine;

public class StartGreenLightManager : MonoBehaviour
{
    [SerializeField] private GameObject[] lightModels;
    [SerializeField] private float interval = 1f;

    public bool CanStart { get; private set; } = false;

    public event Action OnCountdownComplete;

    public IEnumerator StartCountdown()
    {
        CanStart = false;
        foreach (var light in lightModels)
        {
            SetLightColor(light, Color.red);
        }

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < lightModels.Length; i++)
        {
            SetLightColor(lightModels[i], Color.green);
            yield return new WaitForSeconds(interval);
        }

        CanStart = true;
        OnCountdownComplete?.Invoke();
    }

    private void SetLightColor(GameObject lightObj, Color color)
    {
        var renderer = lightObj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }
    }
}

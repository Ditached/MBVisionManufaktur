using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class ChipStateReactor : MonoBehaviour
{
    [HideInInspector] public UnityEvent<int> OnChipStateChanged;
    [ReadOnly] public int activeSensor;
    [ReadOnly] public bool multipleSensorsActive;

    private ChipState _chipState;
    private ushort _lastState;

    private void Awake()
    {
        _chipState = GetComponent<ChipState>();
        _lastState = _chipState.chipState;
        activeSensor = _chipState.GetFirstActiveSensor();
    }

    private void Update()
    {
        multipleSensorsActive = _chipState.AreMultipleSensorsActive();
        activeSensor = _chipState.GetFirstActiveSensor();

        if (_lastState != _chipState.chipState && !multipleSensorsActive)
        {
            OnChipStateChanged.Invoke(_chipState.GetFirstActiveSensor());
        }

        _lastState = _chipState.chipState;
    }
}
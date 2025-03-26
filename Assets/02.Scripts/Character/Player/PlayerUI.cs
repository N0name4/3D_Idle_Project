using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerUI : MonoBehaviour
{
    public Player player;

    [Header("UI 연결")]
    public Slider hpSlider;
    public Slider mpSlider;
    public Slider expSlider;
    public Text goldText;
    public Text floorText;

    private void Start()
    {
        player = GameManager.Instance.player;
    }

    private void Update()
    {
        UpdateHPUI();
        UpdateMpUI();
        UpdateExpUI();
        UpdateGoldUI();
        UpdateFloorUI();
    }

    void UpdateHPUI()
    {
        if (player.condition == null) return;

        hpSlider.maxValue = player.condition.maxHp;
        hpSlider.value = player.condition.currentHp;
    }

    void UpdateMpUI()
    {
        if (player== null) return;

        mpSlider.maxValue = player.maxMP;
        mpSlider.value = player.mp;
    }

    void UpdateExpUI()
    {
        if (player == null) return;

        expSlider.maxValue = player.maxExp;
        expSlider.value = player.exp;
    }

    void UpdateGoldUI()
    {
        goldText.text = $"GOLD: {player.gold}";
    }
    public void UpdateFloorUI()
    {
        floorText.text = $"B{GameManager.Instance.currentFloor}";
    }
}

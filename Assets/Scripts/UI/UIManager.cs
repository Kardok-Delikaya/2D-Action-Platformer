using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Bars")]
    [SerializeField] Slider healthSlider;
    [SerializeField] Slider manaSlider;
    [SerializeField] Slider ultimateSlider;

    [Header("Potion Icons")]
    [SerializeField] Sprite potionBottle;
    [SerializeField] Sprite emptyBottle;
    [SerializeField] Image[] potionIcons;

    public void HandleUIStart(int maxHealth, int maxMana, int maxUltimate, int maxPotion, int health, int mana, int ultimate, int potionCount)
    {
        healthSlider.maxValue = maxHealth;
        manaSlider.maxValue = maxMana;
        ultimateSlider.maxValue = maxUltimate;

        for (int i = 0; i < potionIcons.Length; i++) 
        {
            if (i < maxPotion)
            {
                potionIcons[i].enabled = true;
            }
            else
            {
                potionIcons[i].enabled = false;
            }
        }

        UpdateHealthSlider(health);
        UpdateManaSlider(mana);
        UpdateUltimateSlider(ultimate);
        UpdateBottleIcons(potionCount);
    }

    public void UpdateHealthSlider(int health)
    {
        healthSlider.value=health;
    }

    public void UpdateManaSlider(int mana)
    {
        manaSlider.value=mana;
    }

    public void UpdateUltimateSlider(int ultimate) 
    {
        ultimateSlider.value=ultimate;
    }

    public void UpdateBottleIcons(int bottleCount)
    {
        for (int i = 0; i < potionIcons.Length; i++) 
        {
            if (i < bottleCount) 
            {
                potionIcons[i].sprite = potionBottle;
            }
            else
            {
                potionIcons[i].sprite = emptyBottle;
            }
        }
    }
}

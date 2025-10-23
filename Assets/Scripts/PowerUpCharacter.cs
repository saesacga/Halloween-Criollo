using UnityEngine;
using UnityEngine.EventSystems;

public class PowerUpCharacter : Character
{
    protected override void ClickEffect()
    {
        EffectsManager.Instance.GiveEffectToPlayer(transform);
    }
}

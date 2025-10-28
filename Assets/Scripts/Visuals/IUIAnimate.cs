using UnityEngine;
using System;

public interface IUIAnimated
{
    event Action OnAnimationStart;
    event Action OnAnimationEnd;
}

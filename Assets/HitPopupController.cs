using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HitPopupController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI hitAmountText;

    public void SetHitAmount(int hitAmount)
    {
        hitAmountText.text = hitAmount.ToString();
    }

}

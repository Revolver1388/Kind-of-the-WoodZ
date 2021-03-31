using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HitPopupController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI hitAmountText;
    [SerializeField] GameObject hitpopup;

    private void Start()
    {
        hitpopup.SetActive(false);
    }

    public void SetHitAmount(int hitAmount/*,Transform objectTransform*/)
    {
        hitAmountText.text = hitAmount.ToString();

        //if (objectTransform.rotation.y == 180)
        //{
        //    //hitAmountText.transform.rotation = 
        //}

        StartCoroutine(HitPopupHelper());
    }

    private IEnumerator HitPopupHelper()
    {
        hitpopup.SetActive(true);
        yield return new WaitForSeconds(1);
        hitpopup.SetActive(false);

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HitPopupController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI hitAmountText;
    [SerializeField] GameObject hitpopup;
    [SerializeField] Vector3 _offset;
    private void Start()
    {
        hitpopup.SetActive(false);
        this.transform.parent = null;
    }
    private void Update()
    {
        transform.position = GameManager.Instance.GetPlayerTransform().position + _offset;
        if(transform.rotation.y != 0)
        {
            transform.rotation = new Quaternion(0, 0, 0, 0);
        }
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

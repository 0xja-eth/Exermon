using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Animator anim;
    private void Start()
    {
        anim = this.GetComponent<Animator>();
    }
    void Update()
    {
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        anim.SetBool("enter", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        anim.SetBool("enter", false);
    }
}

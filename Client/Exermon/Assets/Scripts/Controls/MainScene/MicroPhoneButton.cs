using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class MicroPhone : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    Image myImage;
    public Sprite frontImage;
    public Sprite backImage;

    // Start is called before the first frame update
    void Start()
    {
        myImage = this.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        myImage.sprite = frontImage;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        myImage.sprite = backImage;
    }
}

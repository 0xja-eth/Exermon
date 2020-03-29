using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ChatAnimation : MonoBehaviour
{
    public Button enterButton;
    public Button leaveButton;
    public Animator anim;
    public GameObject obj;
    // Start is called before the first frame update
    void Start()
    {
        leaveButton.onClick.AddListener(this.leaveChatRoom);
        enterButton.onClick.AddListener(this.enterChatRoom);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void leaveChatRoom()
    {
        anim.SetTrigger("Leave");
        obj.SetActive(false);
    }

    void enterChatRoom()
    {
        obj.SetActive(true);
    }
}

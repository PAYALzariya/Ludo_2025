using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmojiData : MonoBehaviour
{
    public int emojiId;
    public Image selectimg;
    private float destroyTimeInterval = 4;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {        
        Invoke("DestroyEmoji", destroyTimeInterval);
    }

    private void DestroyEmoji()
    {
        Destroy(this.gameObject);
    }
}

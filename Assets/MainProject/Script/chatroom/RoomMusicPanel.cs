using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class RoomMusicPanel : MonoBehaviour
{
    public RoomMusicListItem roomMusicListItemPrefb;
    public Transform myMusicListTransform;
    public Transform musicListTransform;
    public List<RoomMusicListItem> mymusicList = new List<RoomMusicListItem>();  
    public List<RoomMusicListItem> musicList = new List<RoomMusicListItem>();
    public Transform allMusicPanel;
    public Transform myMusicPanel;
    public Sprite addMusicSprite;
    public Sprite addedMusicSprite;
    public Sprite playMusicSprite;
    private void OnEnable()
    {
       
        DisplayAllMusicList();
    }
    internal void CloneMusicList()
    {


        Debug.Log("Cloning music list::"+MusicManager.Instance.AllFoundMusic.Count);
        if (musicList.Count != MusicManager.Instance.AllFoundMusic.Count)
        {
            foreach (var item in musicList)
            {
                Destroy(item.gameObject);
            }
            musicList.Clear();
            
                
                foreach (var track in MusicManager.Instance.AllFoundMusic)
                {



                RoomMusicListItem cloneItem = Instantiate(roomMusicListItemPrefb);
                cloneItem.transform.SetParent(musicListTransform);
                musicList.Add(cloneItem);
                cloneItem.musicNameText.text = track.Title;
                cloneItem.isMyMusic = track.ISMyMusic;
                cloneItem.transform.localScale = Vector3.one;
                cloneItem.transform.position = musicListTransform.position;
                if (track.ISMyMusic)
                {
                    cloneItem.button.image.sprite = addedMusicSprite;
                    cloneItem.button.interactable = false;
                }
                else
                {

                cloneItem.button.image.sprite = addMusicSprite;
                cloneItem.button.onClick.AddListener(() =>
                    {
                        AddMusicButtonClick(cloneItem, track).Forget();
                    });

                }
            }

            

        }
        
    

        
       
    }
    internal async UniTask AddMusicButtonClick(RoomMusicListItem selectedItem, MusicTrack track)
    {

        if (!selectedItem.audioClip)
        {
          
          await AddMusic_OnMyMusic(selectedItem,track);
            if (mymusicList.Count == 0)
            {

                Destroy(myMusicListTransform.GetChild(0).gameObject);
            }
            CloneMyMusicListItem(selectedItem, track);
        }

    }
    internal async UniTask AddMusic_OnMyMusic(RoomMusicListItem item, MusicTrack track)
    {
        item.audioClip = await MusicManager.Instance.LoadAndGetAudioClipAsync(track);

        item.button.image.sprite = addedMusicSprite;
        item.button.interactable = false;
        item.isMyMusic = true;
        track.ISMyMusic = true;




    }
    void CloneMyMusicList()
    {
        if (mymusicList.Count != MusicManager.Instance.MyMusicListRepsonse.MyMusic.Count)
        {
            foreach (var item in mymusicList)
            {
                Destroy(item.gameObject);
            }
            mymusicList.Clear();
            if (mymusicList.Count == 0&& MusicManager.Instance.MyMusicListRepsonse.MyMusic.Count == 0)
            {

                Destroy(myMusicListTransform.GetChild(0).gameObject);
            }
            
            else
            {
                /*foreach (var track in MusicManager.Instance.MyMusicListRepsonse.MyMusic)
                {
                    CloneMyMusicListItem(roomMusicListItemPrefb, track);*/
                    for (int i = 0; i < MusicManager.Instance.MyMusicListRepsonse.MyMusic.Count; i++)
                    {

                Debug.Log("Cloning my music list::" + MusicManager.Instance.MyMusicListRepsonse.MyMusic[i]);
                    CloneMyMusicListItem(roomMusicListItemPrefb, MusicManager.Instance.MyMusicListRepsonse.MyMusic[i]);
                    }

                    
                
            }
        }
    }
  void  CloneMyMusicListItem(RoomMusicListItem PrefabItem, MusicTrack track)
    {



        RoomMusicListItem cloneItem = Instantiate(PrefabItem);
        cloneItem.transform.SetParent(myMusicListTransform);
        cloneItem.transform.localScale = Vector3.one;
        cloneItem.transform.position = myMusicListTransform.position;
        
        AddMusic_OnMyMusic(cloneItem,track).Forget();
        cloneItem.button.onClick.AddListener(() =>
        {
            OnPlayMusicButtonClick(cloneItem);
        });
        cloneItem.button.image.sprite = playMusicSprite;
        cloneItem.musicNameText.text= track.Title;
        cloneItem.button.interactable = true;
        mymusicList.Add(cloneItem);
        track.ISMyMusic = true;
        MusicManager.Instance.MyMusicListRepsonse.MyMusic.Add(track);
        MusicManager.Instance.SaveMyMusicListToJson(track);


    }

    internal void OnPlayMusicButtonClick(RoomMusicListItem selectedItem)
    {
        MusicManager.Instance.audioSource.clip = selectedItem.audioClip;
        MusicManager.Instance.audioSource.Play();
    }

    
     void DisplayAllMusicList()
    {


        // Create a new UI Text for each song in our final list

        myMusicPanel.gameObject.SetActive(true);
        allMusicPanel.gameObject.SetActive(false);
        CloneMusicList();
        if (MusicManager.Instance.MyMusicListRepsonse.MyMusic.Count ==0)
        {
            // Show a "not found" message if the list is empty
            if( myMusicListTransform.childCount > 0)
            {
                return;
            }
            RoomMusicListItem cloneItem = Instantiate(roomMusicListItemPrefb, myMusicListTransform);
            cloneItem.musicNameText.text = "No music files were found add yours.";
            cloneItem.name = "Remove";
            cloneItem.button.gameObject.SetActive(false);

            return;
        }
        else
        {
            

            

            CloneMyMusicList();

        }
        




    }

    public void OnAddMusicButtonClick()
    {
        allMusicPanel.gameObject.SetActive(true);
        allMusicPanel.SetAsLastSibling();
     
    }
}

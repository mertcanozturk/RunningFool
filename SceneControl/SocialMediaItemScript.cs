using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SocialMediaItemScript : MonoBehaviour
{
    public string SocialMediaName;
    Button ButtonSocialMedia;

    void Start()
    {
        ButtonSocialMedia = GetComponent<Button>();
        ButtonSocialMedia.onClick.AddListener(OpenSocialMedia);
    }

    void OpenSocialMedia()
    {
        var item = InitializerManager.instance.Data.FollowSystem.FollowList.FirstOrDefault(x => x.Name == SocialMediaName);
        if(item != null)
        {
            Application.OpenURL(item.URL);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioLibrary : MonoBehaviour
{
    [Serializable]
    public class Item
    {
        public SoundName name;
        public AudioClip clip;
    }

    public List<Item> items = new List<Item>();

    public AudioLibrary.Item GetItem(SoundName soundName)
    {
        var item = items.FirstOrDefault(i => i.name == soundName);
        if (item == null)
        {
            Debug.LogWarningFormat("Audio item not found: {0}", soundName);
        }
        return item;
    }
}

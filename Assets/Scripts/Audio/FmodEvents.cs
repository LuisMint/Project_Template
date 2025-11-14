using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

[System.Serializable]
public class SoundEntry
{
    public string soundName; // Custom name for the sound
    public EventReference eventReference; // FMOD EventReference for the sound
}

public class FmodEvents : MonoBehaviour
{
    [Header("Custom FMOD Sounds")]
    [SerializeField] private List<SoundEntry> soundEntries = new();

    private Dictionary<string, EventReference> soundDictionary;

    public static FmodEvents Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one FmodEvents instance found!");
            Destroy(this);
            return;
        }
        Instance = this;

        // Initialize the dictionary for quick lookup
        soundDictionary = new Dictionary<string, EventReference>();
        foreach (var sound in soundEntries)
        {
            if (!soundDictionary.ContainsKey(sound.soundName))
            {
                soundDictionary.Add(sound.soundName, sound.eventReference);
            }
            else
            {
                Debug.LogWarning($"Duplicate sound name found: {sound.soundName}. Only the first occurrence will be used.");
            }
        }
    }

    /// <summary>
    /// Retrieves the EventReference for a given sound name.
    /// Logs an error if the sound name is not found.
    /// </summary>
    public EventReference GetSound(string soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out var eventReference))
        {
            return eventReference;
        }
        else
        {
            Debug.LogError($"Sound '{soundName}' not found in FmodEvents.");
            return default;
        }
    }


  
}

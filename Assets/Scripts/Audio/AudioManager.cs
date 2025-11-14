using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{

    [Header("Volume")]

    [Range(0,1)]
    public float masterVolume = 1f;
    [Range(0, 1)]
    public float musicVolume = 1f;
    [Range(0, 1)]
    public float sfxVolume = 1f;

    private Bus masterBus;
    private Bus musicBus;
    private Bus sfxBus;

    //public FloatVariable SoMasterVolume;

    //public FloatVariable SoMorseVolume;

    private void Start()
    {
        Cursor.visible = false;
        //StartCoroutine(waitinteract());
    }
    private bool interactionReceived = false;
    private IEnumerator waitinteract()
    {
        while (!interactionReceived)
        {
            if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.touchCount > 0)
            {
                interactionReceived = true;
            }

            yield return null; // Wait for the next frame
        }
        InitializeBgMusic();
    }




    public static AudioManager instance { get; private set; }

    private void Update()//TEMP
    {
        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
    }




    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;

    public void UpdateMasterVolume(float volume) => masterBus.setVolume(volume);
    public void UpdateMusicVolume(float volume) => musicBus.setVolume(volume);
    public void UpdateSFXVolume(float volume) => sfxBus.setVolume(volume);

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("more than one audio manager");
        }
        instance = this;

        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();

        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");

        //masterVolume= SoMasterVolume.value;
        //musicVolume = SoMorseVolume.value;

    }

    private EventInstance ambienceEventInstance;
    private EventInstance musicEventInstance;
    //CALL AMBIENCE SOUND AND MUSIC (MAYBE TEMP)
    private void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = CreateEventInstance(musicEventReference);
        musicEventInstance.start();
    }
     
    public void SetMusicParameter(string parameterName, float parameterValue)
    {
        musicEventInstance.setParameterByName(parameterName, parameterValue);
    }
    private void InitializeAmbience(EventReference ambienceEventReference)
    {
        ambienceEventInstance = CreateEventInstance(ambienceEventReference);
        ambienceEventInstance.start();
    }
    public void SetAmbienceParameter(string parameterName, float parameterValue)
    {
        ambienceEventInstance.setParameterByName(parameterName, parameterValue);
    }    
    public void InitializeBgMusic()
    {
        InitializeAmbience(FmodEvents.Instance.GetSound("Music1"));
    }

    //OVERIDE FMOD EMITTER ON OBJECT (WITH EMITTER COMPONENT)
    public StudioEventEmitter InitializeEventEmitter(EventReference eventRefrence, GameObject emitterGameObject)
    {
        StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventRefrence;
        eventEmitters.Add(emitter);
        return emitter;
    }

    //PLAY ONE SHOT AUDIO


    public void PlayOneShotString(string sound,Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(FmodEvents.Instance.GetSound(sound), worldPos);
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);

    }
    
    //CREATE EVENT INSTANCE (PLAYS SOURCE OF AUDIO)
    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }







    private void CleanUp()
    {
        foreach(EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
        foreach(StudioEventEmitter emitter in eventEmitters)
        {
            emitter.Stop();
        }

    }

    private void OnDestroy()
    {
        CleanUp();
    }

}

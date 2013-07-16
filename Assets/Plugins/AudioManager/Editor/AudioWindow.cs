using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

public class AudioWindow : EditorWindow
{
    private Vector2 scrollPos;

    private AudioItem itemToRemove;

    private static AudioWindow window;

    private AudioManager audioManagerInScene;

    private AudioManager audioManagerPrefab;

    private int selectedAudioIndex;

    private bool isInEditor;

    private AudioManager AudioManagerPrefab
    {
        get
        {
            if (audioManagerPrefab == null)
            {
                // Load asset
                audioManagerPrefab =
                    (Resources.LoadAssetAtPath("Assets/Plugins/AudioManager/AudioManager.prefab", typeof (GameObject))
                     as GameObject).GetComponent<AudioManager>();
            }

            return audioManagerPrefab;
        }
    }


    [MenuItem("Window/Audio Manager")]
	static void Init ()
    {
        // Get existing open window or if none, make a new one:
        window = (AudioWindow)GetWindow(typeof(AudioWindow));

        // Set window title
        window.title = "Audio Manager";
	}

    private void OnGameStart()
    {
        Repaint();
    }

    private void OnGameStop()
    {
        Repaint();
    }

    private void Update()
    {

        if (isInEditor && (EditorApplication.isPlaying || EditorApplication.isPaused))
        {
            OnGameStart();
        }
        else if (!isInEditor && !EditorApplication.isPaused && !EditorApplication.isPlaying)
        {
            OnGameStop();
        }
        
        // Check if there is an audio manager in the scene
        if (audioManagerInScene == null)
        {
            // Check if amount of audio managers in the scene is correct
            Object[] audioManagers = FindSceneObjectsOfType(typeof(AudioManager));

            if (audioManagers.Length == 0)
            {
                // Instantiate an audiomanager in the scene
                audioManagerInScene = (PrefabUtility.InstantiatePrefab(AudioManagerPrefab.gameObject) as GameObject).GetComponent<AudioManager>();
            }
            else if (audioManagers.Length >= 1)
            {
                for (int i = audioManagers.Length - 1; i >= 0; i--)
                {
                    if (i == 0)
                    {
                        // Assign to found audio manager
                        audioManagerInScene = (audioManagers[i] as AudioManager);
                    }
                    else
                    {
                        // Remove potential extra audio managers
                        DestroyImmediate((audioManagers[i] as AudioManager).gameObject);
                    }
                }
            }
        }
        else
        {
            PrefabUtility.ResetToPrefabState(audioManagerInScene);
            //ApplySettingsToAudioManagerInstance();
        }

        isInEditor = !EditorApplication.isPaused && !EditorApplication.isPlaying;
    }

    private void OnGUI()
    {
        if (audioManagerInScene == null)
        {
            return;
        }

        if (EditorApplication.isPlaying || EditorApplication.isPaused)
        {
            GUILayout.Label("Editing in play mode is not supported :-(");

            return;
        }
        
        DrawSeperator("Global Settings");

        // Master volume
        GUILayout.BeginHorizontal();
        
        AudioManagerPrefab.Settings.MasterVolume = 0.01f * EditorGUILayout.Slider("Master", (int)(AudioManagerPrefab.Settings.MasterVolume * 100), 0, 100);
        GUILayout.Label("%");
        
        GUILayout.EndHorizontal();

        // Music volume
        GUILayout.BeginHorizontal();
        AudioManagerPrefab.Settings.MusicVolume = 0.01f * EditorGUILayout.Slider("Music", (int)(AudioManagerPrefab.Settings.MusicVolume * 100), 0, 100);
        GUILayout.Label("%");
        GUILayout.EndHorizontal();

        // Sfx volume
        GUILayout.BeginHorizontal();
        AudioManagerPrefab.Settings.SoundEffectsVolume = 0.01f * EditorGUILayout.Slider("SFX", (int)(AudioManagerPrefab.Settings.SoundEffectsVolume * 100), 0, 100);
        GUILayout.Label("%");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        if(GUILayout.Button(AudioManagerPrefab.Settings.IsAllMuted ? "Unmute All" : "Mute All"))
        {
            if (AudioManagerPrefab.IsAllMuted)
            {
                AudioManagerPrefab.UnMuteAll();
            }
            else
            {
                AudioManagerPrefab.MuteAll();
            }
        }

        if(GUILayout.Button(AudioManagerPrefab.Settings.IsMusicMuted ? "Unmute Music" : "Mute Music"))
        {
            if (AudioManagerPrefab.IsMusicMuted)
            {
                AudioManagerPrefab.UnMuteAllMusic();
            }
            else
            {
                AudioManagerPrefab.MuteAllMusic();
            }
        }

        if (GUILayout.Button(AudioManagerPrefab.Settings.IsSoundEffectsMuted ? "Unmute SFX" : "Mute SFX"))
        {
            if (AudioManagerPrefab.IsSoundEffectsMuted)
            {
                AudioManagerPrefab.UnMuteAllSoundEffects();
            }
            else
            {
                AudioManagerPrefab.MuteAllSoundEffects();
            }
        }

        GUILayout.EndHorizontal();

        DrawSeperator("Audio Files");

        GUILayout.BeginHorizontal();

        if (AudioManagerPrefab.AudioItems.Length > 0)
        {
            selectedAudioIndex = EditorGUILayout.Popup(selectedAudioIndex,
                                                       AudioManagerPrefab.AudioItems.Select(a => a.Name).ToArray());
        }
        else
        {
            EditorGUILayout.Popup(0, new string[]{"No audio files added"});
        }

        if (GUILayout.Button("Add Selected files"))
        {
            AddSelectedItems();
        }

        GUILayout.EndHorizontal();

        if (AudioManagerPrefab.AudioItems.Length > 0)
        {
            DrawAudioItemGui(AudioManagerPrefab.AudioItems[selectedAudioIndex]);
        }

        if (GUI.changed)
        { 
            EditorUtility.SetDirty(AudioManagerPrefab.gameObject);

            Repaint();
        }
    }

    private void ApplySettingsToAudioManagerInstance()
    {
        // Clone audio items and settings from prefab to scene instance
        audioManagerInScene.AudioItems = audioManagerPrefab.AudioItems;
        audioManagerInScene.Settings = audioManagerPrefab.Settings;
    }

    private void DrawAudioItemGui(AudioItem audioItem)
    {
        GUI.changed = false;

        bool isPlaying = false;
        
        // Cache the old color
        Color oldColor = GUI.color;

        EditorGUILayout.BeginHorizontal();

        audioItem.Type = (AudioItem.SoundType)System.Convert.ToInt32(EditorGUILayout.EnumPopup(audioItem.Type));

        // If the sound is playing, allow us to stop it
        if (audioManagerInScene.IsAudioItemPlaying(audioItem))
        {
            isPlaying = true;
            
            if (GUILayout.Button("Stop"))
            {
                audioManagerInScene.StopAudioItem(audioItem);

                isPlaying = false;
            }
        }
        else if (GUILayout.Button("Play"))
        {
            // Add any potential leaked audio sources
            audioManagerInScene.AddLeakedAudioSources();
            
            audioManagerInScene.PlaySound(audioItem);
        }

        // Make sure this audio item is not currently being removed
        if (audioItem != itemToRemove)
        {
            // Change color to red
            GUI.color = new Color(1, 0.3f, 0.3f);

            if (GUILayout.Button("Remove"))
            {
                itemToRemove = audioItem;
            }

            // Reset color
            GUI.color = oldColor;
        }
        else
        {
            // Don't remove if nope is pressed
            if (GUILayout.Button("Nope"))
            {
                itemToRemove = null;
            }

            // Change color to red
            GUI.color = new Color(1, 0.3f, 0.3f);

            // Remove the item if ok is pressed
            if (GUILayout.Button("Ok"))
            {
                RemoveAudioItem(audioItem);
            }

            // Reset color
            GUI.color = oldColor;
        }

        // End the top horizontal layout for this item
        EditorGUILayout.EndHorizontal();

        // Clip object field
        audioItem.Clips[0] = EditorGUILayout.ObjectField("Clip", audioItem.Clips[0], typeof(AudioClip), false) as AudioClip;

        // Volume slider
        EditorGUILayout.BeginHorizontal();
        audioItem.Volume = 0.01f * EditorGUILayout.Slider("Volume", (int)(audioItem.Volume * 100), 0, 100);
        GUILayout.Label("%");
        EditorGUILayout.EndHorizontal();

        // Random Volume slider
        EditorGUILayout.BeginHorizontal();
        audioItem.RandomVolume = 0.01f * EditorGUILayout.Slider("Random Volume", (int)(audioItem.RandomVolume * 100), 0, 100);
        GUILayout.Label("%±");
        EditorGUILayout.EndHorizontal();

        // Random Pitch slider
        EditorGUILayout.BeginHorizontal();
        audioItem.RandomPitch = EditorGUILayout.Slider("Random Pitch", audioItem.RandomPitch, 0, 1);
        GUILayout.Label("±");
        EditorGUILayout.EndHorizontal();

        // Pitch slider
        audioItem.Pitch = EditorGUILayout.Slider("Pitch", audioItem.Pitch, 0, 3);

        // Pan 2D slider
        audioItem.Pan2D = EditorGUILayout.Slider("Pan 2D", audioItem.Pan2D, -1, 1);

        // Looping toggle
        audioItem.Loop = EditorGUILayout.Toggle("Looping", audioItem.Loop);

        // Play on awake toggle
        audioItem.PlayOnAwake = EditorGUILayout.Toggle("Play on Awake", audioItem.PlayOnAwake);

        // Don't destroy on load toggle
        audioItem.DontDestroyOnLoad = EditorGUILayout.Toggle("Don't destroy on load", audioItem.DontDestroyOnLoad);

        EditorGUILayout.BeginHorizontal();

        // Sync settings
        audioItem.SyncWithOtherAudioClip = EditorGUILayout.Toggle("Sync settings from", audioItem.SyncWithOtherAudioClip);

        if (audioItem.SyncWithOtherAudioClip)
        {
            // Check if sync source is still available, otherwise don't sync
            if (audioItem.NameOfSyncSource != string.Empty &&
                !AudioManagerPrefab.AudioItems.Select(a => a.Name).Contains(audioItem.NameOfSyncSource))
            {
                audioItem.NameOfSyncSource = string.Empty;

                audioItem.SyncWithOtherAudioClip = false;
            }
            else
            {
                int selectionIndex = 0;
                
                for (int i = 0; i < AudioManagerPrefab.AudioItems.Length; i++)
                {
                    // Find index for sync source
                    if (AudioManagerPrefab.AudioItems[i].Name == audioItem.NameOfSyncSource)
                    {
                        selectionIndex = i;
                        break;
                    }
                }
                
                // Make sync popup
                audioItem.NameOfSyncSource = AudioManagerPrefab.AudioItems[EditorGUILayout.Popup(selectionIndex, AudioManagerPrefab.AudioItems.Select(a => a.Name).ToArray())].Name;
            }
        }

        EditorGUILayout.EndHorizontal();

        // Is collection toggle
        audioItem.IsCollection = EditorGUILayout.Toggle("Is A Collection", audioItem.IsCollection);

        if (audioItem.IsCollection)
        {
            EditorGUILayout.BeginHorizontal();
            
            // Play mode popup
            audioItem.Mode = (AudioItem.PlayMode)System.Convert.ToInt32(EditorGUILayout.EnumPopup(audioItem.Mode));
        
            if (GUILayout.Button("Add selected files"))
            {
                AddSelectedItemsToCollection();
            }

            EditorGUILayout.EndHorizontal();

            int? itemIndexToRemove = null;

            for (int i = 1; i < audioItem.Clips.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();

                // Change color to red
                GUI.color = new Color(1, 0.3f, 0.3f);

                if (GUILayout.Button("x", EditorStyles.miniButton, GUILayout.MaxWidth(20)))
                {
                    itemIndexToRemove = i;
                }

                // Reset color
                GUI.color = oldColor;
                
                audioItem.Clips[i] = EditorGUILayout.ObjectField(audioItem.Clips[i], typeof(AudioClip), false) as AudioClip;

                EditorGUILayout.EndHorizontal();
            }

            if (itemIndexToRemove != null)
            {
                RemoveClipFromCollection((int)itemIndexToRemove);
            }
        }

#if UNITY_WEBPLAYER
        GUILayout.Box("Note: Webplayer platform doesn't generate static play methods, instead you have to play the audio files by name. For Example: \nAudioManager.PlaySound(\"" + audioItem.Name + "\");");
#endif

        if (GUI.changed)
        {
            // Sync other audio items, that might be synced to this one
            foreach (AudioItem otherAudioItem in AudioManagerPrefab.AudioItems)
            {
                if (otherAudioItem.SyncWithOtherAudioClip && otherAudioItem.NameOfSyncSource == audioItem.Name)
                {
                    // Sync misc settings
                    otherAudioItem.Loop = audioItem.Loop;
                    otherAudioItem.Pan2D = audioItem.Pan2D;
                    otherAudioItem.Pitch = audioItem.Pitch;
                    otherAudioItem.Volume = audioItem.Volume;
                    otherAudioItem.PlayOnAwake = audioItem.PlayOnAwake;
                    otherAudioItem.RandomPitch = audioItem.RandomPitch;
                    otherAudioItem.RandomVolume = audioItem.RandomVolume;
                }
            }

            if (isPlaying)
            {
                audioManagerInScene.UpdateAudioSourcesWithNewSettings(audioItem);
            }
        }
    }

    private void DrawSeperator(string text)
    {
        // Cache the old color
        Color oldColor = GUI.color;
        
        GUI.color = new Color(1f, 1f, 1);

        //Rect seperator = GUILayoutUtility.GetRect(0, 20, GUILayout.ExpandWidth(true));
        //GUI.Box(seperator, text);
        EditorGUILayout.LabelField("- " + text, EditorStyles.largeLabel);

        // Reset color
        GUI.color = oldColor;
    }


    /// <summary>
    /// This is automaticly called when the window loses focus.
    /// </summary>
    private void OnLostFocus()
    {
        // Cancel removing an item when window loses focus
        itemToRemove = null;

        // Repaint the window
        Repaint();
    }

    /// <summary>
    /// This method gets called for each audio file you drop in the drop area.
    /// </summary>
    /// <param name="filePath">
    /// The path of the file dropped.
    /// </param>
    private bool AddAudioFile(AudioClip audioClip)
    {
        // Make sure the file isn't already added
        if (audioManagerPrefab.AudioItems.Select(item => item.Clips[0]).Contains(audioClip))
        {
            // Return if the file is already in the list
            return false;
        }

        // Add an audio item to the array
        List<AudioItem> audioItems = audioManagerPrefab.AudioItems.ToList();

        AudioClip[] clip = {audioClip};

        AudioItem newAudiItem = new AudioItem
            {
                Clips = clip,
                Volume = 1,
                Name = audioClip.name,
                Type = audioClip.length > 8 ? AudioItem.SoundType.Music : AudioItem.SoundType.SoundEffect
            };

        audioItems.Add(newAudiItem);
        audioManagerPrefab.AudioItems = audioItems.ToArray();

        Debug.Log("Added " + audioClip.name);

        return true;
    }

    /// <summary>
    /// Removes an audio item and deletes its save data.
    /// </summary>
    /// <param name="item">
    /// The item to delete.
    /// </param>
    public void RemoveAudioItem(AudioItem item)
    {
        // Stop any sources with this sound from playing
        audioManagerInScene.StopAudioItem(item);

        foreach (AudioClip clip in item.Clips)
        {
            audioManagerInScene.RemoveAllAudioSourcesWithClip(clip);
        }

        // Remove an audio item from the array
        List<AudioItem> audioItems = audioManagerPrefab.AudioItems.ToList();
        audioItems.Remove(item);
        audioManagerPrefab.AudioItems = audioItems.ToArray();

        selectedAudioIndex = 0;

        ApplyChanges();
    }

    /// <summary>
    /// Apply all changes made, and generate the required code.
    /// </summary>
    private void ApplyChanges()
    {
        // Generate partial AudioManager class
        GenerateCode();

        EditorUtility.SetDirty(AudioManagerPrefab.gameObject);

        // Repaint
        Repaint();
    }

    /// <summary>
    /// Creates the generated AudioManagerGenerated.cs file
    /// </summary>
    private void GenerateCode()
    {

#if UNITY_WEBPLAYER
#else
        // Generate AudioManager methods
        using (TextWriter writer = File.CreateText(@"Assets\Plugins\AudioManager\AudioManagerGenerated.cs"))
        {
            writer.WriteLine(@"// THIS FILE IS AUTO GENERATED, DO NOT MODIFY!");
            writer.WriteLine(@"#region Auto generated code");
            writer.WriteLine(@"public partial class AudioManager");
            writer.WriteLine(@"{");

            for (int i = 0; i < AudioManagerPrefab.AudioItems.Length; i++)
            {
                string methodSuffix = AudioManagerPrefab.AudioItems[i].Name;

                string methodSuffixTrimmed = string.Empty;

                // Make sure we are not using any illegal characters, if so replace them with an _
                foreach (char c in methodSuffix)
                {
                    if ((c >= 48 && c <= 57) || (c >= 65 && c <= 90) || (c >= 97 && c <= 122))
                    {
                        methodSuffixTrimmed += c;
                    }
                    else
                    {
                        methodSuffixTrimmed += '_';
                    }
                }
                
                // PLay sound method
                writer.WriteLine(@"    public static void Play{0}()", methodSuffixTrimmed);
                writer.WriteLine(@"    {");
                writer.WriteLine(@"        PlaySound({0}, null);", i);
                writer.WriteLine(@"    }");

                // PLay sound method
                writer.WriteLine(@"    public static void Play{0}(float volume)", methodSuffixTrimmed);
                writer.WriteLine(@"    {");
                writer.WriteLine(@"        PlaySound({0}, volume);", i);
                writer.WriteLine(@"    }");

                // Stop sound method
                writer.WriteLine(@"    public static void Stop{0}()", methodSuffixTrimmed);
                writer.WriteLine(@"    {");
                writer.WriteLine(@"        StopSound({0});", i);
                writer.WriteLine(@"    }");
            }

            writer.WriteLine(@"}");
            writer.WriteLine(@"#endregion");
        }

        // Reimport generated file
        AssetDatabase.ImportAsset(@"Assets\Plugins\AudioManager\AudioManagerGenerated.cs", ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive);

#endif
    }

    private void RemoveClipFromCollection(int itemIndex)
    {
        AudioItem selectedAudioItem = AudioManagerPrefab.AudioItems[selectedAudioIndex];
        
        List<AudioClip> clips = new List<AudioClip>(AudioManagerPrefab.AudioItems[selectedAudioIndex].Clips);

        clips.RemoveAt(itemIndex);

        selectedAudioItem.Clips = clips.ToArray();
    }

    private void AddSelectedItemsToCollection()
    {
        // Make sure that at least one audio item is added
        if (AudioManagerPrefab.AudioItems.Length == 0)
        {
            return;  
        }
        
        AudioItem selectedAudioItem = AudioManagerPrefab.AudioItems[selectedAudioIndex];

        List<AudioClip> clipsToAdd = new List<AudioClip>(selectedAudioItem.Clips);

        foreach (Object obj in Selection.objects)
        {
            if (AssetDatabase.Contains(obj) && obj is AudioClip && !clipsToAdd.Contains(obj as AudioClip))
            {
                clipsToAdd.Add(obj as AudioClip);
            }
        }

        // Apply new collection
        selectedAudioItem.Clips = clipsToAdd.ToArray();

        // Set dirty and repaint
        EditorUtility.SetDirty(AudioManagerPrefab.gameObject);
        Repaint();
    }

    private void AddSelectedItems()
    {
        bool didAddAudio = false;
        
        foreach (Object obj in Selection.objects)
        {
            if (AssetDatabase.Contains(obj) && obj is AudioClip)
            {
                didAddAudio = AddAudioFile(obj as AudioClip) || didAddAudio;
            }
        }

        if (didAddAudio)
        {
            ApplyChanges();
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>Game</c> is a Unity script used to manage the global game behavior.
/// </summary>
public class Game : MonoBehaviour
{
    #region Fields / Properties

    /// <summary>
    /// Instance variable <c>prefab</c> is a Unity <c>Transform</c> component representing the position, rotation and scale of the prefabricated object to spawn in the game.
    /// </summary>
    public Transform prefab;

    /// <summary>
    /// Instance variable <c>createKey</c> is a Unity <c>Keycode</c> enumeration instance representing the key to press to spawn an object in the game.
    /// </summary>
    public KeyCode createKey = KeyCode.C;

    /// <summary>
    /// Instance variable <c>newGameKey</c> is a Unity <c>Keycode</c> enumeration instance representing the key to press to start a new game.
    /// </summary>    
    public KeyCode newGameKey = KeyCode.N;

    /// <summary>
    /// Instance variable <c>objects</c> is a list of Unity <c>Transform</c> components representing the prefabritcated object instances of the game.
    /// </summary>   
    private List<Transform> objects;

    #endregion

    #region MonoBehavior

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        objects = new List<Transform>();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        // KeyDown is true only during the first frame of the input going from not-pressed to press.
        if (Input.GetKeyDown(createKey)) {
            CreateObject();
        } 
        else if (Input.GetKey(newGameKey)) {
            BeginNewGame();
        }
    }

    #endregion

    #region Private

    /// <summary>
    /// This function is responsible to create one instance of the game prefab object.
    /// </summary>
    private void CreateObject() {
        Transform prefabTransform = Instantiate(prefab);
        // Randomize position, rotation and scale of the instantiated prefab object.
        // Use "insideUnitSphere" to get a random point in a sphere of 1 unit of distance from the world origin.
        prefabTransform.localPosition = Random.insideUnitSphere * 5f;
        prefabTransform.localRotation = Random.rotation;
        // Vary size but keep it uniformly-scaled.
        prefabTransform.localScale = Vector3.one * Random.Range(0.1f, 1f);
        objects.Add(prefabTransform);
    }

    /// <summary>
    /// This function is responsible to begin a new game.
    /// </summary>
    private void BeginNewGame() {
        for (int i = 0; i < objects.Count; i++) {
            Destroy(objects[i].gameObject);
        }
        objects.Clear();
    }

    #endregion
}
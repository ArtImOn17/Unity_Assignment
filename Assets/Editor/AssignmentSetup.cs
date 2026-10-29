using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

[InitializeOnLoad]
public static class AssignmentSetup
{
    static AssignmentSetup()
    {
        if (!File.Exists("Assets/Scenes/Platformer.unity")) EditorApplication.delayCall += Build;
    }
    [MenuItem("Assignment/Recreate scene")]
    public static void Build()
    {
        Directory.CreateDirectory("Assets/Input");
        Directory.CreateDirectory("Assets/Sprites");
        Directory.CreateDirectory("Assets/Scenes");
        var input = ScriptableObject.CreateInstance<InputActionAsset>();
        var map = input.AddActionMap("Move");
        map.AddAction("Jump", InputActionType.Button, "<Keyboard>/space");
        var move = map.AddAction("Move", InputActionType.Value, expectedControlLayout: "Vector2");
        move.AddCompositeBinding("2DVector").With("Up", "<Keyboard>/w").With("Down", "<Keyboard>/s").With("Left", "<Keyboard>/a").With("Right", "<Keyboard>/d");
        File.WriteAllText("Assets/Input/PlatformerInput.inputactions", input.ToJson());
        var texture = new Texture2D(32,32);
        var pixels = new Color[1024];
        for(int i=0;i<pixels.Length;i++) pixels[i]=Color.white;
        texture.SetPixels(pixels); texture.Apply();
        File.WriteAllBytes("Assets/Sprites/Square.png", texture.EncodeToPNG());
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        AssetDatabase.ImportAsset("Assets/Input/PlatformerInput.inputactions", ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
        var importer = (TextureImporter)AssetImporter.GetAtPath("Assets/Sprites/Square.png");
        importer.textureType = TextureImporterType.Sprite;
        importer.spritePixelsPerUnit = 32;
        importer.filterMode = FilterMode.Point;
        importer.SaveAndReimport();
        var sprite=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Square.png");
        var actions=AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/Input/PlatformerInput.inputactions");
        if (sprite == null || actions == null) throw new System.InvalidOperationException("Scene assets have not imported. Run Assignment > Recreate scene after import completes.");
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
        var camera=new GameObject("Main Camera",typeof(Camera),typeof(AudioListener));
        camera.tag="MainCamera"; camera.transform.position=new Vector3(0,1,-10);
        var cam=camera.GetComponent<Camera>(); cam.orthographic=true; cam.orthographicSize=5;
        cam.backgroundColor=new Color(0.16f,0.19f,0.24f); cam.clearFlags=CameraClearFlags.SolidColor;
        var player=new GameObject("Player",typeof(SpriteRenderer),typeof(Rigidbody2D),typeof(BoxCollider2D),typeof(PlayerMotor));
        player.GetComponent<SpriteRenderer>().sprite=sprite;
        player.GetComponent<BoxCollider2D>().size = Vector2.one;
        player.GetComponent<Rigidbody2D>().constraints=RigidbodyConstraints2D.FreezeRotation;
        AddInput(player,actions);
        var platform=new GameObject("Platform",typeof(SpriteRenderer),typeof(Rigidbody2D),typeof(BoxCollider2D));
        platform.GetComponent<SpriteRenderer>().sprite=sprite;
        platform.GetComponent<BoxCollider2D>().size = Vector2.one;
        platform.transform.position=new Vector3(0,-2,0); platform.transform.localScale=new Vector3(16,1,1);
        platform.GetComponent<Rigidbody2D>().bodyType=RigidbodyType2D.Static;
        var debug=new GameObject("InputDebug",typeof(InputDebug));
        AddInput(debug,actions);
        // The separate debug object is an optional exercise; leave it off to avoid a second paired PlayerInput.
        debug.SetActive(false);
        var settings=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
        var active=settings.FindProperty("activeInputHandler"); if(active!=null) active.intValue=1;
        settings.ApplyModifiedPropertiesWithoutUndo();
        PlayerSettings.productName="Unity 28.10.26 - Platformer";
        EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),"Assets/Scenes/Platformer.unity");
        EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Scenes/Platformer.unity",true)};
        AssetDatabase.SaveAssets();
        Debug.Log("ASSIGNMENT_SETUP_SUCCESS");
    }
    static void AddInput(GameObject go, InputActionAsset actions)
    {
        var pi=go.AddComponent<PlayerInput>(); pi.actions=actions;
        pi.defaultActionMap="Move"; pi.notificationBehavior=PlayerNotifications.SendMessages;
        var serialized = new SerializedObject(pi);
        serialized.FindProperty("m_Actions").objectReferenceValue = actions;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }
}





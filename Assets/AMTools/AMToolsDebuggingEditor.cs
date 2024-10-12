#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using AMTools.AMToolsController;
using AMTools.AMToolsSaveManager;

public class AMToolsDebuggingEditor : EditorWindow
{
    private Texture2D _logoTex;
    private string _debugSaveValue = "Nothing :3";
    private int _testInt;
    private float _testFloat;
    private bool _testBool;
    private string _testString;

    [MenuItem("Tools/AMTools/AMTools Debugging Window")]
    public static void ShowWindow()
    {
        GetWindow<AMToolsDebuggingEditor>("AMTools Debugging Window");
    }

    void OnEnable()
    {
        _logoTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/AMTools/Sprites/amtoolslogo.png", typeof(Texture2D));
    }

    void OnGUI()
    {
        GUILayout.Label("AMTools Debugging Window", EditorStyles.boldLabel);
        GUILayout.Space(10);

        GUILayout.Label(_logoTex, EditorStyles.centeredGreyMiniLabel);
        GUILayout.Space(10);

        GUILayout.Label("AMTools Cursor", EditorStyles.label);
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Enable Cursor"))
        {
            AMToolsController.EnableCursor();
        }

        if(GUILayout.Button("Disable Cursor"))
        {
            AMToolsController.DisableCursor();
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(15);

        GUILayout.Label("AMTools Staic Save Manager", EditorStyles.label);
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Save Data to Json"))
        {
            AMToolsSaveManager.SaveDataToJson();
            AssetDatabase.Refresh();
        }

        if(GUILayout.Button("Json to Save Data"))
        {
            AMToolsSaveManager.JsonToSaveData();
            AssetDatabase.Refresh();
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(5);

        GUILayout.Label("AMTools Save Data Test", EditorStyles.label);
        GUILayout.Label("Debug save value is: " + _debugSaveValue, EditorStyles.helpBox);
        GUILayout.BeginHorizontal();
        //EditorGUILayout.IntField("", _testInt);

        if(GUILayout.Button("Write Int Test"))
        {
            AMToolsSaveManager.SaveInt("IntTest", 100);
        }

        if(GUILayout.Button("Write Read Test"))
        {
            _debugSaveValue = (AMToolsSaveManager.GetInt("IntTest").ToString());
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Float Int Test"))
        {
            AMToolsSaveManager.SaveFloat("FloatTest", 200.5f);
        }

        if(GUILayout.Button("Float Read Test"))
        {
            _debugSaveValue = (AMToolsSaveManager.GetFloat("FloatTest").ToString());
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Bool Int Test"))
        {
            AMToolsSaveManager.SaveBool("BoolTest", true);
        }

        if(GUILayout.Button("Bool Read Test"))
        {
            _debugSaveValue = (AMToolsSaveManager.GetBool("BoolTest").ToString());
        }
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("String Int Test"))
        {
            AMToolsSaveManager.SaveString("StringTest", "string");
        }

        if(GUILayout.Button("String Read Test"))
        {
            _debugSaveValue = (AMToolsSaveManager.GetString("StringTest").ToString());
        }
        GUILayout.EndHorizontal();
    }
}
#endif
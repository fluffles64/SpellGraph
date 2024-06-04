using UnityEditor;
using UnityEngine;

public enum EditorType { None, Abilities, Stats, Resources, Items, NPCs }

/// <summary>
/// Extension of the EditorWindow class that adds styling, initialization...
/// Inherited by the RealmsTool class.
/// </summary>

public class ExtendedEditorWindow : EditorWindow
{
    [SerializeField] private Font font1;
    [SerializeField] private Font font2;
    [SerializeField] private Texture2D backgroundImage;
    [SerializeField] private Sprite defaultIcon;
    private Color gold = new Color(1f, 0.75f, 0f, 1f);

    protected static bool showMainMenu = true;
    protected static EditorType editorType = EditorType.None;
    
    protected static float contentWidth;
    protected static Vector2 mainMenuScrollPosition;
    protected static GUIStyles styles;
    protected static AbilityEditor abilityEditor;
    protected static ResourceEditor resourceEditor;
    protected static StatsEditor statsEditor;

    #region Window Creation
    [MenuItem("Tools/Realms Tool")]
    public static void ShowWindow()
    {
        RealmsTool window = GetWindow<RealmsTool>("Realms Tool");
        window.minSize = styles.minWindowSize;
        Load();
        window.Show();
    }
    
    public static void ShowWindow<T>(SerializedObject obj)
    {
        RealmsTool window = GetWindow<RealmsTool>("Realms Tool");
        window.minSize = styles.minWindowSize;
        ShowEditor();

        if (typeof(T) == typeof(Ability))
        {
            abilityEditor.LoadObject<Ability>(obj);
            editorType = EditorType.Abilities;
        }
        if (typeof(T) == typeof(Resource))
        {
            resourceEditor.LoadObject<Resource>(obj);
            editorType = EditorType.Resources;
        }
        if (typeof(T) == typeof(Stats))
        {
            statsEditor.LoadObject<Stats>(obj);
            editorType = EditorType.Stats;
        }
        window.Show();
    }
    #endregion

    #region Styles
    public struct GUIStyles
    {
        public bool isInitialized;

        public GUIStyle titleStyle;
        public GUIStyle clickableStyle;
        public GUIStyle buttonStyle;
        public GUIStyle optionsButtonStyle;
        public GUIStyle titleButtonStyle;
        public GUIStyle backgroundStyle;
        public GUIStyle foldoutStyle;

        public int spaceTiny; // 1
        public int spaceSmall; // 5
        public int spaceMedium; // 10
        public int spaceBig; // 20

        public int spaceBetweenIconAndTitle; // 7
        public int spaceTitleVertical; // 27
        public int titleHeight; // 80

        public int buttonSmall; // 20
        public int buttonMedium; // 50
        public int buttonBig; // 200

        public int iconSmall; // 20
        public int iconMedium; // 32
        public int iconBig; // 40

        public float contentMaxWidth; // 0,5f = 50%

        public Vector2 minWindowSize; // 884, 200
    }

    protected GUIStyles InitializeGUIStyles()
    {
        GUIStyles styles = new GUIStyles
        {
            isInitialized = false
        };

        styles.spaceTiny = 1;
        styles.spaceSmall = 5;
        styles.spaceMedium = 10;
        styles.spaceBig = 20;

        styles.spaceBetweenIconAndTitle = 7;
        styles.spaceTitleVertical = 27;
        styles.titleHeight = 80;

        styles.buttonSmall = 20;
        styles.buttonMedium = 50;
        styles.buttonBig = 200;

        styles.iconSmall = 20;
        styles.iconMedium = 32;
        styles.iconBig = 40;

        styles.contentMaxWidth = 0.5f;

        styles.minWindowSize = new Vector2(884, 200);

        // Titles
        styles.titleStyle = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleCenter,
            stretchWidth = true,
            stretchHeight = true,
            fontSize = 30,
            font = font1,
        }; styles.titleStyle.hover.textColor = gold;
        styles.titleStyle.normal.textColor = gold;


        // Text
        styles.clickableStyle = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleCenter,
            font = font2,
            fontSize = 14,
        }; styles.clickableStyle.hover.textColor = Color.cyan;
        styles.clickableStyle.normal.textColor = Color.white;


        // Buttons
        styles.buttonStyle = new GUIStyle(GUI.skin.box)
        {
            fixedHeight = 30,
            stretchWidth = true,
            font = font2,
        }; styles.buttonStyle.hover.textColor = Color.cyan;
        styles.buttonStyle.normal.textColor = Color.white;

        // Options buttons
        styles.optionsButtonStyle = new GUIStyle(EditorStyles.label)
        {
            stretchHeight = true,
            stretchWidth = true,
            alignment = TextAnchor.MiddleCenter,
            fontSize = 16,
            //fontStyle = FontStyle.Bold,
            font = font2,
        }; styles.optionsButtonStyle.hover.textColor = gold;
        styles.optionsButtonStyle.normal.textColor = Color.white;
        styles.optionsButtonStyle.hover.background = null;
        styles.optionsButtonStyle.normal.background = null;

        // Title button
        styles.titleButtonStyle = new GUIStyle(GUI.skin.box)
        {
            stretchWidth = true,
            alignment = TextAnchor.MiddleCenter,
            fontSize = 16,
            //fontStyle = FontStyle.Bold,
            font = font2,
        }; styles.titleButtonStyle.hover.textColor = gold;
        styles.titleButtonStyle.normal.textColor = Color.white;

        // Background = Box
        styles.backgroundStyle = new GUIStyle(GUI.skin.box)
        {
        };


        // Foldouts
        styles.foldoutStyle = new GUIStyle(EditorStyles.foldout)
        {
            margin = new RectOffset(10, 0, 15, 15),
            padding = new RectOffset(5, 0, 0, 0),
            alignment = TextAnchor.MiddleCenter,
            fontSize = 22,
            font = font1,
        };
        return styles;
    }
    #endregion

    protected void WindowTopBar()
    {
        string editorName = editorType.ToString();
        if (GUILayout.Button(showMainMenu ? "Realms Tool" : $"Realms Tool > {editorName}", styles.titleButtonStyle))
        {
            showMainMenu = true;
        }
    }

    protected void WindowMainMenu()
    {
        mainMenuScrollPosition = GUILayout.BeginScrollView(mainMenuScrollPosition, styles.backgroundStyle, GUILayout.Width(styles.buttonBig), GUILayout.ExpandHeight(true));
        if (GUILayout.Button("Abilities", styles.buttonStyle))
        {
            showMainMenu = false;
            editorType = EditorType.Abilities;
        }
        if (GUILayout.Button("Items", styles.buttonStyle))
        {
            showMainMenu = false;
            editorType = EditorType.Items;
        }
        if (GUILayout.Button("Stats", styles.buttonStyle))
        {
            showMainMenu = false;
            editorType = EditorType.Stats;
        }
        if (GUILayout.Button("Resources", styles.buttonStyle))
        {
            showMainMenu = false;
            editorType = EditorType.Resources;
        }
        if (GUILayout.Button("NPCs", styles.buttonStyle))
        {
            showMainMenu = false;
            editorType = EditorType.NPCs;
        }
        GUILayout.EndScrollView();

        if(backgroundImage!= null)
        {
            Rect backgroundRect = GUILayoutUtility.GetRect(styles.minWindowSize.x - 200, backgroundImage.height / 2);
            GUI.DrawTexture(backgroundRect, backgroundImage, ScaleMode.ScaleToFit);
        }
        else
        {
            GUILayoutUtility.GetRect(styles.minWindowSize.x - 200, styles.minWindowSize.y - 200);
        }
    }

    protected static void Load()
    {
        abilityEditor.LoadObjects<Ability>();
        resourceEditor.LoadObjects<Resource>();
        statsEditor.LoadObjects<Stats>();
    }

    protected void InitializeFoldouts()
    {
        abilityEditor.SetFoldouts();
        resourceEditor.SetFoldouts();
        statsEditor.SetFoldouts();
    }

    protected static void ShowEditor()
    {
        showMainMenu = false;
        switch (editorType)
        {
            case EditorType.None:
                break;
            case EditorType.Abilities:
                abilityEditor.ObjectList();
                abilityEditor.ObjectEditor();
                break;
            case EditorType.Items:
                break;
            case EditorType.Resources:
                resourceEditor.ObjectList();
                resourceEditor.ObjectEditor();
                break;
            case EditorType.Stats:
                statsEditor.ObjectList();
                statsEditor.ObjectEditor();
                break;
            case EditorType.NPCs:
                break;
        }
    }

    protected void CheckWindowSize()
    {
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            // Check if the window size is less than the minimum size.
            if (position.size.x < styles.minWindowSize.x || position.size.y < styles.minWindowSize.y)
            {
                // Set the window size to the minimum size.
                position = new Rect(position.position, styles.minWindowSize);
            }
        }
    }
}
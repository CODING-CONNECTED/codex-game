using System.Collections.Generic;
using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    private enum ScreenState
    {
        Home,
        Tutorial
    }

    private ScreenState currentState = ScreenState.Home;
    private readonly List<GameObject> spawnedObjects = new List<GameObject>();
    private Texture2D pixel;
    private GUIStyle titleStyle;
    private GUIStyle bodyStyle;
    private GUIStyle buttonStyle;
    private PlayerController2D player;

    private void Awake()
    {
        EnsurePixel();
        SetupCamera();
        ShowHomeScreen();
    }

    private void EnsurePixel()
    {
        if (pixel != null)
        {
            return;
        }

        pixel = new Texture2D(1, 1);
        pixel.SetPixel(0, 0, Color.white);
        pixel.Apply();

        titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 30,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.white },
            wordWrap = true
        };

        bodyStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 16,
            alignment = TextAnchor.UpperLeft,
            normal = { textColor = Color.white },
            wordWrap = true
        };

        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 18,
            fixedHeight = 48
        };
    }

    private void SetupCamera()
    {
        Camera.main.backgroundColor = new Color(0.08f, 0.1f, 0.18f);
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = 5.5f;
        Camera.main.transform.position = new Vector3(0f, 1f, -10f);
    }

    private void ClearSpawnedObjects()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }

        spawnedObjects.Clear();
        player = null;
    }

    private void ShowHomeScreen()
    {
        currentState = ScreenState.Home;
        ClearSpawnedObjects();
    }

    private void StartTutorialLevel()
    {
        currentState = ScreenState.Tutorial;
        ClearSpawnedObjects();
        BuildTutorialLevel();
    }

    private void BuildTutorialLevel()
    {
        CreatePlatform(new Vector2(0f, -3.5f), new Vector2(16f, 1f), new Color(0.3f, 0.85f, 0.4f));
        CreatePlatform(new Vector2(-4.5f, -1.5f), new Vector2(3f, 0.5f), new Color(0.25f, 0.75f, 0.35f));
        CreatePlatform(new Vector2(0f, -0.2f), new Vector2(2.75f, 0.5f), new Color(0.25f, 0.75f, 0.35f));
        CreatePlatform(new Vector2(4.25f, 1.1f), new Vector2(3f, 0.5f), new Color(0.25f, 0.75f, 0.35f));
        CreatePlatform(new Vector2(8.2f, 2.4f), new Vector2(4f, 0.5f), new Color(0.3f, 0.8f, 0.45f));

        CreateSpike(new Vector2(-1.75f, -3f));
        CreateSpike(new Vector2(-0.95f, -3f));
        CreateSpike(new Vector2(2f, -3f));
        CreateSpike(new Vector2(2.8f, -3f));
        CreateSpike(new Vector2(6f, 1.55f));

        GameObject goal = CreateBox("Goal", new Vector2(10.4f, 3.4f), new Vector2(0.7f, 1.8f), new Color(1f, 0.9f, 0.2f));
        goal.tag = "Finish";
        BoxCollider2D goalCollider = goal.AddComponent<BoxCollider2D>();
        goalCollider.isTrigger = true;

        GameObject playerObject = CreateBox("Player", new Vector2(-6.5f, -2.3f), new Vector2(0.8f, 1.2f), new Color(0.35f, 0.7f, 1f));
        Rigidbody2D rb = playerObject.AddComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        BoxCollider2D box = playerObject.AddComponent<BoxCollider2D>();
        box.sharedMaterial = null;
        player = playerObject.AddComponent<PlayerController2D>();
        player.spawnPoint = new Vector2(-6.5f, -2.3f);
    }

    private void CreatePlatform(Vector2 position, Vector2 size, Color color)
    {
        GameObject platform = CreateBox("Platform", position, size, color);
        platform.AddComponent<BoxCollider2D>();
    }

    private void CreateSpike(Vector2 position)
    {
        GameObject spike = CreateBox("Spike", position, new Vector2(0.7f, 0.7f), new Color(1f, 0.35f, 0.35f));
        BoxCollider2D collider = spike.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        spike.tag = "Hazard";
    }

    private GameObject CreateBox(string objectName, Vector2 position, Vector2 size, Color color)
    {
        GameObject obj = new GameObject(objectName);
        obj.transform.position = position;
        obj.transform.localScale = Vector3.one;

        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = Sprite.Create(pixel, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
        renderer.color = color;

        obj.transform.localScale = new Vector3(size.x, size.y, 1f);
        spawnedObjects.Add(obj);
        return obj;
    }

    public void ReturnToMenu()
    {
        ShowHomeScreen();
    }

    public void RestartTutorial()
    {
        StartTutorialLevel();
    }

    private void OnGUI()
    {
        EnsurePixel();

        if (currentState == ScreenState.Home)
        {
            DrawHomeScreen();
            return;
        }

        DrawTutorialHud();
    }

    private void DrawHomeScreen()
    {
        GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), pixel, ScaleMode.StretchToFill, true, 0f, new Color(0.08f, 0.1f, 0.18f), 0f, 0f);

        float panelWidth = Mathf.Min(620f, Screen.width - 40f);
        Rect panel = new Rect((Screen.width - panelWidth) * 0.5f, 48f, panelWidth, Screen.height - 96f);
        GUI.color = new Color(0f, 0f, 0f, 0.35f);
        GUI.DrawTexture(panel, pixel);
        GUI.color = Color.white;

        GUILayout.BeginArea(new Rect(panel.x + 24f, panel.y + 24f, panel.width - 48f, panel.height - 48f));
        GUILayout.Label("Appel-Style Platformer Prototype", titleStyle);
        GUILayout.Space(18f);
        GUILayout.Label("This starter scene gives you the core pieces of a Grifpatch-inspired platformer: a controllable player, floating platforms, deadly spikes, a home screen, and a tutorial level that teaches movement, jumping, and avoiding hazards.", bodyStyle);
        GUILayout.Space(14f);
        GUILayout.Label("Tutorial goals:\n• Move with A / D or Left / Right\n• Jump with Space, W, or Up\n• Touch the yellow goal to finish\n• Touch spikes to respawn at the start", bodyStyle);
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Play Tutorial", buttonStyle))
        {
            StartTutorialLevel();
        }

        GUILayout.Space(10f);
        GUILayout.Label("Tip: Open the scripts to tweak jump height, move speed, or add collectibles and enemies next.", bodyStyle);
        GUILayout.EndArea();
    }

    private void DrawTutorialHud()
    {
        GUI.Box(new Rect(16f, 16f, 340f, 110f), GUIContent.none);
        GUI.Label(new Rect(30f, 28f, 310f, 24f), "Tutorial Level", titleStyle);
        GUI.Label(new Rect(30f, 60f, 310f, 40f), "Reach the yellow goal. Avoid red spikes. Press Esc to go back to the home screen.", bodyStyle);

        if (GUI.Button(new Rect(Screen.width - 156f, 18f, 140f, 36f), "Home"))
        {
            ReturnToMenu();
        }

        if (player != null && player.HasFinished)
        {
            GUI.Box(new Rect((Screen.width - 380f) * 0.5f, 90f, 380f, 130f), GUIContent.none);
            GUI.Label(new Rect((Screen.width - 340f) * 0.5f, 112f, 340f, 30f), "Level Complete!", titleStyle);
            GUI.Label(new Rect((Screen.width - 320f) * 0.5f, 148f, 320f, 24f), "Nice jump! Restart the tutorial or go back home.", bodyStyle);

            if (GUI.Button(new Rect((Screen.width - 300f) * 0.5f, 178f, 140f, 30f), "Replay"))
            {
                RestartTutorial();
            }

            if (GUI.Button(new Rect((Screen.width - 300f) * 0.5f + 160f, 178f, 140f, 30f), "Home Screen"))
            {
                ReturnToMenu();
            }
        }
    }

    private void Update()
    {
        if (currentState == ScreenState.Tutorial && Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToMenu();
        }
    }
}

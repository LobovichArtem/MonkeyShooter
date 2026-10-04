using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;

[Overlay(typeof(SceneView), "Scene Inspector", true)]
public class SceneInspectorOverlay : Overlay // Изменено наследование
{
    // В обычном Overlay переопределяем этот метод для UI Toolkit
    public override VisualElement CreatePanelContent()
    {
        VisualElement _root = new VisualElement
        {
            name = "SceneInspectorRoot",
            style = { flexDirection = FlexDirection.Row, paddingBottom = 2, paddingTop = 2 }
        };

        RefreshButtons(_root);

        return _root;
    }

    private void RefreshButtons(VisualElement container)
    {
        container.Clear();

        // 1. Кнопка Play First
        Button _playBtn = new Button(() => StartFromFirstScene()) { text = "▶ First" };
        _playBtn.style.backgroundColor = new Color(0.2f, 0.45f, 0.2f);
        _playBtn.style.marginRight = 10;
        container.Add(_playBtn);

        // 2. Дропдаун для переключения сцен (Single)
        Button _selectSceneBtn = new Button(() => ShowSceneDropdown(OpenSceneMode.Single))
        {
            text = "Go to...",
            tooltip = "Переключиться на другую сцену"
        };
        _selectSceneBtn.style.marginRight = 5;
        container.Add(_selectSceneBtn);

        // 3. Дропдаун для добавления сцен (Additive)
        Button _addSceneBtn = new Button(() => ShowSceneDropdown(OpenSceneMode.Additive))
        {
            text = "+ Add",
            tooltip = "Добавить сцену аддитивно"
        };
        container.Add(_addSceneBtn);
    }

    private void ShowSceneDropdown(OpenSceneMode mode)
    {
        GenericMenu _menu = new GenericMenu();
        var _scenes = EditorBuildSettings.scenes;
        string _activeScenePath = EditorSceneManager.GetActiveScene().path;

        foreach (var _scene in _scenes)
        {
            if (!_scene.enabled) continue;

            string _path = _scene.path;
            string _name = Path.GetFileNameWithoutExtension(_path);

            // Для Single режима галочка стоит на активной сцене
            // Для Additive режима галочка стоит, если сцена уже в иерархии
            bool _isActive = (mode == OpenSceneMode.Single)
                ? _path == _activeScenePath
                : IsSceneLoaded(_path);

            _menu.AddItem(new GUIContent(_name), _isActive, () =>
            {
                if (mode == OpenSceneMode.Single && _path == _activeScenePath) return;

                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(_path, mode);
                }
            });
        }

        if (_scenes.Length == 0)
            _menu.AddDisabledItem(new GUIContent("No scenes in Build Settings"));

        _menu.ShowAsContext();
    }

    private bool IsSceneLoaded(string path)
    {
        for (int i = 0; i < EditorSceneManager.sceneCount; i++)
        {
            if (EditorSceneManager.GetSceneAt(i).path == path) return true;
        }
        return false;
    }

    private void OpenScene(string path, OpenSceneMode mode)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(path, mode);
        }
    }

    private void StartFromFirstScene()
    {
        if (EditorBuildSettings.scenes.Length > 0 && EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(EditorBuildSettings.scenes[0].path);
            EditorApplication.isPlaying = true;
        }
    }
}
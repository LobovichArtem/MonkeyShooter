#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomPropertyDrawer(typeof(AudioKey))]
public class AudioKeyDrawer : PropertyDrawer
{
    private bool _isEditing;
    private string _searchText;
    private string[] _filteredKeys;
    private int _selectedIndex = 0;
    private const int MAX_RESULTS = 10;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty keyProp = property.FindPropertyRelative("Key");
        if (keyProp == null) return;

        EditorGUI.BeginProperty(position, label, property);
        if (!_isEditing) _searchText = keyProp.stringValue;

        Rect fieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        Event e = Event.current;

        // Навигация
        if (_isEditing && e.type == EventType.KeyDown)
        {
            if (e.keyCode == KeyCode.DownArrow) { _selectedIndex = Mathf.Min(_selectedIndex + 1, (_filteredKeys?.Length ?? 1) - 1); e.Use(); }
            else if (e.keyCode == KeyCode.UpArrow) { _selectedIndex = Mathf.Max(_selectedIndex - 1, 0); e.Use(); }
            else if (e.keyCode == KeyCode.Return && _filteredKeys?.Length > _selectedIndex) { ApplyKey(property, keyProp, _filteredKeys[_selectedIndex]); e.Use(); }
            else if (e.keyCode == KeyCode.Escape) _isEditing = false;
        }

        EditorGUI.BeginChangeCheck();
        _searchText = EditorGUI.TextField(fieldRect, label, _searchText);
        if (EditorGUI.EndChangeCheck())
        {
            _isEditing = true;
            _selectedIndex = 0;
            _filteredKeys = AudioKeySearcher.GetAllKeys()
                .Where(k => k.IndexOf(_searchText, System.StringComparison.OrdinalIgnoreCase) >= 0)
                .OrderBy(k => k).Take(MAX_RESULTS).ToArray();
        }

        if (_isEditing && !string.IsNullOrEmpty(_searchText) && _filteredKeys?.Length > 0)
        {
            for (int i = 0; i < _filteredKeys.Length; i++)
            {
                Rect btnRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y + (i + 1) * 18f, position.width - EditorGUIUtility.labelWidth, 18f);
                GUIStyle style = new GUIStyle(EditorStyles.miniButton);
                if (i == _selectedIndex) style.normal.textColor = Color.yellow;
                if (GUI.Button(btnRect, _filteredKeys[i], style)) ApplyKey(property, keyProp, _filteredKeys[i]);
            }
        }
        if (e.type == EventType.MouseDown && !position.Contains(e.mousePosition)) _isEditing = false;
        EditorGUI.EndProperty();
    }

    private void ApplyKey(SerializedProperty parent, SerializedProperty key, string val)
    {
        key.stringValue = val;
        _isEditing = false;
        parent.serializedObject.ApplyModifiedProperties();
        GUI.FocusControl(null);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
        _isEditing && _filteredKeys?.Length > 0 ? 18f + (_filteredKeys.Length * 18f) + 5f : 18f;
}
#endif
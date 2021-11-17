using System.Collections.Generic;
using System.Linq;
using Meta.Core.Runtime;
using UnityEditor;
using UnityEngine;

namespace Meta.Core.Editor
{
    public abstract class MetaRefDrawer<T, D> : PropertyDrawer where T : MetaRef where D : MDatabase
    {
        private SerializedProperty _databaseGuidProperty;
        private SerializedProperty _databaseNameProperty;
        private SerializedProperty _nameProperty;
        private SerializedProperty _guidProperty;

        private D[] _databases;
        private int _selectionDatabaseIndex;
        private int _selectionContainerIndex;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing +
                   EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_databases == null)
            {
                var typeName = typeof(D).Name;

                var assets = AssetDatabase.FindAssets($"t:{typeName}");

                if (assets == null || assets.Length == 0)
                {
                    Debug.LogWarning($"Missing data: {typeName}");
                    return;
                }

                _databases = new D[assets.Length];

                for (var i = 0; i < assets.Length; i++)
                {
                    var asset = assets[i];
                    var assetPath = AssetDatabase.GUIDToAssetPath(asset);
                    _databases[i] = AssetDatabase.LoadAssetAtPath<D>(assetPath);
                }
            }

            if (_databases == null) return;

            DrawMetaProperty(ref position, ref property);
        }

        private void DrawMetaProperty
        (
            ref Rect position,
            ref SerializedProperty property)
        {
            _databaseGuidProperty = property.FindPropertyRelative("_databaseGuid");
            _databaseNameProperty = property.FindPropertyRelative("_databaseName");
            _guidProperty = property.FindPropertyRelative("_guid");
            _nameProperty = property.FindPropertyRelative("_name");

            var guidNameDatabasePairs = new Dictionary<string, string>();

            foreach (var database in _databases)
            {
                guidNameDatabasePairs[database.Guid] = database.name;
            }

            position.height -= EditorGUIUtility.singleLineHeight;

            DrawDropdown(ref position, new GUIContent("Database"), ref _databaseGuidProperty, ref _databaseNameProperty,
                ref _selectionDatabaseIndex, ref guidNameDatabasePairs);

            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (_selectionDatabaseIndex == 0)
            {
                GUI.backgroundColor = Color.red;
                EditorGUI.Popup(position, "Name", 0, new[] { MetaConstants.NONE });
                GUI.backgroundColor = Color.white;
                return;
            }

            DrawDropdown(ref position, new GUIContent("Name"), ref _guidProperty, ref _nameProperty,
                ref _selectionContainerIndex, ref _databases[_selectionDatabaseIndex - 1].GuidPathPairs);
        }

        private void DrawDropdown
        (
            ref Rect position,
            GUIContent label,
            ref SerializedProperty gp,
            ref SerializedProperty np,
            ref int i,
            ref Dictionary<string, string> gnp)
        {
            if (string.IsNullOrEmpty(gp.stringValue))
            {
                gp.stringValue = MetaConstants.GUID;
                np.stringValue = MetaConstants.NONE;
            }

            if (!gnp.ContainsKey(gp.stringValue))
            {
                gp.stringValue = MetaConstants.GUID;
                np.stringValue = MetaConstants.NONE;
            }

            var containerNames = gnp.Values.ToList();

            containerNames.Insert(0, MetaConstants.NONE);

            var name = gnp.ContainsKey(gp.stringValue)
                ? gnp[gp.stringValue]
                : MetaConstants.NONE;

            GUI.backgroundColor = gp.stringValue == MetaConstants.GUID ? Color.red : Color.white;

            i = name == MetaConstants.NONE ? 0 : containerNames.IndexOf(name);

            EditorGUI.BeginChangeCheck();

            i = EditorGUI.Popup(position, label.text, i, containerNames.ToArray());

            if (EditorGUI.EndChangeCheck())
            {
                if (i == 0)
                {
                    gp.stringValue = MetaConstants.GUID;
                    np.stringValue = MetaConstants.NONE;
                }
                else
                {
                    var guid = gnp.Keys.ToArray()[i - 1];
                    np.stringValue = gnp[guid];
                    gp.stringValue = guid;
                }
            }

            GUI.backgroundColor = Color.white;
        }
    }
}
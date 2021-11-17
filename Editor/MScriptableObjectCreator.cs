using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace Meta.Core.Editor
{
    public static class MScriptableObjectCreator
    {
        public static void ShowDialog<T>(string defaultDestinationPath, Action<T> onScritpableObjectCreated = null)
            where T : ScriptableObject
        {
            var selector = new ScriptableObjectSelector<T>(defaultDestinationPath, onScritpableObjectCreated);

            if (selector.SelectionTree.EnumerateTree().Count() == 1)
            {
                // If there is only one scriptable object to choose from in the selector, then 
                // we'll automatically select it and confirm the selection. 
                selector.SelectionTree.EnumerateTree().First().Select();
                selector.SelectionTree.Selection.ConfirmSelection();
            }
            else
            {

                selector.ShowInPopup(200);
            }
        }

        private class ScriptableObjectSelector<T> : OdinSelector<Type> where T : ScriptableObject
        {
            private Action<T> onScritpableObjectCreated;
            private string defaultDestinationPath;

            public ScriptableObjectSelector(string defaultDestinationPath, Action<T> onScritpableObjectCreated = null)
            {
                this.onScritpableObjectCreated = onScritpableObjectCreated;
                this.defaultDestinationPath = defaultDestinationPath;
                this.SelectionConfirmed += this.ShowSaveFileDialog;
            }

            protected override void BuildSelectionTree(OdinMenuTree tree)
            {
                var scriptableObjectTypes = AssemblyUtilities.GetTypes(AssemblyTypeFlags.CustomTypes)
                    .Where(x => x.IsClass && !x.IsAbstract && x.InheritsFrom(typeof(T)));

                tree.Selection.SupportsMultiSelect = false;
                tree.Config.DrawSearchToolbar = true;
                tree.AddRange(scriptableObjectTypes, x => x.GetNiceName())
                    .AddThumbnailIcons();
            }

            private void ShowSaveFileDialog(IEnumerable<Type> selection)
            {
                var obj = ScriptableObject.CreateInstance(selection.FirstOrDefault()) as T;

                string dest = this.defaultDestinationPath.TrimEnd('/');

                if (!Directory.Exists(dest))
                {
                    Directory.CreateDirectory(dest);
                    AssetDatabase.Refresh();
                }

                dest = EditorUtility.SaveFilePanel("Save object as", dest, "New " + typeof(T).GetNiceName(), "asset");

                if (!string.IsNullOrEmpty(dest) &&
                    PathUtilities.TryMakeRelative(Path.GetDirectoryName(Application.dataPath), dest, out dest))
                {
                    AssetDatabase.CreateAsset(obj, dest);
                    AssetDatabase.Refresh();

                    if (this.onScritpableObjectCreated != null)
                    {
                        this.onScritpableObjectCreated(obj);
                    }
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(obj);
                }
            }
        }
    }
}
#endif
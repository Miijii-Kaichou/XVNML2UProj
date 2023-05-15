using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;

namespace XVNML2U.Mono.Core
{
#if UNITY_EDITOR
    public sealed class XVNMLEditor : EditorWindow
    {
        private static XVNMLEditor Instance;

        // Explorer Data
        private XVNMLFileTreeView m_treeView;
        private TreeViewState m_treeViewState;

        // TextEditorField
        private XVNMLTextEditor m_textEditor;

        private void OnEnable()
        {
            // Explore has File Tree Drawn based on the base directory;
            m_treeViewState ??= new TreeViewState();
            m_treeView = new XVNMLFileTreeView(m_treeViewState);
            m_textEditor = new XVNMLTextEditor();
        }

        [MenuItem("Window/XVNML2U/XVNML Editor")]
        public static void OpenTextEditorWindow()
        {
            Instance ??= GetWindow<XVNMLEditor>();
            Instance.titleContent = new GUIContent("XVNML IDE");
        }

        public void OnGUI()
        {
            DrawExplorer();
            DrawTextEditor();
        }

        private void DrawExplorer()
        {
            m_treeView.OnGUI(new Rect(0, 0, position.width/4, position.height));
        }

        private void DrawTextEditor()
        {
            m_textEditor.ViewTextEditor(new Rect(position.x/10, 15, position.width - (position.x/9.5f), position.height - 30));
        }
    }

    public sealed class XVNMLFileTreeView : TreeView
    {

        public XVNMLFileTreeView(TreeViewState state) : base(state) 
        {
            Reload();
        }
        
        protected override TreeViewItem BuildRoot()
        {
            // BuildRoot is called every time Reload is called to ensure that TreeViewItems 
            // are created from data. Here we create a fixed set of items. In a real world example,
            // a data model should be passed into the TreeView and the items created from the model.

            // This section illustrates that IDs should be unique. The root item is required to 
            // have a depth of -1, and the rest of the items increment from that.
            var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
            var allItems = new List<TreeViewItem>
        {
            new TreeViewItem {id = 1, depth = 0, displayName = "Animals"},
            new TreeViewItem {id = 2, depth = 1, displayName = "Mammals"},
            new TreeViewItem {id = 3, depth = 2, displayName = "Tiger"},
            new TreeViewItem {id = 4, depth = 2, displayName = "Elephant"},
            new TreeViewItem {id = 5, depth = 2, displayName = "Okapi"},
            new TreeViewItem {id = 6, depth = 2, displayName = "Armadillo"},
            new TreeViewItem {id = 7, depth = 1, displayName = "Reptiles"},
            new TreeViewItem {id = 8, depth = 2, displayName = "Crocodile"},
            new TreeViewItem {id = 9, depth = 2, displayName = "Lizard"},
        };

            // Utility method that initializes the TreeViewItem.children and .parent for all items.
            SetupParentsAndChildrenFromDepths(root, allItems);

            // Return root of the tree
            return root;
        }
    }

    internal sealed class XVNMLTextEditor : TextEditor
    {
        string? content = string.Empty;
        Vector2 scrollPos;
        internal void ViewTextEditor(Rect rect)
        {
            GUIStyle style = new GUIStyle();
            GUILayout.BeginArea(rect);
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            content = GUILayout.TextArea(content, GUILayout.MaxHeight(rect.height));
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
    }
#endif
}
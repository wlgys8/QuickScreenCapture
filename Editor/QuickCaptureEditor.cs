using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditorInternal;

namespace MS.ScreenCapture{
    public class QuickCaptureEditor :EditorWindow
    {
        private bool _dirty = false;
        private List<string> _dirChoices = new List<string>();
        private ReorderableList _dirListEditor;
        private bool _editing = false;

        private static QuickCaptureEditor _instance;

        void OnEnable(){
            _instance = this;
            _dirChoices = new List<string>(QuickCaptureEditorSetting.dirChoices);
            _dirListEditor = new ReorderableList(_dirChoices,typeof(string));
            _dirListEditor.onAddCallback = (ReorderableList list)=>{
                _dirChoices.Add("");
            };
            _dirListEditor.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused)=>{
                if(isActive && _editing){
                    _dirChoices[index] = EditorGUI.TextField(rect,_dirChoices[index]);
                }else{
                    EditorGUI.LabelField(rect,_dirChoices[index]);
                }
            };
            _dirListEditor.onSelectCallback = (ReorderableList list)=>{
                QuickCaptureEditorSetting.selectedDirIndex = list.index;
                QuickCaptureEditorSetting.Save();
            };
            _dirListEditor.drawHeaderCallback = (Rect rect)=>{
                GUI.Label(rect,"OutputDirs");
            };
            _dirListEditor.index = QuickCaptureEditorSetting.selectedDirIndex;

            EndEdit();
        }

        void OnDisable(){
            _instance = null;
        }

        private void StartEdit(){
            _editing = true;
            _dirListEditor.displayAdd = true;
            _dirListEditor.displayRemove = true;
            _dirListEditor.draggable = true;

        }
        private void EndEdit(){
            _editing = false;
            _dirListEditor.displayAdd = false;
            _dirListEditor.displayRemove = false;
            _dirListEditor.draggable = false;
        }
        void OnGUI(){
            EditorGUI.BeginChangeCheck();
            _dirListEditor.DoLayoutList();
            if(EditorGUI.EndChangeCheck()){
                
            }

            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Edit")){
                StartEdit();
            }
            EditorGUI.BeginDisabledGroup(!_editing);

            if(GUILayout.Button("Save")){
                QuickCaptureEditorSetting.ClearDirChoices();
                foreach(var dir in _dirChoices){
                    QuickCaptureEditorSetting.AddDirChoice(dir);
                }
                QuickCaptureEditorSetting.Save();
                EndEdit();
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();

            if(GUILayout.Button("Capture it!",GUILayout.Height(100))){
                Take();
                
            }
        }


        [MenuItem("Window/QuickScreenCapture")]
        private static void Open(){
            var win = EditorWindow.GetWindow<QuickCaptureEditor>();
            win.titleContent = new GUIContent("QuickScreenCapture");
            win.Show();
        }


        [MenuItem("Edit/CaptureScreen &#s")]
        public static void Take(){
            var dir = QuickCaptureEditorSetting.saveDir.Trim();
            if(!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)){
                Directory.CreateDirectory(dir);
            }
            var name = string.Format("/IMG_{0}.jpg",System.DateTime.Now.ToString("MM-dd hh:mm:ss"));
            var outPath = dir + name; 
            UnityEngine.ScreenCapture.CaptureScreenshot(outPath);
            Debug.Log("Capture saved at : " + outPath);
            if(_instance){
                _instance.ShowNotification(new GUIContent("Capture Success!"));
            }
        }
    }
}
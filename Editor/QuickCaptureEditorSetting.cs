using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace MS.ScreenCapture{

    [System.Serializable]
    public class QuickCaptureEditorSetting 
    {

        [SerializeField]
        private string _saveDirPath = "";

        [SerializeField]
        private List<string> _saveDirChoices = new List<string>();

        [SerializeField]
        private int _choiceIndex = 0;

        private const string _defaultSettingFilePath = "ProjectSettings/QuickScreenCapture.json";

        private static QuickCaptureEditorSetting _instance;

        private static QuickCaptureEditorSetting Instance{
            get{
                if(_instance == null){
                    if(File.Exists(_defaultSettingFilePath)){
                        var text = File.ReadAllText(_defaultSettingFilePath);
                        _instance = JsonUtility.FromJson<QuickCaptureEditorSetting>(text);
                    }else{
                        _instance = new QuickCaptureEditorSetting();
                    }
                }
                return _instance;
            }
        }

        internal static void Save(){
            if(_instance == null){
                return;
            }
            var text = JsonUtility.ToJson(_instance);
            File.WriteAllText(_defaultSettingFilePath,text);
        }

        public static int dirChoicesCount{
            get{
                return Instance._saveDirChoices.Count;
            }
        }

        public static int selectedDirIndex{
            get{
                return Instance._choiceIndex;
            }internal set{
                if(value < 0){
                    value = 0;
                }
                if(value >= dirChoicesCount){
                    value = 0;
                }
                Instance._choiceIndex = value;
            }
        }
        public static string saveDir{
            get{
                if(Instance._choiceIndex < 0 || Instance._choiceIndex >= dirChoicesCount){
                    return "";
                }
                return Instance._saveDirChoices[Instance._choiceIndex];
            }
        }
        internal static string[] dirChoices{
            get{
                return Instance._saveDirChoices.ToArray();
            }
        }

        internal static void AddDirChoice(string dir){
            Instance._saveDirChoices.Add(dir);
        }

        internal static void RemoveDirChoice(int index){
            Instance._saveDirChoices.RemoveAt(index);
        }

        internal static void ClearDirChoices(){
            Instance._saveDirChoices.Clear();
        }
        
        internal static string GetDirChoice(int index){
            return Instance._saveDirChoices[index];
        }

        internal static void SetDirChoice(int index,string text){
            Instance._saveDirChoices[index] = text;
        }

    }
}

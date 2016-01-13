using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;


public class UPPEditor : EditorWindow {
    static System.Text.Encoding utf_8 = System.Text.Encoding.UTF8;

    #region Unity MenuItem
    [MenuItem("Window/Tools/UPPEditor")]
    public static void OpenUPPEditor() {
        UPPEditor E = EditorWindow.CreateInstance<UPPEditor>();
        E.Show();
    }
    #endregion Unity MenuItem

    #region PlayerPrefs classes
    private class PP_base {
        public string prefName;
        public virtual bool DrawGUI() { return false; }
    }
    private class PP<T> : PP_base {
        public T value;
        public T originalValue;
        public PP(string aName, T aValue) {
            prefName = aName;
            originalValue = value = aValue;
        }
        public override bool DrawGUI() {
            GUI.enabled = (value.ToString() != originalValue.ToString());
            if (GUILayout.Button("reset", GUILayout.Width(50)))
                value = originalValue;
            GUI.enabled = true;
            return GUILayout.Button("x", GUILayout.Width(20));
        }

    }
    private class PP_int : PP<int> {
        public PP_int(string aName, int aValue) : base(aName, aValue) { }
        public override bool DrawGUI() {
            GUILayout.Label("integer", GUILayout.Width(100));
            value = EditorGUILayout.IntField(prefName, value, GUILayout.MaxWidth(700));
            return base.DrawGUI();
        }
    }
    private class PP_float : PP<float> {
        public PP_float(string aName, float aValue) : base(aName, aValue) { }
        public override bool DrawGUI() {
            GUILayout.Label("float", GUILayout.Width(100));
            value = EditorGUILayout.FloatField(prefName, value, GUILayout.MaxWidth(700));
            return base.DrawGUI();
        }
    }
    private class PP_string : PP<string> {
        public PP_string(string aName, string aValue) : base(aName, aValue) { }
        public override bool DrawGUI() {
            if (utf_8.GetByteCount(value) < 128)
                GUILayout.Label("short string", GUILayout.Width(100));
            else
                GUILayout.Label("long string", GUILayout.Width(100));

            value = EditorGUILayout.TextField(prefName, value, GUILayout.MaxWidth(700));
            return base.DrawGUI();
        }
    }
    #endregion PlayerPrefs classes


    private List<PP_base> m_Prefs = null;
    private Vector2 scrollPos = Vector2.zero;
    private string m_FileName = "";
    private int m_NewPrefType = 0;
    private string m_NewPrefName = "";

    void OpenFile() {
        string tmp = EditorUtility.OpenFilePanel("Select UPP file", m_FileName, "UPP");
        if (tmp != "") {
            m_FileName = tmp;
            if (System.IO.File.Exists(m_FileName))
                m_Prefs = ReadFile(m_FileName);
        }
    }


    PP_base ReadPref(BinaryReader reader) {
        byte nameLength = reader.ReadByte();
        string name = utf_8.GetString(reader.ReadBytes(nameLength));
        byte type = reader.ReadByte();
        if (type < 0x80) {
            return new PP_string(name, utf_8.GetString(reader.ReadBytes(type)));
        } else if (type == 0x80) {
            int length = reader.ReadInt32();
            return new PP_string(name, utf_8.GetString(reader.ReadBytes(length)));
        } else if (type == 0xFD) {
            return new PP_float(name, reader.ReadSingle());
        } else if (type == 0xFE) {
            return new PP_int(name, reader.ReadInt32());
        }
        return null;
    }


    void WritePref(BinaryWriter writer, PP_base aPref) {
        byte[] tmp = utf_8.GetBytes(aPref.prefName);
        writer.Write((byte) tmp.Length);
        writer.Write(tmp);
        if (aPref is PP_int) {
            writer.Write((byte) 0xFE);
            writer.Write(((PP_int) aPref).value);
        } else
            if (aPref is PP_float) {
                writer.Write((byte) 0xFD);
                writer.Write(((PP_float) aPref).value);
            } else
                if (aPref is PP_string) {
                    PP_string P = (PP_string) aPref;
                    tmp = utf_8.GetBytes(P.value);
                    if (tmp.Length < 128)
                        writer.Write((byte) tmp.Length);
                    else {
                        writer.Write((byte) 0x80);
                        writer.Write(tmp.Length);
                    }
                    writer.Write(tmp);
                }
    }

    List<PP_base> ReadFile(string aFileName) {
        List<PP_base> result = new List<PP_base>();

        using (FileStream FS = File.OpenRead(aFileName))
        using (BinaryReader reader = new BinaryReader(FS)) {
            string header = new string(reader.ReadChars(8));
            if (header != "UnityPrf") {
                Debug.LogError("Not a valid UPP file. No 'UnityPrf' in header");
                return null;
            }
            int version1 = reader.ReadInt32();
            int version2 = reader.ReadInt32();
            if (version1 != 0x10000 || version2 != 0x100000) {
                Debug.LogWarning("Different file version detected. Prepare for unforeseen consequences!");
                if (!EditorUtility.DisplayDialog("Unknown file version", "The file seems to have a newer format. You can continue reading but on your own risk", "ok, do it", "no, stop here")) {
                    Debug.Log("Reading UPP file aborted by user");
                    return null;
                }
            }
            try {
                while (true) {
                    PP_base pref = ReadPref(reader);
                    if (pref != null)
                        result.Add(pref);
                }
            } catch (EndOfStreamException) { }
        }
        return result;
    }

    void WriteFile() {
        Debug.Log("SaveFile");
        string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "/Unity/WebPlayerPrefs";
        string name = "PlayerPref.UPP";
        if (File.Exists(m_FileName)) {
            FileInfo FI = new FileInfo(m_FileName);
            path = FI.DirectoryName;
            name = FI.Name;
        }
        string tmp = EditorUtility.SaveFilePanel("Save new UPP file", path, name, "UPP");
        if (tmp == "")
            return;
        m_FileName = tmp;
        using (FileStream FS = File.Create(tmp))
        using (BinaryWriter writer = new BinaryWriter(FS)) {
            writer.Write("UnityPrf".ToCharArray());
            writer.Write((int) 0x10000);
            writer.Write((int) 0x100000);
            foreach (PP_base P in m_Prefs) {
                WritePref(writer, P);
            }
        }
    }

    void AddPref(int aType, string aName) {
        foreach (PP_base P in m_Prefs) {
            if (P.prefName.ToUpper() == m_NewPrefName.ToUpper()) {
                ShowNotification(new GUIContent("Error! duplicate pref name"));
                return;
            }
        }
        switch (aType) {
            case 0: m_Prefs.Add(new PP_int(aName, 0)); break;
            case 1: m_Prefs.Add(new PP_float(aName, 0)); break;
            case 2: m_Prefs.Add(new PP_string(aName, "")); break;
        }
    }


    #region Unity events
    void OnEnable() {
        if (m_FileName == "") {
            m_FileName = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "/Unity/WebPlayerPrefs";
        } else if (System.IO.File.Exists(m_FileName))
            m_Prefs = ReadFile(m_FileName);
    }

    void OnGUI() {
        GUILayout.BeginHorizontal("box");
        if (GUILayout.Button("create new", GUILayout.Width(80))) {
            m_FileName = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "/Unity/WebPlayerPrefs";
            m_Prefs = new List<PP_base>();
        }
        if (GUILayout.Button("open file", GUILayout.Width(80)))
            OpenFile();
        GUI.enabled = (m_Prefs != null && m_Prefs.Count > 0);
        if (GUILayout.Button("save file", GUILayout.Width(80)))
            WriteFile();
        GUI.enabled = true;
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal("box");
        GUILayout.Label("File:", GUILayout.Width(30));
        GUILayout.TextField(m_FileName);
        GUILayout.EndHorizontal();
        scrollPos = GUILayout.BeginScrollView(scrollPos);
        GUILayout.BeginVertical("box");

        if (m_Prefs != null) {
            for (int i = 0; i < m_Prefs.Count; i++) {
                PP_base current = m_Prefs[i];
                GUILayout.BeginHorizontal();
                if (current.DrawGUI())
                    m_Prefs.RemoveAt(i);
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal("box");
            m_NewPrefType = GUILayout.Toolbar(m_NewPrefType, new string[] { "int", "float", "string" }, GUILayout.Width(150));
            m_NewPrefName = GUILayout.TextField(m_NewPrefName, 254, GUILayout.Width(200));
            if ((m_NewPrefName != "") && GUILayout.Button("[add new]", GUILayout.Width(70))) {
                AddPref(m_NewPrefType, m_NewPrefName);
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }
    #endregion Unity events
}
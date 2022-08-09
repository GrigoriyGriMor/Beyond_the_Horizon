using UnityEngine;
using UnityEditor;

[System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = true, Inherited = true)]
public class JUHeader : PropertyAttribute
{
    public string text;

    public JUHeader(string text)
    {
        this.text = text;
    }
}



[System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = true, Inherited = true)]
public class JUSubHeader : PropertyAttribute
{
    public string text;
    public JUSubHeader(string text)
    {
        this.text = text;
    }
}



[System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = true, Inherited = true)]
public class JUReadOnly : PropertyAttribute
{
    public string ConditionPropertyName;
    public bool Inverse;
    public bool DisableOnFalse;
    public JUReadOnly(string text = "", bool inverse = false, bool disableonfalse = true)
    {
        this.ConditionPropertyName = text;
        this.Inverse = inverse;
        this.DisableOnFalse = disableonfalse;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(JUHeader))]
public class JUHeaderDecoratorDrawer : DecoratorDrawer
{
    JUHeader header
    {
        get { return ((JUHeader)attribute); }
    }

    public override float GetHeight()
    {
        return base.GetHeight() + 5;
    }

    public override void OnGUI(Rect position)
    {
        //float lineX = (position.x + (position.width / 2)) - header.lineWidth / 2;
        float lineY = position.y + 0;
        //float lineWidth = header.lineWidth;

        var g = new GUIStyle(EditorStyles.toolbar);
        g.fontStyle = FontStyle.Bold;
        g.alignment = TextAnchor.LowerLeft;
        //g.font = JUEditor.CustomEditorStyles.JUEditorFont();

        if (EditorGUIUtility.isProSkin == false)
        {
            g.normal.textColor = Color.black;
        }
        else
        {
            g.normal.textColor = Color.white;
        }

        //g.normal.textColor = new Color(1f, 0.7f, 0.5f);
        g.fontSize = 16;
        g.richText = true;
        Rect newposition = new Rect(position.x - 17, lineY, position.width + 28, position.height);
        EditorGUI.LabelField(newposition, "  " + header.text, g);
    }
}





[CustomPropertyDrawer(typeof(JUSubHeader))]
public class JUSubHeaderDecoratorDrawer : DecoratorDrawer
{
    JUSubHeader header
    {
        get { return ((JUSubHeader)attribute); }
    }

    public override float GetHeight()
    {
        return base.GetHeight() + 5;
    }
    public override void OnGUI(Rect position)
    {
        //float lineX = (position.x + (position.width / 2)) - header.lineWidth / 2;
        float lineY = position.y + 1;
        //float lineWidth = header.lineWidth;
        var g = new GUIStyle(EditorStyles.boldLabel);
        g.fontStyle = FontStyle.Bold;
        //g.font = JUEditor.CustomEditorStyles.JUEditorFont();
        g.alignment = TextAnchor.MiddleLeft;

        if (EditorGUIUtility.isProSkin == false)
        {
            g.normal.textColor = Color.black;
        }
        else
        {
            g.normal.textColor = Color.white;
        }

        //g.normal.textColor = new Color(1f, 0.7f, 0.5f);
        g.fontSize = 15;
        g.richText = true;

        Rect newposition = new Rect(position.x - 17, lineY, position.width + 19, position.height);
        EditorGUI.LabelField(newposition, "   " + header.text, g);
    }
}





[CustomPropertyDrawer(typeof(JUReadOnly))]
public class JUReadOnlyPropertyDrawer : PropertyDrawer
{
    JUReadOnly jureadonly
    {
        get { return ((JUReadOnly)attribute); }
    }
    private bool drawing;
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (drawing == true)
        {
            return base.GetPropertyHeight(property, label);
        }
        else
        {
            return 0;
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (jureadonly.DisableOnFalse == true && GetAttributeConditionValue(jureadonly, property) == false)
        {
            drawing = false;
            return;
        }
        drawing = true;
        if (jureadonly.ConditionPropertyName == "")
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
        else
        {
            //var cond = (JUReadOnly)attribute;

            GUI.enabled = GetAttributeConditionValue(jureadonly, property);
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
    public bool GetAttributeConditionValue(JUReadOnly jureadonly, SerializedProperty property)
    {
        if (jureadonly.ConditionPropertyName == "") return true;

        bool enabled = true;
        var booleanCondition = property.serializedObject.FindProperty(jureadonly.ConditionPropertyName);

        enabled = booleanCondition.boolValue;

        if (jureadonly.Inverse == false)
        {
            return enabled;
        }
        else
        {
            return !enabled;
        }
    }
}
#endif
﻿using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

namespace ScriptEditor.Graph {
    [Serializable]
    public abstract class NodePin {

        [SerializeField] public string Name, Description;
        [SerializeField] public bool isConnected = false;
        [SerializeField] public NodeBase node;
        [SerializeField] public VarType varType;
        [SerializeField] public Rect bounds;
        [SerializeField] public object defaultVal;

        [SerializeField] protected object val;

        public object Value {
            get {
                if(val!=null)return val;
                if (isInput)
                    Debug.Log("Pin Val is null... Default: \"" + ((InputPin)this).Default+"\"");
                switch (varType) {
                    case VarType.Bool: return val = false;
                    case VarType.Float: return val = 0;
                    case VarType.Integer: return val = 0;
                    case VarType.String: return val = "";
                    case VarType.Vector2: return val = Vector2.zero;
                    case VarType.Vector3: return val = Vector3.zero;
                    case VarType.Vector4: return val = Vector4.zero;
                    case VarType.Color: return val = Color.white;
                }
                return "";
            }
            set {
                if (value != null)
                    switch (varType) {
                        case VarType.Bool: val = (bool)value; break;
                        case VarType.Float: val = (float)value; break;
                        case VarType.Integer: val = (int)value; break;
                        case VarType.String: val = (string)value; break;
                        case VarType.Vector2: val = (Vector2)value; break;
                        case VarType.Vector3: val = (Vector3)value; break;
                        case VarType.Vector4: val = (Vector4)value; break;
                        case VarType.Color: val = (Color)value; break;
                    }
            }
        }

        public string StyleName {
            get {
                string color = "";
                switch (varType) {
                    case VarType.Bool: color = "Red"; break;
                    case VarType.Actor: color = "Orange"; break;
                    case VarType.Vector2:
                    case VarType.Vector3:
                    case VarType.Color:
                    case VarType.Vector4: color = "Yellow"; break;
                    case VarType.Float: color = "Green"; break;
                    case VarType.Integer: color = "Cyan"; break;
                    case VarType.Object: color = "Blue"; break;
                    case VarType.String: color = "Purple"; break;
                    case VarType.Exec: color = "White"; break;
                }

                return "Pin" + color + (this.isConnected ? "Closed" : "Open");
            }
        }

        public Rect TextBox {
            get {
                Vector2 lblPos = bounds.position;
                Vector2 lblSiz = node.skin.label.CalcSize(new GUIContent(Name + "  "));
                lblPos.x += (isInput ? NodePin.margin.x : -lblSiz.x);
                return new Rect(lblPos, lblSiz);
            }
        }

        public static Vector2 margin = new Vector2(25, 13);
        public static Vector2 pinSize = new Vector2(23, 17);
        public const float padding = 16;
        public static float Top { get { return margin.y + padding; } }


        public NodePin(NodeBase n, object val) : this(n, GetVarType(val)) {
            Value = val;
            // set name of node to name of variable
        }

        public NodePin(NodeBase n, VarType varType) {
            this.varType = varType;
            node = n;
            bounds = new Rect(Vector2.zero, pinSize);

            switch (varType) {
                case VarType.Bool: val = false;break;
                case VarType.Integer:
                case VarType.Float: val = 0; break;
                case VarType.Vector2: val = Vector2.zero; break;
                case VarType.Vector3: val = Vector3.zero; break;
                case VarType.Vector4: val = Vector4.zero; break;
            }
        }

        /// <summary> returns the name of the base connected node </summary>
        public abstract string ConName();

        /// <summary> determines the PinType from the type of the passed variable  </summary>
        public static VarType GetVarType(object variable) {
            if (variable.GetType().Equals(typeof(int))) return VarType.Integer;
            if (variable.GetType().Equals(typeof(bool))) return VarType.Bool;
            if (variable.GetType().Equals(typeof(float))) return VarType.Float;
            if (variable.GetType().Equals(typeof(string))) return VarType.String;
            if (variable.GetType().Equals(typeof(GameObject))) return VarType.Actor; //TODO
            if (variable.GetType().Equals(typeof(Vector2))) return VarType.Vector2;
            if (variable.GetType().Equals(typeof(Vector3))) return VarType.Vector3;
            if (variable.GetType().Equals(typeof(Vector4))) return VarType.Vector4;
            return VarType.Object;
        }

        /// <summary> is the NodePin an InputPin? </summary>
        public bool isInput { get { return this is InputPin; } }

        /// <summary> color respective of the type of the pin </summary>
        public Color _Color {
            get {
                switch (varType) {
                    case VarType.Exec: return Color.white;
                    case VarType.Actor: return Color.Lerp(Color.red, Color.yellow, .5f);
                    case VarType.Bool: return Color.red;
                    case VarType.Float: return Color.green;
                    case VarType.Integer: return Color.cyan;
                    case VarType.String: return Color.Lerp(Color.red, Color.blue, .5f);
                    case VarType.Vector2:
                    case VarType.Vector3:
                    case VarType.Color:
                    case VarType.Vector4: return Color.yellow;
                    default: return Color.blue;
                }
            }
        }
        

#if UNITY_EDITOR
        public bool Contains(Vector2 pos) {
            Rect b = new Rect(bounds);
            b.position += node.GetBody().position;
            return b.Contains(pos);
        }

        /// <summary> determine if mouse is inside textbox for name </summary>
        public bool TxtContains(Vector2 pos) {
            Rect b = new Rect(TextBox);
            b.position += node.GetBody().position;
            return b.Contains(pos);
        }

        public Vector2 Position {
            get {
                if (bounds == Rect.zero) return Vector2.zero;
                return bounds.position + node.GetBody().position;
            }
        }

        public Vector2 Center {
            get {
                return Position + new Vector2(isInput ? -pinSize.y / 2f :
                    pinSize.x*1.5f, 8);
            }
        }

        public void DrawConnection() {
            if (!isConnected || isInput) return;

            // draw bezier curve from output pin to input pin
            try {
                //Debug.Log("Planetarium: "+this.GetType());
                Vector3 start = this.Center;
                Vector3 end = ((OutputPin)this).ConnectedInput.Center;
                Vector2 startTangent, endTangent;

                float offset = Mathf.Max(Mathf.Abs(start.x - end.x) / 1.75f, 1);
                startTangent = new Vector2(start.x + offset, start.y);
                endTangent = new Vector2(end.x - offset, end.y);
                // Debug.Log(start + " | " + end + "\n" + startTangent + " | " + endTangent);
                Handles.BeginGUI();
                {
                    Handles.color = Color.white;
                    Handles.DrawBezier(start, end, startTangent, endTangent, this._Color, null, 2);
                } Handles.EndGUI();
            } catch (Exception e) {
                Debug.Log("Unable to draw: " + this);
                Debug.Log(e);
            }
        }
#endif  
    }



    
}
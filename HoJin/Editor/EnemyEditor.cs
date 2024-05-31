using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HoJin.GameScene;

namespace HoJin.Editor
{
    [CustomEditor(typeof(Enemy))]
    [CanEditMultipleObjects]
    public class EnemyEditor : UnityEditor.Editor
    {
        private Enemy enemy;



        private void OnEnable()
        {
            enemy = target as Enemy;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Run test"))
            {
                
            }
        }
    }
}
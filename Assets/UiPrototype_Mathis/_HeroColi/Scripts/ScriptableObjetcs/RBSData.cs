///-----------------------------------------------------------------
///   Author : #DEVELOPER_NAME#
///   Date   : #DATE#
///-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using UIProto.Data.RBSAdjective;
using UIProto.Scriptable.Enums;
using UnityEngine;

namespace UIProto.Scriptable
{
    [CreateAssetMenu(fileName = "new RBS", menuName = "HeroColi/Bricks/RBS", order = 2)]
    public class RBSData : BricksData
    {
        [Header("RBS Properties")]
        [SerializeField] public float expressionLevel;

        [SerializeField] public List<RBSAdjective> rbsAdjective = new List<RBSAdjective>();

        public override void GenerateDescriptionElements()
        {
            expressionLevel = Mathf.Clamp(expressionLevel, 0, 100);

            _brickDescription = "A RBS with an expression level of " + expressionLevel.ToString() + "%";
        }

        public string GetDescriptionPart (DeviceAction action)
        {
            foreach (RBSAdjective adj in rbsAdjective)
            {
                if (adj.action == action)
                    return String.IsNullOrEmpty(adj.adjective) ? "" : adj.adjective;   
            }

            return "NO ADJECTIVE FOR THIS ACTION";
        }

        protected override void CleanBrickProperties()
        {
            expressionLevel = 0;
        }
    }
}

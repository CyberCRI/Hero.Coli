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
        [SerializeField] private float _expressionLevel;
        public float ExpressionLevel
        {
            get { return _expressionLevel; }
            set
            {
                _expressionLevel = value;

                _state = CheckState() ? DataState.Filled : DataState.Empty;
            }
        }

        [SerializeField] public List<RBSAdjective> rbsAdjective = new List<RBSAdjective>();

        public override void GenerateDescriptionElements()
        {
            ExpressionLevel = Mathf.Clamp(_expressionLevel, 0, 100);

            _brickDescription = "A RBS with an expression level of " + _expressionLevel.ToString() + "%";
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
            base.CleanBrickProperties();

            _expressionLevel = 0;
        }

        protected override bool CheckState()
        {
            if (base.CheckState() && _expressionLevel != 0)
                return true;
            else
                return false;
        }
    }
}

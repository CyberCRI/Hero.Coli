using System;
using System.Collections.Generic;
using UIProto.Scriptable;
using UnityEngine;

namespace UIProto.Data.Crafting
{
    [Serializable]
    public class CraftingBricks
    {
        [SerializeField] private BricksData _brick;
        public BricksData Brick
        {
            get { return _brick; }
        }

        [SerializeField] private int _quantity;
        public int Quantity
        {
            get { return _quantity; }
        }
        
        public int RemainingBricks
        {
            get { return _quantity - deviceContainingThis.Count; }
        }

        public List<DisplayDevice> deviceContainingThis;

        public CraftingBricks (BricksData brickToStore, int quantity)
        {
            _brick = brickToStore;
            _quantity = quantity;

            deviceContainingThis = new List<DisplayDevice>();
        }

        public bool IsAvailableForCrafting
        {
            get { return RemainingBricks > 0; }
        }
    }
}

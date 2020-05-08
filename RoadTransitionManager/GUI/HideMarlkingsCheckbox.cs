namespace NodeController.GUI {
    using ColossalFramework.UI;
    using System;
    using UnityEngine;
    using Util;
    using static Util.HelpersExtensions;

    public class UIHideMarkingsCheckbox : UICheckBox, IDataControllerUI {
        public static UIHideMarkingsCheckbox Instance { get; private set; }

        public override void Awake() {
            base.Awake();
            Instance = this;
            name = nameof(UIHideMarkingsCheckbox);
            height = 30f;
            clipChildren = true;

            UISprite sprite = AddUIComponent<UISprite>();
            sprite.spriteName = "ToggleBase";
            sprite.size = new Vector2(19f, 19f);
            sprite.relativePosition = new Vector2(0, (height-sprite.height)/2 );

            checkedBoxObject = sprite.AddUIComponent<UISprite>();
            ((UISprite)checkedBoxObject).spriteName = "ToggleBaseFocused";
            checkedBoxObject.size = sprite.size;
            checkedBoxObject.relativePosition = Vector3.zero;

            label = AddUIComponent<UILabel>();
            label.text = "No junction markings";
            label.textScale = 0.9f;
            label.relativePosition = new Vector2(sprite.width+5f, (height- label.height)/2+1);
            tooltip = "Removes defuse texture for all segment ends";

            eventCheckChanged += (component, value) => {
                if (refreshing_)
                    return;
                Apply();
            };

        }

        public override void Start() {
            base.Start();
            width = parent.width;
        }

        public void Apply() {
            if (VERBOSE) Log.Debug("UIHideMarkingsCheckbox.Apply called()\n" + Environment.StackTrace);
            NodeData data = UINodeControllerPanel.Instance.NodeData;
            if (data == null)
                return;
            data.ClearMarkings = this.isChecked;
            Assert(!refreshing_, "!refreshing_");
            data.Refresh();
            UINodeControllerPanel.Instance.Refresh();
        }

        // protection against unncessary apply/refresh/infinite recursion.
        bool refreshing_ = false;

        public void Refresh() {
            if (VERBOSE) Log.Debug("Refresh called()\n" + Environment.StackTrace);
            refreshing_ = true;
            NodeData data = UINodeControllerPanel.Instance.NodeData;
            if (data == null) {
                Disable();
                return;
            }

            this.isChecked = data.ClearMarkings;

            parent.isVisible = isVisible = this.isEnabled = data.ShowClearMarkingsToggle();
            parent.Invalidate();
            Invalidate();
            refreshing_ = false;
        }
    }
}



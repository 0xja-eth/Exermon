using UnityEngine;

using LitJson;

using Core.Data.Loaders;

using Core.UI;
using Core.UI.Utils;

namespace UI.Common.Controls.ParamDisplays {

    /// <summary>
    /// 属性条组接受器
    /// </summary>
    public class ParamDisplaysGroupWrapper : ParamDisplay {

        /// <summary>
        /// 属性组
        /// </summary>
        public ParamDisplaysGroup group;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            group = group ?? SceneUtils.get<ParamDisplaysGroup>(transform);
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="value"></param>
        public override void setValue(JsonData value) {
            base.setValue(value);
            var values = DataLoader.load<JsonData[]>(value);
            if (values == null) group?.clearValues();
            else group?.setValues(values);
        }

        public override void clearValue() {
            base.clearValue();
            group?.clearValues();
        }

        #endregion

        /// <summary>
        /// 刷新
        /// </summary>
        protected override void refreshMain() {}
    }
}
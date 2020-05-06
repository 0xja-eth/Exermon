using System;
using System.Collections.Generic;

using UnityEngine.UI;
using UnityEngine.Events;

using ItemModule.Data;

namespace UI.Common.Controls.ItemDisplays {

    /// <summary>
    /// 容器项详情显示
    /// </summary
    public class PackContItemDisplay : SelectableItemDisplay<PackContItem> {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Image icon; // 图片
        public Text name, count;

        /// <summary>
        /// 绘制函数
        /// </summary>
        Dictionary<int, UnityAction<PackContItem>> drawFuncs =
            new Dictionary<int, UnityAction<PackContItem>>();

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            initializeDrawFuncs();
        }

        /// <summary>
        /// 初始化绘制函数
        /// </summary>
        protected virtual void initializeDrawFuncs() { }

        #endregion

        #region 绘制函数控制

        /// <summary>
        /// 注册物品类型
        /// </summary>
        /// <typeparam name="T">物品类型</typeparam>
        /// <param name="func">绘制函数</param>
        public virtual void registerItemType<T>(
            UnityAction<T> func) where T : PackContItem {
            UnityAction<PackContItem> func_ =
                (item) => func?.Invoke((T)item);

            var typeName = typeof(T).Name;
            var enumType = typeof(BaseContItem.Type);
            var type = (int)Enum.Parse(enumType, typeName);
            drawFuncs.Add(type, func_);
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        /// <param name="item">物品</param>
        protected override void drawExactlyItem(PackContItem item) {
            base.drawExactlyItem(item);

            if (item.isNullItem()) drawEmptyItem();
            else if (drawFuncs.ContainsKey(item.type))
                drawFuncs[item.type]?.Invoke(item);
            else drawEmptyItem();
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void clearItem() {
            if (icon) icon.gameObject.SetActive(false);
            if (name) name.text = "";
            if (count) count.text = "";
        }

        #endregion
    }
}
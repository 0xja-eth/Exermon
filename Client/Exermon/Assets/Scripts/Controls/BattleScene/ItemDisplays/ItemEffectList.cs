
using UnityEngine;
using UnityEngine.UI;

using Core.UI;
using Core.UI.Utils;

using ItemModule.Data;
using PlayerModule.Data;
using QuestionModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.BattleScene.Controls.ItemDisplays {

    /// <summary>
    /// 普通星星显示
    /// </summary>
    public class ItemEffectList : ContainerDisplay<EffectData> {

        /// <summary>
        /// 效果显示间隔
        /// </summary>
        const float DeltaShowTime = 0.25f;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            registerUpdateLayout(container);
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 设置物品
        /// </summary>
        /// <param name="count">数目</param>
        public void setItem(HumanItem item) {
            setItems(item.effects);
        }
        public void setItem(QuesSugar item) {
            setItems(item.convertToEffectData());
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 创建子视图
        /// </summary>
        protected override void createSubViews() {
            CoroutineUtils.resetActions();
            base.createSubViews();
            doRoutine(CoroutineUtils.generateCoroutine());
        }

        /// <summary>
        /// 创建子窗口（动画）
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        protected override void createSubView(EffectData item, int index) {
            var subView = createSubView(index);
            CoroutineUtils.addAction(() =>
                subView.startView(item), DeltaShowTime);
        }

        #endregion

    }
}
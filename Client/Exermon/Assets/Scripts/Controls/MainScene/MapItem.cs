
using UnityEngine;
using UnityEngine.UI;

using Core.UI;
using Core.UI.Utils;

namespace UI.MainScene.Controls {
    /// <summary>
    /// 地图项
    /// </summary>
    public class MapItem : BaseView {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Button button; 
        public Animation effect;

        /// <summary>
        /// 外部变量设置
        /// </summary>
        public MainScene.BuildingType type;

        public int maxDelta = 3000, minDelta = 300;

        /// <summary>
        /// 内部变量设置
        /// </summary>
        int counter = 0;
        int nextDelta = 0;
        
        #region 初始化

        /// <summary>
        /// 初次初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            setupOnClick();
        }

        /// <summary>
        /// 配置点击回调
        /// </summary>
        void setupOnClick() {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => scene().onBulidingsClick(type));
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 场景引用
        /// </summary>
        /// <returns></returns>
        MainScene scene() {
            return SceneUtils.getCurrentScene<MainScene>();
        }

        #endregion

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
            updateEffect();
        }

        /// <summary>
        /// 更新闪光效果
        /// </summary>
        void updateEffect() {
            if (!effect || !button.interactable) return;
            if (effect.isPlaying)
                // 正在播放，重置状态
                counter = nextDelta = 0;
            else if (nextDelta <= 0)
                // 播放完毕，开始计时
                nextDelta = Random.Range(minDelta, maxDelta);
            else if (counter++ >= nextDelta)
                // 时间到达，开始播放动画
                effect.Play();
        }

        #endregion

    }
}

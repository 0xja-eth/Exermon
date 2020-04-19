
using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;

using Core.UI;
using Core.UI.Utils;

using ItemModule.Data;

using BattleModule.Data;
using BattleModule.Services;

using UI.Common.Controls.ItemDisplays;

namespace UI.BattleScene.Controls.ItemDisplays {

    /// <summary>
    /// 对战物资槽物品显示
    /// </summary
    public class ItemEffectDisplay : ItemDisplay<EffectData> {

        /// <summary>
        /// 常量定义
        /// </summary>
        const string ShowAniName = "";
        const string HideAniName = "";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Image tagImage;
        public Text shortDesc;

        public Texture2D recoveryTag, promotionTag;
        
        /// <summary>
        /// 外部系统声明
        /// </summary>
        BattleService battleSer;

        /// <summary>
        /// 内部变量定义
        /// </summary>
        BaseWindow selfWindow;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            selfWindow = SceneUtils.get<BaseWindow>(gameObject);
        }

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            battleSer = BattleService.get();
        }

        #endregion

        #region 启动/结束控制

        /// <summary>
        /// 启动视窗
        /// </summary>
        public override void startView() {
            base.startView();
            if (selfWindow) selfWindow.startWindow();
        }

        /// <summary>
        /// 结束视窗
        /// </summary>
        public override void terminateView() {
            shown = false;
            if (selfWindow) selfWindow.terminateWindow();
            else base.terminateView();
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        /// <param name="effect">效果</param>
        protected override void drawExactlyItem(EffectData effect) {
            base.drawExactlyItem(effect);
            Texture2D texture = null;
            if (effect.isRecovery()) texture = recoveryTag;
            if (effect.isPromotion()) texture = promotionTag;
            if (texture == null) drawEmptyItem();
            else {
                tagImage.gameObject.SetActive(true);
                tagImage.overrideSprite =
                    AssetLoader.generateSprite(texture);
                shortDesc.text = effect.shortDescription;
            }
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void clearItem() {
            tagImage.gameObject.SetActive(false);
            shortDesc.text = "";
        }

        #endregion
    }
}
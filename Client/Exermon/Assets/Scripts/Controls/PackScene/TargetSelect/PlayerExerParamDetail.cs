
using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;

using GameModule.Services;

using ItemModule.Data;

using ExermonModule.Data;
using ExermonModule.Services;

using UI.Common.Controls.ParamDisplays;
using UI.Common.Controls.ItemDisplays;

using UI.PackScene.Windows;

namespace UI.PackScene.Controls.TargetSelect {

    /// <summary>
    /// 状态窗口艾瑟萌页属性信息显示
    /// </summary>
    public class PlayerExerParamDetail : ItemDetailDisplay<PlayerExermon> {

        /// <summary>
        /// 常量定义
        /// </summary>
        const string LevelTextFormat = "Lv.{0}";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Text name, level;

        public ParamDisplaysGroup paramsInfo;
        public MultParamsDisplay battlePoint;
        public MultParamsDisplay expBar;

        public TargetSelectWindow selectWindow;

        /// <summary>
        /// 外部系统设置
        /// </summary>
        ExermonService exerSer;
        DataService dataSer;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            configureParamsGroup();
        }

        /// <summary>
        /// 配置属性组
        /// </summary>
        void configureParamsGroup() {
            var params_ = dataSer.staticData.configure.baseParams;
            paramsInfo.configure(params_);
        }

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            dataSer = DataService.get();
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 配置预览
        /// </summary>
        void setupPreview() {
            this.item.clearPreviewObject();
            var preview = this.item.createPreviewObject();
            // 模拟道具使用
            var item = selectWindow.operItem() as UsableItem;
            var count = selectWindow.currentCount();

            if (item != null)
                CalcService.GeneralItemEffectProcessor.
                    process(item, count, preview);

            preview.refresh();
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 是否为空物品
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool isNullItem(PlayerExermon item) {
            return base.isNullItem(item) || item.isNullItem();
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 绘制主体信息
        /// </summary>
        /// <param name="slotItem">艾瑟萌槽项</param>
        protected override void drawExactlyItem(PlayerExermon playerExer) {
            base.drawExactlyItem(playerExer);
            setupPreview();

            drawBaseInfo(playerExer);
            drawParamsInfo(playerExer);
            drawExpInfo(playerExer);
        }

        /// <summary>
        /// 绘制基本信息
        /// </summary>
        /// <param name="slotItem">艾瑟萌槽项</param>
        void drawBaseInfo(PlayerExermon playerExer) {
            var exermon = playerExer.exermon();
            name.text = playerExer.name();
            level.text = string.Format(LevelTextFormat, playerExer.level);
        }

        /// <summary>
        /// 绘制艾瑟萌图像
        /// </summary>
        /// <param name="slotItem">艾瑟萌槽项</param>
        void drawParamsInfo(PlayerExermon playerExer) {
            paramsInfo?.setValues(playerExer, "preview_params");
            battlePoint?.setValue(playerExer, "preview_battle_point");
        }

        /// <summary>
        /// 绘制经验信息
        /// </summary>
        /// <param name="slotItem">艾瑟萌槽项</param>
        void drawExpInfo(PlayerExermon playerExer) {
            expBar?.setValue(playerExer, "preview_exp");
        }
        
        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void drawEmptyItem() {
            base.drawEmptyItem();
            name.text = level.text = "";
            battlePoint?.clearValue();
            paramsInfo?.clearValues();
            expBar?.clearValue();
        }

        #endregion
    }
}
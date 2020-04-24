using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;
using Core.Systems;
using Core.UI;
using Core.UI.Utils;

using PlayerModule.Data;

using GameModule.Services;

using PlayerModule.Services;
using BattleModule.Services;

using UI.Common.Controls.ParamDisplays;

/// <summary>
/// 对战开始场景窗口
/// </summary>
namespace UI.BattleStartScene.Windows {

    using Controls.Main;

    /// <summary>
    /// 主窗口
    /// </summary>
    public class MainWindow : BaseWindow {

        /// <summary>
        /// 半身像高度
        /// </summary>
        const int BustHeight = 616;

        /// <summary>
        /// 文本常量定义
        /// </summary>
        const string BattlePointFormat = "总战斗力：{0}";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Image bust;
        public Text name, battlePoint;
        public MultParamsDisplay expBar;

        public BattleItemSlotDisplay battleItemSlotDisplay;
        public SmallExerSlotDisplay exerSlotDisplay;

        /// <summary>
        /// 内部变量声明
        /// </summary>
        Player player;

        /// <summary>
        /// 场景组件引用
        /// </summary>
        BattleStartScene scene;

        /// <summary>
        /// 外部系统引用
        /// </summary>
        PlayerService playerSer = null;

        #region 初始化

        /// <summary>
        /// 初始化场景
        /// </summary>
        protected override void initializeScene() {
            scene = (BattleStartScene)SceneUtils.getSceneObject("Scene");
        }

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            playerSer = PlayerService.get();
        }

        #endregion

        #region 开启控制
        
        #endregion

        #region 数据控制

        #endregion

        #region 界面控制

        /// <summary>
        /// 绘制基本信息
        /// </summary>
        void drawBaseInfo() {
            name.text = player.name;
            battlePoint.text = string.Format(BattlePointFormat, 
                player.sumBattlePoint());
            expBar.setValue(player, "exp");
            drawBust();
        }

        /// <summary>
        /// 绘制半身像
        /// </summary>
        void drawBust() {
            var bust = AssetLoader.getCharacterBustSprite(
                player.characterId, BustHeight);
            this.bust.gameObject.SetActive(true);
            this.bust.overrideSprite = bust;
        }

        /// <summary>
        /// 绘制艾瑟萌槽
        /// </summary>
        void drawExerSlot() {
            var exerSlot = player.slotContainers.exerSlot;
            exerSlotDisplay.setItems(exerSlot.items);
        }

        /// <summary>
        /// 绘制对战物资槽
        /// </summary>
        void drawBattleItemSlot() {
            var battleItemSlot = player.slotContainers.battleItemSlot;
            battleItemSlotDisplay.setSlotData(battleItemSlot);
        }

        /// <summary>
        /// 刷新窗口
        /// </summary>
        protected override void refresh() {
            player = playerSer.player;
            base.refresh();
            drawBaseInfo();
            drawExerSlot();
            drawBattleItemSlot();
        }
        
        /// <summary>
        /// 清除基本信息
        /// </summary>
        void clearBaseInfo() {
            name.text = battlePoint.text = "";
            expBar.clearValue();

            bust.gameObject.SetActive(false);
            bust.overrideSprite = null;
        }

        /// <summary>
        /// 清除容器
        /// </summary>
        void clearContainers() {
            exerSlotDisplay.requestClear(true);
            battleItemSlotDisplay.requestClear(true);
        }

        /// <summary>
        /// 清除窗口
        /// </summary>
        protected override void clear() {
            base.clear();
            clearBaseInfo();
            clearContainers();
        }

        #endregion
        
    }
}
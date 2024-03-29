﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using LitJson;

using Core.Systems;
using Core.UI;
using Core.UI.Utils;

using ExermonModule.Data;

using GameModule.Services;
using PlayerModule.Services;

using UI.Common.Windows;

using UI.StartScene.Controls.ExerGift;

namespace UI.StartScene.Windows {

    /// <summary>
    /// 选择天赋窗口
    /// </summary>
    public class GiftsWindow : BaseWindow {

        /// <summary>
        /// 文本常量定义
        /// </summary>
        const string InvalidSelectionAlertText = "请为每个艾瑟萌都装备上天赋！";
        const string ConfirmText = "游戏开始后将清空余下天赋，确认当前的选择吗？";

        const string CreateSuccessText = "装备艾瑟萌天赋成功！";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public ExerSlotsContainer exerSlot; // 艾瑟萌槽
        public ExerGiftsContainer exerGifts; // 艾瑟萌天赋

        /// <summary>
        /// 场景组件引用
        /// </summary>
        StartScene scene;

        /// <summary>
        /// 外部系统引用
        /// </summary>
        GameSystem gameSys = null;
        DataService dataSer = null;
        PlayerService playerSer = null;

        #region 初始化

        /// <summary>
        /// 初次初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            configureSubViews();
        }

        /// <summary>
        /// 初始化场景
        /// </summary>
        protected override void initializeScene() {
            scene = SceneUtils.getCurrentScene<StartScene>();
        }

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            gameSys = GameSystem.get();
            dataSer = DataService.get();
            playerSer = PlayerService.get();
        }

        /// <summary>
        /// 配置子组件
        /// </summary>
        void configureSubViews() {
            configureGiftsView();
            configureExermonsView();
        }

        /// <summary>
        /// 配置天赋视窗
        /// </summary>
        void configureGiftsView() {
            var exerGifts = dataSer.staticData.data.exerGifts;
            var initGifts = new List<ExerGift>();
            foreach (var exer in exerGifts)
                if (exer.gType == 1) initGifts.Add(exer);
            this.exerGifts.configure(initGifts);
            this.exerGifts.startView(-1);
        }

        /// <summary>
        /// 配置艾瑟萌视窗
        /// </summary>
        void configureExermonsView() {
            var exerSlot = playerSer.player.slotContainers.exerSlot;
            if (exerSlot == null) this.exerSlot.configure();
            else this.exerSlot.configure(exerSlot.items);
        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 创建角色
        /// </summary>
        public void create() {
            if (check()) confirmCreate();
            else onCheckFailed();
        }

        /// <summary>
        /// 不正确的格式
        /// </summary>
        void onCheckFailed() {
            gameSys.requestAlert(InvalidSelectionAlertText);
        }

        /// <summary>
        /// 确认
        /// </summary>
        void confirmCreate() {
            gameSys.requestAlert(ConfirmText,
                AlertWindow.Type.YesOrNo, doCreate);
        }

        /// <summary>
        /// 执行创建
        /// </summary>
        void doCreate() {
            int[] gids = exerSlot.getGiftIds();

            playerSer.createGifts(gids, onCreateSuccess);
        }

        /// <summary>
        /// 选择艾瑟萌成功回调
        /// </summary>
        void onCreateSuccess() {
            gameSys.requestAlert(CreateSuccessText);
            scene.refresh();
        }

        #region 数据校验

        /// <summary>
        /// 检查是否可以登陆
        /// </summary>
        bool check() {
            return exerSlot.checkSelection();
        }

        #endregion

        #endregion

        #region 界面控制

        #endregion
    }
}
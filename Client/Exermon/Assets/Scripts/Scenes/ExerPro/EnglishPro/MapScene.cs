
using Core.Systems;
using Core.UI;

using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;

/// <summary>
/// 艾瑟萌特训UI
/// </summary>
namespace UI.ExerPro { }

/// <summary>
/// 艾瑟萌英语特训UI
/// </summary>
namespace UI.ExerPro.EnglishPro { }

/// <summary>
/// 地图场景
/// </summary>
namespace UI.ExerPro.EnglishPro.MapScene {

    using Controls;

    /// <summary>
    /// 地图场景
    /// </summary>
    public class MapScene : BaseScene {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public MapDisplay mapDisplay;

        /// <summary>
        /// 外部系统设置
        /// </summary>
        EnglishService engSer;

        #region 初始化

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            engSer = EnglishService.get();
        }

        /// <summary>
        /// 场景名称
        /// </summary>
        /// <returns></returns>
        public override string sceneName() {
            return SceneSystem.Scene.EnglishProMapScene;
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected override void start() {
            base.start();
            mapDisplay.setItem(engSer.record);

        }

        #endregion
    }
}

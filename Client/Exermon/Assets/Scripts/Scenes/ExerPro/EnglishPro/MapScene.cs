
using Core.Systems;
using Core.UI;

using ExerPro.EnglishModule.Data;

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
            test();
        }

        /// <summary>
        /// 测试
        /// </summary>
        void test() {
            mapDisplay.setItem(new MapStageRecord(1, 1));
        }
    }
}

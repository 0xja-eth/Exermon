using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Core.UI;
using Core.UI.Utils;

using Core.Systems;

using RecordModule.Services;

using UI.ExerciseScene.Windows;

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

    /// <summary>
    /// 地图场景
    /// </summary>
    public class MapScene : BaseScene {

        /// <summary>
        /// 场景名称
        /// </summary>
        /// <returns></returns>
        public override string sceneName() {
            return SceneSystem.Scene.EnglishProMapScene;
        }
    }
}

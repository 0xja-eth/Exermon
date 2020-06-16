using Core.Systems;
using Core.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.ExerPro.EnglishPro.ExerProPackScene.Windows;

namespace UI.ExerPro.EnglishPro.ExerProPackScene {
    public class ExerProPackScene : BaseScene {

        public ExerProPackWindow exerProPackWindow;


        /// <summary>
        /// 场景名
        /// </summary>
        /// <returns>场景名</returns>
        public override SceneSystem.Scene sceneIndex() {
            return SceneSystem.Scene.EnglishProPackScene;
        }


        /// <summary>
        /// 开始
        /// </summary>
        protected override void start() {
            base.start();
            refresh();
        }


        #region 场景控制

        /// <summary>
        /// 刷新场景
        /// </summary>
        public void refresh() {
            exerProPackWindow.startWindow();
        }

        #endregion
    }
}

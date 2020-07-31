using Core.Systems;
using Core.UI;
using ExerPro.EnglishModule.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.ExerPro.EnglishPro.CorrectionScene.Controls.Test;

//改错布局
namespace Assets.Scripts.Controls.ExerPro.English.CorrectionScene.Test {
    class CorrectTestScene : BaseScene {
        public ArticleTestDisplay articleDisplay;
        public override SceneSystem.Scene sceneIndex() {
            return SceneSystem.Scene.CorrectTestScene;
        }

        protected override void start() {
            base.start();
            CorrectionQuestion a = new CorrectionQuestion();
            articleDisplay.startView();
            articleDisplay.setItems(a.sentences());
        }
    }
}

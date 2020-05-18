using System;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Core.UI {

    /// <summary>
    /// 布局重建控制器
    /// </summary>
    public class RebuildController : BaseComponent {
        
        /// <summary>
        /// 外部组件设置
        /// </summary>
        public RectTransform[] contents; // 需要重建布局的变换

        /// <summary>
        /// 外部变量设置
        /// </summary>
        public int minLayoutStableFrame = 0; // 最小布局稳定帧
        public int maxLayoutStableFrame = 0; // 最大布局稳定帧

        #region 初始化

        /// <summary>
        /// 内部变量声明
        /// </summary>
        protected override void start() {
            base.start();
            foreach (var c in contents)
                registerUpdateLayout(c);
        }

        #endregion

        #region 更新控制

        /// <summary>
        /// 注册布局更新（仅用于挂载 Layout 的物体）
        /// </summary>
        /// <param name="rect">物体 RectTransform</param>
        public void registerUpdateLayout(Transform rect) {
            registerUpdateLayout((RectTransform)rect);
        }
        public void registerUpdateLayout(RectTransform rect) {
            doRoutine(updateLayout(rect));
        }

        /// <summary>
        /// 更新布局（仅用于挂载 Layout 的物体）
        /// </summary>
        /// <param name="rect">物体 RectTransform</param>
        /// <returns></returns>
        IEnumerator updateLayout(RectTransform rect) {
            int cnt = 0;
            bool active = rect.gameObject.activeInHierarchy;
            float width = rect.rect.width, height = rect.rect.height;
            while (true) {
                try {
                    // LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
                    
                    if (maxLayoutStableFrame == 0)
                        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
                    else if (++cnt > minLayoutStableFrame && cnt <= maxLayoutStableFrame)
                        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
                    else {
                        bool newActive = rect.gameObject.activeInHierarchy;
                        float newWidth = rect.rect.width, newHeight = rect.rect.height;
                        if (newActive != active ||
                            newWidth != width || newHeight != height) {
                            cnt = 0; active = newActive;
                            width = newWidth; height = newHeight;
                        }
                    }
                    
                } catch (Exception e) {
                    Debug.LogWarning(e + ": " + e.StackTrace);
                    break;
                }
                yield return null; // new WaitForEndOfFrame();
            }

        }

        #endregion

        #region 启动/结束控制
        
        #endregion
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpriteSleeper
{
    // MonoBehaviour that shouild be attached to Canvas GameObjects that allow for sleeping
    public class SpriteSleeperCanvas : MonoBehaviour
    {
        // Private variables
        private Canvas _canvas;

        private List<SpriteSleeperImage> _spriteSleepers;

        private SleepState _currentSleepState = SleepState.Awake;

        private SpriteSleeperManager _manager;

        private bool _hasCanvas = false;

        // The current state of the canvas
		// 当前的状态
        private enum SleepState
        {
            Sleeping,
            Awake
        }

        private void Awake()
        {
			// 获取Canvas
            _canvas = GetComponent<Canvas>();
            if (_canvas == null)
            {
                Debug.LogError("SpriteSleeperCanvas is unable to find a Canvas component on the current object. Ensure that this component is added to the GameObject containing the Canvas.");
                return;
            }

			// 只能挂载在主Canvas上
            if( !_canvas.isRootCanvas )
            {
                Debug.LogError("SpriteSleeperCanvas should only be used on root canvases.");
                return;
            }

            _hasCanvas = true;

			// 创建SpriteSleeperManager
			// 并加入此Canvas
            _manager = SpriteSleeperManager.Instance();

            if (_manager == null || _manager.Equals(null))
            {
                Debug.LogError("SpriteSleeperCanvas is unable to find a SpriteSleeperManager.");
                return;
            }

            _manager.AddCanvas(this);

			// 创建SpriteSleeperImage列表
            _spriteSleepers = new List<SpriteSleeperImage>();

			// 遍历Canvas下的，所有Image
			// 在上面挂载SpriteSleeperImage
			// 保存到_spriteSleepers中
            RefreshImageList();
			
			//

            // Allow for canvases that start disabled to unref their images
			// 执行OnCanvasHierarchyChanged函数
            Invoke("OnCanvasHierarchyChanged", 0.0001f);
        }

        private void OnDestroy()
        {
            if(_manager != null && !_manager.Equals(null))
                _manager.RemoveCanvas(this);
        }

        // Re-find all the images in the hierarchy under this GameObject
		// 遍历Canvas下的，所有Image
		// 在上面挂载SpriteSleeperImage
		// 保存到_spriteSleepers中
        void RefreshImageList()
        {
            _spriteSleepers.Clear();
            
            var images = gameObject.GetComponentsInChildren<Image>(true);
            foreach (var image in images)
            {
                // Find or add the SpriteSleeperImage component
                var sleeperImage = image.GetComponent<SpriteSleeperImage>();
                if (sleeperImage == null)
                {
                    sleeperImage = image.gameObject.AddComponent<SpriteSleeperImage>();
                }

                _spriteSleepers.Add(sleeperImage);
            }
        }

		// 根据Canvas状态变化，调用
		// Wake(), Sleep()函数
		// 分别调用SpriteSleeperImage的Sleep()函数
        protected void OnCanvasHierarchyChanged()
        {
            if (_hasCanvas)
            {
				// 根据Canvas，设置SleepState
				// 如果状态发生了变化
                SleepState state = (_canvas.isActiveAndEnabled) ? SleepState.Awake : SleepState.Sleeping;
                if (state != _currentSleepState)
                {
                    _currentSleepState = state;

                    if (state == SleepState.Awake)
                    {
						//
                        Wake();
                    }
                    else
                    {
#if UNITY_EDITOR
                        if (!_canvas.gameObject.activeInHierarchy )
                        {
                            if(UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode && 
                                UnityEditor.EditorApplication.isPlaying)
                                Debug.LogWarning("Did you know it's more efficient to disable the Canvas component than to deactivate the GameObject? Read more here https://unity3d.com/learn/tutorials/topics/best-practices/other-ui-optimization-techniques-and-tips");
                        }
#endif
                        Sleep();
                    }
                }
            }
        }

		// 调用SpriteSleeperImage的Sleep()函数
        void Sleep()
        {
            foreach (var sleeper in _spriteSleepers)
            {
                sleeper.Sleep();
            }
        }

		// 调用SpriteSleeperImage的Wake()函数
        void Wake()
        {
            foreach (var sleeper in _spriteSleepers)
            {
                sleeper.Wake();
            }
        }
    }
}

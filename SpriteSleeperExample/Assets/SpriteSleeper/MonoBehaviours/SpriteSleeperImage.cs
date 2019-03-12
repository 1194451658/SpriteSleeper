using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpriteSleeper
{
    // MonoBehaviour that should be attached to any Image that should be allowed to sleep
    public class SpriteSleeperImage : MonoBehaviour
    {
        // Public variables, possibly for serialization
        public string Tag = null;
        public string SpriteName = null;

        // Private variables
        private Image _image = null;
        private SpriteSleeperManager _manager = null;

        public void Start()
        {
			// 获取到挂载的Image
            _image = GetComponent<Image>();

			// 获取SpriteSleeperManager实例
            _manager = SpriteSleeperManager.Instance();

			// 每加载一个atlas，就寻找下是自己？！
            _manager.OnAtlasLoaded += FindTag;

			// 从SpriteSleeperManager中找到本Image，所在的atlas
			// 记录其tag
			// 并标记引用此tag
            FindTag();

            Invoke("StopListeningForAtlases", 0.0001f);
        }

        private void OnDestroy()
        {
            StopListeningForAtlases();
        }

        private void StopListeningForAtlases()
        {
            // If we haven't found a tag by now, we probably won't
            if (string.IsNullOrEmpty(Tag))
            {
                _manager.OnAtlasLoaded -= FindTag;
            }
        }

		// 从SpriteSleeperManager中找到本Image，所在的atlas
		// 记录其tag
		// 并标记引用此tag
        private void FindTag()
        {
            if (string.IsNullOrEmpty(Tag) && _image != null && _manager != null && !_manager.Equals(null))
            {
				// 获取Image控件的Sprite
                Sprite sprite = _image.sprite;
                if (sprite != null)
                {
					// 保存自己的sprite名称
                    SpriteName = sprite.name;

					// sprite对应的texture
                    Texture2D texture = sprite.texture;
                    if (texture != null)
                    {
						// 从SpriteSleeperManager中，获取texture对应的tag
                        string tag = _manager.GetTag(texture);
                        if (tag != null)
                        {
                            StopListeningForAtlases();
                            Tag = tag;

							//
                            _manager.RefTag(Tag);
                        }
                    }
                }
            }
        }

        public void Sleep()
        {
            if (!string.IsNullOrEmpty(Tag))
            {
                if (_manager != null && !_manager.Equals(null))
                {
                    _manager.UnrefTag(Tag);
					// 将自己的sprite清掉，能够删除？！
                    _image.sprite = null;

                }
            }
        }

		// 重新取回sprite
        public void Wake()
        {
            if (!string.IsNullOrEmpty(Tag))
            {
                if (_manager != null && !_manager.Equals(null))
                {
                    _manager.RefTag(Tag);
                    _image.sprite = _manager.GetSprite(Tag, SpriteName);
                }
            }
        }
    }
}

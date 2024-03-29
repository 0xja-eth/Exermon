﻿
using UnityEngine;

using ItemModule.Data;
using System.Collections.Generic;

/// <summary>
/// 核心框架代码
/// </summary>
namespace Core { }

/// <summary>
/// 核心数据
/// </summary>
namespace Core.Data { }

/// <summary>
/// 数据读取器
/// </summary>
namespace Core.Data.Loaders {

    /// <summary>
    /// 资源管理器
    /// </summary>>
    public static class AssetLoader {

        /// <summary>
        /// 资源路径定义
        /// </summary>
        public const string CharacterBustPath = "Character/Bust/";
        public const string CharacterFacePath = "Character/Face/";
        public const string CharacterBattlePath = "Character/Battle/";
        public const string ItemIconPath = "Item/";
        public const string ExermonFullPath = "Exermon/Full/";
        public const string ExermonIconPath = "Exermon/Icon/";
        public const string ExermonBattlePath = "Exermon/Battle/";
        public const string ExerGiftIconPath = "ExerGift/Icon/";
        public const string ExerSkillIconPath = "Exermon/Skill/Icon";
        public const string ExerSkillAniPath = "Exermon/Skill/Ani";
        public const string ExerSkillTargetPath = "Exermon/Skill/Target";

		public const string SystemPath = "System/";

		public const string ExerProEnemyPath = "ExerPro/Enemy/";
		public const string ExerProItemPath = "ExerPro/Item/";
		public const string ExerProCardPath = "ExerPro/Card/";
		public const string ExerProStatePath = "ExerPro/State/";

		public const string ExerProSystemPath = "ExerPro/System/";

		//public const string ExerProListeningAudioPath = "ExerPro/ListeningAudio/";

		/// <summary>
		/// 文件主体名称定义
		/// </summary>
		public const string IconsFileName = "Icons";
		public const string IconFileName = "Icon";

		public const string CharacterFileName = "Character";
		public const string ExermonFileName = "Exermon";
        public const string ExerGiftFileName = "BigExerGift";
        public const string BigExerGiftFileName = "BigExerGift";
        public const string ExerSkillFileName = "Skill";
        public const string RankIconsFileName = "RankIcons";
        public const string SmallRankIconsFileName = "SmallRankIcons";

		public const string AnimationFileName = "Animation/Animation";

		public const string ExerProNodeIconFileName = "Node/Type";

		public const string ExerProEnemyBattleFileName = "Battle/Enemy";
		public const string ExerProEnemyThinkIconFileName = "Think/Icon";

		public const string ExerProCardSkinFileName = "Skin/Skin";
		public const string ExerProCardCharFrameFileName = "Skin/Character";
		public const string ExerProCardTypeIconFileName = "Skin/Type";

		//public const string ListeningAudioFileName = "ListeningAudio";

		/// <summary>
		/// 其他常量定义
		/// </summary>
		//public const int ItemIconCols = 10; // 物品图标列数
		public const int ItemIconSize = 96; // 物品尺寸（正方形）
		public const int RankIconCnt = 6; // 段位数量
        public const int MaxSubRank = 5; // 最大子段位数目

		public static readonly Vector2 CardIconSize = new Vector2(96, 144); // 卡牌尺寸
		public const int StateIconSize = 32; // 状态图标尺寸（正方形）

		/// <summary>
		/// 纹理/声音缓存
		/// </summary>
		static Dictionary<string, Texture2D> cacheTexture = new Dictionary<string, Texture2D>();
		static Dictionary<string, AudioClip> cacheAudio = new Dictionary<string, AudioClip>();
		static Dictionary<string, AnimationClip> cacheAni = new Dictionary<string, AnimationClip>();

		#region 加载资源封装

		/// <summary>
		/// 读取2D纹理
		/// </summary>
		/// <param name="path">路径</param>
		/// <param name="fileName">文件名</param>
		/// <returns>2D纹理</returns>
		public static Texture2D loadTexture2D(string path, string fileName) {
            var key = path + fileName;
            if (!cacheTexture.ContainsKey(key))
                cacheTexture[key] = Resources.Load<Texture2D>(key);
            Debug.Log("loadTexture2D: " + key + ": " + (cacheTexture[key] != null));
            return cacheTexture[key];
        }
        /// <param name="name">文件主体名称</param>
        /// <param name="id">序号</param>
        static Texture2D loadTexture2D(string path, string name, int id) {
            return loadTexture2D(path, name + "_" + id);
        }

		/// <summary>
		/// 读取音频
		/// </summary>
		/// <param name="path">路径</param>
		/// <param name="fileName">文件名</param>
		/// <returns>音频文件</returns>
		public static AudioClip loadAudio(string path, string fileName) {
			var key = path + fileName;
			if (!cacheAudio.ContainsKey(key))
				cacheAudio[key] = Resources.Load<AudioClip>(key);
			Debug.Log("loadAudioClip: " + key + ": " + (cacheAudio[key] != null));
			return cacheAudio[key];
		}
		/// <param name="id">文件id</param>
		/// <returns>音频文件</returns>
		public static AudioClip loadAudio(string path, string name, int id) {
			return loadAudio(path, name + "_" + id);
		}

		/// <summary>
		/// 读取动画
		/// </summary>
		/// <param name="path">路径</param>
		/// <param name="fileName">文件名</param>
		/// <returns>音频文件</returns>
		public static AnimationClip loadAnimation(string path, string fileName) {
			var key = path + fileName;
			if (!cacheAni.ContainsKey(key))
				cacheAni[key] = Resources.Load<AnimationClip>(key);
			Debug.Log("loadAnimationClip: " + key + ": " + (cacheAni[key] != null));
			return cacheAni[key];
		}
		/// <param name="id">文件id</param>
		/// <returns>音频文件</returns>
		public static AnimationClip loadAnimation(string path, string name, int id) {
			return loadAnimation(path, name + "_" + id);
		}
		public static AnimationClip loadAnimation(int id) {
			return loadAnimation(SystemPath, AnimationFileName, id);
		}

		#region 纹理资源处理

		/// <summary>
		/// 计算纹理截取区域 
		/// </summary>
		/// <param name="texture">纹理</param>
		/// <param name="xSize">单个图标宽度</param>
		/// <param name="ySize">单个图标高度</param>
		/// <param name="xIndex">图标X索引</param>
		/// <param name="yIndex">图标Y索引</param>
		/// <returns>返回要截取的区域</returns>
		public static Rect calcRect(Texture2D texture, 
            int xSize, int ySize, int xIndex, int yIndex) {
            int h = texture.height;
            int x = xIndex * xSize, y = h - (yIndex+1) * ySize;

            return new Rect(x, y, xSize, ySize);
        }
        /// <param name="index">图标索引</param>
        public static Rect calcRect(Texture2D texture, int xSize, int ySize, int index) {
            int w = texture.width; int cols = w / xSize;
            int xIndex = index % cols, yIndex = index / cols;

            return calcRect(texture, xSize, ySize, xIndex, yIndex);
        }

        /// <summary>
        /// 生成精灵
        /// </summary>
        /// <param name="texture">纹理</param>
        /// <param name="rect">截取矩形</param>
        /// <returns></returns>
        public static Sprite generateSprite(Texture2D texture, Rect rect = default) {
            if (rect == default)
                rect = new Rect(0, 0, texture.width, texture.height);
            return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
		}

		#endregion

		#endregion

		#region 加载单个资源

		/// <summary>
		/// 读取人物半身像
		/// </summary>
		/// <param name="id">人物ID</param>
		/// <returns>2D纹理</returns>
		public static Texture2D loadCharacterBust(int id) {
            return loadTexture2D(CharacterBustPath, CharacterFileName, id);
        }

        /// <summary>
        /// 获取指定高度的人物半身像
        /// </summary>
        /// <param name="id">人物ID</param>
        /// <param name="height">高度</param>
        /// <returns><返回对应的精灵/returns>
        public static Sprite getCharacterBustSprite(int id,
            int height = PlayerModule.Data.Character.BustHeight) {
            var bust = loadCharacterBust(id);
            if (height == 0) height = bust.height;
            var rect = new Rect(0, bust.height - height, bust.width, height);
            return generateSprite(bust, rect);
        }

        /// <summary>
        /// 读取人物头像
        /// </summary>
        /// <param name="id">人物ID</param>
        /// <returns>2D纹理</returns>
        public static Texture2D loadCharacterFace(int id) {
            return loadTexture2D(CharacterFacePath, CharacterFileName, id);
        }

        /// <summary>
        /// 获取指定高度的人物半身像
        /// </summary>
        /// <param name="id">人物ID</param>
        /// <param name="height">高度</param>
        /// <returns><返回对应的精灵/returns>
        public static Sprite getCharacterFaceSprite(int id) {
            return generateSprite(loadCharacterFace(id));
        }

        /// <summary>
        /// 读取人物战斗图
        /// </summary>
        /// <param name="id">人物ID</param>
        /// <returns>2D纹理</returns>
        public static Texture2D loadCharacterBattle(int id) {
            return loadTexture2D(CharacterBattlePath, CharacterFileName, id);
        }

        /// <summary>
        /// 读取艾瑟萌全身像
        /// </summary>
        /// <param name="id">艾瑟萌ID</param>
        /// <returns>2D纹理</returns>
        public static Texture2D loadExermonFull(int id) {
            return loadTexture2D(ExermonFullPath, ExermonFileName, id);
        }

        /// <summary>
        /// 读取艾瑟萌缩略图
        /// </summary>
        /// <param name="id">艾瑟萌ID</param>
        /// <returns>2D纹理</returns>
        public static Texture2D loadExermonIcon(int id) {
            return loadTexture2D(ExermonIconPath, ExermonFileName, id);
        }

        /// <summary>
        /// 读取艾瑟萌战斗图
        /// </summary>
        /// <param name="id">艾瑟萌ID</param>
        /// <returns>2D纹理</returns>
        public static Texture2D loadExermonBattle(int id) {
            return loadTexture2D(ExermonBattlePath, ExermonFileName, id);
        }

        /// <summary>
        /// 读取艾瑟萌战斗图
        /// </summary>
        /// <param name="id">艾瑟萌ID</param>
        /// <returns>2D纹理</returns>
        public static Texture2D loadExerGift(int id) {
            return loadTexture2D(ExerGiftIconPath, ExerGiftFileName, id);
        }

        /// <summary>
        /// 读取艾瑟萌战斗图
        /// </summary>
        /// <param name="id">艾瑟萌ID</param>
        /// <returns>2D纹理</returns>
        public static Texture2D loadBigExerGift(int id) {
            return loadTexture2D(ExerGiftIconPath, BigExerGiftFileName, id);
        }

        /// <summary>
        /// 读取艾瑟萌技能图标
        /// </summary>
        /// <param name="id">艾瑟萌技能ID</param>
        /// <returns>2D纹理</returns>
        public static Texture2D loadExerSkillIcon(int id) {
            return loadTexture2D(ExerSkillIconPath, ExerSkillFileName, id);
        }

        /// <summary>
        /// 读取艾瑟萌技能动画
        /// </summary>
        /// <param name="id">艾瑟萌技能ID</param>
        /// <returns>2D纹理</returns>
        public static Texture2D loadExerSkillAni(int id) {
            return loadTexture2D(ExerSkillAniPath, ExerSkillFileName, id);
        }

        /// <summary>
        /// 读取艾瑟萌技能目标动画
        /// </summary>
        /// <param name="id">艾瑟萌技能ID</param>
        /// <returns>2D纹理</returns>
        public static Texture2D loadExerSkillTarget(int id) {
            return loadTexture2D(ExerSkillTargetPath, ExerSkillFileName, id);
        }

		#region ExerPro

		/// <summary>
		/// 读取据点类型图标
		/// </summary>
		/// <param name="id">据点类型ID</param>
		/// <returns>2D纹理</returns>
		public static Texture2D loadNodeIcon(int id) {
            return loadTexture2D(ExerProSystemPath, ExerProNodeIconFileName, id);
		}

		/// <summary>
		/// 读取敌人行动思考类型图标
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static Texture2D loadEnemyThink(int id) {
			Debug.Log("loadEnemyThink: " + id);
			return loadTexture2D(ExerProEnemyPath, ExerProEnemyThinkIconFileName, id);
		}

		/// <summary>
		/// 读取特训敌人全身像
		/// </summary>
		/// <param name="id">艾瑟萌ID</param>
		/// <returns>2D纹理</returns>
		public static Texture2D loadExerProEnemyBattle(int id) {
			return loadTexture2D(ExerProEnemyPath, ExerProEnemyBattleFileName, id);
		}

		/// <summary>
		/// 获取卡牌图标所占的区域
		/// </summary>
		/// <param name="index">图标索引</param>
		/// <returns>返回对应图标索引的精灵</returns>
		public static Sprite getExerProCardIconSprite(int index) {
			var texture = loadTexture2D(ExerProCardPath, IconFileName, index);
			if (texture != null) return generateSprite(texture);
			return null;
		}

		/// <summary>
		/// 诅咒卡牌皮肤索引
		/// </summary>
		static int EvilCardSkinIndex = 0;

		/// <summary>
		/// 读取特训敌人全身像
		/// </summary>
		/// <param name="star">星级</param>
		/// <param name="type">类型</param>
		/// <param name="skinIndex">皮肤索引</param>
		/// <returns>2D纹理</returns>
		public static Texture2D loadCardSkin(int star, int type, int skinIndex = 0) {
			var index = skinIndex > 0 ? skinIndex : star;
			if (type == (int)ExerPro.EnglishModule.Data.ExerProCard.Type.Evil)
				index = EvilCardSkinIndex;

			return loadTexture2D(ExerProCardPath, ExerProCardSkinFileName, index);
		}

		/// <summary>
		/// 读取性质框
		/// </summary>
		/// <param name="star">星级</param>
		/// <returns>2D纹理</returns>
		public static Texture2D loadCardCharFrame(int star, int skinIndex = 0) {
			var index = skinIndex > 0 ? skinIndex : star;
			return loadTexture2D(ExerProCardPath, ExerProCardCharFrameFileName, index);
		}

		/// <summary>
		/// 读取类型图标
		/// </summary>
		/// <param name="star">星级</param>
		/// <returns>2D纹理</returns>
		public static Texture2D loadCardTypeIcon(int type) {
			return loadTexture2D(ExerProCardPath, ExerProCardTypeIconFileName, type);
		}

		///// <summary>
		///// 读取听力音频
		///// </summary>
		///// <param name="id">音频ID</param>
		///// <returns>音频</returns>
		//public static AudioClip loadListeningAudioClip(int id) {
		//	return loadAudio(ExerProListeningAudioPath, ListeningAudioFileName, id);
		//}

		#endregion

		#endregion

		#region 加载组合资源

		/// <summary>
		/// 获取组合资源所占的区域（图标类资源）
		/// </summary>
		/// <param name="index">图标索引</param>
		/// <param name="path">路径</param>
		/// <param name="name">文件名</param>
		/// <param name="xSize">X尺寸</param>
		/// <param name="ySize">Y尺寸</param>
		/// <returns>返回对应图标索引的矩形区域</returns>
		public static Rect getGroupAssetsRect(int index, 
			string path, string name, int xSize, int ySize) {
			return calcRect(loadTexture2D(path, name), xSize, ySize, index);
		}

		/// <summary>
		/// 读取组合资源精灵（图标类资源）
		/// </summary>
		/// <param name="index">图标索引</param>
		/// <param name="path">路径</param>
		/// <param name="name">文件名</param>
		/// <param name="xSize">X尺寸</param>
		/// <param name="ySize">Y尺寸</param>
		/// <returns>返回对应图标索引的精灵</returns>
		public static Sprite getGroupAssetsSprite(int index,
			string path, string name, int xSize, int ySize) {
			var texture = loadTexture2D(path, name);
			var rect = calcRect(texture, xSize, ySize, index);
			return generateSprite(texture, rect);
		}

		/// <summary>
		/// 获取物品图标精灵
		/// </summary>
		/// <param name="index">图标索引</param>
		/// <returns>返回对应图标索引的精灵</returns>
		public static Sprite getItemIconSprite(int index) {
			return getGroupAssetsSprite(index,
				ItemIconPath, IconsFileName, 
				ItemIconSize, ItemIconSize);
		}

		/// <summary>
		/// 获取特训物品图标精灵
		/// </summary>
		/// <param name="index">图标索引</param>
		/// <returns>返回对应图标索引的精灵</returns>
		public static Sprite getExerProItemIconSprite(int index) {
			return getGroupAssetsSprite(index,
				ExerProItemPath, IconsFileName, 
				ItemIconSize, ItemIconSize);
		}

		/// <summary>
		/// 获取特训物品图标精灵
		/// </summary>
		/// <param name="index">图标索引</param>
		/// <returns>返回对应图标索引的精灵</returns>
		public static Sprite getExerProStateIconSprite(int index) {
			return getGroupAssetsSprite(index,
				ExerProStatePath, IconsFileName, 
				StateIconSize, StateIconSize);
		}
		
		/// <summary>
		/// 读取段位图标
		/// </summary>
		/// <param name="id">物品ID</param>
		/// <returns>返回段位图标集纹理</returns>
		public static Texture2D loadRankIcons(bool small = false) {
            return loadTexture2D(SystemPath, small ? 
                SmallRankIconsFileName : RankIconsFileName);
        }

        /// <summary>
        /// 获取段位图标所占的区域 
        /// </summary>
        /// <param name="id">段位ID</param>
        /// <param name="subRank">子段位编号</param>
        /// <param name="small">是否加载小图标</param>
        /// <returns>返回对应段位的矩形区域</returns>
        public static Rect getRankIconRect(int id, int subRank = 0, bool small = false) {
            var texture = loadRankIcons(small);
            int w = texture.width, h = texture.height;
            int xSize, ySize;
            if (small) {
                xSize = w / MaxSubRank; ySize = h / RankIconCnt;
                return calcRect(texture, xSize, ySize, id - 1, subRank);
            } else {
                xSize = w / RankIconCnt; ySize = h;
                return calcRect(texture, xSize, ySize, id - 1);
            }
        }

        /// <summary>
        /// 获取段位图标精灵
        /// </summary>
        /// <param name="id">段位ID</param>
        /// <param name="subRank">子段位编号</param>
        /// <param name="small">是否加载小图标</param>
        /// <returns>返回对应段位的精灵</returns>
        public static Sprite getRankIconSprite(int id, int subRank = 0, bool small = false) {
            var texture = loadRankIcons(small);
            var rect = getRankIconRect(id, subRank, small);
            return generateSprite(texture, rect);
        }

        #endregion

    }
}
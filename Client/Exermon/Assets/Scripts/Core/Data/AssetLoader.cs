
using UnityEngine;

using ItemModule.Data;

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
        public const string ItemIconPath = "Item/Icon/";
        public const string ExermonFullPath = "Exermon/Full/";
        public const string ExermonIconPath = "Exermon/Icon/";
        public const string ExermonBattlePath = "Exermon/Battle/";
        public const string ExerGiftIconPath = "ExerGift/Icon/";
        public const string ExerSkillIconPath = "Exermon/Skill/Icon";
        public const string ExerSkillAniPath = "Exermon/Skill/Ani";
        public const string ExerSkillTargetPath = "Exermon/Skill/Target";

        /// <summary>
        /// 文件主体名称定义
        /// </summary>
        public const string CharacterFileName = "Character";
        //public const string ItemFileName = "Item";
        public const string ExermonFileName = "Exermon";
        public const string ExerGiftFileName = "ExerGift";
        public const string BigExerGiftFileName = "BigExerGift";
        public const string ExerSkillFileName = "Skill";

        /// <summary>
        /// 读取2D纹理
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="fileName">文件名</param>
        /// <returns>2D纹理</returns>
        static Texture2D loadTexture2D(string path, string fileName) {
            return Resources.Load<Texture2D>(path + fileName);
        }
        /// <param name="name">文件主体名称</param>
        /// <param name="id">序号</param>
        static Texture2D loadTexture2D(string path, string name, int id) {
            return loadTexture2D(path, name + "_" + id);
        }

        /// <summary>
        /// 读取人物半身像
        /// </summary>
        /// <param name="id">人物ID</param>
        /// <returns>2D纹理</returns>
        public static Texture2D loadCharacterBust(int id) {
            return loadTexture2D(CharacterBustPath, CharacterFileName, id);
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
        /// 读取人物战斗图
        /// </summary>
        /// <param name="id">人物ID</param>
        /// <returns>2D纹理</returns>
        public static Texture2D loadCharacterBattle(int id) {
            return loadTexture2D(CharacterBattlePath, CharacterFileName, id);
        }

        /// <summary>
        /// 读取物品图标
        /// </summary>
        /// <param name="id">物品ID</param>
        /// <returns>2D纹理</returns>
        public static Texture2D loadItemIcon(int type, int id) {
            return loadItemIcon((BaseItem.Type)type, id);
        }
        public static Texture2D loadItemIcon(BaseItem.Type type, int id) {
            return loadTexture2D(ItemIconPath, type.ToString(), id);
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

    }
}
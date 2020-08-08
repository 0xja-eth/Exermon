from enum import Enum

# region 枚举设定


# ===================================================
#  物品类型枚举
# ===================================================
class ItemType(Enum):

	Unset = 0  # 未知物品
	# General
	GameItem = 1  # 游戏道具
	GameEquip = 2  # 游戏装备
	GameCard = 11  # 游戏卡牌
	# Exermon
	Exermon = 101  # 艾瑟萌
	ExerGift = 102  # 艾瑟萌天赋
	ExerFrag = 103  # 艾瑟萌碎片
	ExerSkill = 104  # 艾瑟萌技能
	# Battle
	BattleEnemy = 201  # 战斗敌人
	BattleState = 202  # 战斗状态

	# ExerProItem 301+
	ExerProItem = 301  # 特训物品
	ExerProPotion = 302  # 特训药水
	ExerProCard = 303  # 特训卡片
	ExerProEnemy = 310  # 特训敌人
	ExerProState = 311  # 特训状态


ITEM_TYPES = [

	(ItemType.Unset.value, "未知物品"),
	# General
	(ItemType.GameItem.value, "游戏道具"),
	(ItemType.GameEquip.value, "游戏装备"),
	(ItemType.GameCard.value, "游戏卡牌"),
	# Exermon
	(ItemType.Exermon.value, "艾瑟萌"),
	(ItemType.ExerGift.value, "艾瑟萌天赋"),
	(ItemType.ExerFrag.value, "艾瑟萌碎片"),
	(ItemType.ExerSkill.value, "艾瑟萌技能"),
	# Battle
	(ItemType.BattleEnemy.value, "战斗敌人"),
	(ItemType.BattleState.value, "战斗状态"),
	# ExerProItem
	(ItemType.ExerProItem.value, '特训物品'),
	(ItemType.ExerProPotion.value, '特训药水'),
	(ItemType.ExerProCard.value, '特训卡片'),
	(ItemType.ExerProEnemy.value, '特训敌人'),
	(ItemType.ExerProState.value, '特训状态'),
]


# ===================================================
#  容器类型枚举
# ===================================================
class ContainerType(Enum):

	Unset = 0  # 未知容器
	# General
	ItemPack = 1  # 道具背包
	EquipPack = 2  # 装备背包
	CardGroup = 11  # 卡组
	# Exermon
	ExerHub = 101  # 艾瑟萌
	ExerGiftPool = 102  # 艾瑟萌天赋池
	ExerFragPack = 103  # 艾瑟萌碎片
	ExerSlot = 111  # 艾瑟萌槽
	ExerEquipSlot = 112  # 艾瑟萌装备槽
	ExerSkillSlot = 113  # 艾瑟萌技能槽
	# Battle
	BattleItemSlot = 201  # 战斗物资槽


CONTAINER_TYPES = [

	(ContainerType.Unset.value, "未知容器"),
	# General
	(ContainerType.ItemPack.value, "道具背包"),
	(ContainerType.EquipPack.value, "装备背包"),
	(ContainerType.CardGroup.value, "卡组"),
	# Exermon
	(ContainerType.ExerHub.value, "艾瑟萌"),
	(ContainerType.ExerGiftPool.value, "艾瑟萌天赋池"),
	(ContainerType.ExerFragPack.value, "艾瑟萌碎片"),
	(ContainerType.ExerSlot.value, "艾瑟萌槽"),
	(ContainerType.ExerEquipSlot.value, "艾瑟萌装备槽"),
	(ContainerType.ExerSkillSlot.value, "艾瑟萌技能槽"),
	# Battle
	(ContainerType.BattleItemSlot.value, "战斗物资槽"),
]


# ===================================================
#  容器物品枚举
# ===================================================
class ContItemType(Enum):

	Unset = 0  # 未知容器
	# General
	ItemPackItem = 1  # 道具背包
	EquipPackItem = 2  # 装备背包
	CardGroupItem = 11  # 卡组
	# Exermon
	PlayerExermon = 101  # 艾瑟萌
	PlayerExerGift = 102  # 艾瑟萌天赋池
	ExerFragPackItem = 103  # 艾瑟萌碎片
	ExerSlotItem = 111  # 艾瑟萌槽
	ExerEquipSlotItem = 112  # 艾瑟萌装备槽
	ExerSkillSlotItem = 113  # 艾瑟萌技能槽
	# Battle
	BattleItemSlotItem = 201  # 战斗物资槽


CONT_ITEM_TYPES = [

	(ContItemType.Unset.value, "未知容器"),
	# General
	(ContItemType.ItemPackItem.value, "道具背包"),
	(ContItemType.EquipPackItem.value, "装备背包"),
	(ContItemType.CardGroupItem.value, "卡组"),
	# Exermon
	(ContItemType.PlayerExermon.value, "艾瑟萌"),
	(ContItemType.PlayerExerGift.value, "艾瑟萌天赋池"),
	(ContItemType.ExerFragPackItem.value, "艾瑟萌碎片"),
	(ContItemType.ExerSlotItem.value, "艾瑟萌槽"),
	(ContItemType.ExerEquipSlotItem.value, "艾瑟萌装备槽"),
	(ContItemType.ExerSkillSlotItem.value, "艾瑟萌技能槽"),
	# Battle
	(ContItemType.BattleItemSlotItem.value, "战斗物资槽"),
]

# endregion

from game_module.models import *
from player_module.models import *
from exermon_module.models import *
from ..models import *
import csv, os


PATH_ROOT = "item_module/raw_data/"

FILENAMES = {
	HumanItem: "human_item.csv",
	ExerEquip: "exer_equip.csv"
}


# 上传数据库
def upload(type_: type):

	if type_ not in FILENAMES: return []

	filename = os.path.join(PATH_ROOT, FILENAMES[type_])

	with open(filename, 'r', encoding="GBK") as f:
		reader = csv.reader(f)
		keys = next(reader)

		for row in reader:
			row = processRow(keys, row)
			if row != {}: saveData(type_, row)


# 处理行数据
def processRow(keys, row):

	data = {}
	for i in range(len(keys)):
		if row[i] != '': data[keys[i]] = row[i]

	return data


# 保存一项数据
def saveData(type_, data: dict):

	if type_ == HumanItem: return _saveHumanItem(data)
	if type_ == ExerEquip: return _saveExerEquip(data)

	return None


def load(d: dict, key, empty=None):
	if key in d: return d[key]
	return empty

# region 人类物品保存


def _saveHumanItem(d: dict):

	id = load(d, 'id')

	if id is None: return

	print("Processing HumanItem "+id)

	item = HumanItem.objects.filter(id=id)
	flag = item.exists()

	if not flag:
		item = HumanItem()
		print("[Created new]")
	else:
		item = item.first()
		print("[Loaded from database]")

	item.id = id
	item.name = load(d, 'name', "")
	item.description = load(d, 'description', "")
	item.icon_index = load(d, 'icon_index', 0)
	item.i_type_id = load(d, 'i_type_id', 1)
	item.star_id = load(d, 'star_id', 1)
	item.max_count = load(d, 'max_count', 0)
	item.battle_use = load(d, 'battle_use', 'False').upper() == 'TRUE'
	item.menu_use = load(d, 'menu_use', 'False').upper() == 'TRUE'
	item.adventure_use = load(d, 'adventure_use', 'False').upper() == 'TRUE'
	item.consumable = load(d, 'consumable', 'False').upper() == 'TRUE'
	item.discardable = load(d, 'discardable', 'True').upper() == 'TRUE'
	item.tradable = load(d, 'tradable', 'True').upper() == 'TRUE'
	item.sell_price = load(d, 'sell_price', 0)
	item.batch_count = load(d, 'batch_count', 0)
	item.target = load(d, 'target', 0)
	item.freeze = load(d, 'freeze', 0)

	item.save()

	if flag: price = item.buyPrice()
	else: price = HumanItemPrice()

	price.gold = load(d, 'gold_price', 0)
	price.ticket = load(d, 'ticket_price', 0)
	price.bound_ticket = load(d, 'bound_ticket_price', 0)
	price.item_id = id
	price.save()

	__saveHumanItemEffects(item, flag, d)


def __saveHumanItemEffects(item: HumanItem, flag, d: list):

	index = 1
	code_key_format = "code%d"
	params_key_format = "params%d"
	code_value_format = "ItemEffectCode.%s.value"
	params_format = "[%s]"

	code_key = code_key_format % index
	params_key = params_key_format % index

	effects = []

	while code_key in d and params_key in d:
		effect = HumanItemEffect()
		effect.code = eval(code_value_format % d[code_key])
		effect.params = eval(params_format % d[params_key])

		effects.append(effect)

		index += 1

		code_key = code_key_format % index
		params_key = params_key_format % index

	if flag: ori_effects = item.effects() or []
	else: ori_effects = []

	ori_cnt = len(ori_effects)
	new_cnt = len(effects)

	for i in range(ori_cnt):
		ori_effect = ori_effects[i]
		if i >= new_cnt:  # 有多余的效果
			ori_effect.delete()
		else:
			effect = effects[i]
			ori_effect.code = effect.code
			ori_effect.params = effect.params
			ori_effect.save()

	if new_cnt > ori_cnt: # 新数目比旧数目多
		for i in range(ori_cnt, new_cnt):
			effect = effects[i]
			effect.item_id = item.id
			effect.save()

# endregion

# region 艾瑟萌装备保存


def _saveExerEquip(d: dict):

	id = load(d, 'id')

	if id == "": return

	print("Processing ExerEquip "+id)

	item = ExerEquip.objects.filter(id=id)
	flag = item.exists()

	if not flag:
		item = ExerEquip()
		print("[Created new]")
	else:
		item = item.first()
		print("[Loaded from database]")

	item.id = id
	item.name = load(d, 'name', "")
	item.description = load(d, 'description', "")
	item.icon_index = load(d, 'icon_index', 0)
	item.min_level = load(d, 'min_level', 0)
	item.e_type_id = load(d, 'e_type_id', 1)
	item.star_id = load(d, 'star_id', 1)
	item.discardable = load(d, 'discardable', "True").upper() == 'TRUE'
	item.tradable = load(d, 'tradable', "True").upper() == 'TRUE'
	item.sell_price = load(d, 'sell_price', 0)

	item.save()

	if flag: price = item.buyPrice()
	else: price = ExerEquipPrice()

	price.gold = load(d, 'gold_price', 0)
	price.ticket = load(d, 'ticket_price', 0)
	price.bound_ticket = load(d, 'bound_ticket_price', 0)
	price.item_id = id
	price.save()

	__saveExerEquipParams(item, flag, d)


def __saveExerEquipParams(item: ExerEquip, flag, d: dict):

	level_param_format = "level_param%d"
	base_param_format = "base_param%d"

	level_params = []
	base_params = []

	params = BaseParam.objs()

	for param in params:
		id = param.id

		level_param_key = level_param_format % id
		base_param_key = base_param_format % id

		if level_param_key in d:
			p = ExerEquipLevelParam(param_id=id)
			p.setValue(float(d[level_param_key]))

			level_params.append(p)

		if base_param_key in d:
			p = ExerEquipBaseParam(param_id=id)
			val = float(d[base_param_key])
			if id == 5 or id == 6: val /= 100
			p.setValue(val)

			base_params.append(p)

	if flag: ori_level_params = item.levelParams() or []
	else: ori_level_params = []

	ori_cnt = len(ori_level_params)
	new_cnt = len(level_params)

	for i in range(ori_cnt):
		ori_param = ori_level_params[i]
		if i >= new_cnt:  # 有多余的效果
			ori_param.delete()
		else:
			param = level_params[i]
			ori_param.value = param.value
			ori_param.param_id = param.param_id
			ori_param.save()

	if new_cnt > ori_cnt: # 新数目比旧数目多
		for i in range(ori_cnt, new_cnt):
			param = level_params[i]
			param.equip_id = item.id
			param.save()

	if flag: ori_base_params = item.baseParams() or []
	else: ori_base_params = []

	ori_cnt = len(ori_base_params)
	new_cnt = len(base_params)

	for i in range(ori_cnt):
		ori_param = ori_base_params[i]
		if i >= new_cnt:  # 有多余的效果
			ori_param.delete()
		else:
			param = base_params[i]
			ori_param.value = param.value
			ori_param.param_id = param.param_id
			ori_param.save()

	if new_cnt > ori_cnt: # 新数目比旧数目多
		for i in range(ori_cnt, new_cnt):
			param = base_params[i]
			param.equip_id = item.id
			param.save()


# endregion


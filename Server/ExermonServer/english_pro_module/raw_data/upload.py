from game_module.models import *
from player_module.models import *
from exermon_module.models import *
from ..models import *
import csv, os


PATH_ROOT = "english_pro_module/raw_data/"

FILENAMES = {
	ExerProCard: "exer_pro_card.csv",
	ExerProEnemy: "exer_pro_enemy.csv",
	ExerProPotion: "exer_pro_potion.csv",
	ExerProItem: "exer_pro_item.csv",
	ExerProState: "exer_pro_state.csv",
}


# 上传数据库
def upload(type_):
	type_: type = eval(type_)

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

	if type_ == ExerProCard: return _saveExerProCard(data)
	if type_ == ExerProEnemy: return _saveExerProEnemy(data)
	if type_ == ExerProPotion: return _saveExerProPotion(data)
	if type_ == ExerProItem: return _saveExerProItem(data)
	if type_ == ExerProState: return _saveExerProState(data)

	return None


def load(d: dict, key, empty=None, choices=None):

	if empty is not None:
		type_ = type(empty)
	else: type_ = None

	if key in d:
		val = d[key]

		if choices is None:
			if type_ is None: return val
			return type_(val)

		elif isinstance(choices, type) and \
			GroupConfigure in choices.__bases__:
			choices = choices.objs()

			for choice in choices:
				if val == choice.name: return choice.id

		elif isinstance(choices, list):

			for choice in choices:
				if val == choice[1]: return choice[0]

	return empty

# region ExerProCard


def _saveExerProCard(d: dict):

	id = load(d, 'id')

	if id is None: return

	print("Processing ExerProCard "+id)

	item = ExerProCard.objects.filter(id=id)
	flag = item.exists()

	impl = load(d, 'impl', 0)
	if impl == 0:
		if flag:
			item.first().delete()
			print("[Deleted]")
		return None

	if not flag:
		item = ExerProCard()
		print("[Created new]")
	else:
		item = item.first()
		print("[Loaded from database]")

	print(d)

	item.id = id
	item.name = load(d, 'name', "")
	item.description = load(d, 'description', "")
	item.character = load(d, 'character', "")
	item.icon_index = load(d, 'icon_index', 0)
	item.start_ani_index = load(d, 'start_ani_index', 0)
	item.target_ani_index = load(d, 'target_ani_index', 0)
	item.cost = load(d, 'cost', 0)
	item.inherent = load(d, 'inherent', 0) == 1
	item.disposable = load(d, 'disposable', 0) == 1
	item.card_type = load(d, 'card_type', 1, ExerProCard.CARD_TYPES)
	item.target = load(d, 'target', 0, ExerProCard.TARGETS)
	item.star_id = load(d, 'star', 1, ExerProItemStar)

	item.save()

	__saveExerProCardEffects(item, flag, d)


def __saveExerProCardEffects(item: ExerProCard, flag, d: list):

	index = 1
	code_key_format = "code%d"
	params_key_format = "params%d"
	code_value_format = "ExerProEffectCode.%s.value"
	params_format = "[%s]"

	code_key = code_key_format % index
	params_key = params_key_format % index

	effects = []

	while code_key in d and params_key in d:

		effect = ExerProCardEffect()
		effect.code = eval(code_value_format % d[code_key])
		effect.params = eval(params_format % d[params_key])

		if effect.code != ExerProEffectCode.Unset.value:
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

# region ExerProEnemy


def _saveExerProEnemy(d: dict):

	id = load(d, 'id')

	if id is None: return

	print("Processing ExerProEnemy %s: \n%s" % (id, d))

	item = ExerProEnemy.objects.filter(id=id)
	flag = item.exists()

	impl = load(d, 'impl', 0)
	if impl == 0:
		if flag:
			item.first().delete()
			print("[Deleted]")
		return None

	if not flag:
		item = ExerProEnemy()
		print("[Created new]")
	else:
		item = item.first()
		print("[Loaded from database] %s" % item)

	item.id = id
	item.name = load(d, 'name', "")
	item.description = load(d, 'description', "")
	item.character = load(d, 'character', "")
	item.type = load(d, 'type', 0, ExerProEnemy.ENEMY_TYPES)
	item.mhp = load(d, 'mhp', 0)
	item.power = load(d, 'power', 0)
	item.defense = load(d, 'defense', 0)

	item.save()

	__saveExerProEnemyActions(item, flag, d)
	__saveExerProEnemyEffects(item, flag, d)


def __saveExerProEnemyEffects(item: ExerProEnemy, flag, d: list):

	index = 1
	code_key_format = "code%d"
	params_key_format = "params%d"
	code_value_format = "ExerProEffectCode.%s.value"
	params_format = "[%s]"

	code_key = code_key_format % index
	params_key = params_key_format % index

	effects = []

	while code_key in d:

		params = load(d, params_key, "")

		effect = EnemyEffect()
		effect.code = eval(code_value_format % d[code_key])
		effect.params = eval(params_format % params)

		if effect.code != ExerProEffectCode.Unset.value:
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
			effect.enemy_id = item.id
			effect.save()


def __saveExerProEnemyActions(item: ExerProEnemy, flag, d: list):

	index = 1
	rounds_key_format = "rounds%d"
	type_key_format = "act_type%d"
	params_key_format = "params%d"
	rate_key_format = "rate%d"

	params_format = "[%s]"

	type_key = type_key_format % index
	rounds_key = rounds_key_format % index
	params_key = params_key_format % index
	rate_key = rate_key_format % index

	actions = []

	while type_key in d and rate_key in d:

		rate = d[rate_key]
		if rate != "":

			rounds = load(d, rounds_key, "")
			params = load(d, params_key, "")

			action = EnemyAction()
			action.rounds = eval(params_format % rounds)
			action.params = eval(params_format % params)
			action.rate = int(rate)

			action.type = load(d, type_key, 1, EnemyAction.TYPES)

			actions.append(action)

		index += 1

		type_key = type_key_format % index
		rounds_key = rounds_key_format % index
		params_key = params_key_format % index
		rate_key = rate_key_format % index

	print("Actions: ")
	print(actions)

	if flag: ori_actions = item.actions() or []
	else: ori_actions = []

	ori_cnt = len(ori_actions)
	new_cnt = len(actions)

	for i in range(ori_cnt):
		ori_action = ori_actions[i]
		if i >= new_cnt:  # 有多余的效果
			ori_action.delete()
		else:
			action = actions[i]
			ori_action.rounds = action.rounds
			ori_action.type = action.type
			ori_action.params = action.params
			ori_action.rate = action.rate
			ori_action.save()

	if new_cnt > ori_cnt: # 新数目比旧数目多
		for i in range(ori_cnt, new_cnt):
			action = actions[i]
			action.enemy_id = item.id
			action.save()


# endregion


# region ExerProPotion


def _saveExerProPotion(d: dict):

	id = load(d, 'id')

	if id is None: return

	print("Processing ExerProPotion "+id)

	item = ExerProPotion.objects.filter(id=id)
	flag = item.exists()

	impl = load(d, 'impl', 0)
	if impl == 0:
		if flag:
			item.first().delete()
			print("[Deleted]")
		return None

	if not flag:
		item = ExerProPotion()
		print("[Created new]")
	else:
		item = item.first()
		print("[Loaded from database]")

	item.id = id
	item.name = load(d, 'name', "")
	item.description = load(d, 'description', "")
	item.icon_index = load(d, 'icon_index', 0)
	item.start_ani_index = load(d, 'start_ani_index', 0)
	item.target_ani_index = load(d, 'target_ani_index', 0)
	item.star_id = load(d, 'star', 1, ExerProItemStar)

	item.save()

	__saveExerProPotionEffects(item, flag, d)


def __saveExerProPotionEffects(item: ExerProPotion, flag, d: list):

	index = 1
	code_key_format = "code%d"
	params_key_format = "params%d"
	code_value_format = "ExerProEffectCode.%s.value"
	params_format = "[%s]"

	code_key = code_key_format % index
	params_key = params_key_format % index

	effects = []

	while code_key in d and params_key in d:

		effect = ExerProPotionEffect()
		effect.code = eval(code_value_format % d[code_key])
		effect.params = eval(params_format % d[params_key])

		if effect.code != ExerProEffectCode.Unset.value:
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


# region ExerProItem


def _saveExerProItem(d: dict):

	id = load(d, 'id')

	if id is None: return

	print("Processing ExerProItem "+id)

	item = ExerProItem.objects.filter(id=id)
	flag = item.exists()

	impl = load(d, 'impl', 0)
	if impl == 0:
		if flag:
			item.first().delete()
			print("[Deleted]")
		return None

	if not flag:
		item = ExerProItem()
		print("[Created new]")
	else:
		item = item.first()
		print("[Loaded from database]")

	item.id = id
	item.name = load(d, 'name', "")
	item.description = load(d, 'description', "")
	item.icon_index = load(d, 'icon_index', 0)
	item.star_id = load(d, 'star', 1, ExerProItemStar)

	item.save()

	__saveExerProItemTraits(item, flag, d)


def __saveExerProItemTraits(item: ExerProItem, flag, d: list):

	index = 1
	code_key_format = "trait%d"
	params_key_format = "params%d"
	code_value_format = "ExerProTraitCode.%s.value"
	params_format = "[%s]"

	code_key = code_key_format % index
	params_key = params_key_format % index

	traits = []

	while code_key in d and params_key in d:

		trait = ExerProItemTrait()
		trait.code = eval(code_value_format % d[code_key])
		trait.params = eval(params_format % d[params_key])

		if trait.code != ExerProTraitCode.Unset.value:
			traits.append(trait)

		index += 1

		code_key = code_key_format % index
		params_key = params_key_format % index

	if flag: ori_traits = item.traits() or []
	else: ori_traits = []

	ori_cnt = len(ori_traits)
	new_cnt = len(traits)

	for i in range(ori_cnt):
		ori_trait = ori_traits[i]
		if i >= new_cnt:  # 有多余的效果
			ori_trait.delete()
		else:
			trait = traits[i]
			ori_trait.code = trait.code
			ori_trait.params = trait.params
			ori_trait.save()

	if new_cnt > ori_cnt:  # 新数目比旧数目多
		for i in range(ori_cnt, new_cnt):
			trait = traits[i]
			trait.item_id = item.id
			trait.save()

# endregion


# region ExerProState


def _saveExerProState(d: dict):

	id = load(d, 'id')

	if id is None: return

	print("Processing ExerProState "+id)

	item = ExerProState.objects.filter(id=id)
	flag = item.exists()

	impl = load(d, 'impl', 0)
	if impl == 0:
		if flag:
			item.first().delete()
			print("[Deleted]")
		return None

	if not flag:
		item = ExerProState()
		print("[Created new]")
	else:
		item = item.first()
		print("[Loaded from database]")

	item.id = id
	item.name = load(d, 'name', "")
	item.description = load(d, 'description', "")
	item.icon_index = load(d, 'icon_index', 0)
	item.is_nega = load(d, 'is_nega', False)
	item.max_turns = load(d, 'max_turns', 0)
	item.star_id = load(d, 'star', 1, ExerProItemStar)

	item.save()

	__saveExerProStateTraits(item, flag, d)


def __saveExerProStateTraits(item: ExerProState, flag, d: list):

	index = 1
	code_key_format = "trait%d"
	params_key_format = "params%d"
	code_value_format = "ExerProTraitCode.%s.value"
	params_format = "[%s]"

	code_key = code_key_format % index
	params_key = params_key_format % index

	traits = []

	while code_key in d and params_key in d:

		trait = ExerProStateTrait()
		trait.code = eval(code_value_format % d[code_key])
		trait.params = eval(params_format % d[params_key])

		if trait.code != ExerProTraitCode.Unset.value:
			traits.append(trait)

		index += 1

		code_key = code_key_format % index
		params_key = params_key_format % index

	if flag: ori_traits = item.traits() or []
	else: ori_traits = []

	ori_cnt = len(ori_traits)
	new_cnt = len(traits)

	for i in range(ori_cnt):
		ori_trait = ori_traits[i]
		if i >= new_cnt:  # 有多余的效果
			ori_trait.delete()
		else:
			trait = traits[i]
			ori_trait.code = trait.code
			ori_trait.params = trait.params
			ori_trait.save()

	if new_cnt > ori_cnt:  # 新数目比旧数目多
		for i in range(ori_cnt, new_cnt):
			trait = traits[i]
			trait.item_id = item.id
			trait.save()

# endregion

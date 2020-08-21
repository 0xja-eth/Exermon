"""
用于数据迁移的脚本
"""

from utils.model_utils import CoreDataManager, DataManager

from .models import *

import json

base_path = './game_module/raw_data/'


def export():
	
	for model in CoreDataManager.getCoreData():
		exportModel(model)

	exportModel(GameVersion)
	exportModel(GameConfigure)


def exportModel(model):

	file_name = model.__name__
	file_name = DataManager.hump2Underline(file_name)
	path = base_path + file_name + '.json'

	print("Exporting model %s to %s" % (model.__name__, path))

	objects = model.objects.all()

	res = [object.export() for object in objects]

	with open(path, 'w+', encoding='utf-8') as f:
		json.dump(res, f)


def import_():

	for model in CoreDataManager.getCoreData():
		importModel(model)

	importModel(GameVersion)
	importModel(GameConfigure)


def importModel(model):

	file_name = model.__name__
	file_name = DataManager.hump2Underline(file_name)
	path = base_path + file_name + '.json'

	print("Importing model %s from %s" % (model.__name__, path))

	with open(path, 'r', encoding='utf-8') as f:
		data = json.load(f)

	objects = model.objects.all()

	old_len = len(objects)
	new_len = len(data)

	for i in range(old_len):
		old_obj = objects[i]
		if i >= new_len:
			old_obj.delete()
		else:
			old_obj.load(data[i])

	for i in range(old_len, new_len):
		model(_data=data[i])


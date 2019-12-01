
# from django.conf import settings
# from utils.interface_manager import getObject, getObjects
# from utils.calc_func_utils import DTB_CHOICES, GraduationSeasonGenerate, \
# 	BattleRoundGenerator, BattleDamageCalc
# from utils.exception import ErrorType, ErrorException, WebscoketCloseCode
# from player_module.code_manager import getSelections, getSchools
# from question_module.utils import getSubjects, getLevels
# from exam_module.utils import getExamSets
# from rank_module.utils import getRankLevels, getColleges, \
# 	getGraduationSeasons, getCompetitionSeasons
# from minigame_module.utils import getMiniGames
# from exam_module.models import ExamSet
# from player_module.models import Player, School
# from question_module.models import QuestionReport, Question
# from rank_module.models import GraduationRecord, GraduationSeason
# from competition_module.models import BattleRecord, BattleRound
# from competition_module.runtimes import BattleRecordInfo
#
# # 获取静态数据（游戏中加载一次后不再改变的数据）
# def getStaticData(verison):
#
# 	checkVerison(verison)
#
# 	data = {}
# 	"""
# 	data['login_verification'] = {
# 		'username_reg' : str(Player.USERNAME_REG),
# 		'password_reg' : str(Player.PASSWORD_REG),
# 		'phone_reg' : str(Player.PHONE_REG),
# 		'email_reg' : str(Player.EMAIL_REG),
# 		'name_reg' : str(Player.NAME_REG),
# 		'school_reg' : str(School.NAME_REG),
# 	}
# 	"""
# 	data['version'] = settings.VERSION
#
# 	data['update_note'] = settings.UPDATE_NOTE
#
# 	data['player_statuses'] = Player.PLAYER_STATUSES
# 	data['player_types'] = Player.PLAYER_TYPES
#
# 	data['question_dtb_types'] = DTB_CHOICES
# 	data['question_report_types'] = QuestionReport.TYPE_CHOICES
# 	data['question_types'] = Question.QUESTION_TYPES
# 	data['question_statuses'] = Question.QUESTION_STATUSES
#
# 	data['battle_modes'] = BattleRecord.BATTLE_MODES
# 	data['question_select_modes'] = BattleRound.QUESTION_SELECT_MODES
#
# 	data['first_exam_tag'] = ExamSet.FIRST_EXAM_TAG
# 	data['final_exam_tag'] = ExamSet.FINAL_EXAM_TAG
#
# 	data['normal_exam_day'] = GraduationSeasonGenerate.NORMAL_EXAM_DAY
# 	data['final_week'] = GraduationSeasonGenerate.FINAL_WEEK
# 	data['final_exam_duration'] = GraduationSeasonGenerate.FINAL_EXAM_DURATION
#
# 	data['exam_publish_day'] = GraduationSeason.EXAM_PUBLISH_DAY
# 	data['end_will_time'] = GraduationSeason.END_WILL_TIME
# 	data['admission_time'] = GraduationSeason.ADMISSION_TIME
#
# 	data['max_will_count'] = GraduationRecord.MAX_WILL_COUNT
#
# 	data['under_select_factor'] = Question.UNDER_SELECT_SCORE_FACTOR
#
# 	data['weapon_correct_rate'] = Player.WEAPON_CORRECT_RATE
# 	data['min_comp_weapon_count'] = Player.MIN_COMP_WEAPON_COUNT
#
# 	data["battle_init_hp"] = BattleRecordInfo.MAX_INIT_HP
#
# 	data['battle_max_round'] = BattleRoundGenerator.MAX_ROUND
#
# 	data['battle_delta_step1'] = BattleDamageCalc.DELTA_STEP1
# 	data['battle_delta_step2'] = BattleDamageCalc.DELTA_STEP2
# 	data['battle_delta_step3'] = BattleDamageCalc.DELTA_STEP3
# 	data['battle_delta_max'] = BattleDamageCalc.DELTA_MAX
#
# 	data['subjects'] = getSubjects('dict')
# 	data['question_levels'] = getLevels('dict')
# 	data['subject_selections'] = getSelections('dict')
# 	data['rank_levels'] = getRankLevels('dict')
#
# 	data['colleges'] = getColleges('dict')
#
# 	data['minigames'] = getMiniGames('dict')
#
# 	return data
#
# # 获取动态数据（非静态数据）
# def getDynamicData():
#
# 	data = {}
#
# 	data['schools'] = getSchools(return_type='dict')
# 	data['exam_sets'] = getExamSets(return_type='dict')
# 	data['graduations'] = getGraduationSeasons(return_type='dict')
# 	data['competitions'] = getCompetitionSeasons(return_type='dict')
#
# 	return data
#
# def checkVerison(verison):
# 	if verison in settings.VERSIONS:
#
# 		if not versionAcceptable(verison):
# 			raise ErrorException(ErrorType.RequestUpdate)
#
# 	else:
# 		raise ErrorException(ErrorType.ErrorVersion)
#
# # 查询版本是否支持（若支持但版本号不一致，需提醒更新）
# def versionAcceptable(verison):
# 	return verison in settings.VERSION_ACCEPT

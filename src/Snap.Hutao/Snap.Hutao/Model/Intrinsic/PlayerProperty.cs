// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Intrinsic;

internal enum PlayerProperty
{
    /// <summary>
    /// 空
    /// </summary>
    PROP_NONE = 0,

    /// <summary>
    /// 经验值
    /// </summary>
    PROP_EXP = 1001,

    /// <summary>
    /// 突破等级
    /// </summary>
    PROP_BREAK_LEVEL = 1002,

    /// <summary>
    /// 饱食度
    /// </summary>
    PROP_SATIATION_VAL = 1003,

    /// <summary>
    /// 饱食度冷却
    /// </summary>
    PROP_SATIATION_PENALTY_TIME = 1004,

    /// <summary>
    /// 等级
    /// </summary>
    PROP_LEVEL = 4001,

    /// <summary>
    /// 上次切换角色的时间
    /// </summary>
    PROP_LAST_CHANGE_AVATAR_TIME = 10001,

    /// <summary>
    /// Maximum volume of the Statue of the Seven for the player [0, 8500000]
    /// </summary>
    PROP_MAX_SPRING_VOLUME = 10002,

    /// <summary>
    /// Current volume of the Statue of the Seven [0, PROP_MAX_SPRING_VOLUME]
    /// </summary>
    PROP_CUR_SPRING_VOLUME = 10003,

    /// <summary>
    /// Auto HP recovery when approaching the Statue of the Seven [0, 1]
    /// </summary>
    PROP_IS_SPRING_AUTO_USE = 10004,

    /// <summary>
    /// Auto HP recovery percentage [0, 100]
    /// </summary>
    PROP_SPRING_AUTO_USE_PERCENT = 10005,

    /// <summary>
    /// Are you in a state that disables your flying ability? e.g. new player [0, 1]
    /// </summary>
    PROP_IS_FLYABLE = 10006,

    /// <summary>
    /// 天气是否锁定
    /// </summary>
    PROP_IS_WEATHER_LOCKED = 10007,

    /// <summary>
    /// 游戏时间是否锁定
    /// </summary>
    PROP_IS_GAME_TIME_LOCKED = 10008,

    /// <summary>
    /// ？
    /// </summary>
    PROP_IS_TRANSFERABLE = 10009,

    /// <summary>
    /// Maximum stamina of the player =0 - 24000
    /// </summary>
    PROP_MAX_STAMINA = 10010,

    /// <summary>
    /// Used stamina of the player =0 - PROP_MAX_STAMINA
    /// </summary>
    PROP_CUR_PERSIST_STAMINA = 10011,

    /// <summary>
    /// 临时体力，食物？
    /// </summary>
    PROP_CUR_TEMPORARY_STAMINA = 10012,

    /// <summary>
    /// 玩家等级
    /// </summary>
    PROP_PLAYER_LEVEL = 10013,

    /// <summary>
    /// 玩家经验
    /// </summary>
    PROP_PLAYER_EXP = 10014,

    /// <summary>
    /// Primogem =-inf, +inf
    /// It is known that Mihoyo will make Primogem negative in the cases that a player spends
    /// his gems and then got a money refund, so negative is allowed.
    /// </summary>
    PROP_PLAYER_HCOIN = 10015,

    /// <summary>
    /// Mora [0, +inf
    /// </summary>
    PROP_PLAYER_SCOIN = 10016,

    /// <summary>
    /// Do you allow other players to join your game? [0=no 1=direct 2=approval]
    /// </summary>
    PROP_PLAYER_MP_SETTING_TYPE = 10017,

    /// <summary>
    /// 0 if in quest or something that disables MP [0, 1]
    /// </summary>
    PROP_IS_MP_MODE_AVAILABLE = 10018,

    /// <summary>
    /// [0, 8]
    /// </summary>
    PROP_PLAYER_WORLD_LEVEL = 10019,

    /// <summary>
    /// Original Resin [0, 2000] - note that values above 160 require refills
    /// </summary>
    PROP_PLAYER_RESIN = 10020,

    /// <summary>
    /// ？
    /// </summary>
    PROP_PLAYER_WAIT_SUB_HCOIN = 10022,

    /// <summary>
    /// ？
    /// </summary>
    PROP_PLAYER_WAIT_SUB_SCOIN = 10023,

    /// <summary>
    /// Is only MP with PlayStation players? [0, 1]
    /// </summary>
    PROP_IS_ONLY_MP_WITH_PS_PLAYER = 10024,

    /// <summary>
    /// Genesis Crystal =-inf, +inf see 10015
    /// </summary>
    PROP_PLAYER_MCOIN = 10025,

    /// <summary>
    /// ？
    /// </summary>
    PROP_PLAYER_WAIT_SUB_MCOIN = 10026,

    /// <summary>
    /// ？
    /// </summary>
    PROP_PLAYER_LEGENDARY_KEY = 10027,

    /// <summary>
    /// ？
    /// </summary>
    PROP_IS_HAS_FIRST_SHARE = 10028,

    /// <summary>
    /// ？
    /// </summary>
    PROP_PLAYER_FORGE_POINT = 10029,

    /// <summary>
    /// ？
    /// </summary>
    PROP_CUR_CLIMATE_METER = 10035,

    /// <summary>
    /// ？
    /// </summary>
    PROP_CUR_CLIMATE_TYPE = 10036,

    /// <summary>
    /// ？
    /// </summary>
    PROP_CUR_CLIMATE_AREA_ID = 10037,

    /// <summary>
    /// ？
    /// </summary>
    PROP_CUR_CLIMATE_AREA_CLIMATE_TYPE = 10038,

    /// <summary>
    /// ？
    /// </summary>
    PROP_PLAYER_WORLD_LEVEL_LIMIT = 10039,

    /// <summary>
    /// ？
    /// </summary>
    PROP_PLAYER_WORLD_LEVEL_ADJUST_CD = 10040,

    /// <summary>
    /// ？
    /// </summary>
    PROP_PLAYER_LEGENDARY_DAILY_TASK_NUM = 10041,

    /// <summary>
    /// Realm currency [0, +inf)
    /// </summary>
    PROP_PLAYER_HOME_COIN = 10042,

    PROP_PLAYER_WAIT_SUB_HOME_COIN = 10043,
    PROP_IS_AUTO_UNLOCK_SPECIFIC_EQUIP = 10044,
    PROP_PLAYER_GCG_COIN = 10045,
    PROP_PLAYER_WAIT_SUB_GCG_COIN = 10046,
    PROP_PLAYER_ONLINE_TIME = 10047,
    PROP_IS_DIVEABLE = 10048,
    PROP_MAX_DIVE_STAMINA = 10049,
    PROP_CUR_PERSIST_DIVE_STAMINA = 10050,
}
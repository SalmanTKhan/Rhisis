﻿namespace Rhisis.Core.Data
{
    public enum DefineJob
    {
        JTYPE_BASE = 0,
        JTYPE_EXPERT = 1,
        JTYPE_PRO = 2,
        JTYPE_TROUPE = 3,
        JTYPE_COMMON = 4,
        JTYPE_MASTER = 5,
        JTYPE_HERO = 6,

        MAX_JOB_SKILL = 3,
        MAX_EXPERT_SKILL = 20,
        MAX_PRO_SKILL = 20,
        MAX_TROUPE_SKILL = 9,
        MAX_MASTER_SKILL = 1,
        MAX_HERO_SKILL = 1,

        MAX_JOB_LEVEL = 15,
        MAX_EXP_LEVEL = 45,
        MAX_PRO_LEVEL = 30,
        MAX_TROUPE_LEVEL = 1,

        MAX_LEGEND_LEVEL = 129,
        MAX_MONSTER_LEVEL = 160,
        MAX_LEVEL = 120,

        JOB_VAGRANT = 0,
        MAX_JOBBASE = 1,

        // Expert
        JOB_MERCENARY = 1,
        JOB_ACROBAT = 2,
        JOB_ASSIST = 3,
        JOB_MAGICIAN = 4,
        JOB_PUPPETEER = 5,
        MAX_EXPERT = 6,

        // Professional
        JOB_KNIGHT = 6,
        JOB_BLADE = 7,
        JOB_JESTER = 8,
        JOB_RANGER = 9,
        JOB_RINGMASTER = 10,
        JOB_BILLPOSTER = 11,
        JOB_PSYCHIKEEPER = 12,
        JOB_ELEMENTOR = 13,
        JOB_GATEKEEPER = 14,
        JOB_DOPPLER = 15,
        MAX_PROFESSIONAL = 16,

        // Master
        JOB_KNIGHT_MASTER = 16,
        JOB_BLADE_MASTER = 17,
        JOB_JESTER_MASTER = 18,
        JOB_RANGER_MASTER = 19,
        JOB_RINGMASTER_MASTER = 20,
        JOB_BILLPOSTER_MASTER = 21,
        JOB_PSYCHIKEEPER_MASTER = 22,
        JOB_ELEMENTOR_MASTER = 23,
        MAX_MASTER = 24,

        // Hero
        JOB_KNIGHT_HERO = 24,
        JOB_BLADE_HERO = 25,
        JOB_JESTER_HERO = 26,
        JOB_RANGER_HERO = 27,
        JOB_RINGMASTER_HERO = 28,
        JOB_BILLPOSTER_HERO = 29,
        JOB_PSYCHIKEEPER_HERO = 30,
        JOB_ELEMENTOR_HERO = 31,
        MAX_HERO = 32,

        MAX_JOB = 32,
        JOB_ALL = 32,

        // SkillGroup (Disciple)
        DIS_VAGRANT = 0,
        DIS_SWORD = 1,
        DIS_DOUBLE = 2,
        DIS_CASE = 3,
        DIS_JUGGLING = 4,
        DIS_YOYO = 5,
        DIS_RIFLE = 6,
        DIS_MARIONETTE = 7,
        DIS_BOW = 32, // 새로 추가된 것
                      // 방어기술군 
        DIS_SHIELD = 8,
        DIS_DANCE = 9,
        DIS_ACROBATIC = 10,
        DIS_SUPPORT = 23, // 머서너리용 보조스킬

        // 마법연계 기술군
        DIS_HEAL = 11,
        DIS_CHEER = 12,
        DIS_ACTING = 13,
        DIS_POSTER = 14,
        DIS_FIRE = 15,
        DIS_WIND = 16,
        DIS_WATER = 17,
        DIS_EARTH = 18,
        DIS_ELECTRICITY = 24,

        // 특수 기술
        DIS_STRINGDANCE = 19,
        DIS_GIGAPUPPET = 20,
        DIS_KNUCKLE = 21,
        DIS_MAGIC = 22,

        DIS_MULTY = 23,
        DIS_PSYCHIC = 24,
        DIS_CURSE = 25,
        DIS_HOLY = 26,
        DIS_TWOHANDWEAPON = 27,
        DIS_TWOHANDSWORD = 28,
        DIS_TWOHANDAXE = 29,
        DIS_DOUBLESWORD = 30,
        DIS_DOUBLEAXE = 31,

        // 극단 소속
        TRO_MASTER = 0, // 단장
        TRO_MEMBERE = 1, // 멤버

        // 길드소속
        GUD_MASTER = 0, // 마스터
        GUD_KINGPIN = 1, // 킹핀
        GUD_CAPTAIN = 2, // 캡틴
        GUD_SUPPORTER = 3, // 서포터
        GUD_ROOKIE = 4, // 루키
    }
}

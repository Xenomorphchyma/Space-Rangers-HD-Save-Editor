// Semantic compatibility definitions. Layout and artwork are created by this project.
namespace SpaceRangersHdSaveEditor
{
    internal static class EditorFormDefinitions
    {
        internal static readonly string[] AllResources = new string[]
        {
            "TACHIEVEMENTSFORM", "TASTEROIDFORM", "TBONUSALERTFORM", "TCUSTOMSHIPINFOFORM", "TCUSTOMSYSTEMINFOFORM", "TCUSTOMWEAPONINFOFORM", "TEXTRASPECIALFRAME", "TGALAXYEVENTFORM", "TGALAXYFORM", "TGATEFORM", "THOLEFORM", "TILLNESSFORM", "TINTERFACEOVERRIDEFORM", "TITEMFORM", "TJOURNALRECORDFORM", "TMAINFORM", "TMESSAGEFORM", "TMISSILEFORM", "TMODSLISTFORM", "TOLDQUESTFORM", "TORDERFORM", "TPATHDIALOGFORM", "TPLANETFORM", "TPLANETGONEITEMFORM", "TPLANETNEWSFORM", "TQUESTFORM", "TRELATIONFORM", "TREWARDFORM", "TROBOTMAPSTATFORM", "TSCALCFORM", "TSCOLORDIALOGFORM", "TSCRIPTCACHEFORM", "TSCRIPTFORM", "TSCRIPTITEMFORM", "TSCRIPTSHIPFORM", "TSETTINGSFORM", "TSHIPFORM", "TSPECIALBONUSFORM", "TSPOPUPCALENDAR", "TSPUTNIKFORM", "TSTARDROPITEMFORM", "TSTARFORM", "TSTARMAPFORM", "TSTATUSEFFECTFORM", "TSTORAGEITEMFORM", "TVARARRAYVIEWFORM", "TVARFORM", "TWAROPERATIONFORM"
        };

        internal static EditorFormDefinition Get(string resource)
        {
            switch ((resource ?? string.Empty).ToUpperInvariant())
            {
                case "TACHIEVEMENTSFORM": return Build_TACHIEVEMENTSFORM();
                case "TASTEROIDFORM": return Build_TASTEROIDFORM();
                case "TBONUSALERTFORM": return Build_TBONUSALERTFORM();
                case "TCUSTOMSHIPINFOFORM": return Build_TCUSTOMSHIPINFOFORM();
                case "TCUSTOMSYSTEMINFOFORM": return Build_TCUSTOMSYSTEMINFOFORM();
                case "TCUSTOMWEAPONINFOFORM": return Build_TCUSTOMWEAPONINFOFORM();
                case "TEXTRASPECIALFRAME": return Build_TEXTRASPECIALFRAME();
                case "TGALAXYEVENTFORM": return Build_TGALAXYEVENTFORM();
                case "TGALAXYFORM": return Build_TGALAXYFORM();
                case "TGATEFORM": return Build_TGATEFORM();
                case "THOLEFORM": return Build_THOLEFORM();
                case "TILLNESSFORM": return Build_TILLNESSFORM();
                case "TINTERFACEOVERRIDEFORM": return Build_TINTERFACEOVERRIDEFORM();
                case "TITEMFORM": return Build_TITEMFORM();
                case "TJOURNALRECORDFORM": return Build_TJOURNALRECORDFORM();
                case "TMAINFORM": return Build_TMAINFORM();
                case "TMESSAGEFORM": return Build_TMESSAGEFORM();
                case "TMISSILEFORM": return Build_TMISSILEFORM();
                case "TMODSLISTFORM": return Build_TMODSLISTFORM();
                case "TOLDQUESTFORM": return Build_TOLDQUESTFORM();
                case "TORDERFORM": return Build_TORDERFORM();
                case "TPATHDIALOGFORM": return Build_TPATHDIALOGFORM();
                case "TPLANETFORM": return Build_TPLANETFORM();
                case "TPLANETGONEITEMFORM": return Build_TPLANETGONEITEMFORM();
                case "TPLANETNEWSFORM": return Build_TPLANETNEWSFORM();
                case "TQUESTFORM": return Build_TQUESTFORM();
                case "TRELATIONFORM": return Build_TRELATIONFORM();
                case "TREWARDFORM": return Build_TREWARDFORM();
                case "TROBOTMAPSTATFORM": return Build_TROBOTMAPSTATFORM();
                case "TSCALCFORM": return Build_TSCALCFORM();
                case "TSCOLORDIALOGFORM": return Build_TSCOLORDIALOGFORM();
                case "TSCRIPTCACHEFORM": return Build_TSCRIPTCACHEFORM();
                case "TSCRIPTFORM": return Build_TSCRIPTFORM();
                case "TSCRIPTITEMFORM": return Build_TSCRIPTITEMFORM();
                case "TSCRIPTSHIPFORM": return Build_TSCRIPTSHIPFORM();
                case "TSETTINGSFORM": return Build_TSETTINGSFORM();
                case "TSHIPFORM": return Build_TSHIPFORM();
                case "TSPECIALBONUSFORM": return Build_TSPECIALBONUSFORM();
                case "TSPOPUPCALENDAR": return Build_TSPOPUPCALENDAR();
                case "TSPUTNIKFORM": return Build_TSPUTNIKFORM();
                case "TSTARDROPITEMFORM": return Build_TSTARDROPITEMFORM();
                case "TSTARFORM": return Build_TSTARFORM();
                case "TSTARMAPFORM": return Build_TSTARMAPFORM();
                case "TSTATUSEFFECTFORM": return Build_TSTATUSEFFECTFORM();
                case "TSTORAGEITEMFORM": return Build_TSTORAGEITEMFORM();
                case "TVARARRAYVIEWFORM": return Build_TVARARRAYVIEWFORM();
                case "TVARFORM": return Build_TVARFORM();
                case "TWAROPERATIONFORM": return Build_TWAROPERATIONFORM();
                default: return null;
            }
        }

        private static EditorFormDefinition Build_TACHIEVEMENTSFORM()
        {
            return new EditorFormDefinition(
                "TACHIEVEMENTSFORM", "Статистика достижений",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbAchievements", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAchievements", "lblAsteroidsDestroyed", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAchievements", "lblFriedShips", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAchievements", "lblDefendedSystem", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAchievements", "lblPirateSystems", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAchievements", "lblScienceProgress", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAchievements", "lblProgramsUsed", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAchievements", "lblPiratesFreed", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAchievements", "lblHealthDrained", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAchievements", "lblFuelGottenFromSun", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAchievements", "lblFuelTankLastId", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAchievements", "lblPlanetsVisited", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAchievements", "edAsteroidsDestroyed", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAchievements", "edFriedShips", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAchievements", "edDefendedSystem", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAchievements", "edPirateSystems", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAchievements", "edScienceProgress", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAchievements", "edProgramsUsed", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAchievements", "edPiratesFreed", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAchievements", "edHealthDrained", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAchievements", "edFuelGottenFromSun", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAchievements", "edFuelTankLastId", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAchievements", "edPlanetsVisited", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAchievements", "gbAchAlreadyReceived", "group", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAchAlreadyReceived", "mmAchAlreadyReceived", "memo", false, true, true, true, false, true, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TASTEROIDFORM()
        {
            return new EditorFormDefinition(
                "TASTEROIDFORM", "Параметры астероида",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbAsteroid", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAsteroid", "lblStar", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAsteroid", "lblPos", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAsteroid", "lblSpeed", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAsteroid", "lblMass", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAsteroid", "lblMinerals", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAsteroid", "lblGraphName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAsteroid", "cbStar", "combo", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbAsteroid", "edPosX", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAsteroid", "edPosY", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAsteroid", "edSpeedX", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAsteroid", "edSpeedY", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAsteroid", "edMass", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAsteroid", "edMinerals", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAsteroid", "cbGraphName", "combo", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TBONUSALERTFORM()
        {
            return new EditorFormDefinition(
                "TBONUSALERTFORM", "Проверка каталогов",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "sImage1", "image", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "lblBonusCRCTitle", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "lblBonusCRCOptions", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "btnCorrection", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "btnReadAsIs", "button", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TCUSTOMSHIPINFOFORM()
        {
            return new EditorFormDefinition(
                "TCUSTOMSHIPINFOFORM", "Дополнительные данные корабля",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbModInfo", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbModInfo", "lblInfoName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbModInfo", "lblDescription", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbModInfo", "lblInfoData1", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbModInfo", "lblInfoData2", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbModInfo", "lblInfoData3", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbModInfo", "lblInfoTextData1", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbModInfo", "lblInfoTextData2", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbModInfo", "lblInfoTextData3", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbModInfo", "edInfoName", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbModInfo", "edInfoData1", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbModInfo", "edInfoData2", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbModInfo", "edInfoData3", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbModInfo", "mmInfoTextData1", "memo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbModInfo", "mmInfoTextData2", "memo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbModInfo", "mmInfoTextData3", "memo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbModInfo", "chbHideTags", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbModInfo", "mmDescription", "memo", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TCUSTOMSYSTEMINFOFORM()
        {
            return new EditorFormDefinition(
                "TCUSTOMSYSTEMINFOFORM", "Дополнительные данные системы",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbCustomSystemInfo", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomSystemInfo", "lblCustomSystemName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomSystemInfo", "lblCustomSystemIcon", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomSystemInfo", "lblCustomSystemInfo", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomSystemInfo", "lblCustomSystemType", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomSystemInfo", "lblCustomSystemDist", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomSystemInfo", "edCustomSystemDist", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomSystemInfo", "edCustomSystemIcon", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomSystemInfo", "edCustomSystemInfo", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomSystemInfo", "edCustomSystemName", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomSystemInfo", "edCustomSystemType", "edit", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TCUSTOMWEAPONINFOFORM()
        {
            return new EditorFormDefinition(
                "TCUSTOMWEAPONINFOFORM", "Модифицированное оружие",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbCustomWeaponInfo", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "lblSysName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "lblType", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "lblTechLevel", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "lblTechRadius", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "lblModCost", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "lblDamage", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "lblAvgSize", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "lblAvgRadius", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "lblSpeed", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "lblDamageType", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "lblShotType", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "lblShotCount", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "lblAttackCount", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "lblSecondaryDamageRadius", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "lblMiningFactor", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "lblWeaponDamageSet", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "lblPrimarySE", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "lblSecondarySE", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "lblAreaSE", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "lblDefaultPalette", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "lblAvailability", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "lblABWeaponType", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "lblRnd", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "lblMissileRadius", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "lblMissileSpeed", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "lblMissileChanceToBeHit", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "edSysName", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "edType", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "edTechLevel", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "edModCost", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "edMinDamage", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "edMaxDamage", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "edAvgSize", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "edAvgRadius", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "edSpeed", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "clbDamageType", "checklist", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "edShotCount", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "edAttackCount", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "edSecondaryDamageRadius", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "edMiningFactor", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "vleWeaponDamageSet", "grid", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "edPrimarySE", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "edSecondarySE", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "edAreaSE", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "edDefaultPalette", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "cbAvailability", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "cbShotType", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "cbABWeaponType", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "edRnd", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "edMissileChanceToBeHit", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "edMissileMaxSpeed", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "edMissileMinSpeed", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "edMissileRadius", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeaponInfo", "cbTechRadius", "combo", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TEXTRASPECIALFRAME()
        {
            return new EditorFormDefinition(
                "TEXTRASPECIALFRAME", "Особые свойства",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbExtraSpecial", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbExtraSpecial", "lblExtraSpecialNum", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbExtraSpecial", "lblExtraSpecialBlockName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbExtraSpecial", "lblExtraSpecialCRC", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbExtraSpecial", "lblExtraSpecialCount", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbExtraSpecial", "lblExtraSpecialName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbExtraSpecial", "cbExtraSpecialName", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbExtraSpecial", "edExtraSpecialBlockName", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbExtraSpecial", "edExtraSpecialCount", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbExtraSpecial", "edExtraSpecialCRC", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbExtraSpecial", "edExtraSpecialNum", "edit", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TGALAXYEVENTFORM()
        {
            return new EditorFormDefinition(
                "TGALAXYEVENTFORM", "Событие галактики",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbGalaxyEvent", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGalaxyEvent", "lblType", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGalaxyEvent", "lblTurn", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGalaxyEvent", "lblData", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGalaxyEvent", "lblTextData", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGalaxyEvent", "edType", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbGalaxyEvent", "edTurn", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbGalaxyEvent", "lbData", "list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGalaxyEvent", "lbTextData", "list", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TGALAXYFORM()
        {
            return new EditorFormDefinition(
                "TGALAXYFORM", "Настройки галактики",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbGalaxy", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGalaxy", "pcGalaxy", "tabs", false, true, true, true, false, false, -1, null, "tsMain"),
                new EditorNodeDefinition("pcGalaxy", "tsMain", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsMain", "gbDifficulty", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbDifficulty", "lblPirateDifLevel", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbDifficulty", "lblTradeDifLevel", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbDifficulty", "lblScnDifLevel", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbDifficulty", "lblRepairDifLevel", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbDifficulty", "lblTechDifLevel", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbDifficulty", "lblQuestDifLevel", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbDifficulty", "lblHoleDifLevel", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbDifficulty", "lblBalanceDifLevel", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbDifficulty", "cbPirateDifLevel", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbDifficulty", "cbTradeDifLevel", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbDifficulty", "cbScnDifLevel", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbDifficulty", "cbRepairDifLevel", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbDifficulty", "cbTechDifLevel", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbDifficulty", "cbQuestDifLevel", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbDifficulty", "cbHoleDifLevel", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbDifficulty", "cbBalanceDifLevel", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsMain", "gbResearch", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbResearch", "lblBlazerResearch", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbResearch", "lblKellerResearch", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbResearch", "lblTerronResearch", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbResearch", "lblBlazerMaterial", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbResearch", "lblKellerMaterial", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbResearch", "lblTerronMaterial", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbResearch", "edBlazerResearch", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbResearch", "edKellerResearch", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbResearch", "edTerronResearch", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbResearch", "edBlazerMaterial", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbResearch", "edKellerMaterial", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbResearch", "edTerronMaterial", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsMain", "gbWarDeltaWin", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbWarDeltaWin", "lblWarDeltaWinDominators", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbWarDeltaWin", "lblWarDeltaWinPirates", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbWarDeltaWin", "lblWarDeltaWinCoalition", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbWarDeltaWin", "edWarDeltaWinDominators", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbWarDeltaWin", "edWarDeltaWinPirates", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbWarDeltaWin", "edWarDeltaWinCoalition", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsMain", "gbPlanetNews", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlanetNews", "lbPlanetNews", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsMain", "gbQuestOld", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbQuestOld", "lbOldQuest", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsMain", "gbWarOperations", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbWarOperations", "lbWarOperations", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsMain", "gbGates", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGates", "lbGates", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsMain", "gbKellerAttack", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbKellerAttack", "lblKellerAttackState", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbKellerAttack", "lblKellerAttackStar", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbKellerAttack", "edKellerAttackState", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbKellerAttack", "cbKellerAttackStar", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsMain", "gbFileFlags", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbFileFlags", "chbIronWill", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbFileFlags", "chbRejectedPB", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcGalaxy", "tsCustomRules", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRules", "gbCustomRules", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomRules", "pcCustomRules", "tabs", false, true, true, true, false, false, -1, null, "tsCustomRulesBalance"),
                new EditorNodeDefinition("pcCustomRules", "tsCustomRulesBalance", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesBalance", "lblKlingStrength", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesBalance", "lblKlingAggro", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesBalance", "lblKlingSpawn", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesBalance", "lblPirateAggro", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesBalance", "lblCoalAggro", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesBalance", "lblExtraInventions", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesBalance", "lblExtraRangers", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesBalance", "lblHullGrowth", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesBalance", "lblKlingStrengthVal", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesBalance", "lblKlingAggroVal", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesBalance", "lblKlingSpawnVal", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesBalance", "lblPirateAggroVal", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesBalance", "lblCoalAggroVal", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesBalance", "lblExtraInventionsVal", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesBalance", "lblExtraRangersVal", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesBalance", "chbDominatorsRacialWeapons", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesBalance", "chbZeroExp", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesBalance", "chbMaxRangeMissiles", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesBalance", "tbDominatorsStrength", "track", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesBalance", "tbDominatorsAggro", "track", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesBalance", "tbDominatorsSpawn", "track", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesBalance", "tbPirateAggro", "track", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesBalance", "tbCoalAggro", "track", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesBalance", "tbExtraInventions", "track", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesBalance", "tbExtraRangers", "track", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesBalance", "cbHullGrowth", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcCustomRules", "tsCustomRulesGalaxy", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesGalaxy", "lblAsteroidMod", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesGalaxy", "lblSunDamageMod", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesGalaxy", "lblAgPlanets", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesGalaxy", "lblMiPlanets", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesGalaxy", "lblInPlanets", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesGalaxy", "lblAsteroidModVal", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesGalaxy", "lblSunDamageModVal", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesGalaxy", "lblAgPlanetsVal", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesGalaxy", "lblMiPlanetsVal", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesGalaxy", "lblInPlanetsVal", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesGalaxy", "chbStartCenter", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesGalaxy", "tbAsteroidMod", "track", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesGalaxy", "tbSunDamageMod", "track", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesGalaxy", "tbAgPlanets", "track", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesGalaxy", "tbMiPlanets", "track", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesGalaxy", "tbInPlanets", "track", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcCustomRules", "tsCustomRulesOthers", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomRulesOthers", "sbCustomRulesOther", "scroll", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "lblABDamageMod", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "lblABHitpointsMod", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "lblAITolerateJunk", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "lblABDropValueMod", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "lblDropValueMod", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "lblAkrinMod", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "lblNodeDropMod", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "lblAkrinModVal", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "lblNodeDropModVal", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "lblDropValueModVal", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "lblABDropValueModVal", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "lblABHitpointsModVal", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "lblABDamageModVal", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "lblAITolerateJunkVal", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "chbABChangeEq", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "chbOldMissileBonuses", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "chbOldSpeedCalc", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "chbAIBuysEqFromShops", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "chbABattleRoyale", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "chbDuplicateArtsAllowed", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "chbRuinsUsingShop", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "chbTechKnowledge", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "chbSpecialShips", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "chbRuinsTargetting", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "chbRnd", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "chbRuinsPos", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "chbPirateNodes", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "chbOldHyper", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "tbAkrinMod", "track", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "tbNodeDropMod", "track", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "tbDropValueMod", "track", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "tbABDropValueMod", "track", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "tbABHitpointsMod", "track", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "tbABDamageMod", "track", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sbCustomRulesOther", "tbAITolerateJunk", "track", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TGATEFORM()
        {
            return new EditorFormDefinition(
                "TGATEFORM", "Гиперпереход",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbGate", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGate", "lblAngle", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGate", "lblPos", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGate", "lblText", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGate", "lblSize", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGate", "edAngle", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGate", "edPosX", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGate", "edPosY", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGate", "edText", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGate", "edSizeX", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGate", "edSizeY", "edit", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_THOLEFORM()
        {
            return new EditorFormDefinition(
                "THOLEFORM", "Чёрная дыра",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbHole", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbHole", "lblStar1", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbHole", "lblStar2", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbHole", "lblPos1", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbHole", "lblPos2", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbHole", "lblTurn", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbHole", "lblType", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbHole", "lblGraph", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbHole", "lblABMapName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbHole", "cbStar1", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbHole", "cbStar2", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbHole", "edPosXStar1", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbHole", "edPosYStar1", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbHole", "edPosXStar2", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbHole", "edPosYStar2", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbHole", "edTurn", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbHole", "edType", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbHole", "edGraph", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbHole", "edABMapName", "edit", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TILLNESSFORM()
        {
            return new EditorFormDefinition(
                "TILLNESSFORM", "Состояние здоровья",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbIllness", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbIllness", "lblInfection", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbIllness", "lblInfectionDay", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbIllness", "lblInfectionEndDay", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbIllness", "lblInfectionCount", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbIllness", "edInfection", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbIllness", "edInfectionDay", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbIllness", "edInfectionEndDay", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbIllness", "edInfectionCount", "edit", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TINTERFACEOVERRIDEFORM()
        {
            return new EditorFormDefinition(
                "TINTERFACEOVERRIDEFORM", "Переопределение интерфейса",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbInterfaceOverride", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbInterfaceOverride", "lblMLName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbInterfaceOverride", "lblGIName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbInterfaceOverride", "pcInterfaceOverride", "tabs", false, true, true, true, false, false, -1, null, "ts1"),
                new EditorNodeDefinition("pcInterfaceOverride", "ts1", "tab", false, true, false, true, false, false, -1, null, ""),
                new EditorNodeDefinition("ts1", "lblNewState", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("ts1", "lblOldState", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("ts1", "cbNewState", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("ts1", "cbOldState", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcInterfaceOverride", "ts2", "tab", false, true, false, true, false, false, -1, null, ""),
                new EditorNodeDefinition("ts2", "lblNewText", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("ts2", "lblOldText", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("ts2", "mmNewText", "memo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("ts2", "mmOldText", "memo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcInterfaceOverride", "ts3", "tab", false, true, false, true, false, false, -1, null, ""),
                new EditorNodeDefinition("ts3", "lblNewImage", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("ts3", "lblOldImage", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("ts3", "edNewImage", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("ts3", "edOldImage", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcInterfaceOverride", "ts4", "tab", false, true, false, true, false, false, -1, null, ""),
                new EditorNodeDefinition("ts4", "lblNewX", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("ts4", "lblNewY", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("ts4", "lblNewZ", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("ts4", "lblOldY", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("ts4", "lblOldX", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("ts4", "lblOldZ", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("ts4", "edNewX", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("ts4", "edNewY", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("ts4", "edNewZ", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("ts4", "edOldY", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("ts4", "edOldZ", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("ts4", "edOldX", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcInterfaceOverride", "ts5", "tab", false, true, false, true, false, false, -1, null, ""),
                new EditorNodeDefinition("ts5", "lblNewSizeX", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("ts5", "lblNewSizeY", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("ts5", "lblOldSizeY", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("ts5", "lblOldSizeX", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("ts5", "edOldSizeX", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("ts5", "edNewSizeX", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("ts5", "edOldSizeY", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("ts5", "edNewSizeY", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbInterfaceOverride", "edMLName", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbInterfaceOverride", "edGIName", "edit", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TITEMFORM()
        {
            return new EditorFormDefinition(
                "TITEMFORM", "Редактор предмета",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbItem", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbItem", "pcItem", "tabs", false, true, true, true, false, false, -1, null, "tsParams"),
                new EditorNodeDefinition("pcItem", "tsParams", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsParams", "pcParams", "tabs", false, true, true, true, false, false, -1, null, "tsMain"),
                new EditorNodeDefinition("pcParams", "tsMain", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsMain", "gbCommon", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblPos", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblItemType", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblWeight", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblOwner", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblCost", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblCustomName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblScriptItem", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblStoredItem", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblItemDestroy", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblNoDrop", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblCustomFaction", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblSysName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblStrength", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblSlot", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblDominatorSeries", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblScriptItemVal", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblStoredItemVal", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edPosX", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edPosY", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edCustomName", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "cbOwner", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edWeight", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edCost", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edItemDestroy", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edNoDrop", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "cbDominatorSeries", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "chbBroken", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "chbExplotable", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edCustomFaction", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edStrength", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edSlot", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edSysName", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "cbItemType", "combo", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("pcParams", "tsHull", "tab", false, true, false, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsHull", "gbHull", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbHull", "lblHitPoints", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbHull", "lblHullTechLevel", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbHull", "lblArmor", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbHull", "lblShipType", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbHull", "edHitPoints", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbHull", "edArmor", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbHull", "edHullTechLevel", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbHull", "cbShipType", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbHull", "gbSeries", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSeries", "lblSeriesNum", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSeries", "lblSeriesName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSeries", "lblSeriesBlockName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSeries", "lblSeriesCRC", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSeries", "cbSeriesName", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSeries", "edSeriesNum", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSeries", "edSeriesBlockName", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbSeries", "edSeriesCRC", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbHull", "chbBuiltByPirate", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcParams", "tsSW", "tab", false, true, false, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsSW", "gbSW", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSW", "lblBridgeType", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSW", "lblEnergyMax", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSW", "lblEnergy", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSW", "edBridgeType", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSW", "edEnergy", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSW", "edEnergyMax", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSW", "chbImpulseShields", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSW", "gbInterceptors", "group", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbInterceptors", "lblInterceptorsStrategy", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbInterceptors", "lblInterceptorsDuration", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbInterceptors", "lblInterceptorsNextTarget", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbInterceptors", "cbInterceptorsNextTarget", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbInterceptors", "cbInterceptorsStrategy", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbInterceptors", "edInterceptorsDuration", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcParams", "tsTransmitter", "tab", false, true, false, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsTransmitter", "gbTransmitter", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTransmitter", "lblTransmitterPower", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTransmitter", "edTransmitterPower", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcParams", "tsWeapon", "tab", false, true, false, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsWeapon", "gbWeapon", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbWeapon", "lblWeaponTarget", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbWeapon", "lblWeaponRadius", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbWeapon", "lblWeaponDamage", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbWeapon", "lblAmmunition", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbWeapon", "lblWeaponTechLevel", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbWeapon", "lblWeaponTargetType", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbWeapon", "cbWeaponTarget", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbWeapon", "edWeaponRadius", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbWeapon", "edMinDamage", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbWeapon", "edWeaponTechLevel", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbWeapon", "edMaxDamage", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbWeapon", "edAmmunition", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbWeapon", "edMaxAmmunition", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbWeapon", "edWeaponTargetType", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbWeapon", "btnCustomWeapon", "button", false, false, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcParams", "tsTreasureMap", "tab", false, true, false, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsTreasureMap", "gbTreasureMap", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTreasureMap", "lblTreasureMapPlanet", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTreasureMap", "lblShipName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTreasureMap", "lblPlanetInfo1", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTreasureMap", "lblPlanetInfo2", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTreasureMap", "cbTreasureMapPlanet", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTreasureMap", "edShipName", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbTreasureMap", "chbHideTags", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbTreasureMap", "mmPlanetInfo1", "memo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTreasureMap", "mmPlanetInfo2", "memo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcParams", "tsEngine", "tab", false, true, false, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsEngine", "gbEngine", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbEngine", "lblEngineTechLevel", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbEngine", "lblSpeed", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbEngine", "lblJump", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbEngine", "lblEnginePower", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbEngine", "edEngineTechLevel", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbEngine", "edSpeed", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbEngine", "edJump", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbEngine", "edEnginePower", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcParams", "tsFuelTank", "tab", false, true, false, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsFuelTank", "gbFuelTank", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbFuelTank", "lblFuelTanksTechLevel", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbFuelTank", "lblFuel", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbFuelTank", "lblCapacity", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbFuelTank", "edFuelTanksTechLevel", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbFuelTank", "edFuel", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbFuelTank", "edCapacity", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcParams", "tsRadar", "tab", false, true, false, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsRadar", "gbRadar", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRadar", "lblRadarTechLevel", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRadar", "lblRadius", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRadar", "edRadarTechLevel", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRadar", "edRadius", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcParams", "tsScaner", "tab", false, true, false, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsScaner", "gbScaner", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScaner", "lblScanerTechLevel", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScaner", "lblScanProtect", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScaner", "edScanerTechLevel", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScaner", "edScanProtect", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcParams", "tsArtefactCustom", "tab", false, true, false, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsArtefactCustom", "gbArtefactCustom", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbArtefactCustom", "lblCustomArtData1", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbArtefactCustom", "lblCustomArtData2", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbArtefactCustom", "lblCustomArtData3", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbArtefactCustom", "lblCustomArtTextData1", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbArtefactCustom", "lblCustomArtTextData2", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbArtefactCustom", "lblCustomArtTextData3", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbArtefactCustom", "edCustomArtData1", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbArtefactCustom", "edCustomArtData2", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbArtefactCustom", "edCustomArtData3", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbArtefactCustom", "edCustomArtTextData1", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbArtefactCustom", "edCustomArtTextData2", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbArtefactCustom", "edCustomArtTextData3", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcParams", "tsUselessItem", "tab", false, true, false, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsUselessItem", "gbUselessItem", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbUselessItem", "lblUselessItemData1", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbUselessItem", "lblUselessItemCustomText", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbUselessItem", "lblUselessItemData3", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbUselessItem", "lblUselessItemData2", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbUselessItem", "edUselessItemData1", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbUselessItem", "mmUselessItemCustomText", "memo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbUselessItem", "edUselessItemData3", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbUselessItem", "edUselessItemData2", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcParams", "tsRepairRobot", "tab", false, true, false, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsRepairRobot", "gbRepairRobot", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRepairRobot", "lblRepairRobotTechLevel", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRepairRobot", "lblRecoverHitPoints", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRepairRobot", "edRepairRobotTechLevel", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRepairRobot", "edRecoverHitPoints", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcParams", "tsDefGenerator", "tab", false, true, false, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsDefGenerator", "gbDefGenerator", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbDefGenerator", "lblDefGeneratorTechLevel", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbDefGenerator", "lblDefPower", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbDefGenerator", "edDefGeneratorTechLevel", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbDefGenerator", "edDefPower", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcParams", "tsCargoHook", "tab", false, true, false, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCargoHook", "gbCargoHook", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCargoHook", "lblCargoHookTechLevel", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCargoHook", "lblPickUpSize", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCargoHook", "lblHookRadius", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCargoHook", "lblSpeedMin", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCargoHook", "lblSpeedMax", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCargoHook", "edCargoHookTechLevel", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCargoHook", "edPickUpSize", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCargoHook", "edHookRadius", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCargoHook", "edSpeedMin", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCargoHook", "edSpeedMax", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcParams", "tsSatellite", "tab", false, true, false, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsSatellite", "gbSatellite", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSatellite", "lblSatelliteType", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSatellite", "lblPlace", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSatellite", "lblWear", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSatellite", "lblWaterSpeed", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSatellite", "lblLandSpeed", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSatellite", "lblSatellitePlanet", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSatellite", "lblHillSpeed", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSatellite", "edSatelliteType", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSatellite", "edPlace", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSatellite", "edWear", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSatellite", "edWaterSpeed", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSatellite", "edLandSpeed", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSatellite", "cbSatellitePlanet", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSatellite", "edHillSpeed", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcParams", "tsCistern", "tab", false, true, false, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCistern", "gbCistern", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCistern", "lblCisternFuel", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCistern", "lblCisternCapacity", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCistern", "edCisternFuel", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCistern", "edCisternCapacity", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcParams", "tsGoodsItem", "tab", false, true, false, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsGoodsItem", "gbGoodsItem", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsItem", "lblGoodsItemCount", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsItem", "edGoodsItemCount", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsItem", "chbGoodsItemNatural", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcParams", "tsCountableItem", "tab", false, true, false, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCountableItem", "gbCountableItem", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCountableItem", "lblCountableItemCount", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCountableItem", "edCountableItemCount", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCountableItem", "chbCountableItemNatural", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcParams", "tsTranclucator", "tab", false, true, false, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsTranclucator", "gbTranclucator", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTranclucator", "btnTranclucator", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcItem", "tsBonus", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsBonus", "gbBonus", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbBonus", "lblBonusNum", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbBonus", "lblBonusName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbBonus", "lblBonusBlockName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbBonus", "lblBonusCRC", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbBonus", "cbBonusName", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbBonus", "edBonusNum", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbBonus", "edBonusBlockName", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbBonus", "edBonusCRC", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsBonus", "gbSpecial", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSpecial", "lblSpecialNum", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSpecial", "lblSpecialName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSpecial", "lblSpecialBlockName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSpecial", "lblSpecialCRC", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSpecial", "cbSpecialName", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSpecial", "edSpecialNum", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSpecial", "edSpecialBlockName", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbSpecial", "edSpecialCRC", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcItem", "tsExtraBonus", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsExtraBonus", "sbExtraSpecial", "scroll", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TJOURNALRECORDFORM()
        {
            return new EditorFormDefinition(
                "TJOURNALRECORDFORM", "Запись журнала",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbJournalRecord", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbJournalRecord", "lblTurn", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbJournalRecord", "lblText", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbJournalRecord", "edTurn", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbJournalRecord", "mmText", "memo", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TMAINFORM()
        {
            return new EditorFormDefinition(
                "TMAINFORM", "Space Rangers HD Save Editor",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "plTopBar", "panel", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("plTopBar", "PaintBoxFile", "image", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("plTopBar", "lblFileName", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("plTopBar", "lblVersionSav", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("plTopBar", "btnRefresh", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("plTopBar", "btnOpen", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("plTopBar", "btnSave", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("plTopBar", "btnSettings", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "plMain", "panel", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("plMain", "pcMain", "tabs", false, true, true, true, false, false, -1, null, "tsFile"),
                new EditorNodeDefinition("pcMain", "tsFile", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsFile", "gbMap", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMap", "Image2", "image", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsFile", "gbPreview", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPreview", "lblPlayerMoney", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPreview", "lblPlayerMoneyVal", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPreview", "lblPlayerName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPreview", "lblPlayerNameVal", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPreview", "lblPlayerRace", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPreview", "lblPlayerRaceVal", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPreview", "lblSaveName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPreview", "lblSaveNameVal", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPreview", "lblTurnNum", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPreview", "lblTurnNumVal", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPreview", "lblDate", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPreview", "lblDateVal", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsFile", "gbScreenshot", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScreenshot", "Image1", "image", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsFile", "gbGalaxyMap", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGalaxyMap", "ImageGalaxyMap", "image", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsFile", "gbSaveFileInfo", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSaveFileInfo", "lblDifficultyVal", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSaveFileInfo", "lblDifficulty", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSaveFileInfo", "lblIronWillVal", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSaveFileInfo", "lblIronWill", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSaveFileInfo", "lblLoadCount", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSaveFileInfo", "lblSaveCount", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSaveFileInfo", "lblLoadCountVal", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSaveFileInfo", "lblSaveCountVal", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSaveFileInfo", "lblCustomRules", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSaveFileInfo", "lblCustomRulesVal", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSaveFileInfo", "lblStatusPB", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSaveFileInfo", "lblStatusPBVal", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsFile", "gbModsInfo", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbModsInfo", "lblModsCount", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbModsInfo", "lblModsCountVal", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbModsInfo", "lblModInfoCount", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbModsInfo", "lblModCustomWeaponCount", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbModsInfo", "lblModCustomWeaponCountVal", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbModsInfo", "lblModInfoCountVal", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsFile", "gbStatus", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStatus", "lblStatusCRC", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStatus", "lblLegalStatus", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStatus", "lblItemsCRC", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStatus", "lblReadStatus", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStatus", "lblGameDatStatus", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStatus", "lblDumpStatus", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsFile", "gbStats", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStats", "lblStarsCountVal", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStats", "lblRangersCount", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStats", "lblShipsCount", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStats", "lblRuinsCount", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStats", "lblStarsCount", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStats", "lblPlanetsCount", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStats", "lblItemsCount", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStats", "lblItemsCountVal", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStats", "lblPlanetsCountVal", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStats", "lblRangersCountVal", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStats", "lblRuinsCountVal", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStats", "lblShipsCountVal", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsFile", "gbBosses", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbBosses", "lblBlazer", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbBosses", "lblTerron", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbBosses", "lblKeller", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbBosses", "lblBlazerVal", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbBosses", "lblKellerVal", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbBosses", "lblTerronVal", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsFile", "gbFileEditOperation", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbFileEditOperation", "btnPlayerShip", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbFileEditOperation", "btnGalaxy", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsFile", "gbFileViewOperation", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbFileViewOperation", "btnModsList", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbFileViewOperation", "btnAchievements", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcMain", "tsGalaxy", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsGalaxy", "gbConstellations", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbConstellations", "lbConstellations", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsGalaxy", "gbStars", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStars", "lbStars", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsGalaxy", "gbObjects", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbObjects", "lbObjects", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsGalaxy", "gbFilterOptions", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbFilterOptions", "gbFilterShips", "group", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbFilterShips", "chbFilterRangers", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbFilterShips", "chbFilterWarriors", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbFilterShips", "chbFilterPirates", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbFilterShips", "chbFilterDominators", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbFilterShips", "chbFilterTranclucators", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbFilterShips", "chbFilterTransports", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbFilterShips", "chbFilterClanPirates", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbFilterShips", "chbFilterFlagmans", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbFilterShips", "chbFilterBosses", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbFilterShips", "chbFilterBertors", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbFilterShips", "chbFilterLiners", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbFilterShips", "chbFilterDiplomats", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbFilterOptions", "gbFilterObjects", "group", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbFilterObjects", "chbFilterEquipments", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbFilterObjects", "chbFilterMissiles", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbFilterObjects", "chbFilterPlanets", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbFilterObjects", "chbFilterRuins", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbFilterObjects", "chbFilterAsteroids", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbFilterObjects", "chbFilterHoles", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbFilterObjects", "chbFilterGoods", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbFilterObjects", "chbFilterNods", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbFilterObjects", "chbFilterUseless", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("tsGalaxy", "gbView", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbView", "btnStarMap", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcMain", "tsStorageItems", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsStorageItems", "gbStorageItems", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStorageItems", "lbStorageItems", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcMain", "tsSatellites", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsSatellites", "gbSatellites", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSatellites", "lbSatellites", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcMain", "tsScripts", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsScripts", "gbScripts", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScripts", "lbScripts", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsScripts", "gbGlobalVars", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGlobalVars", "lbGlobalVars", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsScripts", "gbScriptCache", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScriptCache", "lbScriptCache", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcMain", "tsEvents", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsEvents", "gbGalaxyEvents", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGalaxyEvents", "lbGalaxyEvents", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcMain", "tsMessages", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsMessages", "gbPlayerMsgs", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerMsgs", "lbPlayerMsgs", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcMain", "tsMods", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsMods", "pcMods", "tabs", false, true, true, true, false, false, -1, null, "tsStoredItems"),
                new EditorNodeDefinition("pcMods", "tsStoredItems", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsStoredItems", "gbStoredItems", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStoredItems", "lbStoredItems", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcMods", "tsCustomWeapons", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsCustomWeapons", "gbCustomWeapons", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomWeapons", "lbCustomWeapons", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcMods", "tsInterfaceOverride", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsInterfaceOverride", "gbInterfacePosOverride", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbInterfacePosOverride", "lbInterfacePosOverride", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsInterfaceOverride", "gbInterfaceSizeOverride", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbInterfaceSizeOverride", "lbInterfaceSizeOverride", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsInterfaceOverride", "gbInterfaceImageOverride", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbInterfaceImageOverride", "lbInterfaceImageOverride", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsInterfaceOverride", "gbInterfaceStateOverride", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbInterfaceStateOverride", "lbInterfaceStateOverride", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsInterfaceOverride", "gbInterfaceTextOverride", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbInterfaceTextOverride", "lbInterfaceTextOverride", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcMods", "tsModInfo", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsModInfo", "gbModInfoShips", "group", true, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbModInfoShips", "lbModInfoShips", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsModInfo", "gbModInfoStars", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbModInfoStars", "lbModInfoStars", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcMain", "tsSearch", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsSearch", "gbSearch", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSearch", "lbSearchResult", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSearch", "gbSearchParams", "group", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSearchParams", "lblSearchName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSearchParams", "lblSearchID", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSearchParams", "lblSearchItemType", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSearchParams", "btnSearch", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSearchParams", "gbSearchFilter", "group", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbSearchFilter", "chbSearchAsteroids", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbSearchFilter", "chbSearchGoneItems", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbSearchFilter", "chbSearchItemsInShops", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbSearchFilter", "chbSearchItemsInSpace", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbSearchFilter", "chbSearchMissiles", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbSearchFilter", "chbSearchPlanets", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbSearchFilter", "chbSearchSatelliteItems", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbSearchFilter", "chbSearchShips", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbSearchFilter", "chbSearchStars", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbSearchFilter", "chbSearchStorageItems", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbSearchFilter", "chbSearchStoredItems", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbSearchFilter", "chbSearchRuins", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbSearchFilter", "chbSearchItemsHold", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbSearchFilter", "chbSearchDropItems", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbSearchFilter", "chbSearchTranclucators", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbSearchParams", "edSearchID", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSearchParams", "edSearchName", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSearchParams", "cbSearchItemType", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcMain", "tsLog", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsLog", "pcLog", "tabs", false, true, true, true, false, false, -1, null, "tsSaveFileLog"),
                new EditorNodeDefinition("pcLog", "tsSaveFileLog", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsSaveFileLog", "edtSaveLog", "memo", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("tsSaveFileLog", "plLogBottom", "panel", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("plLogBottom", "lblSearchFound", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("plLogBottom", "btnSaveLog", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("plLogBottom", "edLogSearch", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("plLogBottom", "btnFindPrevious", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("plLogBottom", "btnFindNext", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcLog", "tsErrorLog", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsErrorLog", "edtErrorLog", "memo", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("pcLog", "tsWarningLog", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsWarningLog", "edtWarningLog", "memo", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("pcLog", "tsSuccessLog", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsSuccessLog", "edtSuccessLog", "memo", false, true, true, true, false, true, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TMESSAGEFORM()
        {
            return new EditorFormDefinition(
                "TMESSAGEFORM", "Сообщение",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbMessage", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMessage", "lblID", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMessage", "lblMessageType", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMessage", "lblCustomType", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMessage", "lblSoundType", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMessage", "lblTurn", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMessage", "lblTextMessage", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMessage", "lblObjShip1", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMessage", "lblObjPlanet1", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMessage", "lblObjShip2", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMessage", "lblObjPlanet2", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMessage", "lblObjShip3", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMessage", "lblObjPlanet3", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMessage", "edID", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMessage", "edCustomType", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMessage", "cbMessageType", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMessage", "edSoundType", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMessage", "edTurn", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMessage", "edObjShip1", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMessage", "edObjPlanet1", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMessage", "edObjShip2", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMessage", "edObjPlanet2", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMessage", "edObjShip3", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMessage", "edObjPlanet3", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMessage", "chbPlayerRead", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMessage", "chbNoSound", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMessage", "chbHideTags", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbMessage", "mmTextMessage", "memo", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TMISSILEFORM()
        {
            return new EditorFormDefinition(
                "TMISSILEFORM", "Ракета",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbMissile", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMissile", "pcMissile", "tabs", false, true, true, true, false, false, -1, null, "tsParams"),
                new EditorNodeDefinition("pcMissile", "tsParams", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsParams", "gbMain", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "lblWeaponID", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "lblWeaponType", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "lblTechLevel", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "lblDamage", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "lblPos", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "lblAngle", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "lblFromAngle", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "lblShip", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "lblStar", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "lblTarget", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "lblTargetType", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "lblMissileNo", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "lblLive", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "lblFromAngleOld", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "lblSpeed", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "lblBaseSpeed", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "lblTargetLost", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "lblTargetLastPos", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "lblTargetLastDistanceMin", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "lblTargetTypeLost", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "edWeaponID", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbMain", "edTechLevel", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "edDamageMin", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "edDamageMax", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "cbWeaponType", "combo", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbMain", "edPosX", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "edPosY", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "edAngle", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "edFromAngle", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "cbShip", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "cbTarget", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "edTargetType", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbMain", "edMissileNo", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "edLive", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "edFromAngleOld", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "edSpeed", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "edBaseSpeed", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "cbTargetLost", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "edTargetLastPosX", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "edTargetLastPosY", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "edTargetLastDistanceMin", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbMain", "edTargetTypeLost", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbMain", "cbStar", "combo", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("pcMissile", "tsBonusSpecial", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsBonusSpecial", "gbBonus", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbBonus", "lblBonusNum", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbBonus", "lblBonusName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbBonus", "lblBonusBlockName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbBonus", "lblBonusCRC", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbBonus", "cbBonusName", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbBonus", "edBonusNum", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbBonus", "edBonusBlockName", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbBonus", "edBonusCRC", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsBonusSpecial", "gbSpecial", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSpecial", "lblSpecialNum", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSpecial", "lblSpecialName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSpecial", "lblSpecialBlockName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSpecial", "lblSpecialCRC", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSpecial", "cbSpecialName", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSpecial", "edSpecialNum", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSpecial", "edSpecialBlockName", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbSpecial", "edSpecialCRC", "edit", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TMODSLISTFORM()
        {
            return new EditorFormDefinition(
                "TMODSLISTFORM", "Моды сохранения",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "lblModsDeleteWarning", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "btnModsCfg", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "mmModsList", "memo", false, false, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("$form", "clbModsList", "checklist", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TOLDQUESTFORM()
        {
            return new EditorFormDefinition(
                "TOLDQUESTFORM", "Завершённое задание",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbOldQuest", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOldQuest", "lblTypeQuest", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOldQuest", "lblQuestNumber", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOldQuest", "lblPlanet", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOldQuest", "lblTextQuest", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOldQuest", "cbTypeQuest", "combo", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbOldQuest", "edQuestNumber", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOldQuest", "chbSuccessful", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOldQuest", "chbHideTags", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbOldQuest", "cbPlanet", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOldQuest", "chbRejection", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOldQuest", "mmTextQuest", "memo", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TORDERFORM()
        {
            return new EditorFormDefinition(
                "TORDERFORM", "Приказ корабля",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbOrder", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOrder", "lblOrderType", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOrder", "lblOrderObj", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOrder", "lblOrderDes", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOrder", "lblOrderEnd", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOrder", "lblOrderEndTime", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOrder", "cbOrderType", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOrder", "cbOrderObj", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOrder", "edOrderDesX", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOrder", "edOrderDesY", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOrder", "cbOrderEnd", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOrder", "edOrderEndTime", "edit", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TPATHDIALOGFORM()
        {
            return new EditorFormDefinition(
                "TPATHDIALOGFORM", "Выбор папки",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "sLabel1", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "sBitBtn1", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "sBitBtn2", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "sBitBtn3", "button", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "sShellTreeView1", "tree", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "sScrollBox1", "scroll", false, false, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TPLANETFORM()
        {
            return new EditorFormDefinition(
                "TPLANETFORM", "Редактор планеты",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbPlanet", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlanet", "pcPlanet", "tabs", false, true, true, true, false, false, -1, null, "tsParams"),
                new EditorNodeDefinition("pcPlanet", "tsParams", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsParams", "pcParams", "tabs", false, true, true, true, false, false, -1, null, "tsMain"),
                new EditorNodeDefinition("pcParams", "tsMain", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsMain", "gbCommon", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblPlanetName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblPos", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblPeopleCnt", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblMoney", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblCustomFaction", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblGoverment", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblOwner", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblRace", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblEconomy", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblNoPlanetShopUpdate", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblPolarPosAngle", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblPolarPosRadius", "label", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edPlanetName", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edPolarPosAngle", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edPolarPosRadius", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edPeopleCnt", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edMoney", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edCustomFaction", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "cbEconomy", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "cbGoverment", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "cbOwner", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "cbRace", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "chbIsMainPiratePlanet", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "chbNoBuyShips", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "chbNoLanding", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "chbNoRandomEvents", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edNoPlanetShopUpdate", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "chbVisitedByPlayer", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsMain", "gbLocation", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbLocation", "lblStar", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbLocation", "lblStarVal", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbLocation", "lblConstellation", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbLocation", "lblConstellationVal", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsMain", "gbSys", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSys", "lblQuestNumber", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSys", "lblRangerCnt", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSys", "lblTransportCnt", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSys", "lblRnd", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSys", "lblRndOut", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSys", "lblAngleSpeed", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSys", "lblRadius", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSys", "edQuestNumber", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSys", "edRangerCnt", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSys", "edTransportCnt", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSys", "edRnd", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSys", "edRndOut", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSys", "edAngleSpeed", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSys", "edRadius", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsMain", "gbGraph", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGraph", "lblGraphRadius", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGraph", "lblGraphName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGraph", "lblGraphSpeedRotate", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGraph", "lblGraphStepRotate", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGraph", "lblGraphRing", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGraph", "edGraphRadius", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGraph", "edGraphSpeedRotate", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGraph", "edGraphStepRotate", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGraph", "edGraphRing", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGraph", "cbGraphName", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsMain", "gbSurface", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSurface", "lblWaterSpace", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSurface", "lblWaterSpaceDone", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSurface", "lblLandSpace", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSurface", "lblLandSpaceDone", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSurface", "lblHillSpace", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSurface", "lblHillSpaceDone", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSurface", "lblOrbitCnt", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSurface", "edHillSpaceDone", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSurface", "edHillSpace", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSurface", "edLandSpaceDone", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSurface", "edLandSpace", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSurface", "edWaterSpaceDone", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSurface", "edWaterSpace", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSurface", "edOrbitCnt", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcParams", "tsAdditional", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsAdditional", "gbInvention", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbInvention", "lblCurrentInvention", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbInvention", "lblOpenPointsInvention", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbInvention", "lblNecessaryPercent", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbInvention", "lblNecessaryPercentK", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbInvention", "cbCurrentInvention", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbInvention", "edOpenPointsInvention", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbInvention", "edNecessaryPercent", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbInvention", "edNecessaryPercentK", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbInvention", "gbTL1", "group", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL1", "lblOpenInvention1", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL1", "lblOpenInvention2", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL1", "lblOpenInvention3", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL1", "lblOpenInvention4", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL1", "edOpenInvention1", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL1", "edOpenInvention2", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL1", "edOpenInvention3", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL1", "edOpenInvention4", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbInvention", "gbTL2", "group", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL2", "lblOpenInvention5", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL2", "lblOpenInvention6", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL2", "lblOpenInvention7", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL2", "lblOpenInvention8", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL2", "edOpenInvention5", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL2", "edOpenInvention6", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL2", "edOpenInvention7", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL2", "edOpenInvention8", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbInvention", "gbTL3", "group", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL3", "lblOpenInvention9", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL3", "lblOpenInvention10", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL3", "lblOpenInvention11", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL3", "lblOpenInvention12", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL3", "edOpenInvention10", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL3", "edOpenInvention11", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL3", "edOpenInvention12", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL3", "edOpenInvention9", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbInvention", "gbTL4", "group", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL4", "lblOpenInvention13", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL4", "lblOpenInvention14", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL4", "lblOpenInvention15", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL4", "lblOpenInvention16", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL4", "edOpenInvention13", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL4", "edOpenInvention14", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL4", "edOpenInvention15", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL4", "edOpenInvention16", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbInvention", "gbTL5", "group", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL5", "lblOpenInvention17", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL5", "lblOpenInvention18", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL5", "lblOpenInvention19", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL5", "lblOpenInvention20", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL5", "edOpenInvention17", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL5", "edOpenInvention18", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL5", "edOpenInvention19", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTL5", "edOpenInvention20", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsAdditional", "gbWarriors", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbWarriors", "lbWarriors", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsAdditional", "gbSputniks", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSputniks", "lbSputniks", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsAdditional", "gbRelationToRangers", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRelationToRangers", "lbRelationToRangers", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcPlanet", "tsPlanetShop", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsPlanetShop", "gbEquipmentShop", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbEquipmentShop", "lbEquipmentShop", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsPlanetShop", "gbShopGoods", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "lblNarcotics", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "lblMinerals", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "lblTechnics", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "lblMedicine", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "lblArms", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "lblAlcohol", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "lblLuxury", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "lblFood", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "lblCount", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "gbShopGoodsPrice", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "lblSale", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "lblBuy", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "edShopGoods12", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "edShopGoods13", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "edShopGoods22", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "edShopGoods23", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "edShopGoods32", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "edShopGoods33", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "edShopGoods42", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "edShopGoods43", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "edShopGoods52", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "edShopGoods53", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "edShopGoods62", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "edShopGoods63", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "edShopGoods72", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "edShopGoods73", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "edShopGoods82", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "edShopGoods83", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "edShopGoods11", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "edShopGoods21", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "edShopGoods31", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "edShopGoods41", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "edShopGoods51", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "edShopGoods61", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "edShopGoods71", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "edShopGoods81", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "gbShopGoodsEvents", "group", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsEvents", "lblDecay", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsEvents", "lblUpsurge", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsEvents", "edShopGoods14", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsEvents", "edShopGoods15", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsEvents", "edShopGoods24", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsEvents", "edShopGoods25", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsEvents", "edShopGoods34", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsEvents", "edShopGoods35", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsEvents", "edShopGoods44", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsEvents", "edShopGoods45", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsEvents", "edShopGoods54", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsEvents", "edShopGoods55", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsEvents", "edShopGoods64", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsEvents", "edShopGoods65", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsEvents", "edShopGoods74", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsEvents", "edShopGoods75", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsEvents", "edShopGoods84", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsEvents", "edShopGoods85", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcPlanet", "tsSurface", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsSurface", "gbGoneItems", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoneItems", "lbGoneItems", "owner-list", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TPLANETGONEITEMFORM()
        {
            return new EditorFormDefinition(
                "TPLANETGONEITEMFORM", "Потерянный предмет",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbGoneItem", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoneItem", "lblPos", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoneItem", "lblLandType", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoneItem", "lblRegion", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoneItem", "edPosX", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoneItem", "edPosY", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoneItem", "btnItemEdit", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoneItem", "edLandType", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoneItem", "chbMiss", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoneItem", "edRegion", "edit", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TPLANETNEWSFORM()
        {
            return new EditorFormDefinition(
                "TPLANETNEWSFORM", "Новость планеты",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbPlanetNews", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlanetNews", "lblID", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlanetNews", "lblTurn", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlanetNews", "lblType", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlanetNews", "lblNewsText", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlanetNews", "edTurn", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlanetNews", "edId", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlanetNews", "cbType", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlanetNews", "mmNewsText", "memo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlanetNews", "chbHideTags", "checkbox", false, true, true, true, true, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TQUESTFORM()
        {
            return new EditorFormDefinition(
                "TQUESTFORM", "Активное задание",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbQuest", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbQuest", "lblTypeQuest", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbQuest", "lblQuestNumber", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbQuest", "lblPlanet", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbQuest", "lblTurn", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbQuest", "lblSumm", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbQuest", "lblObj", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbQuest", "lblText", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbQuest", "lblCongratulations", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbQuest", "lblSpecial", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbQuest", "cbTypeQuest", "combo", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbQuest", "edQuestNumber", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbQuest", "edTurn", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbQuest", "edSumm", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbQuest", "chbSuccessful", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbQuest", "chbHideTags", "checkbox", false, true, true, true, true, false, -1, null, ""),
                new EditorNodeDefinition("gbQuest", "cbPlanet", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbQuest", "cbObj", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbQuest", "mmText", "memo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbQuest", "mmCongratulations", "memo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbQuest", "mmSpecial", "memo", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TRELATIONFORM()
        {
            return new EditorFormDefinition(
                "TRELATIONFORM", "Отношение",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbRelation", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRelation", "lblRelation", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRelation", "edRelation", "edit", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TREWARDFORM()
        {
            return new EditorFormDefinition(
                "TREWARDFORM", "Награда",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbReward", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbReward", "cbReward", "combo", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TROBOTMAPSTATFORM()
        {
            return new EditorFormDefinition(
                "TROBOTMAPSTATFORM", "Карта робота",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbRobotMapStat", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRobotMapStat", "lblID", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRobotMapStat", "lblTime", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRobotMapStat", "lblBuildRobot", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRobotMapStat", "lblKillRobot", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRobotMapStat", "lblBuildTurret", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRobotMapStat", "lblKillTurret", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRobotMapStat", "lblKillBuilding", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRobotMapStat", "lblBonus", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRobotMapStat", "lblState", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRobotMapStat", "lblTurn", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRobotMapStat", "edId", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRobotMapStat", "edTime", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRobotMapStat", "edBuildRobot", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRobotMapStat", "edKillRobot", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRobotMapStat", "edBuildTurret", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRobotMapStat", "edKillTurret", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRobotMapStat", "edKillBuilding", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRobotMapStat", "edBonus", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRobotMapStat", "edState", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRobotMapStat", "edTurn", "edit", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TSCALCFORM()
        {
            return new EditorFormDefinition(
                "TSCALCFORM", "Калькулятор",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "FMainPanel", "panel", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("FMainPanel", "FCalculatorPanel", "panel", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("FCalculatorPanel", "sSpeedButton1", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("FCalculatorPanel", "sSpeedButton2", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("FCalculatorPanel", "sSpeedButton3", "button", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("FCalculatorPanel", "sSpeedButton4", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("FCalculatorPanel", "sSpeedButton5", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("FCalculatorPanel", "sSpeedButton6", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("FCalculatorPanel", "sSpeedButton7", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("FCalculatorPanel", "sSpeedButton8", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("FCalculatorPanel", "sSpeedButton9", "button", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("FCalculatorPanel", "sSpeedButton10", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("FCalculatorPanel", "sSpeedButton11", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("FCalculatorPanel", "sSpeedButton12", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("FCalculatorPanel", "sSpeedButton13", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("FCalculatorPanel", "sSpeedButton14", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("FCalculatorPanel", "sSpeedButton15", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("FCalculatorPanel", "sSpeedButton16", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("FCalculatorPanel", "sSpeedButton17", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("FCalculatorPanel", "sSpeedButton18", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("FCalculatorPanel", "sSpeedButton19", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("FCalculatorPanel", "sSpeedButton20", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("FCalculatorPanel", "sSpeedButton21", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("FCalculatorPanel", "sSpeedButton22", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("FCalculatorPanel", "sSpeedButton23", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("FCalculatorPanel", "sSpeedButton24", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("FCalculatorPanel", "sSpeedButton25", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("FCalculatorPanel", "sSpeedButton26", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("FCalculatorPanel", "sSpeedButton27", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("FCalculatorPanel", "FCalcPanel", "panel", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("FMainPanel", "sDragBar1", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sDragBar1", "sToolButton3", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("sDragBar1", "sToolButton1", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("FMainPanel", "FDisplayPanel", "panel", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("FMainPanel", "sPanel2", "panel", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TSCOLORDIALOGFORM()
        {
            return new EditorFormDefinition(
                "TSCOLORDIALOGFORM", "Выбор цвета",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "sLabel2", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "sLabel4", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "sLabel5", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "sLabel6", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "sSpeedButton1", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "sBitBtn1", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "sBitBtn2", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "ColorPanel", "panel", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "GradPanel", "panel", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "sREdit", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "sGEdit", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "sBEdit", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "sBitBtn3", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "sBitBtn4", "button", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "sHEdit", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "sSEdit", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "sVEdit", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "MainPal", "panel", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "AddPal", "panel", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "sEditDecimal", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "sEditHex", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "sBitBtn5", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "Panel1", "panel", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("Panel1", "SelectedPanel", "image", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("Panel1", "OldPanel", "image", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "sDragBar1", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("$form", "sAEdit", "edit", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TSCRIPTCACHEFORM()
        {
            return new EditorFormDefinition(
                "TSCRIPTCACHEFORM", "Кэш скрипта",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbScriptCache", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScriptCache", "lblName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScriptCache", "lblCntUse", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScriptCache", "lblLastTurn", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScriptCache", "lblRunScript", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScriptCache", "edName", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScriptCache", "edCntUse", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScriptCache", "edLastTurn", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScriptCache", "edRunScript", "edit", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TSCRIPTFORM()
        {
            return new EditorFormDefinition(
                "TSCRIPTFORM", "Редактор скрипта",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbScript", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScript", "gbInitVars", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbInitVars", "lbInitVars", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScript", "gbItems", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbItems", "lbItems", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScript", "gbShips", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShips", "lbShips", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScript", "gbTurnVars", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTurnVars", "lbTurnVars", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScript", "gbEthers", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbEthers", "lbEthers", "owner-list", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TSCRIPTITEMFORM()
        {
            return new EditorFormDefinition(
                "TSCRIPTITEMFORM", "Предмет скрипта",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbScriptItem", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScriptItem", "lblItemName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScriptItem", "lblItemData1", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScriptItem", "lblItemData2", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScriptItem", "lblItemData3", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScriptItem", "lblItemText1", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScriptItem", "lblItemText2", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScriptItem", "lblItemText3", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScriptItem", "lblOnUseCode", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScriptItem", "lblOnActCode", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScriptItem", "edItemName", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbScriptItem", "edData1", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbScriptItem", "edData2", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbScriptItem", "edData3", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbScriptItem", "chbCanSell", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScriptItem", "btnItemEdit", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScriptItem", "edTextData3", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbScriptItem", "edTextData2", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbScriptItem", "edTextData1", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbScriptItem", "mmOnUseCode", "memo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScriptItem", "mmOnActCode", "memo", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TSCRIPTSHIPFORM()
        {
            return new EditorFormDefinition(
                "TSCRIPTSHIPFORM", "Корабль скрипта",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbScriptShip", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScriptShip", "lblShipName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScriptShip", "lblShipGroup", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScriptShip", "lblShipData0", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScriptShip", "lblShipData1", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScriptShip", "lblShipData2", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScriptShip", "lblShipData3", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScriptShip", "lblShipState", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScriptShip", "lblCustomFaction", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScriptShip", "edShipName", "edit", false, true, true, false, false, true, -1, null, ""),
                new EditorNodeDefinition("gbScriptShip", "edGroup", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbScriptShip", "edData0", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbScriptShip", "edData1", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbScriptShip", "edData2", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbScriptShip", "edData3", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbScriptShip", "edStateNum", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbScriptShip", "edCustomFaction", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbScriptShip", "chbHit", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScriptShip", "chbHitPlayer", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbScriptShip", "btnShipEdit", "button", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TSETTINGSFORM()
        {
            return new EditorFormDefinition(
                "TSETTINGSFORM", "Настройки редактора",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbCommon", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblLanguage", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblGamePath", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "cbLanguage", "combo", false, true, true, true, false, false, 0, new string[] { "Русский", "English" }, ""),
                new EditorNodeDefinition("gbCommon", "edGamePath", "directory", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "chbFullLog", "checkbox", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TSHIPFORM()
        {
            return new EditorFormDefinition(
                "TSHIPFORM", "Редактор корабля",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbShip", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShip", "pcShip", "tabs", false, true, true, true, false, false, -1, null, "tsParams"),
                new EditorNodeDefinition("pcShip", "tsParams", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsParams", "pcParams", "tabs", false, true, true, true, false, false, -1, null, "tsMain"),
                new EditorNodeDefinition("pcParams", "tsMain", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsMain", "gbCommon", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblType", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblOwner", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblCustomTypeName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblPilotRace", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblPos", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblMoney", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblDay", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblFace", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblShipPartner", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblShipPartnerDay", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblPoints", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblFreePoints", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblDayWithoutPlayer", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblShipBad", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblShipGood", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblCurStanding", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "cbOwner", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "cbType", "combo", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edName", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "cbPilotRace", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edCustomTypeName", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "chbAbducted", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "chbForsage", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "chbInHiperSpace", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edDay", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edFace", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edMoney", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edPosX", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edPosY", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edShipPartnerDay", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edFreePoints", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edPoints", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edDayWithoutPlayer", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "cbShipBad", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "cbShipGood", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "cbShipPartner", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "cbCurStanding", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsMain", "gbLocation", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbLocation", "lblCurStar", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbLocation", "lblCurStarVal", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbLocation", "lblCurConstellation", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbLocation", "lblCurConstellationVal", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbLocation", "lblCurPlanet", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbLocation", "lblCurPlanetVal", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbLocation", "lblHomePlanet", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbLocation", "lblHomePlanetVal", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbLocation", "lblCurShip", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbLocation", "lblCurShipVal", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbLocation", "lblScriptShip", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbLocation", "lblScriptShipVal", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsMain", "gbOrder", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOrder", "lblOrderData", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOrder", "lblOrderType", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOrder", "lblOrderObj", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOrder", "lblOrderDes", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOrder", "lblScriptOrderAbsolute", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOrder", "cbOrderType", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOrder", "edOrderData", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOrder", "cbOrderObj", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOrder", "edOrderDesY", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOrder", "edOrderDesX", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOrder", "chbOrderAbsolute", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOrder", "cbFollowType", "combo", false, false, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOrder", "edScriptOrderAbsolute", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsMain", "gbSkills", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSkills", "lblAccuracy", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSkills", "lblMobility", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSkills", "lblTechnical", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSkills", "lblTrader", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSkills", "lblLeadership", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSkills", "lblCharm", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSkills", "edAccuracy", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSkills", "edMobility", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSkills", "edTechnical", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSkills", "edTrader", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSkills", "edCharm", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSkills", "edLeadership", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsMain", "gbSys", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSys", "lblRnd", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSys", "lblRndOut", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSys", "lblAngle", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSys", "lblRadiusStop", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSys", "edRnd", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSys", "edRndOut", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSys", "edAngle", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSys", "edRadiusStop", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSys", "chbShipDestroy", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSys", "chbRobbedByPlayer", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsMain", "gbGraph", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGraph", "lblGraphName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGraph", "lblGraphShipTrans", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGraph", "chbGraphDominator", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGraph", "edGraphShipTrans", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGraph", "cbGraphName", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGraph", "chbScriptChameleon", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsMain", "gbTakeItems", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTakeItems", "lbTakeItems", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsMain", "gbRelationToRangers", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRelationToRangers", "lbRelationToRangers", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcParams", "tsAdditional", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsAdditional", "gbAdditional", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "lblDaysLanded", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "lblGroupOrder", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "lblLastNextDay", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "lblTechLevelKnowledge", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "lblTradePenalty", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "lblTradePoints", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "lblRewardViewCount", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "lblContrabandPoints", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "lblCountOfDeflectedPlayerShots", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "lblAverageSpeed", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "lblAverageEnemySpeed", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "lblAverageEqValue", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "lblAverageCapital", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "lblAverageMoneyToCapital", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "lblAverageFreeSpaceRatio", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "lblRatioOfTooCostlyEqInShop", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "lblProtoplasm", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "edDaysLanded", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "edGroupOrder", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "edLastNextDay", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "edTechLevelKnowledge", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "edTradePenalty", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "edTradePoints", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "edRewardViewCount", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "edContrabandPoints", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "edAverageCapital", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "edAverageEnemySpeed", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "edAverageEqValue", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "edAverageFreeSpaceRatio", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "edAverageMoneyToCapital", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "edAverageSpeed", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "edCountOfDeflectedPlayerShots", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "edRatioOfTooCostlyEqInShop", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "gbSwarm", "group", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSwarm", "lblSwarmed", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSwarm", "lblSwarmAnimation", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSwarm", "lblSwarmedByShip", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSwarm", "cbSwarmedByShip", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSwarm", "edSwarmAnimation", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSwarm", "edSwarmed", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "gbProhibitions", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbProhibitions", "lblNoTarget", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbProhibitions", "chbNoDrop", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbProhibitions", "chbNoScan", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbProhibitions", "chbNoTalk", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbProhibitions", "cbNoTarget", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbAdditional", "edProtoplasm", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsAdditional", "gbChameleon", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbChameleon", "lblChameleonSeries", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbChameleon", "cbChameleonSeries", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbChameleon", "gbChameleonDetect", "group", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbChameleonDetect", "chbBlazerChameleonDetect", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbChameleonDetect", "chbKellerChameleonDetect", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbChameleonDetect", "chbTerronChameleonDetect", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbChameleon", "gbChameleonCharge", "group", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbChameleonCharge", "lblBlazerChameleonCharge", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbChameleonCharge", "lblTerronChameleonCharge", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbChameleonCharge", "lblKellerChameleonCharge", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbChameleonCharge", "edBlazerChameleonCharge", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbChameleonCharge", "edKellerChameleonCharge", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbChameleonCharge", "edTerronChameleonCharge", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsAdditional", "gbIllness", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbIllness", "lbIllness", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsAdditional", "gbRecentlyDroppedItems", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRecentlyDroppedItems", "lbRecentlyDroppedItems", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsAdditional", "gbSpecialBonuses", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSpecialBonuses", "lbSpecialBonuses", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsAdditional", "gbStatusEffects", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStatusEffects", "lbStatusEffects", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsAdditional", "gbRewards", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRewards", "lbRewards", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcParams", "tsSubType", "tab", false, true, false, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsSubType", "gbNormalShip", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "lblKillPirates", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "lblLiberationSystems", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "lblKillPacifics", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "lblKillInCurSystemNormals", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "lblLiberationKills", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "lblKillAllShips", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "lblKillDominators", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "lblKillWarriors", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "lblKillRangers", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "lblKillInCurSystemDominators", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "lblKillCustomInCurSystem", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "lblKillInCurSystemPirates", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "lblLiberationPlanet", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "lblRank", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "lblRankPoints", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "lblPirateRank", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "lblPirateRankPoints", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "lblLastPlanet", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "lblTurnPlayerMoneyGoods", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "edKillPacifics", "edit", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "edLiberationKills", "edit", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "edKillPirates", "edit", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "edKillInCurSystemDominators", "edit", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "edKillAllShips", "edit", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "edKillInCurSystemNormals", "edit", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "edKillInCurSystemPirates", "edit", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "edLiberationSystems", "edit", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "edKillDominators", "edit", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "edKillCustomInCurSystem", "edit", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "edKillRangers", "edit", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "edKillWarriors", "edit", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "cbRank", "combo", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "edRankPoints", "edit", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "cbPirateRank", "combo", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "edPirateRankPoints", "edit", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "edTurnPlayerMoneyGoods", "edit", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "cbLastPlanet", "combo", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbNormalShip", "cbLiberationPlanet", "combo", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("tsSubType", "gbWarriorShip", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbWarriorShip", "lblWarriorType", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbWarriorShip", "cbWarriorType", "combo", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("tsSubType", "gbTransportShip", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTransportShip", "lblTransportType", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTransportShip", "cbTransportType", "combo", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("tsSubType", "gbPirateShip", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPirateShip", "lblPirateType", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPirateShip", "lblPiratePrison", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPirateShip", "lblDesireConflict", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPirateShip", "cbPirateType", "combo", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPirateShip", "edPiratePrison", "edit", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPirateShip", "edDesireConflict", "edit", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("tsSubType", "gbDominatorShip", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbDominatorShip", "lblDominatorType", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbDominatorShip", "lblRunProgrammDate", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbDominatorShip", "lblRunProgrammName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbDominatorShip", "cbDominatorType", "combo", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbDominatorShip", "cbDominatorSeries", "combo", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbDominatorShip", "edRunProgrammDate", "edit", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbDominatorShip", "cbRunProgrammName", "combo", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("tsSubType", "gbRangerShip", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRangerShip", "lblRangerPrison", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRangerShip", "lblLastShip", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRangerShip", "lblNods", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRangerShip", "lblStatusTrader", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRangerShip", "lblStatusPirate", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRangerShip", "lblStatusWarrior", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRangerShip", "lblEminentPointsTrader", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRangerShip", "lblEminentPointsPirate", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRangerShip", "lblEminentPointsWarrior", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRangerShip", "lblMoral", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRangerShip", "lblCourageous", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRangerShip", "lblStatusChangePirate", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRangerShip", "lblStatusChangeTrader", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRangerShip", "lblStatusChangeWarrior", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRangerShip", "gbQuests", "group", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbQuests", "lbQuests", "owner-list", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRangerShip", "edRangerPrison", "edit", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRangerShip", "edNods", "edit", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRangerShip", "cbLastShip", "combo", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRangerShip", "gbProgramms", "group", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbProgramms", "sgProgramms", "grid", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRangerShip", "cbMoral", "combo", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRangerShip", "chbExcludedFromRating", "checkbox", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRangerShip", "edCourageous", "edit", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRangerShip", "edEminentPointsPirate", "edit", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRangerShip", "edEminentPointsTrader", "edit", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRangerShip", "edEminentPointsWarrior", "edit", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRangerShip", "edStatusChangePirate", "edit", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRangerShip", "edStatusChangeTrader", "edit", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRangerShip", "edStatusChangeWarrior", "edit", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRangerShip", "edStatusPirate", "edit", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRangerShip", "edStatusTrader", "edit", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRangerShip", "edStatusWarrior", "edit", false, true, true, false, false, false, -1, null, ""),
                new EditorNodeDefinition("pcParams", "tsPlayer", "tab", false, true, false, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsPlayer", "gbPlayerShip", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "lblKillShipInGiperSpace", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "lblKillShipInHole", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "lblDebt", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "lblDebtDate", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "lblDebtCnt", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "lblDeposit", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "lblDepositDate", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "lblDepositDay", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "lblDepositPercent", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "lblMedPolicy", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "lblPirateLicense", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "lblPiratePoints", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "lblPirateNewPoints", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "lblFlyToStar", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "lblImmunity", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "lblDayWBGiveProgramms", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "lblPlanetBattlesWin", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "lblLastPlanetBattleDate", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "lblCntIll", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "lblCntStim", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "lblCntPrison", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "lblUnkPlanetComplete", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "lblCntChangeRace", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "lblCntChangeSide", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "lblHotEquipmentCur", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "lblGotoGov", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "lblExpPointsForKills", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "lblExpPointsForTrade", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "lblHitEnemyAfterTakeProgramms", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "chbPlayerPrison", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "chbTalkLocked", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "chbScanLocked", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "edKillShipInGiperSpace", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "edKillShipInHole", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "edDebt", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "edDebtDate", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "edDebtCnt", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "edDeposit", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "edDepositDate", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "edDepositDay", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "edDepositPercent", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "edMedPolicy", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "edPirateLicense", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "edPiratePoints", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "edPirateNewPoints", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "cbFlyToStar", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "edImmunity", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "edDayWBGiveProgramms", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "edPlanetBattlesWin", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "edLastPlanetBattleDate", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "edCntIll", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "edCntStim", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "edCntPrison", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "edUnkPlanetComplete", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "edCntChangeRace", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "edCntChangeSide", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "edHotEquipmentCur", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "gbJournal", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbJournal", "lbJournal", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "gbPlanetNews", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlanetNews", "lbPlanetNews", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "btnSets", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "edGotoGov", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "chbNoJump", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "chbPirateClanReal", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "edExpPointsForDominatorKills", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "edExpPointsForPirateKills", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "edExpPointsForGoodShipKills", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "edExpPointsForTrade", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "gbBridge", "group", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbBridge", "lblCaptainOnTheBridge", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbBridge", "lblBridgeCurShip", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbBridge", "lblBridgeCurPlanet", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbBridge", "lblBridgeBGReplace", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbBridge", "edCaptainOnTheBridge", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbBridge", "cbBridgeCurShip", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbBridge", "cbBridgeCurPlanet", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbBridge", "edBridgeBGReplace", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbBridge", "btnBridge", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "gbRobotMap", "group", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRobotMap", "lbRobotMaps", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "gbProgrammsInWB", "group", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbProgrammsInWB", "sgProgrammsInWB", "grid", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "gbInvestmentDay", "group", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbInvestmentDay", "sgInvestmentDay", "grid", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "btnInfectionsPlace", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "gbKillDominatorsByType", "group", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbKillDominatorsByType", "sgKillDominatorsByType", "grid", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "gbChameleonLogic", "group", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbChameleonLogic", "lblChameleonLogicBlazer", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbChameleonLogic", "lblChameleonLogicKeller", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbChameleonLogic", "lblChameleonLogicTerron", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbChameleonLogic", "edChameleonLogicBlazer", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbChameleonLogic", "edChameleonLogicKeller", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbChameleonLogic", "edChameleonLogicTerron", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbPlayerShip", "edHitEnemyAfterTakeProgramms", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcParams", "tsTranclucator", "tab", false, true, false, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsTranclucator", "gbTranclucator", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTranclucator", "lblArtSize", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTranclucator", "lblArtSysName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTranclucator", "lblProprietor", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTranclucator", "edArtSize", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTranclucator", "edArtSysName", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTranclucator", "chbDocking", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTranclucator", "chbAutoArrange", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTranclucator", "cbProprietor", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTranclucator", "gbSeekItems", "group", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSeekItems", "clbSeekPermit", "checklist", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbTranclucator", "gbLandStorage", "group", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbLandStorage", "chbLandPermit1", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbLandStorage", "chbLandPermit2", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcShip", "tsHold", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsHold", "gbEquipments", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbEquipments", "lbEquipments", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsHold", "gbArtefacts", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbArtefacts", "lbArtefacts", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsHold", "gbGoods", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoods", "lblArms", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoods", "lblTechnics", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoods", "lblNarcotics", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoods", "lblMedicine", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoods", "lblFoods", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoods", "lblAlcohol", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoods", "lblLuxury", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoods", "lblMinerals", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoods", "gbGoodsStatistic", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsStatistic", "lblGoodsBuyCnt", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsStatistic", "lblGoodsBuyCost", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsStatistic", "edGoods13", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsStatistic", "edGoods14", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsStatistic", "edGoods23", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsStatistic", "edGoods24", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsStatistic", "edGoods33", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsStatistic", "edGoods34", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsStatistic", "edGoods43", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsStatistic", "edGoods44", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsStatistic", "edGoods53", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsStatistic", "edGoods54", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsStatistic", "edGoods63", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsStatistic", "edGoods64", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsStatistic", "edGoods73", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsStatistic", "edGoods74", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsStatistic", "edGoods83", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsStatistic", "edGoods84", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoods", "gbGoodsHold", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsHold", "lblGoodsCnt", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsHold", "lblGoodsCost", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsHold", "edGoods11", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsHold", "edGoods12", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsHold", "edGoods22", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsHold", "edGoods21", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsHold", "edGoods52", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsHold", "edGoods62", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsHold", "edGoods41", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsHold", "edGoods51", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsHold", "edGoods72", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsHold", "edGoods61", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsHold", "edGoods71", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsHold", "edGoods31", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsHold", "edGoods32", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsHold", "edGoods82", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsHold", "edGoods42", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbGoodsHold", "edGoods81", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcShip", "tsRuins", "tab", false, true, false, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsRuins", "gbEquipmentShop", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbEquipmentShop", "lbEquipmentShop", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsRuins", "gbShopGoods", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "lblShopNarcotics", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "lblShopMinerals", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "lblShopTechnics", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "lblShopMedicine", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "lblShopArms", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "lblShopAlcohol", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "lblShopLuxury", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "lblShopFoods", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "lblShopCount", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "gbShopGoodsPrice", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "lblShopSale", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "lblShopBuy", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "edShopGoods12", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "edShopGoods13", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "edShopGoods22", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "edShopGoods23", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "edShopGoods32", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "edShopGoods33", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "edShopGoods42", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "edShopGoods43", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "edShopGoods52", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "edShopGoods53", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "edShopGoods62", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "edShopGoods63", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "edShopGoods72", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "edShopGoods73", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "edShopGoods82", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoodsPrice", "edShopGoods83", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "edShopGoods11", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "edShopGoods21", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "edShopGoods31", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "edShopGoods41", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "edShopGoods51", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "edShopGoods61", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "edShopGoods71", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShopGoods", "edShopGoods81", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsRuins", "gbRuins", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRuins", "lblRuinsEnergy", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRuins", "lblRuinsFlyToStar", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRuins", "lblFlyDate", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRuins", "lblNoShopUpdate", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRuins", "edRuinsEnergy", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRuins", "cbRuinsFlyToStar", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRuins", "edFlyDate", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRuins", "gbSaleSatellites", "group", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSaleSatellites", "lbSaleSatellites", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRuins", "chbSponsor", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRuins", "chbSpecialShip", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRuins", "chbNoLanding", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRuins", "edNoShopUpdate", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("pcShip", "tsMods", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsMods", "gbModsDropList", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbModsDropList", "lbDropList", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsMods", "gbCustomShipInfos", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomShipInfos", "lbCustomShipInfos", "owner-list", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TSPECIALBONUSFORM()
        {
            return new EditorFormDefinition(
                "TSPECIALBONUSFORM", "Особый бонус",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbSpecialBonus", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSpecialBonus", "lblBonusType", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSpecialBonus", "lblBonusValue", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSpecialBonus", "cbBonusType", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSpecialBonus", "edBonusValue", "edit", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TSPOPUPCALENDAR()
        {
            return new EditorFormDefinition(
                "TSPOPUPCALENDAR", "Календарь",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "sMonthCalendar1", "panel", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TSPUTNIKFORM()
        {
            return new EditorFormDefinition(
                "TSPUTNIKFORM", "Планетарный зонд",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbSputnik", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSputnik", "lblGraphName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSputnik", "lblAngleCur", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSputnik", "edAngleCur", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbSputnik", "cbGraphName", "combo", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TSTARDROPITEMFORM()
        {
            return new EditorFormDefinition(
                "TSTARDROPITEMFORM", "Предмет в системе",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbStarDropItem", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarDropItem", "lblShipName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarDropItem", "lblPos", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarDropItem", "lblShipID", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarDropItem", "edPosX", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarDropItem", "edPosY", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarDropItem", "btnItemEdit", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarDropItem", "edShipID", "edit", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbStarDropItem", "chbInStar", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarDropItem", "chbInUse", "checkbox", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TSTARFORM()
        {
            return new EditorFormDefinition(
                "TSTARFORM", "Звёздная система",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbStar", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStar", "pcStar", "tabs", false, true, true, true, false, false, -1, null, "tsParams"),
                new EditorNodeDefinition("pcStar", "tsParams", "tab", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsParams", "gbCommon", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblStarName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblPos", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblGraphStar", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblMapLabel", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "lblSystemBackground", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edStarName", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edPosX", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edPosY", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edMapLabel", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "cbGraphStar", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCommon", "edSystemBackground", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsParams", "gbStarStatus", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarStatus", "lblSafety", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarStatus", "lblOwners", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarStatus", "lblLastOwners", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarStatus", "lblDominatorSeries", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarStatus", "lblCustomFaction", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarStatus", "lblDayBeforeOccupy", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarStatus", "lblDayWithoutPlayer", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarStatus", "lblDayWithoutCreateShip", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarStatus", "lblLastDominatorDate", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarStatus", "lblLastPirateDate", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarStatus", "lblLiberationDate", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarStatus", "lblDayInvadeInertia", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarStatus", "lblDominion", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarStatus", "chbBattle", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarStatus", "cbOwners", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarStatus", "cbLastOwners", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarStatus", "cbDominatorSeries", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarStatus", "edCustomFaction", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarStatus", "chbNoComeKling", "checkbox", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarStatus", "edDayBeforeOccupy", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarStatus", "edDayWithoutPlayer", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarStatus", "edDayWithoutCreateShip", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarStatus", "edLastDominatorDate", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarStatus", "edLastPirateDate", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarStatus", "edLiberationDate", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarStatus", "edDayInvadeInertia", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarStatus", "edSafety", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStarStatus", "cbDominion", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsParams", "gbRadius", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRadius", "lblSafeRadius", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRadius", "lblDamageRadius", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRadius", "lblGraphRadius", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRadius", "lblRadius", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRadius", "edDamageRadius", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRadius", "edGraphRadius", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRadius", "edSafeRadius", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbRadius", "edRadius", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsParams", "gbItemsDrop", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbItemsDrop", "lbItemsDrop", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("tsParams", "gbCustomStarInfo", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbCustomStarInfo", "lbCustomStarInfo", "owner-list", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TSTARMAPFORM()
        {
            return new EditorFormDefinition(
                "TSTARMAPFORM", "Карта звёздной системы",
                new EditorNodeDefinition[]
                {

                });
        }

        private static EditorFormDefinition Build_TSTATUSEFFECTFORM()
        {
            return new EditorFormDefinition(
                "TSTATUSEFFECTFORM", "Эффект состояния",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbStatusEffect", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStatusEffect", "lblEffectType", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStatusEffect", "lblEffectStrength", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStatusEffect", "lblEffectShipId", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStatusEffect", "sLabelShipName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStatusEffect", "cbEffectType", "combo", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStatusEffect", "edEffectLastSourceShipId", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStatusEffect", "edEffectStrength", "edit", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TSTORAGEITEMFORM()
        {
            return new EditorFormDefinition(
                "TSTORAGEITEMFORM", "Предмет склада",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbStorageItem", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStorageItem", "lblSlot", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStorageItem", "lblItemPlace", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStorageItem", "btnItemEdit", "button", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStorageItem", "edSlot", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbStorageItem", "cbItemPlace", "combo", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TVARARRAYVIEWFORM()
        {
            return new EditorFormDefinition(
                "TVARARRAYVIEWFORM", "Массив переменных",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "lvArray", "listview", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("$form", "edSearch", "edit", false, true, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TVARFORM()
        {
            return new EditorFormDefinition(
                "TVARFORM", "Переменная",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbVar", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbVar", "lblVarName", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbVar", "lblVarType", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbVar", "lblVarValue", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbVar", "edVarName", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbVar", "cbVarType", "combo", false, true, true, true, false, true, -1, null, ""),
                new EditorNodeDefinition("gbVar", "edVarValue", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbVar", "lbArray", "list", false, false, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbVar", "btnShowArray", "button", false, false, true, true, false, false, -1, null, "")
                });
        }

        private static EditorFormDefinition Build_TWAROPERATIONFORM()
        {
            return new EditorFormDefinition(
                "TWAROPERATIONFORM", "Военная операция",
                new EditorNodeDefinition[]
                {
                new EditorNodeDefinition("$form", "gbWarOperation", "group", true, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbWarOperation", "lblTurn", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbWarOperation", "lblRnd", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbWarOperation", "lblRndOut", "label", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbWarOperation", "edTurn", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbWarOperation", "edRnd", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbWarOperation", "edRndOut", "edit", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbWarOperation", "gbShips", "group", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbShips", "lbShips", "owner-list", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbWarOperation", "gbOrders", "group", false, true, true, true, false, false, -1, null, ""),
                new EditorNodeDefinition("gbOrders", "lbOrders", "owner-list", false, true, true, true, false, false, -1, null, "")
                });
        }
    }
}

// ReSharper disable InconsistentNaming
namespace Koppen
{
    //This is unused in the current version because I opted for a string-based
    //classification approach. I think it still may be useful in the future, so
    //I left the enum defined.
    public enum ClimateZone
    {
        EF_Icecap,
        ET_Tundra,

        Dfa_HotSummerHumidContinental,
        Dfb_WarmSummerHumidContinental,
        Dfc_Subarctic,
        Dfd_ColdSubarctic,

        Dwa_MonsoonInfluencedHotSummerHumidContinental,
        Dwb_MonsoonInfluencedWarmSummerHumidContinental,
        Dwc_MonsoonInfluencedSubarctic,
        Dwd_MonsoonInfluencedColdSubarctic,

        Dsa_MediterraneanInfluencedHotSummerHumidContinental,
        Dsb_MediterraneanInfluencedWarmSummerHumidContinental,
        Dsc_MediterraneanInfluencedSubarctic,
        Dsd_MediterraneanInfluencedColdSubarctic,

        Cfa_HumidSubtropical,
        Cfb_TemperateOceanic,
        Cfc_SubpolarOceanic,

        Cwa_MonsoonInfluencedHumidSubtropical,
        Cwb_MonsoonInfluencedTemperateOceanic,
        Cwc_MonsoonInfluencedSubpolarOceanic,

        Csa_HotSummerMediterranean,
        Csb_WarmSummerMediterranean,
        Csc_ColdSummerMediterranean,

        BWh_HotDesert,
        BWk_ColdDesert,

        BSh_HotSteppe,
        BSk_ColdSteppe,

        Af_TropicalRainforest,
        Am_TropicalMonsoon,
        Aw_TropicalSavanna

    }
}
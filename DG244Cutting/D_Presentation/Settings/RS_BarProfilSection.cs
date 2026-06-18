namespace DG244Cutting.D_Presentation.Settings
{
    /// <summary>
    /// Référentiel statique de données des sections de profil de barre disponibles dans l'application.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Classe de référence interne à la couche Presentation, consommée par
    /// <see cref="SE_BarProfilSection"/> et son contrat <c>ISE_BarProfilSection</c> qui constituent
    /// l'intermédiation canonique de la présente classe vers les ViewModels et les contrôles visuels.
    /// Le SE_ consommateur expose à ses consommateurs la résolution centralisée d'une référence
    /// textuelle de section vers son URI, avec repli explicite sur la section par défaut portée
    /// par <see cref="DefaultBarProfilSection_Source"/>.</para>
    /// <para>Objectif : Centraliser les URI des sections de profil de barre et le dictionnaire
    /// de résolution par référence textuelle. Cette classe est une donnée de référence purement
    /// statique — elle ne porte aucun état mutable.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item>Déclarer les URI absolues de chaque section de profil de barre disponible</item>
    /// <item>Exposer le dictionnaire de résolution référence → URI</item>
    /// <item>Fournir l'URI de repli par défaut</item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item>Aucune logique métier ni sélection contextuelle</item>
    /// <item>Aucun état mutable</item>
    /// </list>
    /// <para>
    /// Nature « Référentiel Statique » : conformément à la section 2.7.5 du
    /// référentiel, un RS_ contient des données stables au runtime, non persistées
    /// en base, et résolues en compilation lorsque c'est techniquement possible.
    /// Les URI exposés ici sont construits par concaténation à partir d'une
    /// constante <c>const string</c>, ce qui supprime tout problème d'ordre
    /// d'initialisation statique.
    /// </para>
    /// </remarks>
    internal static class RS_BarProfilSection
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Préfixe <c>pack://</c> pointant vers le dossier <c>Resources/BarProfilSection</c>
        /// de l'assembly courante <c>DG244Cutting</c>. Constante compilée afin de
        /// supprimer toute dépendance à l'ordre d'initialisation des champs
        /// statiques.
        /// </summary>
        private const string _base = "pack://application:,,,/DG244Cutting;component/D_Presentation/Resources/BarProfilSection/";

        #endregion

        #region === Dépendances privées ===

        // A compléter

        #endregion

        #region === Propriétés publiques ===

        /// <summary>URI de la section de profil de barre par défaut utilisée en cas de repli.</summary>
        public static readonly Uri DefaultBarProfilSection_Source = GetBarProfilSection("RE_XXXXXXX.png");

        public static readonly Uri BSP_02CJ_Source = GetBarProfilSection("RE_02CJ.png");
        public static readonly Uri BSP_10200_Source = GetBarProfilSection("RE_10200.png");
        public static readonly Uri BSP_10201_Source = GetBarProfilSection("RE_10201.png");
        public static readonly Uri BSP_10202_Source = GetBarProfilSection("RE_10202.png");
        public static readonly Uri BSP_10203_Source = GetBarProfilSection("RE_10203.png");
        public static readonly Uri BSP_10204_Source = GetBarProfilSection("RE_10204.png");
        public static readonly Uri BSP_10205_Source = GetBarProfilSection("RE_10205.png");
        public static readonly Uri BSP_10206_Source = GetBarProfilSection("RE_10206.png");
        public static readonly Uri BSP_10211_Source = GetBarProfilSection("RE_10211.png");
        public static readonly Uri BSP_10213_Source = GetBarProfilSection("RE_10213.png");
        public static readonly Uri BSP_10214_Source = GetBarProfilSection("RE_10214.png");
        public static readonly Uri BSP_10215_Source = GetBarProfilSection("RE_10215.png");
        public static readonly Uri BSP_10250_Source = GetBarProfilSection("RE_10250.png");
        public static readonly Uri BSP_10252_Source = GetBarProfilSection("RE_10252.png");
        public static readonly Uri BSP_10253_Source = GetBarProfilSection("RE_10253.png");
        public static readonly Uri BSP_10254_Source = GetBarProfilSection("RE_10254.png");
        public static readonly Uri BSP_10260_Source = GetBarProfilSection("RE_10260.png");
        public static readonly Uri BSP_10302_Source = GetBarProfilSection("RE_10302.png");
        public static readonly Uri BSP_10303_Source = GetBarProfilSection("RE_10303.png");
        public static readonly Uri BSP_10304_Source = GetBarProfilSection("RE_10304.png");
        public static readonly Uri BSP_10305_Source = GetBarProfilSection("RE_10305.png");
        public static readonly Uri BSP_10306_Source = GetBarProfilSection("RE_10306.png");
        public static readonly Uri BSP_10309_Source = GetBarProfilSection("RE_10309.png");
        public static readonly Uri BSP_11034_Source = GetBarProfilSection("RE_11034.png");
        public static readonly Uri BSP_11035_Source = GetBarProfilSection("RE_11035.png");
        public static readonly Uri BSP_11660_Source = GetBarProfilSection("RE_11660.png");
        public static readonly Uri BSP_12100_Source = GetBarProfilSection("RE_12100.png");
        public static readonly Uri BSP_12101_Source = GetBarProfilSection("RE_12101.png");
        public static readonly Uri BSP_12102_Source = GetBarProfilSection("RE_12102.png");
        public static readonly Uri BSP_12103_Source = GetBarProfilSection("RE_12103.png");
        public static readonly Uri BSP_12104_Source = GetBarProfilSection("RE_12104.png");
        public static readonly Uri BSP_12105_Source = GetBarProfilSection("RE_12105.png");
        public static readonly Uri BSP_12106_Source = GetBarProfilSection("RE_12106.png");
        public static readonly Uri BSP_12107_Source = GetBarProfilSection("RE_12107.png");
        public static readonly Uri BSP_12108_Source = GetBarProfilSection("RE_12108.png");
        public static readonly Uri BSP_12109_Source = GetBarProfilSection("RE_12109.png");
        public static readonly Uri BSP_12110_Source = GetBarProfilSection("RE_12110.png");
        public static readonly Uri BSP_12111_Source = GetBarProfilSection("RE_12111.png");
        public static readonly Uri BSP_12112_Source = GetBarProfilSection("RE_12112.png");
        public static readonly Uri BSP_12113_Source = GetBarProfilSection("RE_12113.png");
        public static readonly Uri BSP_12114_Source = GetBarProfilSection("RE_12114.png");
        public static readonly Uri BSP_12115_Source = GetBarProfilSection("RE_12115.png");
        public static readonly Uri BSP_12116_Source = GetBarProfilSection("RE_12116.png");
        public static readonly Uri BSP_12117_Source = GetBarProfilSection("RE_12117.png");
        public static readonly Uri BSP_12118_Source = GetBarProfilSection("RE_12118.png");
        public static readonly Uri BSP_12119_Source = GetBarProfilSection("RE_12119.png");
        public static readonly Uri BSP_12120_Source = GetBarProfilSection("RE_12120.png");
        public static readonly Uri BSP_12121_Source = GetBarProfilSection("RE_12121.png");
        public static readonly Uri BSP_12122_Source = GetBarProfilSection("RE_12122.png");
        public static readonly Uri BSP_12123_Source = GetBarProfilSection("RE_12123.png");
        public static readonly Uri BSP_12124_Source = GetBarProfilSection("RE_12124.png");
        public static readonly Uri BSP_12125_Source = GetBarProfilSection("RE_12125.png");
        public static readonly Uri BSP_12126_Source = GetBarProfilSection("RE_12126.png");
        public static readonly Uri BSP_12127_Source = GetBarProfilSection("RE_12127.png");
        public static readonly Uri BSP_12128_Source = GetBarProfilSection("RE_12128.png");
        public static readonly Uri BSP_12129_Source = GetBarProfilSection("RE_12129.png");
        public static readonly Uri BSP_12130_Source = GetBarProfilSection("RE_12130.png");
        public static readonly Uri BSP_12131_Source = GetBarProfilSection("RE_12131.png");
        public static readonly Uri BSP_12132_Source = GetBarProfilSection("RE_12132.png");
        public static readonly Uri BSP_12133_Source = GetBarProfilSection("RE_12133.png");
        public static readonly Uri BSP_12134_Source = GetBarProfilSection("RE_12134.png");
        public static readonly Uri BSP_12135_Source = GetBarProfilSection("RE_12135.png");
        public static readonly Uri BSP_12136_Source = GetBarProfilSection("RE_12136.png");
        public static readonly Uri BSP_12139_Source = GetBarProfilSection("RE_12139.png");
        public static readonly Uri BSP_12157_Source = GetBarProfilSection("RE_12157.png");
        public static readonly Uri BSP_14010_Source = GetBarProfilSection("RE_14010.png");
        public static readonly Uri BSP_14347_Source = GetBarProfilSection("RE_14347.png");
        public static readonly Uri BSP_14403_Source = GetBarProfilSection("RE_14403.png");
        public static readonly Uri BSP_14591_Source = GetBarProfilSection("RE_14591.png");
        public static readonly Uri BSP_14592_Source = GetBarProfilSection("RE_14592.png");
        public static readonly Uri BSP_14594_Source = GetBarProfilSection("RE_14594.png");
        public static readonly Uri BSP_14711_Source = GetBarProfilSection("RE_14711.png");
        public static readonly Uri BSP_15419_Source = GetBarProfilSection("RE_15419.png");
        public static readonly Uri BSP_15441_Source = GetBarProfilSection("RE_15441.png");
        public static readonly Uri BSP_15442_Source = GetBarProfilSection("RE_15442.png");
        public static readonly Uri BSP_15491_Source = GetBarProfilSection("RE_15491.png");
        public static readonly Uri BSP_15492_Source = GetBarProfilSection("RE_15492.png");
        public static readonly Uri BSP_15511_Source = GetBarProfilSection("RE_15511.png");
        public static readonly Uri BSP_15512_Source = GetBarProfilSection("RE_15512.png");
        public static readonly Uri BSP_15600_Source = GetBarProfilSection("RE_15600.png");
        public static readonly Uri BSP_15602_Source = GetBarProfilSection("RE_15602.png");
        public static readonly Uri BSP_15712_Source = GetBarProfilSection("RE_15712.png");
        public static readonly Uri BSP_15750_Source = GetBarProfilSection("RE_15750.png");
        public static readonly Uri BSP_15760_Source = GetBarProfilSection("RE_15760.png");
        public static readonly Uri BSP_15837_Source = GetBarProfilSection("RE_15837.png");
        public static readonly Uri BSP_15850_Source = GetBarProfilSection("RE_15850.png");
        public static readonly Uri BSP_16969_Source = GetBarProfilSection("RE_16969.png");
        public static readonly Uri BSP_16978_Source = GetBarProfilSection("RE_16978.png");
        public static readonly Uri BSP_16990_Source = GetBarProfilSection("RE_16990.png");
        public static readonly Uri BSP_16997_Source = GetBarProfilSection("RE_16997.png");
        public static readonly Uri BSP_16998_Source = GetBarProfilSection("RE_16998.png");
        public static readonly Uri BSP_17045_Source = GetBarProfilSection("RE_17045.png");
        public static readonly Uri BSP_17051_Source = GetBarProfilSection("RE_17051.png");
        public static readonly Uri BSP_17088_Source = GetBarProfilSection("RE_17088.png");
        public static readonly Uri BSP_17131_Source = GetBarProfilSection("RE_17131.png");
        public static readonly Uri BSP_17133_Source = GetBarProfilSection("RE_17133.png");
        public static readonly Uri BSP_17208_Source = GetBarProfilSection("RE_17208.png");
        public static readonly Uri BSP_2020_Source = GetBarProfilSection("RE_2020.png");
        public static readonly Uri BSP_214700_Source = GetBarProfilSection("RE_214700.png");
        public static readonly Uri BSP_2290_Source = GetBarProfilSection("RE_2290.png");
        public static readonly Uri BSP_2402_Source = GetBarProfilSection("RE_2402.png");
        public static readonly Uri BSP_26004_Source = GetBarProfilSection("RE_26004.png");
        public static readonly Uri BSP_26005_Source = GetBarProfilSection("RE_26005.png");
        public static readonly Uri BSP_26217_Source = GetBarProfilSection("RE_26217.png");
        public static readonly Uri BSP_26218_Source = GetBarProfilSection("RE_26218.png");
        public static readonly Uri BSP_26219_Source = GetBarProfilSection("RE_26219.png");
        public static readonly Uri BSP_26221_Source = GetBarProfilSection("RE_26221.png");
        public static readonly Uri BSP_26222_Source = GetBarProfilSection("RE_26222.png");
        public static readonly Uri BSP_26223_Source = GetBarProfilSection("RE_26223.png");
        public static readonly Uri BSP_26224_Source = GetBarProfilSection("RE_26224.png");
        public static readonly Uri BSP_26225_Source = GetBarProfilSection("RE_26225.png");
        public static readonly Uri BSP_26226_Source = GetBarProfilSection("RE_26226.png");
        public static readonly Uri BSP_26227_Source = GetBarProfilSection("RE_26227.png");
        public static readonly Uri BSP_26318_Source = GetBarProfilSection("RE_26318.png");
        public static readonly Uri BSP_26500_Source = GetBarProfilSection("RE_26500.png");
        public static readonly Uri BSP_26501_Source = GetBarProfilSection("RE_26501.png");
        public static readonly Uri BSP_26502_Source = GetBarProfilSection("RE_26502.png");
        public static readonly Uri BSP_26504_Source = GetBarProfilSection("RE_26504.png");
        public static readonly Uri BSP_26506_Source = GetBarProfilSection("RE_26506.png");
        public static readonly Uri BSP_26511_Source = GetBarProfilSection("RE_26511.png");
        public static readonly Uri BSP_26512_Source = GetBarProfilSection("RE_26512.png");
        public static readonly Uri BSP_26701_Source = GetBarProfilSection("RE_26701.png");
        public static readonly Uri BSP_26702_Source = GetBarProfilSection("RE_26702.png");
        public static readonly Uri BSP_26703_Source = GetBarProfilSection("RE_26703.png");
        public static readonly Uri BSP_26704_Source = GetBarProfilSection("RE_26704.png");
        public static readonly Uri BSP_26705_Source = GetBarProfilSection("RE_26705.png");
        public static readonly Uri BSP_26720_Source = GetBarProfilSection("RE_26720.png");
        public static readonly Uri BSP_26721_Source = GetBarProfilSection("RE_26721.png");
        public static readonly Uri BSP_26722_Source = GetBarProfilSection("RE_26722.png");
        public static readonly Uri BSP_26723_Source = GetBarProfilSection("RE_26723.png");
        public static readonly Uri BSP_26724_Source = GetBarProfilSection("RE_26724.png");
        public static readonly Uri BSP_26725_Source = GetBarProfilSection("RE_26725.png");
        public static readonly Uri BSP_26726_Source = GetBarProfilSection("RE_26726.png");
        public static readonly Uri BSP_26750_Source = GetBarProfilSection("RE_26750.png");
        public static readonly Uri BSP_268400_Source = GetBarProfilSection("RE_268400.png");
        public static readonly Uri BSP_277400_Source = GetBarProfilSection("RE_277400.png");
        public static readonly Uri BSP_30092_Source = GetBarProfilSection("RE_30092.png");
        public static readonly Uri BSP_30093_Source = GetBarProfilSection("RE_30093.png");
        public static readonly Uri BSP_30094_Source = GetBarProfilSection("RE_30094.png");
        public static readonly Uri BSP_303700_Source = GetBarProfilSection("RE_303700.png");
        public static readonly Uri BSP_30604_Source = GetBarProfilSection("RE_30604.png");
        public static readonly Uri BSP_30606_Source = GetBarProfilSection("RE_30606.png");
        public static readonly Uri BSP_30608_Source = GetBarProfilSection("RE_30608.png");
        public static readonly Uri BSP_30610_Source = GetBarProfilSection("RE_30610.png");
        public static readonly Uri BSP_30612_Source = GetBarProfilSection("RE_30612.png");
        public static readonly Uri BSP_30704_Source = GetBarProfilSection("RE_30704.png");
        public static readonly Uri BSP_30706_Source = GetBarProfilSection("RE_30706.png");
        public static readonly Uri BSP_30710_Source = GetBarProfilSection("RE_30710.png");
        public static readonly Uri BSP_30723_Source = GetBarProfilSection("RE_30723.png");
        public static readonly Uri BSP_30724_Source = GetBarProfilSection("RE_30724.png");
        public static readonly Uri BSP_30725_Source = GetBarProfilSection("RE_30725.png");
        public static readonly Uri BSP_30727_Source = GetBarProfilSection("RE_30727.png");
        public static readonly Uri BSP_30728_Source = GetBarProfilSection("RE_30728.png");
        public static readonly Uri BSP_30729_Source = GetBarProfilSection("RE_30729.png");
        public static readonly Uri BSP_30801_Source = GetBarProfilSection("RE_30801.png");
        public static readonly Uri BSP_30802_Source = GetBarProfilSection("RE_30802.png");
        public static readonly Uri BSP_30806_Source = GetBarProfilSection("RE_30806.png");
        public static readonly Uri BSP_30810_Source = GetBarProfilSection("RE_30810.png");
        public static readonly Uri BSP_30811_Source = GetBarProfilSection("RE_30811.png");
        public static readonly Uri BSP_30812_Source = GetBarProfilSection("RE_30812.png");
        public static readonly Uri BSP_30813_Source = GetBarProfilSection("RE_30813.png");
        public static readonly Uri BSP_30814_Source = GetBarProfilSection("RE_30814.png");
        public static readonly Uri BSP_30818_Source = GetBarProfilSection("RE_30818.png");
        public static readonly Uri BSP_32078_Source = GetBarProfilSection("RE_32078.png");
        public static readonly Uri BSP_328100_Source = GetBarProfilSection("RE_328100.png");
        public static readonly Uri BSP_3297_Source = GetBarProfilSection("RE_3297.png");
        public static readonly Uri BSP_33024_Source = GetBarProfilSection("RE_33024.png");
        public static readonly Uri BSP_3312_Source = GetBarProfilSection("RE_3312.png");
        public static readonly Uri BSP_33124_Source = GetBarProfilSection("RE_33124.png");
        public static readonly Uri BSP_3331_Source = GetBarProfilSection("RE_3331.png");
        public static readonly Uri BSP_3333_Source = GetBarProfilSection("RE_3333.png");
        public static readonly Uri BSP_3334_Source = GetBarProfilSection("RE_3334.png");
        public static readonly Uri BSP_3335_Source = GetBarProfilSection("RE_3335.png");
        public static readonly Uri BSP_3340_Source = GetBarProfilSection("RE_3340.png");
        public static readonly Uri BSP_3341_Source = GetBarProfilSection("RE_3341.png");
        public static readonly Uri BSP_3364_Source = GetBarProfilSection("RE_3364.png");
        public static readonly Uri BSP_3700_Source = GetBarProfilSection("RE_3700.png");
        public static readonly Uri BSP_3701_Source = GetBarProfilSection("RE_3701.png");
        public static readonly Uri BSP_371800_Source = GetBarProfilSection("RE_371800.png");
        public static readonly Uri BSP_4294_Source = GetBarProfilSection("RE_4294.png");
        public static readonly Uri BSP_4300_Source = GetBarProfilSection("RE_4300.png");
        public static readonly Uri BSP_4301_Source = GetBarProfilSection("RE_4301.png");
        public static readonly Uri BSP_4302_Source = GetBarProfilSection("RE_4302.png");
        public static readonly Uri BSP_4303_Source = GetBarProfilSection("RE_4303.png");
        public static readonly Uri BSP_4304_Source = GetBarProfilSection("RE_4304.png");
        public static readonly Uri BSP_4305_Source = GetBarProfilSection("RE_4305.png");
        public static readonly Uri BSP_4306_Source = GetBarProfilSection("RE_4306.png");
        public static readonly Uri BSP_4307_Source = GetBarProfilSection("RE_4307.png");
        public static readonly Uri BSP_4308_Source = GetBarProfilSection("RE_4308.png");
        public static readonly Uri BSP_4390_Source = GetBarProfilSection("RE_4390.png");
        public static readonly Uri BSP_477200_Source = GetBarProfilSection("RE_477200.png");
        public static readonly Uri BSP_509900_Source = GetBarProfilSection("RE_509900.png");
        public static readonly Uri BSP_5103_Source = GetBarProfilSection("RE_5103.png");
        public static readonly Uri BSP_5103JNA_Source = GetBarProfilSection("RE_5103JNA.png");
        public static readonly Uri BSP_5103SJ_Source = GetBarProfilSection("RE_5103SJ.png");
        public static readonly Uri BSP_5107_Source = GetBarProfilSection("RE_5107.png");
        public static readonly Uri BSP_5114_Source = GetBarProfilSection("RE_5114.png");
        public static readonly Uri BSP_5114JNA_Source = GetBarProfilSection("RE_5114JNA.png");
        public static readonly Uri BSP_5115_Source = GetBarProfilSection("RE_5115.png");
        public static readonly Uri BSP_5116_Source = GetBarProfilSection("RE_5116.png");
        public static readonly Uri BSP_5120_Source = GetBarProfilSection("RE_5120.png");
        public static readonly Uri BSP_5120JNA_Source = GetBarProfilSection("RE_5120JNA.png");
        public static readonly Uri BSP_5120SJ_Source = GetBarProfilSection("RE_5120SJ.png");
        public static readonly Uri BSP_5120_SJ_Source = GetBarProfilSection("RE_5120_SJ.png");
        public static readonly Uri BSP_5125_Source = GetBarProfilSection("RE_5125.png");
        public static readonly Uri BSP_5125JNA_Source = GetBarProfilSection("RE_5125JNA.png");
        public static readonly Uri BSP_5125SJ_Source = GetBarProfilSection("RE_5125SJ.png");
        public static readonly Uri BSP_5126_Source = GetBarProfilSection("RE_5126.png");
        public static readonly Uri BSP_51300_Source = GetBarProfilSection("RE_51300.png");
        public static readonly Uri BSP_5137_Source = GetBarProfilSection("RE_5137.png");
        public static readonly Uri BSP_5137JNA_Source = GetBarProfilSection("RE_5137JNA.png");
        public static readonly Uri BSP_51400_Source = GetBarProfilSection("RE_51400.png");
        public static readonly Uri BSP_51500_Source = GetBarProfilSection("RE_51500.png");
        public static readonly Uri BSP_5163_Source = GetBarProfilSection("RE_5163.png");
        public static readonly Uri BSP_5177_Source = GetBarProfilSection("RE_5177.png");
        public static readonly Uri BSP_5177JNA_Source = GetBarProfilSection("RE_5177JNA.png");
        public static readonly Uri BSP_5179_Source = GetBarProfilSection("RE_5179.png");
        public static readonly Uri BSP_5180_Source = GetBarProfilSection("RE_5180.png");
        public static readonly Uri BSP_5181_Source = GetBarProfilSection("RE_5181.png");
        public static readonly Uri BSP_5183_Source = GetBarProfilSection("RE_5183.png");
        public static readonly Uri BSP_5185_Source = GetBarProfilSection("RE_5185.png");
        public static readonly Uri BSP_5189_Source = GetBarProfilSection("RE_5189.png");
        public static readonly Uri BSP_5203_Source = GetBarProfilSection("RE_5203.png");
        public static readonly Uri BSP_5208_Source = GetBarProfilSection("RE_5208.png");
        public static readonly Uri BSP_52110_Source = GetBarProfilSection("RE_52110.png");
        public static readonly Uri BSP_5215_Source = GetBarProfilSection("RE_5215.png");
        public static readonly Uri BSP_52220_Source = GetBarProfilSection("RE_52220.png");
        public static readonly Uri BSP_5227_Source = GetBarProfilSection("RE_5227.png");
        public static readonly Uri BSP_52320_Source = GetBarProfilSection("RE_52320.png");
        public static readonly Uri BSP_5263_Source = GetBarProfilSection("RE_5263.png");
        public static readonly Uri BSP_5300_Source = GetBarProfilSection("RE_5300.png");
        public static readonly Uri BSP_5301_Source = GetBarProfilSection("RE_5301.png");
        public static readonly Uri BSP_5302_Source = GetBarProfilSection("RE_5302.png");
        public static readonly Uri BSP_5303_Source = GetBarProfilSection("RE_5303.png");
        public static readonly Uri BSP_5304_Source = GetBarProfilSection("RE_5304.png");
        public static readonly Uri BSP_5307_Source = GetBarProfilSection("RE_5307.png");
        public static readonly Uri BSP_5334_Source = GetBarProfilSection("RE_5334.png");
        public static readonly Uri BSP_5386_Source = GetBarProfilSection("RE_5386.png");
        public static readonly Uri BSP_5387_Source = GetBarProfilSection("RE_5387.png");
        public static readonly Uri BSP_5388_Source = GetBarProfilSection("RE_5388.png");
        public static readonly Uri BSP_5389_Source = GetBarProfilSection("RE_5389.png");
        public static readonly Uri BSP_5390_Source = GetBarProfilSection("RE_5390.png");
        public static readonly Uri BSP_5400_Source = GetBarProfilSection("RE_5400.png");
        public static readonly Uri BSP_5401_Source = GetBarProfilSection("RE_5401.png");
        public static readonly Uri BSP_5402_Source = GetBarProfilSection("RE_5402.png");
        public static readonly Uri BSP_5403_Source = GetBarProfilSection("RE_5403.png");
        public static readonly Uri BSP_5404_Source = GetBarProfilSection("RE_5404.png");
        public static readonly Uri BSP_5405_Source = GetBarProfilSection("RE_5405.png");
        public static readonly Uri BSP_5407_Source = GetBarProfilSection("RE_5407.png");
        public static readonly Uri BSP_5408_Source = GetBarProfilSection("RE_5408.png");
        public static readonly Uri BSP_5409_Source = GetBarProfilSection("RE_5409.png");
        public static readonly Uri BSP_5410_Source = GetBarProfilSection("RE_5410.png");
        public static readonly Uri BSP_5412_Source = GetBarProfilSection("RE_5412.png");
        public static readonly Uri BSP_5415_Source = GetBarProfilSection("RE_5415.png");
        public static readonly Uri BSP_5416_Source = GetBarProfilSection("RE_5416.png");
        public static readonly Uri BSP_5745_Source = GetBarProfilSection("RE_5745.png");
        public static readonly Uri BSP_60140_Source = GetBarProfilSection("RE_60140.png");
        public static readonly Uri BSP_60210_Source = GetBarProfilSection("RE_60210.png");
        public static readonly Uri BSP_60250_Source = GetBarProfilSection("RE_60250.png");
        public static readonly Uri BSP_60310_Source = GetBarProfilSection("RE_60310.png");
        public static readonly Uri BSP_604006_Source = GetBarProfilSection("RE_604006.png");
        public static readonly Uri BSP_60410_Source = GetBarProfilSection("RE_60410.png");
        public static readonly Uri BSP_60510_Source = GetBarProfilSection("RE_60510.png");
        public static readonly Uri BSP_61015_Source = GetBarProfilSection("RE_61015.png");
        public static readonly Uri BSP_63120_Source = GetBarProfilSection("RE_63120.png");
        public static readonly Uri BSP_63170_Source = GetBarProfilSection("RE_63170.png");
        public static readonly Uri BSP_63220_Source = GetBarProfilSection("RE_63220.png");
        public static readonly Uri BSP_63370_Source = GetBarProfilSection("RE_63370.png");
        public static readonly Uri BSP_63510_Source = GetBarProfilSection("RE_63510.png");
        public static readonly Uri BSP_63610_Source = GetBarProfilSection("RE_63610.png");
        public static readonly Uri BSP_6370_Source = GetBarProfilSection("RE_6370.png");
        public static readonly Uri BSP_63700_Source = GetBarProfilSection("RE_63700.png");
        public static readonly Uri BSP_6372_Source = GetBarProfilSection("RE_6372.png");
        public static readonly Uri BSP_6373_Source = GetBarProfilSection("RE_6373.png");
        public static readonly Uri BSP_6374_Source = GetBarProfilSection("RE_6374.png");
        public static readonly Uri BSP_63850_Source = GetBarProfilSection("RE_63850.png");
        public static readonly Uri BSP_6980_Source = GetBarProfilSection("RE_6980.png");
        public static readonly Uri BSP_6981_Source = GetBarProfilSection("RE_6981.png");
        public static readonly Uri BSP_6982_Source = GetBarProfilSection("RE_6982.png");
        public static readonly Uri BSP_6983_Source = GetBarProfilSection("RE_6983.png");
        public static readonly Uri BSP_70200_Source = GetBarProfilSection("RE_70200.png");
        public static readonly Uri BSP_70400_Source = GetBarProfilSection("RE_70400.png");
        public static readonly Uri BSP_70502_Source = GetBarProfilSection("RE_70502.png");
        public static readonly Uri BSP_70503_Source = GetBarProfilSection("RE_70503.png");
        public static readonly Uri BSP_7114_Source = GetBarProfilSection("RE_7114.png");
        public static readonly Uri BSP_71200_Source = GetBarProfilSection("RE_71200.png");
        public static readonly Uri BSP_7125_Source = GetBarProfilSection("RE_7125.png");
        public static readonly Uri BSP_71300_Source = GetBarProfilSection("RE_71300.png");
        public static readonly Uri BSP_71301_Source = GetBarProfilSection("RE_71301.png");
        public static readonly Uri BSP_7137_Source = GetBarProfilSection("RE_7137.png");
        public static readonly Uri BSP_726124_Source = GetBarProfilSection("RE_726124.png");
        public static readonly Uri BSP_73020_Source = GetBarProfilSection("RE_73020.png");
        public static readonly Uri BSP_73030_Source = GetBarProfilSection("RE_73030.png");
        public static readonly Uri BSP_7308_Source = GetBarProfilSection("RE_7308.png");
        public static readonly Uri BSP_7308JNA_Source = GetBarProfilSection("RE_7308JNA.png");
        public static readonly Uri BSP_7309_Source = GetBarProfilSection("RE_7309.png");
        public static readonly Uri BSP_7309JNA_Source = GetBarProfilSection("RE_7309JNA.png");
        public static readonly Uri BSP_73200_Source = GetBarProfilSection("RE_73200.png");
        public static readonly Uri BSP_7324_Source = GetBarProfilSection("RE_7324.png");
        public static readonly Uri BSP_7324JNA_Source = GetBarProfilSection("RE_7324JNA.png");
        public static readonly Uri BSP_7502_Source = GetBarProfilSection("RE_7502.png");
        public static readonly Uri BSP_75030_Source = GetBarProfilSection("RE_75030.png");
        public static readonly Uri BSP_7508_Source = GetBarProfilSection("RE_7508.png");
        public static readonly Uri BSP_7524_Source = GetBarProfilSection("RE_7524.png");
        public static readonly Uri BSP_7531_Source = GetBarProfilSection("RE_7531.png");
        public static readonly Uri BSP_76027_Source = GetBarProfilSection("RE_76027.png");
        public static readonly Uri BSP_76029_Source = GetBarProfilSection("RE_76029.png");
        public static readonly Uri BSP_76070H_Source = GetBarProfilSection("RE_76070H.png");
        public static readonly Uri BSP_76070L_Source = GetBarProfilSection("RE_76070L.png");
        public static readonly Uri BSP_76071_Source = GetBarProfilSection("RE_76071.png");
        public static readonly Uri BSP_76074B_Source = GetBarProfilSection("RE_76074B.png");
        public static readonly Uri BSP_76074H_Source = GetBarProfilSection("RE_76074H.png");
        public static readonly Uri BSP_76075B_Source = GetBarProfilSection("RE_76075B.png");
        public static readonly Uri BSP_76075LH_Source = GetBarProfilSection("RE_76075LH.png");
        public static readonly Uri BSP_76076_Source = GetBarProfilSection("RE_76076.png");
        public static readonly Uri BSP_76077_Source = GetBarProfilSection("RE_76077.png");
        public static readonly Uri BSP_76078B_Source = GetBarProfilSection("RE_76078B.png");
        public static readonly Uri BSP_76078LH_Source = GetBarProfilSection("RE_76078LH.png");
        public static readonly Uri BSP_76079B_Source = GetBarProfilSection("RE_76079B.png");
        public static readonly Uri BSP_76079H_Source = GetBarProfilSection("RE_76079H.png");
        public static readonly Uri BSP_76100H_Source = GetBarProfilSection("RE_76100H.png");
        public static readonly Uri BSP_76100L_Source = GetBarProfilSection("RE_76100L.png");
        public static readonly Uri BSP_76120b_Source = GetBarProfilSection("RE_76120b.png");
        public static readonly Uri BSP_76120H_Source = GetBarProfilSection("RE_76120H.png");
        public static readonly Uri BSP_76120HVR_Source = GetBarProfilSection("RE_76120HVR.png");
        public static readonly Uri BSP_76120L_Source = GetBarProfilSection("RE_76120L.png");
        public static readonly Uri BSP_76130B_Source = GetBarProfilSection("RE_76130B.png");
        public static readonly Uri BSP_76130LH_Source = GetBarProfilSection("RE_76130LH.png");
        public static readonly Uri BSP_76140H_Source = GetBarProfilSection("RE_76140H.png");
        public static readonly Uri BSP_76140HVR_Source = GetBarProfilSection("RE_76140HVR.png");
        public static readonly Uri BSP_76140L_Source = GetBarProfilSection("RE_76140L.png");
        public static readonly Uri BSP_76160B_Source = GetBarProfilSection("RE_76160B.png");
        public static readonly Uri BSP_76160H_Source = GetBarProfilSection("RE_76160H.png");
        public static readonly Uri BSP_76160L_Source = GetBarProfilSection("RE_76160L.png");
        public static readonly Uri BSP_76303_Source = GetBarProfilSection("RE_76303.png");
        public static readonly Uri BSP_76304_Source = GetBarProfilSection("RE_76304.png");
        public static readonly Uri BSP_76308_Source = GetBarProfilSection("RE_76308.png");
        public static readonly Uri BSP_76309_Source = GetBarProfilSection("RE_76309.png");
        public static readonly Uri BSP_76310_Source = GetBarProfilSection("RE_76310.png");
        public static readonly Uri BSP_76313_Source = GetBarProfilSection("RE_76313.png");
        public static readonly Uri BSP_76314_Source = GetBarProfilSection("RE_76314.png");
        public static readonly Uri BSP_76315_Source = GetBarProfilSection("RE_76315.png");
        public static readonly Uri BSP_76316_Source = GetBarProfilSection("RE_76316.png");
        public static readonly Uri BSP_76318_Source = GetBarProfilSection("RE_76318.png");
        public static readonly Uri BSP_76321_Source = GetBarProfilSection("RE_76321.png");
        public static readonly Uri BSP_76322_Source = GetBarProfilSection("RE_76322.png");
        public static readonly Uri BSP_76323_Source = GetBarProfilSection("RE_76323.png");
        public static readonly Uri BSP_76324_Source = GetBarProfilSection("RE_76324.png");
        public static readonly Uri BSP_773201_Source = GetBarProfilSection("RE_773201.png");
        public static readonly Uri BSP_774005_Source = GetBarProfilSection("RE_774005.png");
        public static readonly Uri BSP_776128_Source = GetBarProfilSection("RE_776128.png");
        public static readonly Uri BSP_776132_Source = GetBarProfilSection("RE_776132.png");
        public static readonly Uri BSP_776312N_Source = GetBarProfilSection("RE_776312N.png");
        public static readonly Uri BSP_78030_Source = GetBarProfilSection("RE_78030.png");
        public static readonly Uri BSP_815050_Source = GetBarProfilSection("RE_815050.png");
        public static readonly Uri BSP_836_Source = GetBarProfilSection("RE_836.png");
        public static readonly Uri BSP_84CA002_Source = GetBarProfilSection("RE_84CA002.png");
        public static readonly Uri BSP_84CP010_Source = GetBarProfilSection("RE_84CP010.png");
        public static readonly Uri BSP_84CP011_Source = GetBarProfilSection("RE_84CP011.png");
        public static readonly Uri BSP_84CP104_Source = GetBarProfilSection("RE_84CP104.png");
        public static readonly Uri BSP_84CP105_Source = GetBarProfilSection("RE_84CP105.png");
        public static readonly Uri BSP_84CP106_Source = GetBarProfilSection("RE_84CP106.png");
        public static readonly Uri BSP_84CP107_Source = GetBarProfilSection("RE_84CP107.png");
        public static readonly Uri BSP_84CP108_Source = GetBarProfilSection("RE_84CP108.png");
        public static readonly Uri BSP_84CP109_Source = GetBarProfilSection("RE_84CP109.png");
        public static readonly Uri BSP_84CP201_Source = GetBarProfilSection("RE_84CP201.png");
        public static readonly Uri BSP_84CP202_Source = GetBarProfilSection("RE_84CP202.png");
        public static readonly Uri BSP_84CP203_Source = GetBarProfilSection("RE_84CP203.png");
        public static readonly Uri BSP_84CP204_Source = GetBarProfilSection("RE_84CP204.png");
        public static readonly Uri BSP_84VA001_Source = GetBarProfilSection("RE_84VA001.png");
        public static readonly Uri BSP_84VA016_Source = GetBarProfilSection("RE_84VA016.png");
        public static readonly Uri BSP_84VA018_Source = GetBarProfilSection("RE_84VA018.png");
        public static readonly Uri BSP_84VA022_Source = GetBarProfilSection("RE_84VA022.png");
        public static readonly Uri BSP_84VA025_Source = GetBarProfilSection("RE_84VA025.png");
        public static readonly Uri BSP_84VA026_Source = GetBarProfilSection("RE_84VA026.png");
        public static readonly Uri BSP_84VA029_Source = GetBarProfilSection("RE_84VA029.png");
        public static readonly Uri BSP_84VB101_Source = GetBarProfilSection("RE_84VB101.png");
        public static readonly Uri BSP_84VB201_Source = GetBarProfilSection("RE_84VB201.png");
        public static readonly Uri BSP_84VI206_Source = GetBarProfilSection("RE_84VI206.png");
        public static readonly Uri BSP_84VP010_Source = GetBarProfilSection("RE_84VP010.png");
        public static readonly Uri BSP_84VP012_Source = GetBarProfilSection("RE_84VP012.png");
        public static readonly Uri BSP_84VP023_Source = GetBarProfilSection("RE_84VP023.png");
        public static readonly Uri BSP_84VP033_Source = GetBarProfilSection("RE_84VP033.png");
        public static readonly Uri BSP_84VP041_Source = GetBarProfilSection("RE_84VP041.png");
        public static readonly Uri BSP_84VP042_Source = GetBarProfilSection("RE_84VP042.png");
        public static readonly Uri BSP_84VP043_Source = GetBarProfilSection("RE_84VP043.png");
        public static readonly Uri BSP_84VP101_Source = GetBarProfilSection("RE_84VP101.png");
        public static readonly Uri BSP_84VP102_Source = GetBarProfilSection("RE_84VP102.png");
        public static readonly Uri BSP_84VP103_Source = GetBarProfilSection("RE_84VP103.png");
        public static readonly Uri BSP_84VP104_Source = GetBarProfilSection("RE_84VP104.png");
        public static readonly Uri BSP_84VP105_Source = GetBarProfilSection("RE_84VP105.png");
        public static readonly Uri BSP_84VP106_Source = GetBarProfilSection("RE_84VP106.png");
        public static readonly Uri BSP_84VP107_Source = GetBarProfilSection("RE_84VP107.png");
        public static readonly Uri BSP_84VP108_Source = GetBarProfilSection("RE_84VP108.png");
        public static readonly Uri BSP_84VP109_Source = GetBarProfilSection("RE_84VP109.png");
        public static readonly Uri BSP_84VP201_Source = GetBarProfilSection("RE_84VP201.png");
        public static readonly Uri BSP_84VP202_Source = GetBarProfilSection("RE_84VP202.png");
        public static readonly Uri BSP_84VP203_Source = GetBarProfilSection("RE_84VP203.png");
        public static readonly Uri BSP_84VP204_Source = GetBarProfilSection("RE_84VP204.png");
        public static readonly Uri BSP_84VP310_Source = GetBarProfilSection("RE_84VP310.png");
        public static readonly Uri BSP_84VP311_Source = GetBarProfilSection("RE_84VP311.png");
        public static readonly Uri BSP_84VP312_Source = GetBarProfilSection("RE_84VP312.png");
        public static readonly Uri BSP_84VP313_Source = GetBarProfilSection("RE_84VP313.png");
        public static readonly Uri BSP_84VP314_Source = GetBarProfilSection("RE_84VP314.png");
        public static readonly Uri BSP_85303_Source = GetBarProfilSection("RE_85303.png");
        public static readonly Uri BSP_85304_Source = GetBarProfilSection("RE_85304.png");
        public static readonly Uri BSP_85305_Source = GetBarProfilSection("RE_85305.png");
        public static readonly Uri BSP_86300_Source = GetBarProfilSection("RE_86300.png");
        public static readonly Uri BSP_956059_Source = GetBarProfilSection("RE_956059.png");
        public static readonly Uri BSP_983400_Source = GetBarProfilSection("RE_983400.png");
        public static readonly Uri BSP_986200_Source = GetBarProfilSection("RE_986200.png");
        public static readonly Uri BSP_A_BNF84_Source = GetBarProfilSection("RE_A-BNF84.png");
        public static readonly Uri BSP_A_CV135_Source = GetBarProfilSection("RE_A-CV135.png");
        public static readonly Uri BSP_A_CV146_Source = GetBarProfilSection("RE_A-CV146.png");
        public static readonly Uri BSP_A_CV155_Source = GetBarProfilSection("RE_A-CV155.png");
        public static readonly Uri BSP_A_CV166_Source = GetBarProfilSection("RE_A-CV166.png");
        public static readonly Uri BSP_A_CV235_Source = GetBarProfilSection("RE_A-CV235.png");
        public static readonly Uri BSP_A_CV246_Source = GetBarProfilSection("RE_A-CV246.png");
        public static readonly Uri BSP_A_CV255_Source = GetBarProfilSection("RE_A-CV255.png");
        public static readonly Uri BSP_A_CV266_Source = GetBarProfilSection("RE_A-CV266.png");
        public static readonly Uri BSP_A_L56_Source = GetBarProfilSection("RE_A-L56.png");
        public static readonly Uri BSP_A_L61_Source = GetBarProfilSection("RE_A-L61.png");
        public static readonly Uri BSP_A_L72_115_Source = GetBarProfilSection("RE_A-L72-115.png");
        public static readonly Uri BSP_A_L72_135_Source = GetBarProfilSection("RE_A-L72-135.png");
        public static readonly Uri BSP_A_L72_155_Source = GetBarProfilSection("RE_A-L72-155.png");
        public static readonly Uri BSP_A_L72_95_Source = GetBarProfilSection("RE_A-L72-95.png");
        public static readonly Uri BSP_A_L75_Source = GetBarProfilSection("RE_A-L75.png");
        public static readonly Uri BSP_A_L80_Source = GetBarProfilSection("RE_A-L80.png");
        public static readonly Uri BSP_A_L91_115_Source = GetBarProfilSection("RE_A-L91-115.png");
        public static readonly Uri BSP_A_l91_135_Source = GetBarProfilSection("RE_A-l91-135.png");
        public static readonly Uri BSP_A_l91_155_Source = GetBarProfilSection("RE_A-l91-155.png");
        public static readonly Uri BSP_A_L91_95_Source = GetBarProfilSection("RE_A-L91-95.png");
        public static readonly Uri BSP_A_LZ101_Source = GetBarProfilSection("RE_A-LZ101.png");
        public static readonly Uri BSP_A_LZ120_Source = GetBarProfilSection("RE_A-LZ120.png");
        public static readonly Uri BSP_A_MB68_Source = GetBarProfilSection("RE_A-MB68.png");
        public static readonly Uri BSP_A_MB89_Source = GetBarProfilSection("RE_A-MB89.png");
        public static readonly Uri BSP_A_MD116_Source = GetBarProfilSection("RE_A-MD116.png");
        public static readonly Uri BSP_A_MD127_Source = GetBarProfilSection("RE_A-MD127.png");
        public static readonly Uri BSP_A_MD154_Source = GetBarProfilSection("RE_A-MD154.png");
        public static readonly Uri BSP_A_MD89_Source = GetBarProfilSection("RE_A-MD89.png");
        public static readonly Uri BSP_A_ME84_Source = GetBarProfilSection("RE_A-ME84.png");
        public static readonly Uri BSP_A_MI50_Source = GetBarProfilSection("RE_A-MI50.png");
        public static readonly Uri BSP_A_P28_Source = GetBarProfilSection("RE_A-P28.png");
        public static readonly Uri BSP_A_P32_Source = GetBarProfilSection("RE_A-P32.png");
        public static readonly Uri BSP_A_P44_Source = GetBarProfilSection("RE_A-P44.png");
        public static readonly Uri BSP_A_P46_Source = GetBarProfilSection("RE_A-P46.png");
        public static readonly Uri BSP_A_PF28_Source = GetBarProfilSection("RE_A-PF28.png");
        public static readonly Uri BSP_A_PF32_Source = GetBarProfilSection("RE_A-PF32.png");
        public static readonly Uri BSP_A_PF40_Source = GetBarProfilSection("RE_A-PF40.png");
        public static readonly Uri BSP_A_PL81_Source = GetBarProfilSection("RE_A-PL81.png");
        public static readonly Uri BSP_A_T35_Source = GetBarProfilSection("RE_A-T35.png");
        public static readonly Uri BSP_A_T46_Source = GetBarProfilSection("RE_A-T46.png");
        public static readonly Uri BSP_A_T55_Source = GetBarProfilSection("RE_A-T55.png");
        public static readonly Uri BSP_A_T66_Source = GetBarProfilSection("RE_A-T66.png");
        public static readonly Uri BSP_A_T86_Source = GetBarProfilSection("RE_A-T86.png");
        public static readonly Uri BSP_A_TCV40_Source = GetBarProfilSection("RE_A-TCV40.png");
        public static readonly Uri BSP_A_Z60_26_Source = GetBarProfilSection("RE_A-Z60-26.png");
        public static readonly Uri BSP_A_Z60_30_Source = GetBarProfilSection("RE_A-Z60-30.png");
        public static readonly Uri BSP_A_Z70_Source = GetBarProfilSection("RE_A-Z70.png");
        public static readonly Uri BSP_A_Z71_Source = GetBarProfilSection("RE_A-Z71.png");
        public static readonly Uri BSP_A_ZM40_26_Source = GetBarProfilSection("RE_A-ZM40-26.png");
        public static readonly Uri BSP_A_ZM40_30_Source = GetBarProfilSection("RE_A-ZM40-30.png");
        public static readonly Uri BSP_A_ZM40_32_Source = GetBarProfilSection("RE_A-ZM40-32.png");
        public static readonly Uri BSP_A_ZM78_Source = GetBarProfilSection("RE_A-ZM78.png");
        public static readonly Uri BSP_A85A012_Source = GetBarProfilSection("RE_A85A012.png");
        public static readonly Uri BSP_A85B201_Source = GetBarProfilSection("RE_A85B201.png");
        public static readonly Uri BSP_A85J101_Source = GetBarProfilSection("RE_A85J101.png");
        public static readonly Uri BSP_A85J304_Source = GetBarProfilSection("RE_A85J304.png");
        public static readonly Uri BSP_A85J306_Source = GetBarProfilSection("RE_A85J306.png");
        public static readonly Uri BSP_A85P101_Source = GetBarProfilSection("RE_A85P101.png");
        public static readonly Uri BSP_A85P102_Source = GetBarProfilSection("RE_A85P102.png");
        public static readonly Uri BSP_A85P103_Source = GetBarProfilSection("RE_A85P103.png");
        public static readonly Uri BSP_A85P104_Source = GetBarProfilSection("RE_A85P104.png");
        public static readonly Uri BSP_A85P105_Source = GetBarProfilSection("RE_A85P105.png");
        public static readonly Uri BSP_A85P201_Source = GetBarProfilSection("RE_A85P201.png");
        public static readonly Uri BSP_A85P202_Source = GetBarProfilSection("RE_A85P202.png");
        public static readonly Uri BSP_A85P203_Source = GetBarProfilSection("RE_A85P203.png");
        public static readonly Uri BSP_A85P301_Source = GetBarProfilSection("RE_A85P301.png");
        public static readonly Uri BSP_A85P302_Source = GetBarProfilSection("RE_A85P302.png");
        public static readonly Uri BSP_A85P303_Source = GetBarProfilSection("RE_A85P303.png");
        public static readonly Uri BSP_A85P304_Source = GetBarProfilSection("RE_A85P304.png");
        public static readonly Uri BSP_A85P310_Source = GetBarProfilSection("RE_A85P310.png");
        public static readonly Uri BSP_A85P311_Source = GetBarProfilSection("RE_A85P311.png");
        public static readonly Uri BSP_A85P312_Source = GetBarProfilSection("RE_A85P312.png");
        public static readonly Uri BSP_A85P313_Source = GetBarProfilSection("RE_A85P313.png");
        public static readonly Uri BSP_A85P314_Source = GetBarProfilSection("RE_A85P314.png");
        public static readonly Uri BSP_A85P315_Source = GetBarProfilSection("RE_A85P315.png");
        public static readonly Uri BSP_A85P316_Source = GetBarProfilSection("RE_A85P316.png");
        public static readonly Uri BSP_A85P317_Source = GetBarProfilSection("RE_A85P317.png");
        public static readonly Uri BSP_A85P318_Source = GetBarProfilSection("RE_A85P318.png");
        public static readonly Uri BSP_A85P321_Source = GetBarProfilSection("RE_A85P321.png");
        public static readonly Uri BSP_ACT35_Source = GetBarProfilSection("RE_ACT35.png");
        public static readonly Uri BSP_ACT46_Source = GetBarProfilSection("RE_ACT46.png");
        public static readonly Uri BSP_ACT55_Source = GetBarProfilSection("RE_ACT55.png");
        public static readonly Uri BSP_ACT66_Source = GetBarProfilSection("RE_ACT66.png");
        public static readonly Uri BSP_AD10111_Source = GetBarProfilSection("RE_AD10111.png");
        public static readonly Uri BSP_AD10112_Source = GetBarProfilSection("RE_AD10112.png");
        public static readonly Uri BSP_AD10113_Source = GetBarProfilSection("RE_AD10113.png");
        public static readonly Uri BSP_AD10114_Source = GetBarProfilSection("RE_AD10114.png");
        public static readonly Uri BSP_AD10115_Source = GetBarProfilSection("RE_AD10115.png");
        public static readonly Uri BSP_AD10116_Source = GetBarProfilSection("RE_AD10116.png");
        public static readonly Uri BSP_AD10117_Source = GetBarProfilSection("RE_AD10117.png");
        public static readonly Uri BSP_AD10224_Source = GetBarProfilSection("RE_AD10224.png");
        public static readonly Uri BSP_AD10225_Source = GetBarProfilSection("RE_AD10225.png");
        public static readonly Uri BSP_AD10226_Source = GetBarProfilSection("RE_AD10226.png");
        public static readonly Uri BSP_AD10227_Source = GetBarProfilSection("RE_AD10227.png");
        public static readonly Uri BSP_AD10228_Source = GetBarProfilSection("RE_AD10228.png");
        public static readonly Uri BSP_AD10356_Source = GetBarProfilSection("RE_AD10356.png");
        public static readonly Uri BSP_AD10366_Source = GetBarProfilSection("RE_AD10366.png");
        public static readonly Uri BSP_AD10369_Source = GetBarProfilSection("RE_AD10369.png");
        public static readonly Uri BSP_AD10375_Source = GetBarProfilSection("RE_AD10375.png");
        public static readonly Uri BSP_AD10376_Source = GetBarProfilSection("RE_AD10376.png");
        public static readonly Uri BSP_AD10385_Source = GetBarProfilSection("RE_AD10385.png");
        public static readonly Uri BSP_AD10617_Source = GetBarProfilSection("RE_AD10617.png");
        public static readonly Uri BSP_AD10618_Source = GetBarProfilSection("RE_AD10618.png");
        public static readonly Uri BSP_AD10619_Source = GetBarProfilSection("RE_AD10619.png");
        public static readonly Uri BSP_AD10620_Source = GetBarProfilSection("RE_AD10620.png");
        public static readonly Uri BSP_AD10621_Source = GetBarProfilSection("RE_AD10621.png");
        public static readonly Uri BSP_AD10623_Source = GetBarProfilSection("RE_AD10623.png");
        public static readonly Uri BSP_AD10626_Source = GetBarProfilSection("RE_AD10626.png");
        public static readonly Uri BSP_AD10627_Source = GetBarProfilSection("RE_AD10627.png");
        public static readonly Uri BSP_AD10631_Source = GetBarProfilSection("RE_AD10631.png");
        public static readonly Uri BSP_AD10632_Source = GetBarProfilSection("RE_AD10632.png");
        public static readonly Uri BSP_AD10633_Source = GetBarProfilSection("RE_AD10633.png");
        public static readonly Uri BSP_AD10634_Source = GetBarProfilSection("RE_AD10634.png");
        public static readonly Uri BSP_AD10635_Source = GetBarProfilSection("RE_AD10635.png");
        public static readonly Uri BSP_AD10636_Source = GetBarProfilSection("RE_AD10636.png");
        public static readonly Uri BSP_AD10637_Source = GetBarProfilSection("RE_AD10637.png");
        public static readonly Uri BSP_AD10638_Source = GetBarProfilSection("RE_AD10638.png");
        public static readonly Uri BSP_AD10639_Source = GetBarProfilSection("RE_AD10639.png");
        public static readonly Uri BSP_AD10640_Source = GetBarProfilSection("RE_AD10640.png");
        public static readonly Uri BSP_AD10641_Source = GetBarProfilSection("RE_AD10641.png");
        public static readonly Uri BSP_AD10642_Source = GetBarProfilSection("RE_AD10642.png");
        public static readonly Uri BSP_AD10643_Source = GetBarProfilSection("RE_AD10643.png");
        public static readonly Uri BSP_AD10644_Source = GetBarProfilSection("RE_AD10644.png");
        public static readonly Uri BSP_AD10647_Source = GetBarProfilSection("RE_AD10647.png");
        public static readonly Uri BSP_AD10648_Source = GetBarProfilSection("RE_AD10648.png");
        public static readonly Uri BSP_AD10649_Source = GetBarProfilSection("RE_AD10649.png");
        public static readonly Uri BSP_AD10650_Source = GetBarProfilSection("RE_AD10650.png");
        public static readonly Uri BSP_AD10651_Source = GetBarProfilSection("RE_AD10651.png");
        public static readonly Uri BSP_AD10652_Source = GetBarProfilSection("RE_AD10652.png");
        public static readonly Uri BSP_AD10653_Source = GetBarProfilSection("RE_AD10653.png");
        public static readonly Uri BSP_AD10715_Source = GetBarProfilSection("RE_AD10715.png");
        public static readonly Uri BSP_AD10716_Source = GetBarProfilSection("RE_AD10716.png");
        public static readonly Uri BSP_AD10717_Source = GetBarProfilSection("RE_AD10717.png");
        public static readonly Uri BSP_AD10719_Source = GetBarProfilSection("RE_AD10719.png");
        public static readonly Uri BSP_AD10720_Source = GetBarProfilSection("RE_AD10720.png");
        public static readonly Uri BSP_AD10721_Source = GetBarProfilSection("RE_AD10721.png");
        public static readonly Uri BSP_AD10723_Source = GetBarProfilSection("RE_AD10723.png");
        public static readonly Uri BSP_AD10724_Source = GetBarProfilSection("RE_AD10724.png");
        public static readonly Uri BSP_AD30108_Source = GetBarProfilSection("RE_AD30108.png");
        public static readonly Uri BSP_AD40117_V2_Source = GetBarProfilSection("RE_AD40117-V2.png");
        public static readonly Uri BSP_AH_L117_Source = GetBarProfilSection("RE_AH-L117.png");
        public static readonly Uri BSP_AH_L70_Source = GetBarProfilSection("RE_AH-L70.png");
        public static readonly Uri BSP_AH_LZ102_Source = GetBarProfilSection("RE_AH-LZ102.png");
        public static readonly Uri BSP_AH_MB70_Source = GetBarProfilSection("RE_AH-MB70.png");
        public static readonly Uri BSP_AH_MD94_Source = GetBarProfilSection("RE_AH-MD94.png");
        public static readonly Uri BSP_AH_P275_Source = GetBarProfilSection("RE_AH-P275.png");
        public static readonly Uri BSP_AH_P301_Source = GetBarProfilSection("RE_AH-P301.png");
        public static readonly Uri BSP_AH_PO40C_Source = GetBarProfilSection("RE_AH-PO40C.png");
        public static readonly Uri BSP_AH_Z102_Source = GetBarProfilSection("RE_AH-Z102.png");
        public static readonly Uri BSP_AH_Z115_Source = GetBarProfilSection("RE_AH-Z115.png");
        public static readonly Uri BSP_AH_Z116_Source = GetBarProfilSection("RE_AH-Z116.png");
        public static readonly Uri BSP_AH_Z125_Source = GetBarProfilSection("RE_AH-Z125.png");
        public static readonly Uri BSP_AH_ZM102_Source = GetBarProfilSection("RE_AH-ZM102.png");
        public static readonly Uri BSP_AH_ZM115_Source = GetBarProfilSection("RE_AH-ZM115.png");
        public static readonly Uri BSP_AH_ZM116_Source = GetBarProfilSection("RE_AH-ZM116.png");
        public static readonly Uri BSP_AK10100_Source = GetBarProfilSection("RE_AK10100.png");
        public static readonly Uri BSP_AK10101_Source = GetBarProfilSection("RE_AK10101.png");
        public static readonly Uri BSP_AK10102_Source = GetBarProfilSection("RE_AK10102.png");
        public static readonly Uri BSP_AK10103_Source = GetBarProfilSection("RE_AK10103.png");
        public static readonly Uri BSP_AK10104_Source = GetBarProfilSection("RE_AK10104.png");
        public static readonly Uri BSP_AK10105_Source = GetBarProfilSection("RE_AK10105.png");
        public static readonly Uri BSP_AK10106_Source = GetBarProfilSection("RE_AK10106.png");
        public static readonly Uri BSP_AK10107_Source = GetBarProfilSection("RE_AK10107.png");
        public static readonly Uri BSP_AK10108_Source = GetBarProfilSection("RE_AK10108.png");
        public static readonly Uri BSP_AK10109_Source = GetBarProfilSection("RE_AK10109.png");
        public static readonly Uri BSP_AK10110_Source = GetBarProfilSection("RE_AK10110.png");
        public static readonly Uri BSP_AK10111_Source = GetBarProfilSection("RE_AK10111.png");
        public static readonly Uri BSP_AK10112_Source = GetBarProfilSection("RE_AK10112.png");
        public static readonly Uri BSP_AK10113_Source = GetBarProfilSection("RE_AK10113.png");
        public static readonly Uri BSP_AK10114_Source = GetBarProfilSection("RE_AK10114.png");
        public static readonly Uri BSP_AK10115_Source = GetBarProfilSection("RE_AK10115.png");
        public static readonly Uri BSP_AK10116_Source = GetBarProfilSection("RE_AK10116.png");
        public static readonly Uri BSP_AK10117_Source = GetBarProfilSection("RE_AK10117.png");
        public static readonly Uri BSP_AK10118_Source = GetBarProfilSection("RE_AK10118.png");
        public static readonly Uri BSP_AK10119_Source = GetBarProfilSection("RE_AK10119.png");
        public static readonly Uri BSP_AK10120_Source = GetBarProfilSection("RE_AK10120.png");
        public static readonly Uri BSP_AK10121_Source = GetBarProfilSection("RE_AK10121.png");
        public static readonly Uri BSP_AK10122_Source = GetBarProfilSection("RE_AK10122.png");
        public static readonly Uri BSP_AK10123_Source = GetBarProfilSection("RE_AK10123.png");
        public static readonly Uri BSP_AK10124_Source = GetBarProfilSection("RE_AK10124.png");
        public static readonly Uri BSP_AK10125_Source = GetBarProfilSection("RE_AK10125.png");
        public static readonly Uri BSP_AK10126_Source = GetBarProfilSection("RE_AK10126.png");
        public static readonly Uri BSP_AK10127_Source = GetBarProfilSection("RE_AK10127.png");
        public static readonly Uri BSP_AK10128_Source = GetBarProfilSection("RE_AK10128.png");
        public static readonly Uri BSP_AK10129_Source = GetBarProfilSection("RE_AK10129.png");
        public static readonly Uri BSP_AK10130_Source = GetBarProfilSection("RE_AK10130.png");
        public static readonly Uri BSP_AK10131_Source = GetBarProfilSection("RE_AK10131.png");
        public static readonly Uri BSP_AK10132_Source = GetBarProfilSection("RE_AK10132.png");
        public static readonly Uri BSP_AK10133_Source = GetBarProfilSection("RE_AK10133.png");
        public static readonly Uri BSP_AK10134_Source = GetBarProfilSection("RE_AK10134.png");
        public static readonly Uri BSP_AK10135_Source = GetBarProfilSection("RE_AK10135.png");
        public static readonly Uri BSP_AK10200_Source = GetBarProfilSection("RE_AK10200.png");
        public static readonly Uri BSP_AK10201_Source = GetBarProfilSection("RE_AK10201.png");
        public static readonly Uri BSP_AK10202_Source = GetBarProfilSection("RE_AK10202.png");
        public static readonly Uri BSP_AK10203_Source = GetBarProfilSection("RE_AK10203.png");
        public static readonly Uri BSP_AK10204_Source = GetBarProfilSection("RE_AK10204.png");
        public static readonly Uri BSP_AK10205_Source = GetBarProfilSection("RE_AK10205.png");
        public static readonly Uri BSP_AK10206_Source = GetBarProfilSection("RE_AK10206.png");
        public static readonly Uri BSP_AK10207_Source = GetBarProfilSection("RE_AK10207.png");
        public static readonly Uri BSP_AK10208_Source = GetBarProfilSection("RE_AK10208.png");
        public static readonly Uri BSP_AK10209_Source = GetBarProfilSection("RE_AK10209.png");
        public static readonly Uri BSP_AK10301_Source = GetBarProfilSection("RE_AK10301.png");
        public static readonly Uri BSP_AK10302_Source = GetBarProfilSection("RE_AK10302.png");
        public static readonly Uri BSP_AK10303_Source = GetBarProfilSection("RE_AK10303.png");
        public static readonly Uri BSP_AK10304_Source = GetBarProfilSection("RE_AK10304.png");
        public static readonly Uri BSP_AK10305_Source = GetBarProfilSection("RE_AK10305.png");
        public static readonly Uri BSP_AK10306_Source = GetBarProfilSection("RE_AK10306.png");
        public static readonly Uri BSP_AK10307_Source = GetBarProfilSection("RE_AK10307.png");
        public static readonly Uri BSP_AK10308_Source = GetBarProfilSection("RE_AK10308.png");
        public static readonly Uri BSP_AK10309_Source = GetBarProfilSection("RE_AK10309.png");
        public static readonly Uri BSP_AK10310_Source = GetBarProfilSection("RE_AK10310.png");
        public static readonly Uri BSP_AK10311_Source = GetBarProfilSection("RE_AK10311.png");
        public static readonly Uri BSP_AK10312_Source = GetBarProfilSection("RE_AK10312.png");
        public static readonly Uri BSP_AK10313_Source = GetBarProfilSection("RE_AK10313.png");
        public static readonly Uri BSP_AK10314_Source = GetBarProfilSection("RE_AK10314.png");
        public static readonly Uri BSP_AK10315_Source = GetBarProfilSection("RE_AK10315.png");
        public static readonly Uri BSP_AK10316_Source = GetBarProfilSection("RE_AK10316.png");
        public static readonly Uri BSP_AK10317_Source = GetBarProfilSection("RE_AK10317.png");
        public static readonly Uri BSP_AK10318_Source = GetBarProfilSection("RE_AK10318.png");
        public static readonly Uri BSP_AK10319_Source = GetBarProfilSection("RE_AK10319.png");
        public static readonly Uri BSP_AK10320_Source = GetBarProfilSection("RE_AK10320.png");
        public static readonly Uri BSP_AK10321_Source = GetBarProfilSection("RE_AK10321.png");
        public static readonly Uri BSP_AK10322_Source = GetBarProfilSection("RE_AK10322.png");
        public static readonly Uri BSP_AK10323_Source = GetBarProfilSection("RE_AK10323.png");
        public static readonly Uri BSP_AK10324_Source = GetBarProfilSection("RE_AK10324.png");
        public static readonly Uri BSP_AK10325_Source = GetBarProfilSection("RE_AK10325.png");
        public static readonly Uri BSP_AK10326_Source = GetBarProfilSection("RE_AK10326.png");
        public static readonly Uri BSP_AK10327_Source = GetBarProfilSection("RE_AK10327.png");
        public static readonly Uri BSP_AK10328_Source = GetBarProfilSection("RE_AK10328.png");
        public static readonly Uri BSP_AK10329_Source = GetBarProfilSection("RE_AK10329.png");
        public static readonly Uri BSP_AK10330_Source = GetBarProfilSection("RE_AK10330.png");
        public static readonly Uri BSP_AK10331_Source = GetBarProfilSection("RE_AK10331.png");
        public static readonly Uri BSP_AK10332_Source = GetBarProfilSection("RE_AK10332.png");
        public static readonly Uri BSP_AK10333_Source = GetBarProfilSection("RE_AK10333.png");
        public static readonly Uri BSP_AK10334_Source = GetBarProfilSection("RE_AK10334.png");
        public static readonly Uri BSP_AK10335_Source = GetBarProfilSection("RE_AK10335.png");
        public static readonly Uri BSP_AK10336_Source = GetBarProfilSection("RE_AK10336.png");
        public static readonly Uri BSP_AK10500_Source = GetBarProfilSection("RE_AK10500.png");
        public static readonly Uri BSP_AK30100_Source = GetBarProfilSection("RE_AK30100.png");
        public static readonly Uri BSP_AK30101_Source = GetBarProfilSection("RE_AK30101.png");
        public static readonly Uri BSP_AK30102_Source = GetBarProfilSection("RE_AK30102.png");
        public static readonly Uri BSP_AK30106_Source = GetBarProfilSection("RE_AK30106.png");
        public static readonly Uri BSP_AK30300_Source = GetBarProfilSection("RE_AK30300.png");
        public static readonly Uri BSP_AK30304_Source = GetBarProfilSection("RE_AK30304.png");
        public static readonly Uri BSP_AK40100_Source = GetBarProfilSection("RE_AK40100.png");
        public static readonly Uri BSP_AK40101_Source = GetBarProfilSection("RE_AK40101.png");
        public static readonly Uri BSP_AK40102_Source = GetBarProfilSection("RE_AK40102.png");
        public static readonly Uri BSP_AK40104_Source = GetBarProfilSection("RE_AK40104.png");
        public static readonly Uri BSP_AK40105_Source = GetBarProfilSection("RE_AK40105.png");
        public static readonly Uri BSP_AK40106_Source = GetBarProfilSection("RE_AK40106.png");
        public static readonly Uri BSP_AK40107_Source = GetBarProfilSection("RE_AK40107.png");
        public static readonly Uri BSP_AK40110_Source = GetBarProfilSection("RE_AK40110.png");
        public static readonly Uri BSP_AK40111_Source = GetBarProfilSection("RE_AK40111.png");
        public static readonly Uri BSP_AK40113_Source = GetBarProfilSection("RE_AK40113.png");
        public static readonly Uri BSP_AK40114_Source = GetBarProfilSection("RE_AK40114.png");
        public static readonly Uri BSP_AK40116_Source = GetBarProfilSection("RE_AK40116.png");
        public static readonly Uri BSP_AK40117_Source = GetBarProfilSection("RE_AK40117.png");
        public static readonly Uri BSP_AK40118_Source = GetBarProfilSection("RE_AK40118.png");
        public static readonly Uri BSP_AK40201_Source = GetBarProfilSection("RE_AK40201.png");
        public static readonly Uri BSP_AK40202_Source = GetBarProfilSection("RE_AK40202.png");
        public static readonly Uri BSP_AK40203_Source = GetBarProfilSection("RE_AK40203.png");
        public static readonly Uri BSP_AK40210_Source = GetBarProfilSection("RE_AK40210.png");
        public static readonly Uri BSP_AK40211_Source = GetBarProfilSection("RE_AK40211.png");
        public static readonly Uri BSP_AK40212_Source = GetBarProfilSection("RE_AK40212.png");
        public static readonly Uri BSP_AK40213_Source = GetBarProfilSection("RE_AK40213.png");
        public static readonly Uri BSP_AK40214_Source = GetBarProfilSection("RE_AK40214.png");
        public static readonly Uri BSP_AL10100_Source = GetBarProfilSection("RE_AL10100.png");
        public static readonly Uri BSP_AL10101_Source = GetBarProfilSection("RE_AL10101.png");
        public static readonly Uri BSP_AL10102_Source = GetBarProfilSection("RE_AL10102.png");
        public static readonly Uri BSP_AL10103_Source = GetBarProfilSection("RE_AL10103.png");
        public static readonly Uri BSP_AL10104_Source = GetBarProfilSection("RE_AL10104.png");
        public static readonly Uri BSP_AL10105_Source = GetBarProfilSection("RE_AL10105.png");
        public static readonly Uri BSP_AL10106_Source = GetBarProfilSection("RE_AL10106.png");
        public static readonly Uri BSP_AL10107_Source = GetBarProfilSection("RE_AL10107.png");
        public static readonly Uri BSP_AL10108_Source = GetBarProfilSection("RE_AL10108.png");
        public static readonly Uri BSP_AL10109_Source = GetBarProfilSection("RE_AL10109.png");
        public static readonly Uri BSP_AL10110_Source = GetBarProfilSection("RE_AL10110.png");
        public static readonly Uri BSP_AL10111_Source = GetBarProfilSection("RE_AL10111.png");
        public static readonly Uri BSP_AL10112_Source = GetBarProfilSection("RE_AL10112.png");
        public static readonly Uri BSP_AL10200_Source = GetBarProfilSection("RE_AL10200.png");
        public static readonly Uri BSP_AL10201_Source = GetBarProfilSection("RE_AL10201.png");
        public static readonly Uri BSP_AL10202_Source = GetBarProfilSection("RE_AL10202.png");
        public static readonly Uri BSP_AL10203_Source = GetBarProfilSection("RE_AL10203.png");
        public static readonly Uri BSP_AL10204_Source = GetBarProfilSection("RE_AL10204.png");
        public static readonly Uri BSP_AL10205_Source = GetBarProfilSection("RE_AL10205.png");
        public static readonly Uri BSP_AL10206_Source = GetBarProfilSection("RE_AL10206.png");
        public static readonly Uri BSP_AL10207_Source = GetBarProfilSection("RE_AL10207.png");
        public static readonly Uri BSP_AL10208_Source = GetBarProfilSection("RE_AL10208.png");
        public static readonly Uri BSP_AL10300_Source = GetBarProfilSection("RE_AL10300.png");
        public static readonly Uri BSP_AL10301_Source = GetBarProfilSection("RE_AL10301.png");
        public static readonly Uri BSP_AL10302_Source = GetBarProfilSection("RE_AL10302.png");
        public static readonly Uri BSP_AL10303_Source = GetBarProfilSection("RE_AL10303.png");
        public static readonly Uri BSP_AL10304_Source = GetBarProfilSection("RE_AL10304.png");
        public static readonly Uri BSP_AL10305_Source = GetBarProfilSection("RE_AL10305.png");
        public static readonly Uri BSP_AL10306_Source = GetBarProfilSection("RE_AL10306.png");
        public static readonly Uri BSP_AL10307_Source = GetBarProfilSection("RE_AL10307.png");
        public static readonly Uri BSP_AL10308_Source = GetBarProfilSection("RE_AL10308.png");
        public static readonly Uri BSP_AL10309_Source = GetBarProfilSection("RE_AL10309.png");
        public static readonly Uri BSP_AL10310_Source = GetBarProfilSection("RE_AL10310.png");
        public static readonly Uri BSP_AL10311_Source = GetBarProfilSection("RE_AL10311.png");
        public static readonly Uri BSP_AL10312_Source = GetBarProfilSection("RE_AL10312.png");
        public static readonly Uri BSP_AL10313_Source = GetBarProfilSection("RE_AL10313.png");
        public static readonly Uri BSP_AL10320_Source = GetBarProfilSection("RE_AL10320.png");
        public static readonly Uri BSP_AL10321_Source = GetBarProfilSection("RE_AL10321.png");
        public static readonly Uri BSP_AL10322_Source = GetBarProfilSection("RE_AL10322.png");
        public static readonly Uri BSP_AL10500_Source = GetBarProfilSection("RE_AL10500.png");
        public static readonly Uri BSP_AL10501_Source = GetBarProfilSection("RE_AL10501.png");
        public static readonly Uri BSP_AL10502_Source = GetBarProfilSection("RE_AL10502.png");
        public static readonly Uri BSP_AL10503_Source = GetBarProfilSection("RE_AL10503.png");
        public static readonly Uri BSP_AL30100_Source = GetBarProfilSection("RE_AL30100.png");
        public static readonly Uri BSP_AL30101_Source = GetBarProfilSection("RE_AL30101.png");
        public static readonly Uri BSP_AL40204_Source = GetBarProfilSection("RE_AL40204.png");
        public static readonly Uri BSP_AL40205_Source = GetBarProfilSection("RE_AL40205.png");
        public static readonly Uri BSP_AL40206_Source = GetBarProfilSection("RE_AL40206.png");
        public static readonly Uri BSP_AL5102_Source = GetBarProfilSection("RE_AL5102.png");
        public static readonly Uri BSP_AL5103_Source = GetBarProfilSection("RE_AL5103.png");
        public static readonly Uri BSP_AL70_Source = GetBarProfilSection("RE_AL70.png");
        public static readonly Uri BSP_AL81_Source = GetBarProfilSection("RE_AL81.png");
        public static readonly Uri BSP_AL89_Source = GetBarProfilSection("RE_AL89.png");
        public static readonly Uri BSP_AM10100_Source = GetBarProfilSection("RE_AM10100.png");
        public static readonly Uri BSP_AM10101_Source = GetBarProfilSection("RE_AM10101.png");
        public static readonly Uri BSP_AM10102_Source = GetBarProfilSection("RE_AM10102.png");
        public static readonly Uri BSP_AM10103_Source = GetBarProfilSection("RE_AM10103.png");
        public static readonly Uri BSP_AM10104_Source = GetBarProfilSection("RE_AM10104.png");
        public static readonly Uri BSP_AM10105_Source = GetBarProfilSection("RE_AM10105.png");
        public static readonly Uri BSP_AM10106_Source = GetBarProfilSection("RE_AM10106.png");
        public static readonly Uri BSP_AM10200_Source = GetBarProfilSection("RE_AM10200.png");
        public static readonly Uri BSP_AM10201_Source = GetBarProfilSection("RE_AM10201.png");
        public static readonly Uri BSP_AM10202_Source = GetBarProfilSection("RE_AM10202.png");
        public static readonly Uri BSP_AM10203_Source = GetBarProfilSection("RE_AM10203.png");
        public static readonly Uri BSP_AM10204_Source = GetBarProfilSection("RE_AM10204.png");
        public static readonly Uri BSP_AM10205_Source = GetBarProfilSection("RE_AM10205.png");
        public static readonly Uri BSP_AM10206_Source = GetBarProfilSection("RE_AM10206.png");
        public static readonly Uri BSP_AM10300_Source = GetBarProfilSection("RE_AM10300.png");
        public static readonly Uri BSP_AM10301_Source = GetBarProfilSection("RE_AM10301.png");
        public static readonly Uri BSP_AM10302_Source = GetBarProfilSection("RE_AM10302.png");
        public static readonly Uri BSP_AM10303_Source = GetBarProfilSection("RE_AM10303.png");
        public static readonly Uri BSP_AM10304_Source = GetBarProfilSection("RE_AM10304.png");
        public static readonly Uri BSP_AM10305_Source = GetBarProfilSection("RE_AM10305.png");
        public static readonly Uri BSP_AM10306_Source = GetBarProfilSection("RE_AM10306.png");
        public static readonly Uri BSP_AM10500_Source = GetBarProfilSection("RE_AM10500.png");
        public static readonly Uri BSP_AM40200_Source = GetBarProfilSection("RE_AM40200.png");
        public static readonly Uri BSP_AMB68D_Source = GetBarProfilSection("RE_AMB68D.png");
        public static readonly Uri BSP_AMB89D_Source = GetBarProfilSection("RE_AMB89D.png");
        public static readonly Uri BSP_AP10100_Source = GetBarProfilSection("RE_AP10100.png");
        public static readonly Uri BSP_AP10200_Source = GetBarProfilSection("RE_AP10200.png");
        public static readonly Uri BSP_AP10201_Source = GetBarProfilSection("RE_AP10201.png");
        public static readonly Uri BSP_AP10202_Source = GetBarProfilSection("RE_AP10202.png");
        public static readonly Uri BSP_AP20100_Source = GetBarProfilSection("RE_AP20100.png");
        public static readonly Uri BSP_AP20101_Source = GetBarProfilSection("RE_AP20101.png");
        public static readonly Uri BSP_AP20102_Source = GetBarProfilSection("RE_AP20102.png");
        public static readonly Uri BSP_AP20105_Source = GetBarProfilSection("RE_AP20105.png");
        public static readonly Uri BSP_AP20106_Source = GetBarProfilSection("RE_AP20106.png");
        public static readonly Uri BSP_AP20200_Source = GetBarProfilSection("RE_AP20200.png");
        public static readonly Uri BSP_AP20201_Source = GetBarProfilSection("RE_AP20201.png");
        public static readonly Uri BSP_AP20202_Source = GetBarProfilSection("RE_AP20202.png");
        public static readonly Uri BSP_AP20203_Source = GetBarProfilSection("RE_AP20203.png");
        public static readonly Uri BSP_AP20204_Source = GetBarProfilSection("RE_AP20204.png");
        public static readonly Uri BSP_AP20205_Source = GetBarProfilSection("RE_AP20205.png");
        public static readonly Uri BSP_AP20206_Source = GetBarProfilSection("RE_AP20206.png");
        public static readonly Uri BSP_AP20207_Source = GetBarProfilSection("RE_AP20207.png");
        public static readonly Uri BSP_AP20208_Source = GetBarProfilSection("RE_AP20208.png");
        public static readonly Uri BSP_AP20209_Source = GetBarProfilSection("RE_AP20209.png");
        public static readonly Uri BSP_AP20210_Source = GetBarProfilSection("RE_AP20210.png");
        public static readonly Uri BSP_AP20212_Source = GetBarProfilSection("RE_AP20212.png");
        public static readonly Uri BSP_AP20213_Source = GetBarProfilSection("RE_AP20213.png");
        public static readonly Uri BSP_AP20214_Source = GetBarProfilSection("RE_AP20214.png");
        public static readonly Uri BSP_AP20215_Source = GetBarProfilSection("RE_AP20215.png");
        public static readonly Uri BSP_AP20216_Source = GetBarProfilSection("RE_AP20216.png");
        public static readonly Uri BSP_AP20217_Source = GetBarProfilSection("RE_AP20217.png");
        public static readonly Uri BSP_AP20218_Source = GetBarProfilSection("RE_AP20218.png");
        public static readonly Uri BSP_AP28D_Source = GetBarProfilSection("RE_AP28D.png");
        public static readonly Uri BSP_AP32D_Source = GetBarProfilSection("RE_AP32D.png");
        public static readonly Uri BSP_AP44D_Source = GetBarProfilSection("RE_AP44D.png");
        public static readonly Uri BSP_APE70_Source = GetBarProfilSection("RE_APE70.png");
        public static readonly Uri BSP_APF28D_Source = GetBarProfilSection("RE_APF28D.png");
        public static readonly Uri BSP_APF32D_Source = GetBarProfilSection("RE_APF32D.png");
        public static readonly Uri BSP_APF44D_Source = GetBarProfilSection("RE_APF44D.png");
        public static readonly Uri BSP_AR10110_Source = GetBarProfilSection("RE_AR10110.png");
        public static readonly Uri BSP_AR10111_Source = GetBarProfilSection("RE_AR10111.png");
        public static readonly Uri BSP_AR10112_Source = GetBarProfilSection("RE_AR10112.png");
        public static readonly Uri BSP_AR10113_Source = GetBarProfilSection("RE_AR10113.png");
        public static readonly Uri BSP_AR10114_Source = GetBarProfilSection("RE_AR10114.png");
        public static readonly Uri BSP_AR10115_Source = GetBarProfilSection("RE_AR10115.png");
        public static readonly Uri BSP_AR10116_Source = GetBarProfilSection("RE_AR10116.png");
        public static readonly Uri BSP_AR10117_Source = GetBarProfilSection("RE_AR10117.png");
        public static readonly Uri BSP_AR10118_Source = GetBarProfilSection("RE_AR10118.png");
        public static readonly Uri BSP_AR10119_Source = GetBarProfilSection("RE_AR10119.png");
        public static readonly Uri BSP_AR126_Source = GetBarProfilSection("RE_AR126.png");
        public static readonly Uri BSP_AR65_Source = GetBarProfilSection("RE_AR65.png");
        public static readonly Uri BSP_ARJ_OC_Source = GetBarProfilSection("RE_ARJ-OC.png");
        public static readonly Uri BSP_ARJ_OV_Source = GetBarProfilSection("RE_ARJ-OV.png");
        public static readonly Uri BSP_AS10100_Source = GetBarProfilSection("RE_AS10100.png");
        public static readonly Uri BSP_AS10101_Source = GetBarProfilSection("RE_AS10101.png");
        public static readonly Uri BSP_AS10200_Source = GetBarProfilSection("RE_AS10200.png");
        public static readonly Uri BSP_AS10300_Source = GetBarProfilSection("RE_AS10300.png");
        public static readonly Uri BSP_AS10301_Source = GetBarProfilSection("RE_AS10301.png");
        public static readonly Uri BSP_AS10302_Source = GetBarProfilSection("RE_AS10302.png");
        public static readonly Uri BSP_AS20200_Source = GetBarProfilSection("RE_AS20200.png");
        public static readonly Uri BSP_AS20202_Source = GetBarProfilSection("RE_AS20202.png");
        public static readonly Uri BSP_AS20203_Source = GetBarProfilSection("RE_AS20203.png");
        public static readonly Uri BSP_AS40216_Source = GetBarProfilSection("RE_AS40216.png");
        public static readonly Uri BSP_AU10100_Source = GetBarProfilSection("RE_AU10100.png");
        public static readonly Uri BSP_AU10101_Source = GetBarProfilSection("RE_AU10101.png");
        public static readonly Uri BSP_AU10102_Source = GetBarProfilSection("RE_AU10102.png");
        public static readonly Uri BSP_AU10103_Source = GetBarProfilSection("RE_AU10103.png");
        public static readonly Uri BSP_AU10104_Source = GetBarProfilSection("RE_AU10104.png");
        public static readonly Uri BSP_AU10105_Source = GetBarProfilSection("RE_AU10105.png");
        public static readonly Uri BSP_AU10106_Source = GetBarProfilSection("RE_AU10106.png");
        public static readonly Uri BSP_AU10107_Source = GetBarProfilSection("RE_AU10107.png");
        public static readonly Uri BSP_AU10108_Source = GetBarProfilSection("RE_AU10108.png");
        public static readonly Uri BSP_AU10109_Source = GetBarProfilSection("RE_AU10109.png");
        public static readonly Uri BSP_AU10110_Source = GetBarProfilSection("RE_AU10110.png");
        public static readonly Uri BSP_AU10111_Source = GetBarProfilSection("RE_AU10111.png");
        public static readonly Uri BSP_AU10112_Source = GetBarProfilSection("RE_AU10112.png");
        public static readonly Uri BSP_AU10113_Source = GetBarProfilSection("RE_AU10113.png");
        public static readonly Uri BSP_AU10114_Source = GetBarProfilSection("RE_AU10114.png");
        public static readonly Uri BSP_AU10200_Source = GetBarProfilSection("RE_AU10200.png");
        public static readonly Uri BSP_AU10201_Source = GetBarProfilSection("RE_AU10201.png");
        public static readonly Uri BSP_AU10202_Source = GetBarProfilSection("RE_AU10202.png");
        public static readonly Uri BSP_AU10203_Source = GetBarProfilSection("RE_AU10203.png");
        public static readonly Uri BSP_AU10204_Source = GetBarProfilSection("RE_AU10204.png");
        public static readonly Uri BSP_AU10205_Source = GetBarProfilSection("RE_AU10205.png");
        public static readonly Uri BSP_AU10300_Source = GetBarProfilSection("RE_AU10300.png");
        public static readonly Uri BSP_AU10301_Source = GetBarProfilSection("RE_AU10301.png");
        public static readonly Uri BSP_AU10302_Source = GetBarProfilSection("RE_AU10302.png");
        public static readonly Uri BSP_AU10303_Source = GetBarProfilSection("RE_AU10303.png");
        public static readonly Uri BSP_AU10304_Source = GetBarProfilSection("RE_AU10304.png");
        public static readonly Uri BSP_AU10305_Source = GetBarProfilSection("RE_AU10305.png");
        public static readonly Uri BSP_AU10306_Source = GetBarProfilSection("RE_AU10306.png");
        public static readonly Uri BSP_AU10307_Source = GetBarProfilSection("RE_AU10307.png");
        public static readonly Uri BSP_AU10308_Source = GetBarProfilSection("RE_AU10308.png");
        public static readonly Uri BSP_AU10309_Source = GetBarProfilSection("RE_AU10309.png");
        public static readonly Uri BSP_AU10310_Source = GetBarProfilSection("RE_AU10310.png");
        public static readonly Uri BSP_AU10311_Source = GetBarProfilSection("RE_AU10311.png");
        public static readonly Uri BSP_AU10312_Source = GetBarProfilSection("RE_AU10312.png");
        public static readonly Uri BSP_AU10313_Source = GetBarProfilSection("RE_AU10313.png");
        public static readonly Uri BSP_AU10314_Source = GetBarProfilSection("RE_AU10314.png");
        public static readonly Uri BSP_AU10315_Source = GetBarProfilSection("RE_AU10315.png");
        public static readonly Uri BSP_AU10500_Source = GetBarProfilSection("RE_AU10500.png");
        public static readonly Uri BSP_AU10501_Source = GetBarProfilSection("RE_AU10501.png");
        public static readonly Uri BSP_AU10502_Source = GetBarProfilSection("RE_AU10502.png");
        public static readonly Uri BSP_AU10503_Source = GetBarProfilSection("RE_AU10503.png");
        public static readonly Uri BSP_AU10504_Source = GetBarProfilSection("RE_AU10504.png");
        public static readonly Uri BSP_AU30100_Source = GetBarProfilSection("RE_AU30100.png");
        public static readonly Uri BSP_AU30101_Source = GetBarProfilSection("RE_AU30101.png");
        public static readonly Uri BSP_AU30200_Source = GetBarProfilSection("RE_AU30200.png");
        public static readonly Uri BSP_AU30201_Source = GetBarProfilSection("RE_AU30201.png");
        public static readonly Uri BSP_AV090090_Source = GetBarProfilSection("RE_AV090090.png");
        public static readonly Uri BSP_AV090091_Source = GetBarProfilSection("RE_AV090091.png");
        public static readonly Uri BSP_AV090092_Source = GetBarProfilSection("RE_AV090092.png");
        public static readonly Uri BSP_AV090093_Source = GetBarProfilSection("RE_AV090093.png");
        public static readonly Uri BSP_AV10200_Source = GetBarProfilSection("RE_AV10200.png");
        public static readonly Uri BSP_AV10201_Source = GetBarProfilSection("RE_AV10201.png");
        public static readonly Uri BSP_AV10202_Source = GetBarProfilSection("RE_AV10202.png");
        public static readonly Uri BSP_AV10203_Source = GetBarProfilSection("RE_AV10203.png");
        public static readonly Uri BSP_AV10204_Source = GetBarProfilSection("RE_AV10204.png");
        public static readonly Uri BSP_AV114245_Source = GetBarProfilSection("RE_AV114245.png");
        public static readonly Uri BSP_AV120097_Source = GetBarProfilSection("RE_AV120097.png");
        public static readonly Uri BSP_AV120102_Source = GetBarProfilSection("RE_AV120102.png");
        public static readonly Uri BSP_AV120106_Source = GetBarProfilSection("RE_AV120106.png");
        public static readonly Uri BSP_AV120132_Source = GetBarProfilSection("RE_AV120132.png");
        public static readonly Uri BSP_AV120135_Source = GetBarProfilSection("RE_AV120135.png");
        public static readonly Uri BSP_AV120147_Source = GetBarProfilSection("RE_AV120147.png");
        public static readonly Uri BSP_AV120149_Source = GetBarProfilSection("RE_AV120149.png");
        public static readonly Uri BSP_AV120206_Source = GetBarProfilSection("RE_AV120206.png");
        public static readonly Uri BSP_AV120209_Source = GetBarProfilSection("RE_AV120209.png");
        public static readonly Uri BSP_AV120210_Source = GetBarProfilSection("RE_AV120210.png");
        public static readonly Uri BSP_AV120219_Source = GetBarProfilSection("RE_AV120219.png");
        public static readonly Uri BSP_AV120224_Source = GetBarProfilSection("RE_AV120224.png");
        public static readonly Uri BSP_AV120236_Source = GetBarProfilSection("RE_AV120236.png");
        public static readonly Uri BSP_AV120237_Source = GetBarProfilSection("RE_AV120237.png");
        public static readonly Uri BSP_AV120246_Source = GetBarProfilSection("RE_AV120246.png");
        public static readonly Uri BSP_AV120248_Source = GetBarProfilSection("RE_AV120248.png");
        public static readonly Uri BSP_AV120249_Source = GetBarProfilSection("RE_AV120249.png");
        public static readonly Uri BSP_AV120832_Source = GetBarProfilSection("RE_AV120832.png");
        public static readonly Uri BSP_AV120835_Source = GetBarProfilSection("RE_AV120835.png");
        public static readonly Uri BSP_AV120847_Source = GetBarProfilSection("RE_AV120847.png");
        public static readonly Uri BSP_AV120849_Source = GetBarProfilSection("RE_AV120849.png");
        public static readonly Uri BSP_AV124214_Source = GetBarProfilSection("RE_AV124214.png");
        public static readonly Uri BSP_AV124245_Source = GetBarProfilSection("RE_AV124245.png");
        public static readonly Uri BSP_AV130210_Source = GetBarProfilSection("RE_AV130210.png");
        public static readonly Uri BSP_AV140001_Source = GetBarProfilSection("RE_AV140001.png");
        public static readonly Uri BSP_AV140009_Source = GetBarProfilSection("RE_AV140009.png");
        public static readonly Uri BSP_AV140011_Source = GetBarProfilSection("RE_AV140011.png");
        public static readonly Uri BSP_AV140013_Source = GetBarProfilSection("RE_AV140013.png");
        public static readonly Uri BSP_AV140017_Source = GetBarProfilSection("RE_AV140017.png");
        public static readonly Uri BSP_AV140041_Source = GetBarProfilSection("RE_AV140041.png");
        public static readonly Uri BSP_AV140045_Source = GetBarProfilSection("RE_AV140045.png");
        public static readonly Uri BSP_AV140046_Source = GetBarProfilSection("RE_AV140046.png");
        public static readonly Uri BSP_AV140200_Source = GetBarProfilSection("RE_AV140200.png");
        public static readonly Uri BSP_AV140205_Source = GetBarProfilSection("RE_AV140205.png");
        public static readonly Uri BSP_AV140207_Source = GetBarProfilSection("RE_AV140207.png");
        public static readonly Uri BSP_AV140208_Source = GetBarProfilSection("RE_AV140208.png");
        public static readonly Uri BSP_AV140209_Source = GetBarProfilSection("RE_AV140209.png");
        public static readonly Uri BSP_AV140215_Source = GetBarProfilSection("RE_AV140215.png");
        public static readonly Uri BSP_AV140218_Source = GetBarProfilSection("RE_AV140218.png");
        public static readonly Uri BSP_AV140219_Source = GetBarProfilSection("RE_AV140219.png");
        public static readonly Uri BSP_AV140227_Source = GetBarProfilSection("RE_AV140227.png");
        public static readonly Uri BSP_AV140238_Source = GetBarProfilSection("RE_AV140238.png");
        public static readonly Uri BSP_AV140243_Source = GetBarProfilSection("RE_AV140243.png");
        public static readonly Uri BSP_AV140244_Source = GetBarProfilSection("RE_AV140244.png");
        public static readonly Uri BSP_AV140248_Source = GetBarProfilSection("RE_AV140248.png");
        public static readonly Uri BSP_AV140258_Source = GetBarProfilSection("RE_AV140258.png");
        public static readonly Uri BSP_AV140267_Source = GetBarProfilSection("RE_AV140267.png");
        public static readonly Uri BSP_AV140277_Source = GetBarProfilSection("RE_AV140277.png");
        public static readonly Uri BSP_AV144247_Source = GetBarProfilSection("RE_AV144247.png");
        public static readonly Uri BSP_AV160076_Source = GetBarProfilSection("RE_AV160076.png");
        public static readonly Uri BSP_AV160219_Source = GetBarProfilSection("RE_AV160219.png");
        public static readonly Uri BSP_AV160251_Source = GetBarProfilSection("RE_AV160251.png");
        public static readonly Uri BSP_AV160252_Source = GetBarProfilSection("RE_AV160252.png");
        public static readonly Uri BSP_AV190273_Source = GetBarProfilSection("RE_AV190273.png");
        public static readonly Uri BSP_AV190274_Source = GetBarProfilSection("RE_AV190274.png");
        public static readonly Uri BSP_AV20100_Source = GetBarProfilSection("RE_AV20100.png");
        public static readonly Uri BSP_AV20101_Source = GetBarProfilSection("RE_AV20101.png");
        public static readonly Uri BSP_AV20102_Source = GetBarProfilSection("RE_AV20102.png");
        public static readonly Uri BSP_AV20103_Source = GetBarProfilSection("RE_AV20103.png");
        public static readonly Uri BSP_AV20104_Source = GetBarProfilSection("RE_AV20104.png");
        public static readonly Uri BSP_AV20200_Source = GetBarProfilSection("RE_AV20200.png");
        public static readonly Uri BSP_AV20201_Source = GetBarProfilSection("RE_AV20201.png");
        public static readonly Uri BSP_AV20202_Source = GetBarProfilSection("RE_AV20202.png");
        public static readonly Uri BSP_AV209900_Source = GetBarProfilSection("RE_AV209900.png");
        public static readonly Uri BSP_AV227300_Source = GetBarProfilSection("RE_AV227300.png");
        public static readonly Uri BSP_AV227303_Source = GetBarProfilSection("RE_AV227303.png");
        public static readonly Uri BSP_AV229018_Source = GetBarProfilSection("RE_AV229018.png");
        public static readonly Uri BSP_AV229019_Source = GetBarProfilSection("RE_AV229019.png");
        public static readonly Uri BSP_AV229029_Source = GetBarProfilSection("RE_AV229029.png");
        public static readonly Uri BSP_AV229030_Source = GetBarProfilSection("RE_AV229030.png");
        public static readonly Uri BSP_AV229040_Source = GetBarProfilSection("RE_AV229040.png");
        public static readonly Uri BSP_AV229098_Source = GetBarProfilSection("RE_AV229098.png");
        public static readonly Uri BSP_AV229100_Source = GetBarProfilSection("RE_AV229100.png");
        public static readonly Uri BSP_AV249019_Source = GetBarProfilSection("RE_AV249019.png");
        public static readonly Uri BSP_AV249034_Source = GetBarProfilSection("RE_AV249034.png");
        public static readonly Uri BSP_AV249035_Source = GetBarProfilSection("RE_AV249035.png");
        public static readonly Uri BSP_AV249046_Source = GetBarProfilSection("RE_AV249046.png");
        public static readonly Uri BSP_AV249244_Source = GetBarProfilSection("RE_AV249244.png");
        public static readonly Uri BSP_AV269101_Source = GetBarProfilSection("RE_AV269101.png");
        public static readonly Uri BSP_AV289218_Source = GetBarProfilSection("RE_AV289218.png");
        public static readonly Uri BSP_AV420250_Source = GetBarProfilSection("RE_AV420250.png");
        public static readonly Uri BSP_AV420340_Source = GetBarProfilSection("RE_AV420340.png");
        public static readonly Uri BSP_AV429311_Source = GetBarProfilSection("RE_AV429311.png");
        public static readonly Uri BSP_AV429340_Source = GetBarProfilSection("RE_AV429340.png");
        public static readonly Uri BSP_AV440340_Source = GetBarProfilSection("RE_AV440340.png");
        public static readonly Uri BSP_AV470025_Source = GetBarProfilSection("RE_AV470025.png");
        public static readonly Uri BSP_AV477015_Source = GetBarProfilSection("RE_AV477015.png");
        public static readonly Uri BSP_AV479015_Source = GetBarProfilSection("RE_AV479015.png");
        public static readonly Uri BSP_AV652923_Source = GetBarProfilSection("RE_AV652923.png");
        public static readonly Uri BSP_AV_ALU_4_CA_Source = GetBarProfilSection("RE_AV_ALU_4-CA.png");
        public static readonly Uri BSP_AV_ALU_4R_T_Source = GetBarProfilSection("RE_AV_ALU_4R_T.png");
        public static readonly Uri BSP_AZ60_32_Source = GetBarProfilSection("RE_AZ60-32.png");
        public static readonly Uri BSP_AZ70D_Source = GetBarProfilSection("RE_AZ70D.png");
        public static readonly Uri BSP_AZM78D_Source = GetBarProfilSection("RE_AZM78D.png");
        public static readonly Uri BSP_B813010_Source = GetBarProfilSection("RE_B813010.png");
        public static readonly Uri BSP_B813040_Source = GetBarProfilSection("RE_B813040.png");
        public static readonly Uri BSP_B813071_Source = GetBarProfilSection("RE_B813071.png");
        public static readonly Uri BSP_B813080_Source = GetBarProfilSection("RE_B813080.png");
        public static readonly Uri BSP_B813233_Source = GetBarProfilSection("RE_B813233.png");
        public static readonly Uri BSP_B813235_Source = GetBarProfilSection("RE_B813235.png");
        public static readonly Uri BSP_B813740_Source = GetBarProfilSection("RE_B813740.png");
        public static readonly Uri BSP_B813750_Source = GetBarProfilSection("RE_B813750.png");
        public static readonly Uri BSP_B813770_Source = GetBarProfilSection("RE_B813770.png");
        public static readonly Uri BSP_B813920_Source = GetBarProfilSection("RE_B813920.png");
        public static readonly Uri BSP_B813930_Source = GetBarProfilSection("RE_B813930.png");
        public static readonly Uri BSP_B850001_Source = GetBarProfilSection("RE_B850001.png");
        public static readonly Uri BSP_B850002_Source = GetBarProfilSection("RE_B850002.png");
        public static readonly Uri BSP_B850008_Source = GetBarProfilSection("RE_B850008.png");
        public static readonly Uri BSP_B850034_Source = GetBarProfilSection("RE_B850034.png");
        public static readonly Uri BSP_B850035_Source = GetBarProfilSection("RE_B850035.png");
        public static readonly Uri BSP_B850301_Source = GetBarProfilSection("RE_B850301.png");
        public static readonly Uri BSP_B850302_Source = GetBarProfilSection("RE_B850302.png");
        public static readonly Uri BSP_B850313_Source = GetBarProfilSection("RE_B850313.png");
        public static readonly Uri BSP_B850314_Source = GetBarProfilSection("RE_B850314.png");
        public static readonly Uri BSP_B850315_Source = GetBarProfilSection("RE_B850315.png");
        public static readonly Uri BSP_B850316_Source = GetBarProfilSection("RE_B850316.png");
        public static readonly Uri BSP_B850336_Source = GetBarProfilSection("RE_B850336.png");
        public static readonly Uri BSP_B850609_Source = GetBarProfilSection("RE_B850609.png");
        public static readonly Uri BSP_B852000_Source = GetBarProfilSection("RE_B852000.png");
        public static readonly Uri BSP_B852001_Source = GetBarProfilSection("RE_B852001.png");
        public static readonly Uri BSP_B852200_Source = GetBarProfilSection("RE_B852200.png");
        public static readonly Uri BSP_B852201_Source = GetBarProfilSection("RE_B852201.png");
        public static readonly Uri BSP_B852202_Source = GetBarProfilSection("RE_B852202.png");
        public static readonly Uri BSP_B852203_Source = GetBarProfilSection("RE_B852203.png");
        public static readonly Uri BSP_B852500_Source = GetBarProfilSection("RE_B852500.png");
        public static readonly Uri BSP_B852501_Source = GetBarProfilSection("RE_B852501.png");
        public static readonly Uri BSP_B852600_Source = GetBarProfilSection("RE_B852600.png");
        public static readonly Uri BSP_B852601_Source = GetBarProfilSection("RE_B852601.png");
        public static readonly Uri BSP_B852605_Source = GetBarProfilSection("RE_B852605.png");
        public static readonly Uri BSP_B852615_Source = GetBarProfilSection("RE_B852615.png");
        public static readonly Uri BSP_B888_Source = GetBarProfilSection("RE_B888.png");
        public static readonly Uri BSP_B889_Source = GetBarProfilSection("RE_B889.png");
        public static readonly Uri BSP_BP1_Source = GetBarProfilSection("RE_BP1.png");
        public static readonly Uri BSP_BTCC_Source = GetBarProfilSection("RE_BTCC.png");
        public static readonly Uri BSP_BTCV_Source = GetBarProfilSection("RE_BTCV.png");
        public static readonly Uri BSP_BTD_Source = GetBarProfilSection("RE_BTD.png");
        public static readonly Uri BSP_BVSV_Source = GetBarProfilSection("RE_BVSV.png");
        public static readonly Uri BSP_C135J09_Source = GetBarProfilSection("RE_C135J09.png");
        public static readonly Uri BSP_C135J13_Source = GetBarProfilSection("RE_C135J13.png");
        public static readonly Uri BSP_C235J09_Source = GetBarProfilSection("RE_C235J09.png");
        public static readonly Uri BSP_C235J13_Source = GetBarProfilSection("RE_C235J13.png");
        public static readonly Uri BSP_CA135_Source = GetBarProfilSection("RE_CA135.png");
        public static readonly Uri BSP_CA235_Source = GetBarProfilSection("RE_CA235.png");
        public static readonly Uri BSP_CA801_Source = GetBarProfilSection("RE_CA801.png");
        public static readonly Uri BSP_CA802_V2_Source = GetBarProfilSection("RE_CA802-V2.png");
        public static readonly Uri BSP_CA802_Source = GetBarProfilSection("RE_CA802.png");
        public static readonly Uri BSP_CA811_Source = GetBarProfilSection("RE_CA811.png");
        public static readonly Uri BSP_CA812_Source = GetBarProfilSection("RE_CA812.png");
        public static readonly Uri BSP_CA813_Source = GetBarProfilSection("RE_CA813.png");
        public static readonly Uri BSP_CA815_Source = GetBarProfilSection("RE_CA815.png");
        public static readonly Uri BSP_CA816_Source = GetBarProfilSection("RE_CA816.png");
        public static readonly Uri BSP_CA817_Source = GetBarProfilSection("RE_CA817.png");
        public static readonly Uri BSP_CA819_Source = GetBarProfilSection("RE_CA819.png");
        public static readonly Uri BSP_CA821_Source = GetBarProfilSection("RE_CA821.png");
        public static readonly Uri BSP_CA823_Source = GetBarProfilSection("RE_CA823.png");
        public static readonly Uri BSP_CA824_Source = GetBarProfilSection("RE_CA824.png");
        public static readonly Uri BSP_CA825_Source = GetBarProfilSection("RE_CA825.png");
        public static readonly Uri BSP_CA831_Source = GetBarProfilSection("RE_CA831.png");
        public static readonly Uri BSP_CA832_Source = GetBarProfilSection("RE_CA832.png");
        public static readonly Uri BSP_CA833_Source = GetBarProfilSection("RE_CA833.png");
        public static readonly Uri BSP_CA835_Source = GetBarProfilSection("RE_CA835.png");
        public static readonly Uri BSP_CA836_Source = GetBarProfilSection("RE_CA836.png");
        public static readonly Uri BSP_CA837_Source = GetBarProfilSection("RE_CA837.png");
        public static readonly Uri BSP_CA838_Source = GetBarProfilSection("RE_CA838.png");
        public static readonly Uri BSP_CA839_Source = GetBarProfilSection("RE_CA839.png");
        public static readonly Uri BSP_CA841_Source = GetBarProfilSection("RE_CA841.png");
        public static readonly Uri BSP_CA844_Source = GetBarProfilSection("RE_CA844.png");
        public static readonly Uri BSP_CA845_Source = GetBarProfilSection("RE_CA845.png");
        public static readonly Uri BSP_CA846_Source = GetBarProfilSection("RE_CA846.png");
        public static readonly Uri BSP_CA847_Source = GetBarProfilSection("RE_CA847.png");
        public static readonly Uri BSP_CA848_Source = GetBarProfilSection("RE_CA848.png");
        public static readonly Uri BSP_CA849_Source = GetBarProfilSection("RE_CA849.png");
        public static readonly Uri BSP_CA851_Source = GetBarProfilSection("RE_CA851.png");
        public static readonly Uri BSP_CA852_Source = GetBarProfilSection("RE_CA852.png");
        public static readonly Uri BSP_CA853_Source = GetBarProfilSection("RE_CA853.png");
        public static readonly Uri BSP_CA854_Source = GetBarProfilSection("RE_CA854.png");
        public static readonly Uri BSP_CA855_Source = GetBarProfilSection("RE_CA855.png");
        public static readonly Uri BSP_CA861_Source = GetBarProfilSection("RE_CA861.png");
        public static readonly Uri BSP_CA870_Source = GetBarProfilSection("RE_CA870.png");
        public static readonly Uri BSP_CA871_Source = GetBarProfilSection("RE_CA871.png");
        public static readonly Uri BSP_CA875_Source = GetBarProfilSection("RE_CA875.png");
        public static readonly Uri BSP_CA876_Source = GetBarProfilSection("RE_CA876.png");
        public static readonly Uri BSP_CA890_Source = GetBarProfilSection("RE_CA890.png");
        public static readonly Uri BSP_CA893_Source = GetBarProfilSection("RE_CA893.png");
        public static readonly Uri BSP_CA894_Source = GetBarProfilSection("RE_CA894.png");
        public static readonly Uri BSP_CAD10001_Source = GetBarProfilSection("RE_CAD10001.png");
        public static readonly Uri BSP_CAD10002_Source = GetBarProfilSection("RE_CAD10002.png");
        public static readonly Uri BSP_CAD10003_Source = GetBarProfilSection("RE_CAD10003.png");
        public static readonly Uri BSP_CAD10004_Source = GetBarProfilSection("RE_CAD10004.png");
        public static readonly Uri BSP_CAD10007_Source = GetBarProfilSection("RE_CAD10007.png");
        public static readonly Uri BSP_CAD10008_Source = GetBarProfilSection("RE_CAD10008.png");
        public static readonly Uri BSP_CAD10101_Source = GetBarProfilSection("RE_CAD10101.png");
        public static readonly Uri BSP_CAD10101D_Source = GetBarProfilSection("RE_CAD10101D.png");
        public static readonly Uri BSP_CAD10211_Source = GetBarProfilSection("RE_CAD10211.png");
        public static readonly Uri BSP_CAD10212_Source = GetBarProfilSection("RE_CAD10212.png");
        public static readonly Uri BSP_CAD10213_Source = GetBarProfilSection("RE_CAD10213.png");
        public static readonly Uri BSP_CAD10215_Source = GetBarProfilSection("RE_CAD10215.png");
        public static readonly Uri BSP_CAD10218_Source = GetBarProfilSection("RE_CAD10218.png");
        public static readonly Uri BSP_CAD10221_Source = GetBarProfilSection("RE_CAD10221.png");
        public static readonly Uri BSP_CAD10223_Source = GetBarProfilSection("RE_CAD10223.png");
        public static readonly Uri BSP_CAD10224_Source = GetBarProfilSection("RE_CAD10224.png");
        public static readonly Uri BSP_CAD10225_Source = GetBarProfilSection("RE_CAD10225.png");
        public static readonly Uri BSP_CAD10226_Source = GetBarProfilSection("RE_CAD10226.png");
        public static readonly Uri BSP_CAD10227_Source = GetBarProfilSection("RE_CAD10227.png");
        public static readonly Uri BSP_CAD10228_Source = GetBarProfilSection("RE_CAD10228.png");
        public static readonly Uri BSP_CAD10229_Source = GetBarProfilSection("RE_CAD10229.png");
        public static readonly Uri BSP_CAD10230_Source = GetBarProfilSection("RE_CAD10230.png");
        public static readonly Uri BSP_CAD10231_Source = GetBarProfilSection("RE_CAD10231.png");
        public static readonly Uri BSP_CAD10232_Source = GetBarProfilSection("RE_CAD10232.png");
        public static readonly Uri BSP_CAL155_Source = GetBarProfilSection("RE_CAL155.png");
        public static readonly Uri BSP_CDA66_Source = GetBarProfilSection("RE_CDA66.png");
        public static readonly Uri BSP_CE26_Source = GetBarProfilSection("RE_CE26.png");
        public static readonly Uri BSP_CF1_SNT_Source = GetBarProfilSection("RE_CF1-SNT.png");
        public static readonly Uri BSP_CG813_Source = GetBarProfilSection("RE_CG813.png");
        public static readonly Uri BSP_CG814_Source = GetBarProfilSection("RE_CG814.png");
        public static readonly Uri BSP_CG819_Source = GetBarProfilSection("RE_CG819.png");
        public static readonly Uri BSP_CG820_Source = GetBarProfilSection("RE_CG820.png");
        public static readonly Uri BSP_CG837_Source = GetBarProfilSection("RE_CG837.png");
        public static readonly Uri BSP_CJ_Source = GetBarProfilSection("RE_CJ.png");
        public static readonly Uri BSP_CJ02_Source = GetBarProfilSection("RE_CJ02.png");
        public static readonly Uri BSP_CJ06_Source = GetBarProfilSection("RE_CJ06.png");
        public static readonly Uri BSP_CJ09_Source = GetBarProfilSection("RE_CJ09.png");
        public static readonly Uri BSP_CJ14_Source = GetBarProfilSection("RE_CJ14.png");
        public static readonly Uri BSP_CJ18_Source = GetBarProfilSection("RE_CJ18.png");
        public static readonly Uri BSP_CJ22_Source = GetBarProfilSection("RE_CJ22.png");
        public static readonly Uri BSP_CJ26_Source = GetBarProfilSection("RE_CJ26.png");
        public static readonly Uri BSP_CJ31_Source = GetBarProfilSection("RE_CJ31.png");
        public static readonly Uri BSP_CJ32_42_Source = GetBarProfilSection("RE_CJ32-42.png");
        public static readonly Uri BSP_CJ34_Source = GetBarProfilSection("RE_CJ34.png");
        public static readonly Uri BSP_CJ38_Source = GetBarProfilSection("RE_CJ38.png");
        public static readonly Uri BSP_CJ70_Source = GetBarProfilSection("RE_CJ70.png");
        public static readonly Uri BSP_CL140205_Source = GetBarProfilSection("RE_CL140205.png");
        public static readonly Uri BSP_CL140207_Source = GetBarProfilSection("RE_CL140207.png");
        public static readonly Uri BSP_CL140208_Source = GetBarProfilSection("RE_CL140208.png");
        public static readonly Uri BSP_CL140209_Source = GetBarProfilSection("RE_CL140209.png");
        public static readonly Uri BSP_CL140227_Source = GetBarProfilSection("RE_CL140227.png");
        public static readonly Uri BSP_CL140243_Source = GetBarProfilSection("RE_CL140243.png");
        public static readonly Uri BSP_CL140244_Source = GetBarProfilSection("RE_CL140244.png");
        public static readonly Uri BSP_CL229110_Source = GetBarProfilSection("RE_CL229110.png");
        public static readonly Uri BSP_CL229114_Source = GetBarProfilSection("RE_CL229114.png");
        public static readonly Uri BSP_CL259010_Source = GetBarProfilSection("RE_CL259010.png");
        public static readonly Uri BSP_CRM13_Source = GetBarProfilSection("RE_CRM13.png");
        public static readonly Uri BSP_CTA09_Source = GetBarProfilSection("RE_CTA09.png");
        public static readonly Uri BSP_CV114_Source = GetBarProfilSection("RE_CV114.png");
        public static readonly Uri BSP_CV13_35J09_Source = GetBarProfilSection("RE_CV13-35J09.png");
        public static readonly Uri BSP_CV13_46J09_Source = GetBarProfilSection("RE_CV13-46J09.png");
        public static readonly Uri BSP_CV13_46J13_Source = GetBarProfilSection("RE_CV13-46J13.png");
        public static readonly Uri BSP_CV13_46J13S_Source = GetBarProfilSection("RE_CV13-46J13S.png");
        public static readonly Uri BSP_CV13_55J09_Source = GetBarProfilSection("RE_CV13-55J09.png");
        public static readonly Uri BSP_CV13_55J13_Source = GetBarProfilSection("RE_CV13-55J13.png");
        public static readonly Uri BSP_CV13_60J09_Source = GetBarProfilSection("RE_CV13-60J09.png");
        public static readonly Uri BSP_CV13_60J13_Source = GetBarProfilSection("RE_CV13-60J13.png");
        public static readonly Uri BSP_CV13_66J09_Source = GetBarProfilSection("RE_CV13-66J09.png");
        public static readonly Uri BSP_CV13_66J13_Source = GetBarProfilSection("RE_CV13-66J13.png");
        public static readonly Uri BSP_CV23_35J09_Source = GetBarProfilSection("RE_CV23-35J09.png");
        public static readonly Uri BSP_CV23_46J09_Source = GetBarProfilSection("RE_CV23-46J09.png");
        public static readonly Uri BSP_CV23_46J13_Source = GetBarProfilSection("RE_CV23-46J13.png");
        public static readonly Uri BSP_CV23_55J09_Source = GetBarProfilSection("RE_CV23-55J09.png");
        public static readonly Uri BSP_CV23_55J13_Source = GetBarProfilSection("RE_CV23-55J13.png");
        public static readonly Uri BSP_CV23_66J09_Source = GetBarProfilSection("RE_CV23-66J09.png");
        public static readonly Uri BSP_CV23_66J13_Source = GetBarProfilSection("RE_CV23-66J13.png");
        public static readonly Uri BSP_DRF4_Source = GetBarProfilSection("RE_DRF4.png");
        public static readonly Uri BSP_DRF5_E_Source = GetBarProfilSection("RE_DRF5-E.png");
        public static readonly Uri BSP_EMCT1_Source = GetBarProfilSection("RE_EMCT1.png");
        public static readonly Uri BSP_EQ1016_Source = GetBarProfilSection("RE_EQ1016.png");
        public static readonly Uri BSP_EQ11_Source = GetBarProfilSection("RE_EQ11.png");
        public static readonly Uri BSP_EQ1104_Source = GetBarProfilSection("RE_EQ1104.png");
        public static readonly Uri BSP_EQ1413_Source = GetBarProfilSection("RE_EQ1413.png");
        public static readonly Uri BSP_EQ1915_Source = GetBarProfilSection("RE_EQ1915.png");
        public static readonly Uri BSP_EVLE184_Source = GetBarProfilSection("RE_EVLE184.png");
        public static readonly Uri BSP_FBL1265_K20_Source = GetBarProfilSection("RE_FBL1265-K20.png");
        public static readonly Uri BSP_FE58_Source = GetBarProfilSection("RE_FE58.png");
        public static readonly Uri BSP_GL28H_Source = GetBarProfilSection("RE_GL28H.png");
        public static readonly Uri BSP_GL28V_Source = GetBarProfilSection("RE_GL28V.png");
        public static readonly Uri BSP_HWS1_Source = GetBarProfilSection("RE_HWS1.png");
        public static readonly Uri BSP_I_7_20_Source = GetBarProfilSection("RE_I-7-20.png");
        public static readonly Uri BSP_I7C_Source = GetBarProfilSection("RE_I7C.png");
        public static readonly Uri BSP_IAL61_Source = GetBarProfilSection("RE_IAL61.png");
        public static readonly Uri BSP_IAMD116_Source = GetBarProfilSection("RE_IAMD116.png");
        public static readonly Uri BSP_IH3_76_Source = GetBarProfilSection("RE_IH3-76.png");
        public static readonly Uri BSP_IST_Source = GetBarProfilSection("RE_IST.png");
        public static readonly Uri BSP_JB15_Source = GetBarProfilSection("RE_JB15.png");
        public static readonly Uri BSP_JB84_11_Source = GetBarProfilSection("RE_JB84-11.png");
        public static readonly Uri BSP_JC174_Source = GetBarProfilSection("RE_JC174.png");
        public static readonly Uri BSP_JCD_Source = GetBarProfilSection("RE_JCD.png");
        public static readonly Uri BSP_JCV9_Source = GetBarProfilSection("RE_JCV9.png");
        public static readonly Uri BSP_JD84_Source = GetBarProfilSection("RE_JD84.png");
        public static readonly Uri BSP_JFI27_Source = GetBarProfilSection("RE_JFI27.png");
        public static readonly Uri BSP_JM2302_Source = GetBarProfilSection("RE_JM2302.png");
        public static readonly Uri BSP_JOINT_Source = GetBarProfilSection("RE_JOINT.png");
        public static readonly Uri BSP_JPA84_Source = GetBarProfilSection("RE_JPA84.png");
        public static readonly Uri BSP_JS17_Source = GetBarProfilSection("RE_JS17.png");
        public static readonly Uri BSP_K3_14_Source = GetBarProfilSection("RE_K3-14.png");
        public static readonly Uri BSP_K3_201_1_Source = GetBarProfilSection("RE_K3-201_1.png");
        public static readonly Uri BSP_K3_5_1_Source = GetBarProfilSection("RE_K3-5_1.png");
        public static readonly Uri BSP_K3_5_2_Source = GetBarProfilSection("RE_K3-5_2.png");
        public static readonly Uri BSP_K3_6_1_Source = GetBarProfilSection("RE_K3-6_1.png");
        public static readonly Uri BSP_K3_6_2_Source = GetBarProfilSection("RE_K3-6_2.png");
        public static readonly Uri BSP_K910105_Source = GetBarProfilSection("RE_K910105.png");
        public static readonly Uri BSP_K910150_Source = GetBarProfilSection("RE_K910150.png");
        public static readonly Uri BSP_K920343_Source = GetBarProfilSection("RE_K920343.png");
        public static readonly Uri BSP_K920344_Source = GetBarProfilSection("RE_K920344.png");
        public static readonly Uri BSP_K920345_Source = GetBarProfilSection("RE_K920345.png");
        public static readonly Uri BSP_K920346_Source = GetBarProfilSection("RE_K920346.png");
        public static readonly Uri BSP_K920348_Source = GetBarProfilSection("RE_K920348.png");
        public static readonly Uri BSP_K920351_Source = GetBarProfilSection("RE_K920351.png");
        public static readonly Uri BSP_KNF35_Source = GetBarProfilSection("RE_KNF35.png");
        public static readonly Uri BSP_KP176_Source = GetBarProfilSection("RE_KP176.png");
        public static readonly Uri BSP_KP484_Source = GetBarProfilSection("RE_KP484.png");
        public static readonly Uri BSP_M298_Source = GetBarProfilSection("RE_M298.png");
        public static readonly Uri BSP_MEA1_Source = GetBarProfilSection("RE_MEA1.png");
        public static readonly Uri BSP_MI55_Source = GetBarProfilSection("RE_MI55.png");
        public static readonly Uri BSP_MI81_Source = GetBarProfilSection("RE_MI81.png");
        public static readonly Uri BSP_MI81G_Source = GetBarProfilSection("RE_MI81G.png");
        public static readonly Uri BSP_MRX14_35_Source = GetBarProfilSection("RE_MRX14-35.png");
        public static readonly Uri BSP_MS30_12_Source = GetBarProfilSection("RE_MS30-12.png");
        public static readonly Uri BSP_MX_JC35_Source = GetBarProfilSection("RE_MX-JC35.png");
        public static readonly Uri BSP_MX_JCAL_Source = GetBarProfilSection("RE_MX-JCAL.png");
        public static readonly Uri BSP_MXR12_Source = GetBarProfilSection("RE_MXR12.png");
        public static readonly Uri BSP_MXR20_Source = GetBarProfilSection("RE_MXR20.png");
        public static readonly Uri BSP_MXR40_Source = GetBarProfilSection("RE_MXR40.png");
        public static readonly Uri BSP_N0211_Source = GetBarProfilSection("RE_N0211.png");
        public static readonly Uri BSP_NA3_Source = GetBarProfilSection("RE_NA3.png");
        public static readonly Uri BSP_NA30_Source = GetBarProfilSection("RE_NA30.png");
        public static readonly Uri BSP_NA32_71_Source = GetBarProfilSection("RE_NA32_71.png");
        public static readonly Uri BSP_NA32_84_Source = GetBarProfilSection("RE_NA32_84.png");
        public static readonly Uri BSP_NA6_V2_Source = GetBarProfilSection("RE_NA6-V2.png");
        public static readonly Uri BSP_NA6_Source = GetBarProfilSection("RE_NA6.png");
        public static readonly Uri BSP_NA6_V1_Source = GetBarProfilSection("RE_NA6_V1.png");
        public static readonly Uri BSP_NA874_Source = GetBarProfilSection("RE_NA874.png");
        public static readonly Uri BSP_NF_TA84_Source = GetBarProfilSection("RE_NF-TA84.png");
        public static readonly Uri BSP_NF_TA84d_Source = GetBarProfilSection("RE_NF-TA84d.png");
        public static readonly Uri BSP_NF30_70_Source = GetBarProfilSection("RE_NF30-70.png");
        public static readonly Uri BSP_NF30_76_Source = GetBarProfilSection("RE_NF30-76.png");
        public static readonly Uri BSP_NF3076_Source = GetBarProfilSection("RE_NF3076.png");
        public static readonly Uri BSP_NK3_Source = GetBarProfilSection("RE_NK3.png");
        public static readonly Uri BSP_NK3_V1_Source = GetBarProfilSection("RE_NK3_V1.png");
        public static readonly Uri BSP_NR7_Source = GetBarProfilSection("RE_NR7.png");
        public static readonly Uri BSP_NS28_Source = GetBarProfilSection("RE_NS28.png");
        public static readonly Uri BSP_P494203_Source = GetBarProfilSection("RE_P494203.png");
        public static readonly Uri BSP_P494204_Source = GetBarProfilSection("RE_P494204.png");
        public static readonly Uri BSP_P494233_Source = GetBarProfilSection("RE_P494233.png");
        public static readonly Uri BSP_P494235_Source = GetBarProfilSection("RE_P494235.png");
        public static readonly Uri BSP_P494300_Source = GetBarProfilSection("RE_P494300.png");
        public static readonly Uri BSP_P494517_Source = GetBarProfilSection("RE_P494517.png");
        public static readonly Uri BSP_P494518_Source = GetBarProfilSection("RE_P494518.png");
        public static readonly Uri BSP_P494519_Source = GetBarProfilSection("RE_P494519.png");
        public static readonly Uri BSP_P494520_Source = GetBarProfilSection("RE_P494520.png");
        public static readonly Uri BSP_P494521_Source = GetBarProfilSection("RE_P494521.png");
        public static readonly Uri BSP_P494522_Source = GetBarProfilSection("RE_P494522.png");
        public static readonly Uri BSP_P494523_Source = GetBarProfilSection("RE_P494523.png");
        public static readonly Uri BSP_P494525_Source = GetBarProfilSection("RE_P494525.png");
        public static readonly Uri BSP_P494526_Source = GetBarProfilSection("RE_P494526.png");
        public static readonly Uri BSP_P494531_Source = GetBarProfilSection("RE_P494531.png");
        public static readonly Uri BSP_P494532_Source = GetBarProfilSection("RE_P494532.png");
        public static readonly Uri BSP_P494533_Source = GetBarProfilSection("RE_P494533.png");
        public static readonly Uri BSP_P494534_Source = GetBarProfilSection("RE_P494534.png");
        public static readonly Uri BSP_P494535_Source = GetBarProfilSection("RE_P494535.png");
        public static readonly Uri BSP_P494536_Source = GetBarProfilSection("RE_P494536.png");
        public static readonly Uri BSP_P494537_Source = GetBarProfilSection("RE_P494537.png");
        public static readonly Uri BSP_P494539_Source = GetBarProfilSection("RE_P494539.png");
        public static readonly Uri BSP_P494540_Source = GetBarProfilSection("RE_P494540.png");
        public static readonly Uri BSP_P496171_Source = GetBarProfilSection("RE_P496171.png");
        public static readonly Uri BSP_P599170_Source = GetBarProfilSection("RE_P599170.png");
        public static readonly Uri BSP_P755969_Source = GetBarProfilSection("RE_P755969.png");
        public static readonly Uri BSP_P780561_Source = GetBarProfilSection("RE_P780561.png");
        public static readonly Uri BSP_P803706_Source = GetBarProfilSection("RE_P803706.png");
        public static readonly Uri BSP_P803707_Source = GetBarProfilSection("RE_P803707.png");
        public static readonly Uri BSP_P813650_Source = GetBarProfilSection("RE_P813650.png");
        public static readonly Uri BSP_P813730_Source = GetBarProfilSection("RE_P813730.png");
        public static readonly Uri BSP_P813760_Source = GetBarProfilSection("RE_P813760.png");
        public static readonly Uri BSP_P842607_Source = GetBarProfilSection("RE_P842607.png");
        public static readonly Uri BSP_PA63_Source = GetBarProfilSection("RE_PA63.png");
        public static readonly Uri BSP_PC1812_Source = GetBarProfilSection("RE_PC1812.png");
        public static readonly Uri BSP_PC1812V1_Source = GetBarProfilSection("RE_PC1812V1.png");
        public static readonly Uri BSP_PC1824_Source = GetBarProfilSection("RE_PC1824.png");
        public static readonly Uri BSP_PC1824B_Source = GetBarProfilSection("RE_PC1824B.png");
        public static readonly Uri BSP_PC1832_Source = GetBarProfilSection("RE_PC1832.png");
        public static readonly Uri BSP_PC1832v1_Source = GetBarProfilSection("RE_PC1832v1.png");
        public static readonly Uri BSP_PC2308_Source = GetBarProfilSection("RE_PC2308.png");
        public static readonly Uri BSP_PC2312_Source = GetBarProfilSection("RE_PC2312.png");
        public static readonly Uri BSP_PC2312V1_Source = GetBarProfilSection("RE_PC2312V1.png");
        public static readonly Uri BSP_PC2316_Source = GetBarProfilSection("RE_PC2316.png");
        public static readonly Uri BSP_PC2324_Source = GetBarProfilSection("RE_PC2324.png");
        public static readonly Uri BSP_PC2332_Source = GetBarProfilSection("RE_PC2332.png");
        public static readonly Uri BSP_PC80_Source = GetBarProfilSection("RE_PC80.png");
        public static readonly Uri BSP_PE160_Source = GetBarProfilSection("RE_PE160.png");
        public static readonly Uri BSP_PE710_Source = GetBarProfilSection("RE_PE710.png");
        public static readonly Uri BSP_PE725_Source = GetBarProfilSection("RE_PE725.png");
        public static readonly Uri BSP_PE760_Source = GetBarProfilSection("RE_PE760.png");
        public static readonly Uri BSP_PEM005_Source = GetBarProfilSection("RE_PEM005.png");
        public static readonly Uri BSP_PEM006_Source = GetBarProfilSection("RE_PEM006.png");
        public static readonly Uri BSP_PEM008_Source = GetBarProfilSection("RE_PEM008.png");
        public static readonly Uri BSP_PEM011_Source = GetBarProfilSection("RE_PEM011.png");
        public static readonly Uri BSP_PFK1_Source = GetBarProfilSection("RE_PFK1.png");
        public static readonly Uri BSP_PG1824_Source = GetBarProfilSection("RE_PG1824.png");
        public static readonly Uri BSP_PG1832_Source = GetBarProfilSection("RE_PG1832.png");
        public static readonly Uri BSP_PG2324_Source = GetBarProfilSection("RE_PG2324.png");
        public static readonly Uri BSP_PG2332_Source = GetBarProfilSection("RE_PG2332.png");
        public static readonly Uri BSP_PH36_Source = GetBarProfilSection("RE_PH36.png");
        public static readonly Uri BSP_PH40_Source = GetBarProfilSection("RE_PH40.png");
        public static readonly Uri BSP_PIL200_Source = GetBarProfilSection("RE_PIL200.png");
        public static readonly Uri BSP_POC_J09_Source = GetBarProfilSection("RE_POC-J09.png");
        public static readonly Uri BSP_PPDT_Source = GetBarProfilSection("RE_PPDT.png");
        public static readonly Uri BSP_PTA80_Source = GetBarProfilSection("RE_PTA80.png");
        public static readonly Uri BSP_PTBF2385_2_Source = GetBarProfilSection("RE_PTBF2385-2.png");
        public static readonly Uri BSP_PV36_Source = GetBarProfilSection("RE_PV36.png");
        public static readonly Uri BSP_PV40_Source = GetBarProfilSection("RE_PV40.png");
        public static readonly Uri BSP_RAS1_Source = GetBarProfilSection("RE_RAS1.png");
        public static readonly Uri BSP_RAS2_Source = GetBarProfilSection("RE_RAS2.png");
        public static readonly Uri BSP_RJ70_Source = GetBarProfilSection("RE_RJ70.png");
        public static readonly Uri BSP_RKD2_Source = GetBarProfilSection("RE_RKD2.png");
        public static readonly Uri BSP_RKD9_Source = GetBarProfilSection("RE_RKD9.png");
        public static readonly Uri BSP_SA_T70_Source = GetBarProfilSection("RE_SA-T70.png");
        public static readonly Uri BSP_SA_T70S_Source = GetBarProfilSection("RE_SA-T70S.png");
        public static readonly Uri BSP_SNT74_Source = GetBarProfilSection("RE_SNT74.png");
        public static readonly Uri BSP_SNT84_Source = GetBarProfilSection("RE_SNT84.png");
        public static readonly Uri BSP_SNTA84_Source = GetBarProfilSection("RE_SNTA84.png");
        public static readonly Uri BSP_SRJK75_Source = GetBarProfilSection("RE_SRJK75.png");
        public static readonly Uri BSP_SRT70_Source = GetBarProfilSection("RE_SRT70.png");
        public static readonly Uri BSP_ST20_Source = GetBarProfilSection("RE_ST20.png");
        public static readonly Uri BSP_ST5125_Source = GetBarProfilSection("RE_ST5125.png");
        public static readonly Uri BSP_ST5137_Source = GetBarProfilSection("RE_ST5137.png");
        public static readonly Uri BSP_ST5400_Source = GetBarProfilSection("RE_ST5400.png");
        public static readonly Uri BSP_ST5402D_Source = GetBarProfilSection("RE_ST5402D.png");
        public static readonly Uri BSP_ST5404_Source = GetBarProfilSection("RE_ST5404.png");
        public static readonly Uri BSP_ST5404D_Source = GetBarProfilSection("RE_ST5404D.png");
        public static readonly Uri BSP_STP6090_Source = GetBarProfilSection("RE_STP6090.png");
        public static readonly Uri BSP_TA811_Source = GetBarProfilSection("RE_TA811.png");
        public static readonly Uri BSP_TA812_Source = GetBarProfilSection("RE_TA812.png");
        public static readonly Uri BSP_TA813_Source = GetBarProfilSection("RE_TA813.png");
        public static readonly Uri BSP_TA815_Source = GetBarProfilSection("RE_TA815.png");
        public static readonly Uri BSP_TA818_Source = GetBarProfilSection("RE_TA818.png");
        public static readonly Uri BSP_TA820_Source = GetBarProfilSection("RE_TA820.png");
        public static readonly Uri BSP_TA822_Source = GetBarProfilSection("RE_TA822.png");
        public static readonly Uri BSP_TA84oc001_Source = GetBarProfilSection("RE_TA84oc001.png");
        public static readonly Uri BSP_TA84oc002_Source = GetBarProfilSection("RE_TA84oc002.png");
        public static readonly Uri BSP_TA84oc003_Source = GetBarProfilSection("RE_TA84oc003.png");
        public static readonly Uri BSP_TA84oc004_Source = GetBarProfilSection("RE_TA84oc004.png");
        public static readonly Uri BSP_TA84ov001_Source = GetBarProfilSection("RE_TA84ov001.png");
        public static readonly Uri BSP_TA84ov002_Source = GetBarProfilSection("RE_TA84ov002.png");
        public static readonly Uri BSP_TA84ov003_Source = GetBarProfilSection("RE_TA84ov003.png");
        public static readonly Uri BSP_TA84ov004_Source = GetBarProfilSection("RE_TA84ov004.png");
        public static readonly Uri BSP_TA84_75_001_Source = GetBarProfilSection("RE_TA84_75_001.png");
        public static readonly Uri BSP_TA870_Source = GetBarProfilSection("RE_TA870.png");
        public static readonly Uri BSP_TA875_Source = GetBarProfilSection("RE_TA875.png");
        public static readonly Uri BSP_TA890_Source = GetBarProfilSection("RE_TA890.png");
        public static readonly Uri BSP_TA892_Source = GetBarProfilSection("RE_TA892.png");
        public static readonly Uri BSP_TA893_Source = GetBarProfilSection("RE_TA893.png");
        public static readonly Uri BSP_TD40_Source = GetBarProfilSection("RE_TD40.png");
        public static readonly Uri BSP_TP126_Source = GetBarProfilSection("RE_TP126.png");
        public static readonly Uri BSP_VN_CVA23_Source = GetBarProfilSection("RE_VN-CVA23.png");
        public static readonly Uri BSP_VN_JC_Source = GetBarProfilSection("RE_VN-JC.png");
        public static readonly Uri BSP_VN_JC09_Source = GetBarProfilSection("RE_VN-JC09.png");
        public static readonly Uri BSP_VN_JC13_Source = GetBarProfilSection("RE_VN-JC13.png");
        public static readonly Uri BSP_VRX11_58_Source = GetBarProfilSection("RE_VRX11-58.png");
        public static readonly Uri BSP_VRX13_Source = GetBarProfilSection("RE_VRX13.png");
        public static readonly Uri BSP_VRX14_35_Source = GetBarProfilSection("RE_VRX14-35.png");
        public static readonly Uri BSP_VX10_35_70_Source = GetBarProfilSection("RE_VX10-35-70.png");
        public static readonly Uri BSP_VX10_35_Source = GetBarProfilSection("RE_VX10-35.png");
        public static readonly Uri BSP_VX10_46_Source = GetBarProfilSection("RE_VX10-46.png");
        public static readonly Uri BSP_VX10_55_70_Source = GetBarProfilSection("RE_VX10-55-70.png");
        public static readonly Uri BSP_VX10_66_Source = GetBarProfilSection("RE_VX10-66.png");
        public static readonly Uri BSP_VX15_Source = GetBarProfilSection("RE_VX15.png");
        public static readonly Uri BSP_VX59_Source = GetBarProfilSection("RE_VX59.png");
        public static readonly Uri BSP_WS3625_Source = GetBarProfilSection("RE_WS3625.png");
        public static readonly Uri BSP_WS4125_Source = GetBarProfilSection("RE_WS4125.png");
        public static readonly Uri BSP_XP3_Source = GetBarProfilSection("RE_XP3.png");
        public static readonly Uri BSP_Z599196_Source = GetBarProfilSection("RE_Z599196.png");
        public static readonly Uri BSP_Z902161_Source = GetBarProfilSection("RE_Z902161.png");
        public static readonly Uri BSP_Z914262_Source = GetBarProfilSection("RE_Z914262.png");
        public static readonly Uri BSP_Z918265_Source = GetBarProfilSection("RE_Z918265.png");
        public static readonly Uri BSP_Z921438_Source = GetBarProfilSection("RE_Z921438.png");

        /// <summary>
        /// Dictionnaire associant une référence textuelle de section de profil de barre à l'URI correspondante.
        /// </summary>
        public static readonly IReadOnlyDictionary<string, Uri> ReferenceBSP =
            new Dictionary<string, Uri>
            {
                { "02CJ", BSP_02CJ_Source },
                { "10200", BSP_10200_Source },
                { "10201", BSP_10201_Source },
                { "10202", BSP_10202_Source },
                { "10203", BSP_10203_Source },
                { "10204", BSP_10204_Source },
                { "10205", BSP_10205_Source },
                { "10206", BSP_10206_Source },
                { "10211", BSP_10211_Source },
                { "10213", BSP_10213_Source },
                { "10214", BSP_10214_Source },
                { "10215", BSP_10215_Source },
                { "10250", BSP_10250_Source },
                { "10252", BSP_10252_Source },
                { "10253", BSP_10253_Source },
                { "10254", BSP_10254_Source },
                { "10260", BSP_10260_Source },
                { "10302", BSP_10302_Source },
                { "10303", BSP_10303_Source },
                { "10304", BSP_10304_Source },
                { "10305", BSP_10305_Source },
                { "10306", BSP_10306_Source },
                { "10309", BSP_10309_Source },
                { "11034", BSP_11034_Source },
                { "11035", BSP_11035_Source },
                { "11660", BSP_11660_Source },
                { "12100", BSP_12100_Source },
                { "12101", BSP_12101_Source },
                { "12102", BSP_12102_Source },
                { "12103", BSP_12103_Source },
                { "12104", BSP_12104_Source },
                { "12105", BSP_12105_Source },
                { "12106", BSP_12106_Source },
                { "12107", BSP_12107_Source },
                { "12108", BSP_12108_Source },
                { "12109", BSP_12109_Source },
                { "12110", BSP_12110_Source },
                { "12111", BSP_12111_Source },
                { "12112", BSP_12112_Source },
                { "12113", BSP_12113_Source },
                { "12114", BSP_12114_Source },
                { "12115", BSP_12115_Source },
                { "12116", BSP_12116_Source },
                { "12117", BSP_12117_Source },
                { "12118", BSP_12118_Source },
                { "12119", BSP_12119_Source },
                { "12120", BSP_12120_Source },
                { "12121", BSP_12121_Source },
                { "12122", BSP_12122_Source },
                { "12123", BSP_12123_Source },
                { "12124", BSP_12124_Source },
                { "12125", BSP_12125_Source },
                { "12126", BSP_12126_Source },
                { "12127", BSP_12127_Source },
                { "12128", BSP_12128_Source },
                { "12129", BSP_12129_Source },
                { "12130", BSP_12130_Source },
                { "12131", BSP_12131_Source },
                { "12132", BSP_12132_Source },
                { "12133", BSP_12133_Source },
                { "12134", BSP_12134_Source },
                { "12135", BSP_12135_Source },
                { "12136", BSP_12136_Source },
                { "12139", BSP_12139_Source },
                { "12157", BSP_12157_Source },
                { "14010", BSP_14010_Source },
                { "14347", BSP_14347_Source },
                { "14403", BSP_14403_Source },
                { "14591", BSP_14591_Source },
                { "14592", BSP_14592_Source },
                { "14594", BSP_14594_Source },
                { "14711", BSP_14711_Source },
                { "15419", BSP_15419_Source },
                { "15441", BSP_15441_Source },
                { "15442", BSP_15442_Source },
                { "15491", BSP_15491_Source },
                { "15492", BSP_15492_Source },
                { "15511", BSP_15511_Source },
                { "15512", BSP_15512_Source },
                { "15600", BSP_15600_Source },
                { "15602", BSP_15602_Source },
                { "15712", BSP_15712_Source },
                { "15750", BSP_15750_Source },
                { "15760", BSP_15760_Source },
                { "15837", BSP_15837_Source },
                { "15850", BSP_15850_Source },
                { "16969", BSP_16969_Source },
                { "16978", BSP_16978_Source },
                { "16990", BSP_16990_Source },
                { "16997", BSP_16997_Source },
                { "16998", BSP_16998_Source },
                { "17045", BSP_17045_Source },
                { "17051", BSP_17051_Source },
                { "17088", BSP_17088_Source },
                { "17131", BSP_17131_Source },
                { "17133", BSP_17133_Source },
                { "17208", BSP_17208_Source },
                { "2020", BSP_2020_Source },
                { "214700", BSP_214700_Source },
                { "2290", BSP_2290_Source },
                { "2402", BSP_2402_Source },
                { "26004", BSP_26004_Source },
                { "26005", BSP_26005_Source },
                { "26217", BSP_26217_Source },
                { "26218", BSP_26218_Source },
                { "26219", BSP_26219_Source },
                { "26221", BSP_26221_Source },
                { "26222", BSP_26222_Source },
                { "26223", BSP_26223_Source },
                { "26224", BSP_26224_Source },
                { "26225", BSP_26225_Source },
                { "26226", BSP_26226_Source },
                { "26227", BSP_26227_Source },
                { "26318", BSP_26318_Source },
                { "26500", BSP_26500_Source },
                { "26501", BSP_26501_Source },
                { "26502", BSP_26502_Source },
                { "26504", BSP_26504_Source },
                { "26506", BSP_26506_Source },
                { "26511", BSP_26511_Source },
                { "26512", BSP_26512_Source },
                { "26701", BSP_26701_Source },
                { "26702", BSP_26702_Source },
                { "26703", BSP_26703_Source },
                { "26704", BSP_26704_Source },
                { "26705", BSP_26705_Source },
                { "26720", BSP_26720_Source },
                { "26721", BSP_26721_Source },
                { "26722", BSP_26722_Source },
                { "26723", BSP_26723_Source },
                { "26724", BSP_26724_Source },
                { "26725", BSP_26725_Source },
                { "26726", BSP_26726_Source },
                { "26750", BSP_26750_Source },
                { "268400", BSP_268400_Source },
                { "277400", BSP_277400_Source },
                { "30092", BSP_30092_Source },
                { "30093", BSP_30093_Source },
                { "30094", BSP_30094_Source },
                { "303700", BSP_303700_Source },
                { "30604", BSP_30604_Source },
                { "30606", BSP_30606_Source },
                { "30608", BSP_30608_Source },
                { "30610", BSP_30610_Source },
                { "30612", BSP_30612_Source },
                { "30704", BSP_30704_Source },
                { "30706", BSP_30706_Source },
                { "30710", BSP_30710_Source },
                { "30723", BSP_30723_Source },
                { "30724", BSP_30724_Source },
                { "30725", BSP_30725_Source },
                { "30727", BSP_30727_Source },
                { "30728", BSP_30728_Source },
                { "30729", BSP_30729_Source },
                { "30801", BSP_30801_Source },
                { "30802", BSP_30802_Source },
                { "30806", BSP_30806_Source },
                { "30810", BSP_30810_Source },
                { "30811", BSP_30811_Source },
                { "30812", BSP_30812_Source },
                { "30813", BSP_30813_Source },
                { "30814", BSP_30814_Source },
                { "30818", BSP_30818_Source },
                { "32078", BSP_32078_Source },
                { "328100", BSP_328100_Source },
                { "3297", BSP_3297_Source },
                { "33024", BSP_33024_Source },
                { "3312", BSP_3312_Source },
                { "33124", BSP_33124_Source },
                { "3331", BSP_3331_Source },
                { "3333", BSP_3333_Source },
                { "3334", BSP_3334_Source },
                { "3335", BSP_3335_Source },
                { "3340", BSP_3340_Source },
                { "3341", BSP_3341_Source },
                { "3364", BSP_3364_Source },
                { "3700", BSP_3700_Source },
                { "3701", BSP_3701_Source },
                { "371800", BSP_371800_Source },
                { "4294", BSP_4294_Source },
                { "4300", BSP_4300_Source },
                { "4301", BSP_4301_Source },
                { "4302", BSP_4302_Source },
                { "4303", BSP_4303_Source },
                { "4304", BSP_4304_Source },
                { "4305", BSP_4305_Source },
                { "4306", BSP_4306_Source },
                { "4307", BSP_4307_Source },
                { "4308", BSP_4308_Source },
                { "4390", BSP_4390_Source },
                { "477200", BSP_477200_Source },
                { "509900", BSP_509900_Source },
                { "5103", BSP_5103_Source },
                { "5103JNA", BSP_5103JNA_Source },
                { "5103SJ", BSP_5103SJ_Source },
                { "5107", BSP_5107_Source },
                { "5114", BSP_5114_Source },
                { "5114JNA", BSP_5114JNA_Source },
                { "5115", BSP_5115_Source },
                { "5116", BSP_5116_Source },
                { "5120", BSP_5120_Source },
                { "5120JNA", BSP_5120JNA_Source },
                { "5120SJ", BSP_5120SJ_Source },
                { "5120_SJ", BSP_5120_SJ_Source },
                { "5125", BSP_5125_Source },
                { "5125JNA", BSP_5125JNA_Source },
                { "5125SJ", BSP_5125SJ_Source },
                { "5126", BSP_5126_Source },
                { "51300", BSP_51300_Source },
                { "5137", BSP_5137_Source },
                { "5137JNA", BSP_5137JNA_Source },
                { "51400", BSP_51400_Source },
                { "51500", BSP_51500_Source },
                { "5163", BSP_5163_Source },
                { "5177", BSP_5177_Source },
                { "5177JNA", BSP_5177JNA_Source },
                { "5179", BSP_5179_Source },
                { "5180", BSP_5180_Source },
                { "5181", BSP_5181_Source },
                { "5183", BSP_5183_Source },
                { "5185", BSP_5185_Source },
                { "5189", BSP_5189_Source },
                { "5203", BSP_5203_Source },
                { "5208", BSP_5208_Source },
                { "52110", BSP_52110_Source },
                { "5215", BSP_5215_Source },
                { "52220", BSP_52220_Source },
                { "5227", BSP_5227_Source },
                { "52320", BSP_52320_Source },
                { "5263", BSP_5263_Source },
                { "5300", BSP_5300_Source },
                { "5301", BSP_5301_Source },
                { "5302", BSP_5302_Source },
                { "5303", BSP_5303_Source },
                { "5304", BSP_5304_Source },
                { "5307", BSP_5307_Source },
                { "5334", BSP_5334_Source },
                { "5386", BSP_5386_Source },
                { "5387", BSP_5387_Source },
                { "5388", BSP_5388_Source },
                { "5389", BSP_5389_Source },
                { "5390", BSP_5390_Source },
                { "5400", BSP_5400_Source },
                { "5401", BSP_5401_Source },
                { "5402", BSP_5402_Source },
                { "5403", BSP_5403_Source },
                { "5404", BSP_5404_Source },
                { "5405", BSP_5405_Source },
                { "5407", BSP_5407_Source },
                { "5408", BSP_5408_Source },
                { "5409", BSP_5409_Source },
                { "5410", BSP_5410_Source },
                { "5412", BSP_5412_Source },
                { "5415", BSP_5415_Source },
                { "5416", BSP_5416_Source },
                { "5745", BSP_5745_Source },
                { "60140", BSP_60140_Source },
                { "60210", BSP_60210_Source },
                { "60250", BSP_60250_Source },
                { "60310", BSP_60310_Source },
                { "604006", BSP_604006_Source },
                { "60410", BSP_60410_Source },
                { "60510", BSP_60510_Source },
                { "61015", BSP_61015_Source },
                { "63120", BSP_63120_Source },
                { "63170", BSP_63170_Source },
                { "63220", BSP_63220_Source },
                { "63370", BSP_63370_Source },
                { "63510", BSP_63510_Source },
                { "63610", BSP_63610_Source },
                { "6370", BSP_6370_Source },
                { "63700", BSP_63700_Source },
                { "6372", BSP_6372_Source },
                { "6373", BSP_6373_Source },
                { "6374", BSP_6374_Source },
                { "63850", BSP_63850_Source },
                { "6980", BSP_6980_Source },
                { "6981", BSP_6981_Source },
                { "6982", BSP_6982_Source },
                { "6983", BSP_6983_Source },
                { "70200", BSP_70200_Source },
                { "70400", BSP_70400_Source },
                { "70502", BSP_70502_Source },
                { "70503", BSP_70503_Source },
                { "7114", BSP_7114_Source },
                { "71200", BSP_71200_Source },
                { "7125", BSP_7125_Source },
                { "71300", BSP_71300_Source },
                { "71301", BSP_71301_Source },
                { "7137", BSP_7137_Source },
                { "726124", BSP_726124_Source },
                { "73020", BSP_73020_Source },
                { "73030", BSP_73030_Source },
                { "7308", BSP_7308_Source },
                { "7308JNA", BSP_7308JNA_Source },
                { "7309", BSP_7309_Source },
                { "7309JNA", BSP_7309JNA_Source },
                { "73200", BSP_73200_Source },
                { "7324", BSP_7324_Source },
                { "7324JNA", BSP_7324JNA_Source },
                { "7502", BSP_7502_Source },
                { "75030", BSP_75030_Source },
                { "7508", BSP_7508_Source },
                { "7524", BSP_7524_Source },
                { "7531", BSP_7531_Source },
                { "76027", BSP_76027_Source },
                { "76029", BSP_76029_Source },
                { "76070H", BSP_76070H_Source },
                { "76070L", BSP_76070L_Source },
                { "76071", BSP_76071_Source },
                { "76074B", BSP_76074B_Source },
                { "76074H", BSP_76074H_Source },
                { "76075B", BSP_76075B_Source },
                { "76075LH", BSP_76075LH_Source },
                { "76076", BSP_76076_Source },
                { "76077", BSP_76077_Source },
                { "76078B", BSP_76078B_Source },
                { "76078LH", BSP_76078LH_Source },
                { "76079B", BSP_76079B_Source },
                { "76079H", BSP_76079H_Source },
                { "76100H", BSP_76100H_Source },
                { "76100L", BSP_76100L_Source },
                { "76120b", BSP_76120b_Source },
                { "76120H", BSP_76120H_Source },
                { "76120HVR", BSP_76120HVR_Source },
                { "76120L", BSP_76120L_Source },
                { "76130B", BSP_76130B_Source },
                { "76130LH", BSP_76130LH_Source },
                { "76140H", BSP_76140H_Source },
                { "76140HVR", BSP_76140HVR_Source },
                { "76140L", BSP_76140L_Source },
                { "76160B", BSP_76160B_Source },
                { "76160H", BSP_76160H_Source },
                { "76160L", BSP_76160L_Source },
                { "76303", BSP_76303_Source },
                { "76304", BSP_76304_Source },
                { "76308", BSP_76308_Source },
                { "76309", BSP_76309_Source },
                { "76310", BSP_76310_Source },
                { "76313", BSP_76313_Source },
                { "76314", BSP_76314_Source },
                { "76315", BSP_76315_Source },
                { "76316", BSP_76316_Source },
                { "76318", BSP_76318_Source },
                { "76321", BSP_76321_Source },
                { "76322", BSP_76322_Source },
                { "76323", BSP_76323_Source },
                { "76324", BSP_76324_Source },
                { "773201", BSP_773201_Source },
                { "774005", BSP_774005_Source },
                { "776128", BSP_776128_Source },
                { "776132", BSP_776132_Source },
                { "776312N", BSP_776312N_Source },
                { "78030", BSP_78030_Source },
                { "815050", BSP_815050_Source },
                { "836", BSP_836_Source },
                { "84CA002", BSP_84CA002_Source },
                { "84CP010", BSP_84CP010_Source },
                { "84CP011", BSP_84CP011_Source },
                { "84CP104", BSP_84CP104_Source },
                { "84CP105", BSP_84CP105_Source },
                { "84CP106", BSP_84CP106_Source },
                { "84CP107", BSP_84CP107_Source },
                { "84CP108", BSP_84CP108_Source },
                { "84CP109", BSP_84CP109_Source },
                { "84CP201", BSP_84CP201_Source },
                { "84CP202", BSP_84CP202_Source },
                { "84CP203", BSP_84CP203_Source },
                { "84CP204", BSP_84CP204_Source },
                { "84VA001", BSP_84VA001_Source },
                { "84VA016", BSP_84VA016_Source },
                { "84VA018", BSP_84VA018_Source },
                { "84VA022", BSP_84VA022_Source },
                { "84VA025", BSP_84VA025_Source },
                { "84VA026", BSP_84VA026_Source },
                { "84VA029", BSP_84VA029_Source },
                { "84VB101", BSP_84VB101_Source },
                { "84VB201", BSP_84VB201_Source },
                { "84VI206", BSP_84VI206_Source },
                { "84VP010", BSP_84VP010_Source },
                { "84VP012", BSP_84VP012_Source },
                { "84VP023", BSP_84VP023_Source },
                { "84VP033", BSP_84VP033_Source },
                { "84VP041", BSP_84VP041_Source },
                { "84VP042", BSP_84VP042_Source },
                { "84VP043", BSP_84VP043_Source },
                { "84VP101", BSP_84VP101_Source },
                { "84VP102", BSP_84VP102_Source },
                { "84VP103", BSP_84VP103_Source },
                { "84VP104", BSP_84VP104_Source },
                { "84VP105", BSP_84VP105_Source },
                { "84VP106", BSP_84VP106_Source },
                { "84VP107", BSP_84VP107_Source },
                { "84VP108", BSP_84VP108_Source },
                { "84VP109", BSP_84VP109_Source },
                { "84VP201", BSP_84VP201_Source },
                { "84VP202", BSP_84VP202_Source },
                { "84VP203", BSP_84VP203_Source },
                { "84VP204", BSP_84VP204_Source },
                { "84VP310", BSP_84VP310_Source },
                { "84VP311", BSP_84VP311_Source },
                { "84VP312", BSP_84VP312_Source },
                { "84VP313", BSP_84VP313_Source },
                { "84VP314", BSP_84VP314_Source },
                { "85303", BSP_85303_Source },
                { "85304", BSP_85304_Source },
                { "85305", BSP_85305_Source },
                { "86300", BSP_86300_Source },
                { "956059", BSP_956059_Source },
                { "983400", BSP_983400_Source },
                { "986200", BSP_986200_Source },
                { "A-BNF84", BSP_A_BNF84_Source },
                { "A-CV135", BSP_A_CV135_Source },
                { "A-CV146", BSP_A_CV146_Source },
                { "A-CV155", BSP_A_CV155_Source },
                { "A-CV166", BSP_A_CV166_Source },
                { "A-CV235", BSP_A_CV235_Source },
                { "A-CV246", BSP_A_CV246_Source },
                { "A-CV255", BSP_A_CV255_Source },
                { "A-CV266", BSP_A_CV266_Source },
                { "A-L56", BSP_A_L56_Source },
                { "A-L61", BSP_A_L61_Source },
                { "A-L72-115", BSP_A_L72_115_Source },
                { "A-L72-135", BSP_A_L72_135_Source },
                { "A-L72-155", BSP_A_L72_155_Source },
                { "A-L72-95", BSP_A_L72_95_Source },
                { "A-L75", BSP_A_L75_Source },
                { "A-L80", BSP_A_L80_Source },
                { "A-L91-115", BSP_A_L91_115_Source },
                { "A-l91-135", BSP_A_l91_135_Source },
                { "A-l91-155", BSP_A_l91_155_Source },
                { "A-L91-95", BSP_A_L91_95_Source },
                { "A-LZ101", BSP_A_LZ101_Source },
                { "A-LZ120", BSP_A_LZ120_Source },
                { "A-MB68", BSP_A_MB68_Source },
                { "A-MB89", BSP_A_MB89_Source },
                { "A-MD116", BSP_A_MD116_Source },
                { "A-MD127", BSP_A_MD127_Source },
                { "A-MD154", BSP_A_MD154_Source },
                { "A-MD89", BSP_A_MD89_Source },
                { "A-ME84", BSP_A_ME84_Source },
                { "A-MI50", BSP_A_MI50_Source },
                { "A-P28", BSP_A_P28_Source },
                { "A-P32", BSP_A_P32_Source },
                { "A-P44", BSP_A_P44_Source },
                { "A-P46", BSP_A_P46_Source },
                { "A-PF28", BSP_A_PF28_Source },
                { "A-PF32", BSP_A_PF32_Source },
                { "A-PF40", BSP_A_PF40_Source },
                { "A-PL81", BSP_A_PL81_Source },
                { "A-T35", BSP_A_T35_Source },
                { "A-T46", BSP_A_T46_Source },
                { "A-T55", BSP_A_T55_Source },
                { "A-T66", BSP_A_T66_Source },
                { "A-T86", BSP_A_T86_Source },
                { "A-TCV40", BSP_A_TCV40_Source },
                { "A-Z60-26", BSP_A_Z60_26_Source },
                { "A-Z60-30", BSP_A_Z60_30_Source },
                { "A-Z70", BSP_A_Z70_Source },
                { "A-Z71", BSP_A_Z71_Source },
                { "A-ZM40-26", BSP_A_ZM40_26_Source },
                { "A-ZM40-30", BSP_A_ZM40_30_Source },
                { "A-ZM40-32", BSP_A_ZM40_32_Source },
                { "A-ZM78", BSP_A_ZM78_Source },
                { "A85A012", BSP_A85A012_Source },
                { "A85B201", BSP_A85B201_Source },
                { "A85J101", BSP_A85J101_Source },
                { "A85J304", BSP_A85J304_Source },
                { "A85J306", BSP_A85J306_Source },
                { "A85P101", BSP_A85P101_Source },
                { "A85P102", BSP_A85P102_Source },
                { "A85P103", BSP_A85P103_Source },
                { "A85P104", BSP_A85P104_Source },
                { "A85P105", BSP_A85P105_Source },
                { "A85P201", BSP_A85P201_Source },
                { "A85P202", BSP_A85P202_Source },
                { "A85P203", BSP_A85P203_Source },
                { "A85P301", BSP_A85P301_Source },
                { "A85P302", BSP_A85P302_Source },
                { "A85P303", BSP_A85P303_Source },
                { "A85P304", BSP_A85P304_Source },
                { "A85P310", BSP_A85P310_Source },
                { "A85P311", BSP_A85P311_Source },
                { "A85P312", BSP_A85P312_Source },
                { "A85P313", BSP_A85P313_Source },
                { "A85P314", BSP_A85P314_Source },
                { "A85P315", BSP_A85P315_Source },
                { "A85P316", BSP_A85P316_Source },
                { "A85P317", BSP_A85P317_Source },
                { "A85P318", BSP_A85P318_Source },
                { "A85P321", BSP_A85P321_Source },
                { "ACT35", BSP_ACT35_Source },
                { "ACT46", BSP_ACT46_Source },
                { "ACT55", BSP_ACT55_Source },
                { "ACT66", BSP_ACT66_Source },
                { "AD10111", BSP_AD10111_Source },
                { "AD10112", BSP_AD10112_Source },
                { "AD10113", BSP_AD10113_Source },
                { "AD10114", BSP_AD10114_Source },
                { "AD10115", BSP_AD10115_Source },
                { "AD10116", BSP_AD10116_Source },
                { "AD10117", BSP_AD10117_Source },
                { "AD10224", BSP_AD10224_Source },
                { "AD10225", BSP_AD10225_Source },
                { "AD10226", BSP_AD10226_Source },
                { "AD10227", BSP_AD10227_Source },
                { "AD10228", BSP_AD10228_Source },
                { "AD10356", BSP_AD10356_Source },
                { "AD10366", BSP_AD10366_Source },
                { "AD10369", BSP_AD10369_Source },
                { "AD10375", BSP_AD10375_Source },
                { "AD10376", BSP_AD10376_Source },
                { "AD10385", BSP_AD10385_Source },
                { "AD10617", BSP_AD10617_Source },
                { "AD10618", BSP_AD10618_Source },
                { "AD10619", BSP_AD10619_Source },
                { "AD10620", BSP_AD10620_Source },
                { "AD10621", BSP_AD10621_Source },
                { "AD10623", BSP_AD10623_Source },
                { "AD10626", BSP_AD10626_Source },
                { "AD10627", BSP_AD10627_Source },
                { "AD10631", BSP_AD10631_Source },
                { "AD10632", BSP_AD10632_Source },
                { "AD10633", BSP_AD10633_Source },
                { "AD10634", BSP_AD10634_Source },
                { "AD10635", BSP_AD10635_Source },
                { "AD10636", BSP_AD10636_Source },
                { "AD10637", BSP_AD10637_Source },
                { "AD10638", BSP_AD10638_Source },
                { "AD10639", BSP_AD10639_Source },
                { "AD10640", BSP_AD10640_Source },
                { "AD10641", BSP_AD10641_Source },
                { "AD10642", BSP_AD10642_Source },
                { "AD10643", BSP_AD10643_Source },
                { "AD10644", BSP_AD10644_Source },
                { "AD10647", BSP_AD10647_Source },
                { "AD10648", BSP_AD10648_Source },
                { "AD10649", BSP_AD10649_Source },
                { "AD10650", BSP_AD10650_Source },
                { "AD10651", BSP_AD10651_Source },
                { "AD10652", BSP_AD10652_Source },
                { "AD10653", BSP_AD10653_Source },
                { "AD10715", BSP_AD10715_Source },
                { "AD10716", BSP_AD10716_Source },
                { "AD10717", BSP_AD10717_Source },
                { "AD10719", BSP_AD10719_Source },
                { "AD10720", BSP_AD10720_Source },
                { "AD10721", BSP_AD10721_Source },
                { "AD10723", BSP_AD10723_Source },
                { "AD10724", BSP_AD10724_Source },
                { "AD30108", BSP_AD30108_Source },
                { "AD40117-V2", BSP_AD40117_V2_Source },
                { "AH-L117", BSP_AH_L117_Source },
                { "AH-L70", BSP_AH_L70_Source },
                { "AH-LZ102", BSP_AH_LZ102_Source },
                { "AH-MB70", BSP_AH_MB70_Source },
                { "AH-MD94", BSP_AH_MD94_Source },
                { "AH-P275", BSP_AH_P275_Source },
                { "AH-P301", BSP_AH_P301_Source },
                { "AH-PO40C", BSP_AH_PO40C_Source },
                { "AH-Z102", BSP_AH_Z102_Source },
                { "AH-Z115", BSP_AH_Z115_Source },
                { "AH-Z116", BSP_AH_Z116_Source },
                { "AH-Z125", BSP_AH_Z125_Source },
                { "AH-ZM102", BSP_AH_ZM102_Source },
                { "AH-ZM115", BSP_AH_ZM115_Source },
                { "AH-ZM116", BSP_AH_ZM116_Source },
                { "AK10100", BSP_AK10100_Source },
                { "AK10101", BSP_AK10101_Source },
                { "AK10102", BSP_AK10102_Source },
                { "AK10103", BSP_AK10103_Source },
                { "AK10104", BSP_AK10104_Source },
                { "AK10105", BSP_AK10105_Source },
                { "AK10106", BSP_AK10106_Source },
                { "AK10107", BSP_AK10107_Source },
                { "AK10108", BSP_AK10108_Source },
                { "AK10109", BSP_AK10109_Source },
                { "AK10110", BSP_AK10110_Source },
                { "AK10111", BSP_AK10111_Source },
                { "AK10112", BSP_AK10112_Source },
                { "AK10113", BSP_AK10113_Source },
                { "AK10114", BSP_AK10114_Source },
                { "AK10115", BSP_AK10115_Source },
                { "AK10116", BSP_AK10116_Source },
                { "AK10117", BSP_AK10117_Source },
                { "AK10118", BSP_AK10118_Source },
                { "AK10119", BSP_AK10119_Source },
                { "AK10120", BSP_AK10120_Source },
                { "AK10121", BSP_AK10121_Source },
                { "AK10122", BSP_AK10122_Source },
                { "AK10123", BSP_AK10123_Source },
                { "AK10124", BSP_AK10124_Source },
                { "AK10125", BSP_AK10125_Source },
                { "AK10126", BSP_AK10126_Source },
                { "AK10127", BSP_AK10127_Source },
                { "AK10128", BSP_AK10128_Source },
                { "AK10129", BSP_AK10129_Source },
                { "AK10130", BSP_AK10130_Source },
                { "AK10131", BSP_AK10131_Source },
                { "AK10132", BSP_AK10132_Source },
                { "AK10133", BSP_AK10133_Source },
                { "AK10134", BSP_AK10134_Source },
                { "AK10135", BSP_AK10135_Source },
                { "AK10200", BSP_AK10200_Source },
                { "AK10201", BSP_AK10201_Source },
                { "AK10202", BSP_AK10202_Source },
                { "AK10203", BSP_AK10203_Source },
                { "AK10204", BSP_AK10204_Source },
                { "AK10205", BSP_AK10205_Source },
                { "AK10206", BSP_AK10206_Source },
                { "AK10207", BSP_AK10207_Source },
                { "AK10208", BSP_AK10208_Source },
                { "AK10209", BSP_AK10209_Source },
                { "AK10301", BSP_AK10301_Source },
                { "AK10302", BSP_AK10302_Source },
                { "AK10303", BSP_AK10303_Source },
                { "AK10304", BSP_AK10304_Source },
                { "AK10305", BSP_AK10305_Source },
                { "AK10306", BSP_AK10306_Source },
                { "AK10307", BSP_AK10307_Source },
                { "AK10308", BSP_AK10308_Source },
                { "AK10309", BSP_AK10309_Source },
                { "AK10310", BSP_AK10310_Source },
                { "AK10311", BSP_AK10311_Source },
                { "AK10312", BSP_AK10312_Source },
                { "AK10313", BSP_AK10313_Source },
                { "AK10314", BSP_AK10314_Source },
                { "AK10315", BSP_AK10315_Source },
                { "AK10316", BSP_AK10316_Source },
                { "AK10317", BSP_AK10317_Source },
                { "AK10318", BSP_AK10318_Source },
                { "AK10319", BSP_AK10319_Source },
                { "AK10320", BSP_AK10320_Source },
                { "AK10321", BSP_AK10321_Source },
                { "AK10322", BSP_AK10322_Source },
                { "AK10323", BSP_AK10323_Source },
                { "AK10324", BSP_AK10324_Source },
                { "AK10325", BSP_AK10325_Source },
                { "AK10326", BSP_AK10326_Source },
                { "AK10327", BSP_AK10327_Source },
                { "AK10328", BSP_AK10328_Source },
                { "AK10329", BSP_AK10329_Source },
                { "AK10330", BSP_AK10330_Source },
                { "AK10331", BSP_AK10331_Source },
                { "AK10332", BSP_AK10332_Source },
                { "AK10333", BSP_AK10333_Source },
                { "AK10334", BSP_AK10334_Source },
                { "AK10335", BSP_AK10335_Source },
                { "AK10336", BSP_AK10336_Source },
                { "AK10500", BSP_AK10500_Source },
                { "AK30100", BSP_AK30100_Source },
                { "AK30101", BSP_AK30101_Source },
                { "AK30102", BSP_AK30102_Source },
                { "AK30106", BSP_AK30106_Source },
                { "AK30300", BSP_AK30300_Source },
                { "AK30304", BSP_AK30304_Source },
                { "AK40100", BSP_AK40100_Source },
                { "AK40101", BSP_AK40101_Source },
                { "AK40102", BSP_AK40102_Source },
                { "AK40104", BSP_AK40104_Source },
                { "AK40105", BSP_AK40105_Source },
                { "AK40106", BSP_AK40106_Source },
                { "AK40107", BSP_AK40107_Source },
                { "AK40110", BSP_AK40110_Source },
                { "AK40111", BSP_AK40111_Source },
                { "AK40113", BSP_AK40113_Source },
                { "AK40114", BSP_AK40114_Source },
                { "AK40116", BSP_AK40116_Source },
                { "AK40117", BSP_AK40117_Source },
                { "AK40118", BSP_AK40118_Source },
                { "AK40201", BSP_AK40201_Source },
                { "AK40202", BSP_AK40202_Source },
                { "AK40203", BSP_AK40203_Source },
                { "AK40210", BSP_AK40210_Source },
                { "AK40211", BSP_AK40211_Source },
                { "AK40212", BSP_AK40212_Source },
                { "AK40213", BSP_AK40213_Source },
                { "AK40214", BSP_AK40214_Source },
                { "AL10100", BSP_AL10100_Source },
                { "AL10101", BSP_AL10101_Source },
                { "AL10102", BSP_AL10102_Source },
                { "AL10103", BSP_AL10103_Source },
                { "AL10104", BSP_AL10104_Source },
                { "AL10105", BSP_AL10105_Source },
                { "AL10106", BSP_AL10106_Source },
                { "AL10107", BSP_AL10107_Source },
                { "AL10108", BSP_AL10108_Source },
                { "AL10109", BSP_AL10109_Source },
                { "AL10110", BSP_AL10110_Source },
                { "AL10111", BSP_AL10111_Source },
                { "AL10112", BSP_AL10112_Source },
                { "AL10200", BSP_AL10200_Source },
                { "AL10201", BSP_AL10201_Source },
                { "AL10202", BSP_AL10202_Source },
                { "AL10203", BSP_AL10203_Source },
                { "AL10204", BSP_AL10204_Source },
                { "AL10205", BSP_AL10205_Source },
                { "AL10206", BSP_AL10206_Source },
                { "AL10207", BSP_AL10207_Source },
                { "AL10208", BSP_AL10208_Source },
                { "AL10300", BSP_AL10300_Source },
                { "AL10301", BSP_AL10301_Source },
                { "AL10302", BSP_AL10302_Source },
                { "AL10303", BSP_AL10303_Source },
                { "AL10304", BSP_AL10304_Source },
                { "AL10305", BSP_AL10305_Source },
                { "AL10306", BSP_AL10306_Source },
                { "AL10307", BSP_AL10307_Source },
                { "AL10308", BSP_AL10308_Source },
                { "AL10309", BSP_AL10309_Source },
                { "AL10310", BSP_AL10310_Source },
                { "AL10311", BSP_AL10311_Source },
                { "AL10312", BSP_AL10312_Source },
                { "AL10313", BSP_AL10313_Source },
                { "AL10320", BSP_AL10320_Source },
                { "AL10321", BSP_AL10321_Source },
                { "AL10322", BSP_AL10322_Source },
                { "AL10500", BSP_AL10500_Source },
                { "AL10501", BSP_AL10501_Source },
                { "AL10502", BSP_AL10502_Source },
                { "AL10503", BSP_AL10503_Source },
                { "AL30100", BSP_AL30100_Source },
                { "AL30101", BSP_AL30101_Source },
                { "AL40204", BSP_AL40204_Source },
                { "AL40205", BSP_AL40205_Source },
                { "AL40206", BSP_AL40206_Source },
                { "AL5102", BSP_AL5102_Source },
                { "AL5103", BSP_AL5103_Source },
                { "AL70", BSP_AL70_Source },
                { "AL81", BSP_AL81_Source },
                { "AL89", BSP_AL89_Source },
                { "AM10100", BSP_AM10100_Source },
                { "AM10101", BSP_AM10101_Source },
                { "AM10102", BSP_AM10102_Source },
                { "AM10103", BSP_AM10103_Source },
                { "AM10104", BSP_AM10104_Source },
                { "AM10105", BSP_AM10105_Source },
                { "AM10106", BSP_AM10106_Source },
                { "AM10200", BSP_AM10200_Source },
                { "AM10201", BSP_AM10201_Source },
                { "AM10202", BSP_AM10202_Source },
                { "AM10203", BSP_AM10203_Source },
                { "AM10204", BSP_AM10204_Source },
                { "AM10205", BSP_AM10205_Source },
                { "AM10206", BSP_AM10206_Source },
                { "AM10300", BSP_AM10300_Source },
                { "AM10301", BSP_AM10301_Source },
                { "AM10302", BSP_AM10302_Source },
                { "AM10303", BSP_AM10303_Source },
                { "AM10304", BSP_AM10304_Source },
                { "AM10305", BSP_AM10305_Source },
                { "AM10306", BSP_AM10306_Source },
                { "AM10500", BSP_AM10500_Source },
                { "AM40200", BSP_AM40200_Source },
                { "AMB68D", BSP_AMB68D_Source },
                { "AMB89D", BSP_AMB89D_Source },
                { "AP10100", BSP_AP10100_Source },
                { "AP10200", BSP_AP10200_Source },
                { "AP10201", BSP_AP10201_Source },
                { "AP10202", BSP_AP10202_Source },
                { "AP20100", BSP_AP20100_Source },
                { "AP20101", BSP_AP20101_Source },
                { "AP20102", BSP_AP20102_Source },
                { "AP20105", BSP_AP20105_Source },
                { "AP20106", BSP_AP20106_Source },
                { "AP20200", BSP_AP20200_Source },
                { "AP20201", BSP_AP20201_Source },
                { "AP20202", BSP_AP20202_Source },
                { "AP20203", BSP_AP20203_Source },
                { "AP20204", BSP_AP20204_Source },
                { "AP20205", BSP_AP20205_Source },
                { "AP20206", BSP_AP20206_Source },
                { "AP20207", BSP_AP20207_Source },
                { "AP20208", BSP_AP20208_Source },
                { "AP20209", BSP_AP20209_Source },
                { "AP20210", BSP_AP20210_Source },
                { "AP20212", BSP_AP20212_Source },
                { "AP20213", BSP_AP20213_Source },
                { "AP20214", BSP_AP20214_Source },
                { "AP20215", BSP_AP20215_Source },
                { "AP20216", BSP_AP20216_Source },
                { "AP20217", BSP_AP20217_Source },
                { "AP20218", BSP_AP20218_Source },
                { "AP28D", BSP_AP28D_Source },
                { "AP32D", BSP_AP32D_Source },
                { "AP44D", BSP_AP44D_Source },
                { "APE70", BSP_APE70_Source },
                { "APF28D", BSP_APF28D_Source },
                { "APF32D", BSP_APF32D_Source },
                { "APF44D", BSP_APF44D_Source },
                { "AR10110", BSP_AR10110_Source },
                { "AR10111", BSP_AR10111_Source },
                { "AR10112", BSP_AR10112_Source },
                { "AR10113", BSP_AR10113_Source },
                { "AR10114", BSP_AR10114_Source },
                { "AR10115", BSP_AR10115_Source },
                { "AR10116", BSP_AR10116_Source },
                { "AR10117", BSP_AR10117_Source },
                { "AR10118", BSP_AR10118_Source },
                { "AR10119", BSP_AR10119_Source },
                { "AR126", BSP_AR126_Source },
                { "AR65", BSP_AR65_Source },
                { "ARJ-OC", BSP_ARJ_OC_Source },
                { "ARJ-OV", BSP_ARJ_OV_Source },
                { "AS10100", BSP_AS10100_Source },
                { "AS10101", BSP_AS10101_Source },
                { "AS10200", BSP_AS10200_Source },
                { "AS10300", BSP_AS10300_Source },
                { "AS10301", BSP_AS10301_Source },
                { "AS10302", BSP_AS10302_Source },
                { "AS20200", BSP_AS20200_Source },
                { "AS20202", BSP_AS20202_Source },
                { "AS20203", BSP_AS20203_Source },
                { "AS40216", BSP_AS40216_Source },
                { "AU10100", BSP_AU10100_Source },
                { "AU10101", BSP_AU10101_Source },
                { "AU10102", BSP_AU10102_Source },
                { "AU10103", BSP_AU10103_Source },
                { "AU10104", BSP_AU10104_Source },
                { "AU10105", BSP_AU10105_Source },
                { "AU10106", BSP_AU10106_Source },
                { "AU10107", BSP_AU10107_Source },
                { "AU10108", BSP_AU10108_Source },
                { "AU10109", BSP_AU10109_Source },
                { "AU10110", BSP_AU10110_Source },
                { "AU10111", BSP_AU10111_Source },
                { "AU10112", BSP_AU10112_Source },
                { "AU10113", BSP_AU10113_Source },
                { "AU10114", BSP_AU10114_Source },
                { "AU10200", BSP_AU10200_Source },
                { "AU10201", BSP_AU10201_Source },
                { "AU10202", BSP_AU10202_Source },
                { "AU10203", BSP_AU10203_Source },
                { "AU10204", BSP_AU10204_Source },
                { "AU10205", BSP_AU10205_Source },
                { "AU10300", BSP_AU10300_Source },
                { "AU10301", BSP_AU10301_Source },
                { "AU10302", BSP_AU10302_Source },
                { "AU10303", BSP_AU10303_Source },
                { "AU10304", BSP_AU10304_Source },
                { "AU10305", BSP_AU10305_Source },
                { "AU10306", BSP_AU10306_Source },
                { "AU10307", BSP_AU10307_Source },
                { "AU10308", BSP_AU10308_Source },
                { "AU10309", BSP_AU10309_Source },
                { "AU10310", BSP_AU10310_Source },
                { "AU10311", BSP_AU10311_Source },
                { "AU10312", BSP_AU10312_Source },
                { "AU10313", BSP_AU10313_Source },
                { "AU10314", BSP_AU10314_Source },
                { "AU10315", BSP_AU10315_Source },
                { "AU10500", BSP_AU10500_Source },
                { "AU10501", BSP_AU10501_Source },
                { "AU10502", BSP_AU10502_Source },
                { "AU10503", BSP_AU10503_Source },
                { "AU10504", BSP_AU10504_Source },
                { "AU30100", BSP_AU30100_Source },
                { "AU30101", BSP_AU30101_Source },
                { "AU30200", BSP_AU30200_Source },
                { "AU30201", BSP_AU30201_Source },
                { "AV090090", BSP_AV090090_Source },
                { "AV090091", BSP_AV090091_Source },
                { "AV090092", BSP_AV090092_Source },
                { "AV090093", BSP_AV090093_Source },
                { "AV10200", BSP_AV10200_Source },
                { "AV10201", BSP_AV10201_Source },
                { "AV10202", BSP_AV10202_Source },
                { "AV10203", BSP_AV10203_Source },
                { "AV10204", BSP_AV10204_Source },
                { "AV114245", BSP_AV114245_Source },
                { "AV120097", BSP_AV120097_Source },
                { "AV120102", BSP_AV120102_Source },
                { "AV120106", BSP_AV120106_Source },
                { "AV120132", BSP_AV120132_Source },
                { "AV120135", BSP_AV120135_Source },
                { "AV120147", BSP_AV120147_Source },
                { "AV120149", BSP_AV120149_Source },
                { "AV120206", BSP_AV120206_Source },
                { "AV120209", BSP_AV120209_Source },
                { "AV120210", BSP_AV120210_Source },
                { "AV120219", BSP_AV120219_Source },
                { "AV120224", BSP_AV120224_Source },
                { "AV120236", BSP_AV120236_Source },
                { "AV120237", BSP_AV120237_Source },
                { "AV120246", BSP_AV120246_Source },
                { "AV120248", BSP_AV120248_Source },
                { "AV120249", BSP_AV120249_Source },
                { "AV120832", BSP_AV120832_Source },
                { "AV120835", BSP_AV120835_Source },
                { "AV120847", BSP_AV120847_Source },
                { "AV120849", BSP_AV120849_Source },
                { "AV124214", BSP_AV124214_Source },
                { "AV124245", BSP_AV124245_Source },
                { "AV130210", BSP_AV130210_Source },
                { "AV140001", BSP_AV140001_Source },
                { "AV140009", BSP_AV140009_Source },
                { "AV140011", BSP_AV140011_Source },
                { "AV140013", BSP_AV140013_Source },
                { "AV140017", BSP_AV140017_Source },
                { "AV140041", BSP_AV140041_Source },
                { "AV140045", BSP_AV140045_Source },
                { "AV140046", BSP_AV140046_Source },
                { "AV140200", BSP_AV140200_Source },
                { "AV140205", BSP_AV140205_Source },
                { "AV140207", BSP_AV140207_Source },
                { "AV140208", BSP_AV140208_Source },
                { "AV140209", BSP_AV140209_Source },
                { "AV140215", BSP_AV140215_Source },
                { "AV140218", BSP_AV140218_Source },
                { "AV140219", BSP_AV140219_Source },
                { "AV140227", BSP_AV140227_Source },
                { "AV140238", BSP_AV140238_Source },
                { "AV140243", BSP_AV140243_Source },
                { "AV140244", BSP_AV140244_Source },
                { "AV140248", BSP_AV140248_Source },
                { "AV140258", BSP_AV140258_Source },
                { "AV140267", BSP_AV140267_Source },
                { "AV140277", BSP_AV140277_Source },
                { "AV144247", BSP_AV144247_Source },
                { "AV160076", BSP_AV160076_Source },
                { "AV160219", BSP_AV160219_Source },
                { "AV160251", BSP_AV160251_Source },
                { "AV160252", BSP_AV160252_Source },
                { "AV190273", BSP_AV190273_Source },
                { "AV190274", BSP_AV190274_Source },
                { "AV20100", BSP_AV20100_Source },
                { "AV20101", BSP_AV20101_Source },
                { "AV20102", BSP_AV20102_Source },
                { "AV20103", BSP_AV20103_Source },
                { "AV20104", BSP_AV20104_Source },
                { "AV20200", BSP_AV20200_Source },
                { "AV20201", BSP_AV20201_Source },
                { "AV20202", BSP_AV20202_Source },
                { "AV209900", BSP_AV209900_Source },
                { "AV227300", BSP_AV227300_Source },
                { "AV227303", BSP_AV227303_Source },
                { "AV229018", BSP_AV229018_Source },
                { "AV229019", BSP_AV229019_Source },
                { "AV229029", BSP_AV229029_Source },
                { "AV229030", BSP_AV229030_Source },
                { "AV229040", BSP_AV229040_Source },
                { "AV229098", BSP_AV229098_Source },
                { "AV229100", BSP_AV229100_Source },
                { "AV249019", BSP_AV249019_Source },
                { "AV249034", BSP_AV249034_Source },
                { "AV249035", BSP_AV249035_Source },
                { "AV249046", BSP_AV249046_Source },
                { "AV249244", BSP_AV249244_Source },
                { "AV269101", BSP_AV269101_Source },
                { "AV289218", BSP_AV289218_Source },
                { "AV420250", BSP_AV420250_Source },
                { "AV420340", BSP_AV420340_Source },
                { "AV429311", BSP_AV429311_Source },
                { "AV429340", BSP_AV429340_Source },
                { "AV440340", BSP_AV440340_Source },
                { "AV470025", BSP_AV470025_Source },
                { "AV477015", BSP_AV477015_Source },
                { "AV479015", BSP_AV479015_Source },
                { "AV652923", BSP_AV652923_Source },
                { "AV_ALU_4-CA", BSP_AV_ALU_4_CA_Source },
                { "AV_ALU_4R_T", BSP_AV_ALU_4R_T_Source },
                { "AZ60-32", BSP_AZ60_32_Source },
                { "AZ70D", BSP_AZ70D_Source },
                { "AZM78D", BSP_AZM78D_Source },
                { "B813010", BSP_B813010_Source },
                { "B813040", BSP_B813040_Source },
                { "B813071", BSP_B813071_Source },
                { "B813080", BSP_B813080_Source },
                { "B813233", BSP_B813233_Source },
                { "B813235", BSP_B813235_Source },
                { "B813740", BSP_B813740_Source },
                { "B813750", BSP_B813750_Source },
                { "B813770", BSP_B813770_Source },
                { "B813920", BSP_B813920_Source },
                { "B813930", BSP_B813930_Source },
                { "B850001", BSP_B850001_Source },
                { "B850002", BSP_B850002_Source },
                { "B850008", BSP_B850008_Source },
                { "B850034", BSP_B850034_Source },
                { "B850035", BSP_B850035_Source },
                { "B850301", BSP_B850301_Source },
                { "B850302", BSP_B850302_Source },
                { "B850313", BSP_B850313_Source },
                { "B850314", BSP_B850314_Source },
                { "B850315", BSP_B850315_Source },
                { "B850316", BSP_B850316_Source },
                { "B850336", BSP_B850336_Source },
                { "B850609", BSP_B850609_Source },
                { "B852000", BSP_B852000_Source },
                { "B852001", BSP_B852001_Source },
                { "B852200", BSP_B852200_Source },
                { "B852201", BSP_B852201_Source },
                { "B852202", BSP_B852202_Source },
                { "B852203", BSP_B852203_Source },
                { "B852500", BSP_B852500_Source },
                { "B852501", BSP_B852501_Source },
                { "B852600", BSP_B852600_Source },
                { "B852601", BSP_B852601_Source },
                { "B852605", BSP_B852605_Source },
                { "B852615", BSP_B852615_Source },
                { "B888", BSP_B888_Source },
                { "B889", BSP_B889_Source },
                { "BP1", BSP_BP1_Source },
                { "BTCC", BSP_BTCC_Source },
                { "BTCV", BSP_BTCV_Source },
                { "BTD", BSP_BTD_Source },
                { "BVSV", BSP_BVSV_Source },
                { "C135J09", BSP_C135J09_Source },
                { "C135J13", BSP_C135J13_Source },
                { "C235J09", BSP_C235J09_Source },
                { "C235J13", BSP_C235J13_Source },
                { "CA135", BSP_CA135_Source },
                { "CA235", BSP_CA235_Source },
                { "CA801", BSP_CA801_Source },
                { "CA802-V2", BSP_CA802_V2_Source },
                { "CA802", BSP_CA802_Source },
                { "CA811", BSP_CA811_Source },
                { "CA812", BSP_CA812_Source },
                { "CA813", BSP_CA813_Source },
                { "CA815", BSP_CA815_Source },
                { "CA816", BSP_CA816_Source },
                { "CA817", BSP_CA817_Source },
                { "CA819", BSP_CA819_Source },
                { "CA821", BSP_CA821_Source },
                { "CA823", BSP_CA823_Source },
                { "CA824", BSP_CA824_Source },
                { "CA825", BSP_CA825_Source },
                { "CA831", BSP_CA831_Source },
                { "CA832", BSP_CA832_Source },
                { "CA833", BSP_CA833_Source },
                { "CA835", BSP_CA835_Source },
                { "CA836", BSP_CA836_Source },
                { "CA837", BSP_CA837_Source },
                { "CA838", BSP_CA838_Source },
                { "CA839", BSP_CA839_Source },
                { "CA841", BSP_CA841_Source },
                { "CA844", BSP_CA844_Source },
                { "CA845", BSP_CA845_Source },
                { "CA846", BSP_CA846_Source },
                { "CA847", BSP_CA847_Source },
                { "CA848", BSP_CA848_Source },
                { "CA849", BSP_CA849_Source },
                { "CA851", BSP_CA851_Source },
                { "CA852", BSP_CA852_Source },
                { "CA853", BSP_CA853_Source },
                { "CA854", BSP_CA854_Source },
                { "CA855", BSP_CA855_Source },
                { "CA861", BSP_CA861_Source },
                { "CA870", BSP_CA870_Source },
                { "CA871", BSP_CA871_Source },
                { "CA875", BSP_CA875_Source },
                { "CA876", BSP_CA876_Source },
                { "CA890", BSP_CA890_Source },
                { "CA893", BSP_CA893_Source },
                { "CA894", BSP_CA894_Source },
                { "CAD10001", BSP_CAD10001_Source },
                { "CAD10002", BSP_CAD10002_Source },
                { "CAD10003", BSP_CAD10003_Source },
                { "CAD10004", BSP_CAD10004_Source },
                { "CAD10007", BSP_CAD10007_Source },
                { "CAD10008", BSP_CAD10008_Source },
                { "CAD10101", BSP_CAD10101_Source },
                { "CAD10101D", BSP_CAD10101D_Source },
                { "CAD10211", BSP_CAD10211_Source },
                { "CAD10212", BSP_CAD10212_Source },
                { "CAD10213", BSP_CAD10213_Source },
                { "CAD10215", BSP_CAD10215_Source },
                { "CAD10218", BSP_CAD10218_Source },
                { "CAD10221", BSP_CAD10221_Source },
                { "CAD10223", BSP_CAD10223_Source },
                { "CAD10224", BSP_CAD10224_Source },
                { "CAD10225", BSP_CAD10225_Source },
                { "CAD10226", BSP_CAD10226_Source },
                { "CAD10227", BSP_CAD10227_Source },
                { "CAD10228", BSP_CAD10228_Source },
                { "CAD10229", BSP_CAD10229_Source },
                { "CAD10230", BSP_CAD10230_Source },
                { "CAD10231", BSP_CAD10231_Source },
                { "CAD10232", BSP_CAD10232_Source },
                { "CAL155", BSP_CAL155_Source },
                { "CDA66", BSP_CDA66_Source },
                { "CE26", BSP_CE26_Source },
                { "CF1-SNT", BSP_CF1_SNT_Source },
                { "CG813", BSP_CG813_Source },
                { "CG814", BSP_CG814_Source },
                { "CG819", BSP_CG819_Source },
                { "CG820", BSP_CG820_Source },
                { "CG837", BSP_CG837_Source },
                { "CJ", BSP_CJ_Source },
                { "CJ02", BSP_CJ02_Source },
                { "CJ06", BSP_CJ06_Source },
                { "CJ09", BSP_CJ09_Source },
                { "CJ14", BSP_CJ14_Source },
                { "CJ18", BSP_CJ18_Source },
                { "CJ22", BSP_CJ22_Source },
                { "CJ26", BSP_CJ26_Source },
                { "CJ31", BSP_CJ31_Source },
                { "CJ32-42", BSP_CJ32_42_Source },
                { "CJ34", BSP_CJ34_Source },
                { "CJ38", BSP_CJ38_Source },
                { "CJ70", BSP_CJ70_Source },
                { "CL140205", BSP_CL140205_Source },
                { "CL140207", BSP_CL140207_Source },
                { "CL140208", BSP_CL140208_Source },
                { "CL140209", BSP_CL140209_Source },
                { "CL140227", BSP_CL140227_Source },
                { "CL140243", BSP_CL140243_Source },
                { "CL140244", BSP_CL140244_Source },
                { "CL229110", BSP_CL229110_Source },
                { "CL229114", BSP_CL229114_Source },
                { "CL259010", BSP_CL259010_Source },
                { "CRM13", BSP_CRM13_Source },
                { "CTA09", BSP_CTA09_Source },
                { "CV114", BSP_CV114_Source },
                { "CV13-35J09", BSP_CV13_35J09_Source },
                { "CV13-46J09", BSP_CV13_46J09_Source },
                { "CV13-46J13", BSP_CV13_46J13_Source },
                { "CV13-46J13S", BSP_CV13_46J13S_Source },
                { "CV13-55J09", BSP_CV13_55J09_Source },
                { "CV13-55J13", BSP_CV13_55J13_Source },
                { "CV13-60J09", BSP_CV13_60J09_Source },
                { "CV13-60J13", BSP_CV13_60J13_Source },
                { "CV13-66J09", BSP_CV13_66J09_Source },
                { "CV13-66J13", BSP_CV13_66J13_Source },
                { "CV23-35J09", BSP_CV23_35J09_Source },
                { "CV23-46J09", BSP_CV23_46J09_Source },
                { "CV23-46J13", BSP_CV23_46J13_Source },
                { "CV23-55J09", BSP_CV23_55J09_Source },
                { "CV23-55J13", BSP_CV23_55J13_Source },
                { "CV23-66J09", BSP_CV23_66J09_Source },
                { "CV23-66J13", BSP_CV23_66J13_Source },
                { "DRF4", BSP_DRF4_Source },
                { "DRF5-E", BSP_DRF5_E_Source },
                { "EMCT1", BSP_EMCT1_Source },
                { "EQ1016", BSP_EQ1016_Source },
                { "EQ11", BSP_EQ11_Source },
                { "EQ1104", BSP_EQ1104_Source },
                { "EQ1413", BSP_EQ1413_Source },
                { "EQ1915", BSP_EQ1915_Source },
                { "EVLE184", BSP_EVLE184_Source },
                { "FBL1265-K20", BSP_FBL1265_K20_Source },
                { "FE58", BSP_FE58_Source },
                { "GL28H", BSP_GL28H_Source },
                { "GL28V", BSP_GL28V_Source },
                { "HWS1", BSP_HWS1_Source },
                { "I-7-20", BSP_I_7_20_Source },
                { "I7C", BSP_I7C_Source },
                { "IAL61", BSP_IAL61_Source },
                { "IAMD116", BSP_IAMD116_Source },
                { "IH3-76", BSP_IH3_76_Source },
                { "IST", BSP_IST_Source },
                { "JB15", BSP_JB15_Source },
                { "JB84-11", BSP_JB84_11_Source },
                { "JC174", BSP_JC174_Source },
                { "JCD", BSP_JCD_Source },
                { "JCV9", BSP_JCV9_Source },
                { "JD84", BSP_JD84_Source },
                { "JFI27", BSP_JFI27_Source },
                { "JM2302", BSP_JM2302_Source },
                { "JOINT", BSP_JOINT_Source },
                { "JPA84", BSP_JPA84_Source },
                { "JS17", BSP_JS17_Source },
                { "K3-14", BSP_K3_14_Source },
                { "K3-201_1", BSP_K3_201_1_Source },
                { "K3-5_1", BSP_K3_5_1_Source },
                { "K3-5_2", BSP_K3_5_2_Source },
                { "K3-6_1", BSP_K3_6_1_Source },
                { "K3-6_2", BSP_K3_6_2_Source },
                { "K910105", BSP_K910105_Source },
                { "K910150", BSP_K910150_Source },
                { "K920343", BSP_K920343_Source },
                { "K920344", BSP_K920344_Source },
                { "K920345", BSP_K920345_Source },
                { "K920346", BSP_K920346_Source },
                { "K920348", BSP_K920348_Source },
                { "K920351", BSP_K920351_Source },
                { "KNF35", BSP_KNF35_Source },
                { "KP176", BSP_KP176_Source },
                { "KP484", BSP_KP484_Source },
                { "M298", BSP_M298_Source },
                { "MEA1", BSP_MEA1_Source },
                { "MI55", BSP_MI55_Source },
                { "MI81", BSP_MI81_Source },
                { "MI81G", BSP_MI81G_Source },
                { "MRX14-35", BSP_MRX14_35_Source },
                { "MS30-12", BSP_MS30_12_Source },
                { "MX-JC35", BSP_MX_JC35_Source },
                { "MX-JCAL", BSP_MX_JCAL_Source },
                { "MXR12", BSP_MXR12_Source },
                { "MXR20", BSP_MXR20_Source },
                { "MXR40", BSP_MXR40_Source },
                { "N0211", BSP_N0211_Source },
                { "NA3", BSP_NA3_Source },
                { "NA30", BSP_NA30_Source },
                { "NA32_71", BSP_NA32_71_Source },
                { "NA32_84", BSP_NA32_84_Source },
                { "NA6-V2", BSP_NA6_V2_Source },
                { "NA6", BSP_NA6_Source },
                { "NA6_V1", BSP_NA6_V1_Source },
                { "NA874", BSP_NA874_Source },
                { "NF-TA84", BSP_NF_TA84_Source },
                { "NF-TA84d", BSP_NF_TA84d_Source },
                { "NF30-70", BSP_NF30_70_Source },
                { "NF30-76", BSP_NF30_76_Source },
                { "NF3076", BSP_NF3076_Source },
                { "NK3", BSP_NK3_Source },
                { "NK3_V1", BSP_NK3_V1_Source },
                { "NR7", BSP_NR7_Source },
                { "NS28", BSP_NS28_Source },
                { "P494203", BSP_P494203_Source },
                { "P494204", BSP_P494204_Source },
                { "P494233", BSP_P494233_Source },
                { "P494235", BSP_P494235_Source },
                { "P494300", BSP_P494300_Source },
                { "P494517", BSP_P494517_Source },
                { "P494518", BSP_P494518_Source },
                { "P494519", BSP_P494519_Source },
                { "P494520", BSP_P494520_Source },
                { "P494521", BSP_P494521_Source },
                { "P494522", BSP_P494522_Source },
                { "P494523", BSP_P494523_Source },
                { "P494525", BSP_P494525_Source },
                { "P494526", BSP_P494526_Source },
                { "P494531", BSP_P494531_Source },
                { "P494532", BSP_P494532_Source },
                { "P494533", BSP_P494533_Source },
                { "P494534", BSP_P494534_Source },
                { "P494535", BSP_P494535_Source },
                { "P494536", BSP_P494536_Source },
                { "P494537", BSP_P494537_Source },
                { "P494539", BSP_P494539_Source },
                { "P494540", BSP_P494540_Source },
                { "P496171", BSP_P496171_Source },
                { "P599170", BSP_P599170_Source },
                { "P755969", BSP_P755969_Source },
                { "P780561", BSP_P780561_Source },
                { "P803706", BSP_P803706_Source },
                { "P803707", BSP_P803707_Source },
                { "P813650", BSP_P813650_Source },
                { "P813730", BSP_P813730_Source },
                { "P813760", BSP_P813760_Source },
                { "P842607", BSP_P842607_Source },
                { "PA63", BSP_PA63_Source },
                { "PC1812", BSP_PC1812_Source },
                { "PC1812V1", BSP_PC1812V1_Source },
                { "PC1824", BSP_PC1824_Source },
                { "PC1824B", BSP_PC1824B_Source },
                { "PC1832", BSP_PC1832_Source },
                { "PC1832v1", BSP_PC1832v1_Source },
                { "PC2308", BSP_PC2308_Source },
                { "PC2312", BSP_PC2312_Source },
                { "PC2312V1", BSP_PC2312V1_Source },
                { "PC2316", BSP_PC2316_Source },
                { "PC2324", BSP_PC2324_Source },
                { "PC2332", BSP_PC2332_Source },
                { "PC80", BSP_PC80_Source },
                { "PE160", BSP_PE160_Source },
                { "PE710", BSP_PE710_Source },
                { "PE725", BSP_PE725_Source },
                { "PE760", BSP_PE760_Source },
                { "PEM005", BSP_PEM005_Source },
                { "PEM006", BSP_PEM006_Source },
                { "PEM008", BSP_PEM008_Source },
                { "PEM011", BSP_PEM011_Source },
                { "PFK1", BSP_PFK1_Source },
                { "PG1824", BSP_PG1824_Source },
                { "PG1832", BSP_PG1832_Source },
                { "PG2324", BSP_PG2324_Source },
                { "PG2332", BSP_PG2332_Source },
                { "PH36", BSP_PH36_Source },
                { "PH40", BSP_PH40_Source },
                { "PIL200", BSP_PIL200_Source },
                { "POC-J09", BSP_POC_J09_Source },
                { "PPDT", BSP_PPDT_Source },
                { "PTA80", BSP_PTA80_Source },
                { "PTBF2385-2", BSP_PTBF2385_2_Source },
                { "PV36", BSP_PV36_Source },
                { "PV40", BSP_PV40_Source },
                { "RAS1", BSP_RAS1_Source },
                { "RAS2", BSP_RAS2_Source },
                { "RJ70", BSP_RJ70_Source },
                { "RKD2", BSP_RKD2_Source },
                { "RKD9", BSP_RKD9_Source },
                { "SA-T70", BSP_SA_T70_Source },
                { "SA-T70S", BSP_SA_T70S_Source },
                { "SNT74", BSP_SNT74_Source },
                { "SNT84", BSP_SNT84_Source },
                { "SNTA84", BSP_SNTA84_Source },
                { "SRJK75", BSP_SRJK75_Source },
                { "SRT70", BSP_SRT70_Source },
                { "ST20", BSP_ST20_Source },
                { "ST5125", BSP_ST5125_Source },
                { "ST5137", BSP_ST5137_Source },
                { "ST5400", BSP_ST5400_Source },
                { "ST5402D", BSP_ST5402D_Source },
                { "ST5404", BSP_ST5404_Source },
                { "ST5404D", BSP_ST5404D_Source },
                { "STP6090", BSP_STP6090_Source },
                { "TA811", BSP_TA811_Source },
                { "TA812", BSP_TA812_Source },
                { "TA813", BSP_TA813_Source },
                { "TA815", BSP_TA815_Source },
                { "TA818", BSP_TA818_Source },
                { "TA820", BSP_TA820_Source },
                { "TA822", BSP_TA822_Source },
                { "TA84oc001", BSP_TA84oc001_Source },
                { "TA84oc002", BSP_TA84oc002_Source },
                { "TA84oc003", BSP_TA84oc003_Source },
                { "TA84oc004", BSP_TA84oc004_Source },
                { "TA84ov001", BSP_TA84ov001_Source },
                { "TA84ov002", BSP_TA84ov002_Source },
                { "TA84ov003", BSP_TA84ov003_Source },
                { "TA84ov004", BSP_TA84ov004_Source },
                { "TA84_75_001", BSP_TA84_75_001_Source },
                { "TA870", BSP_TA870_Source },
                { "TA875", BSP_TA875_Source },
                { "TA890", BSP_TA890_Source },
                { "TA892", BSP_TA892_Source },
                { "TA893", BSP_TA893_Source },
                { "TD40", BSP_TD40_Source },
                { "TP126", BSP_TP126_Source },
                { "VN-CVA23", BSP_VN_CVA23_Source },
                { "VN-JC", BSP_VN_JC_Source },
                { "VN-JC09", BSP_VN_JC09_Source },
                { "VN-JC13", BSP_VN_JC13_Source },
                { "VRX11-58", BSP_VRX11_58_Source },
                { "VRX13", BSP_VRX13_Source },
                { "VRX14-35", BSP_VRX14_35_Source },
                { "VX10-35-70", BSP_VX10_35_70_Source },
                { "VX10-35", BSP_VX10_35_Source },
                { "VX10-46", BSP_VX10_46_Source },
                { "VX10-55-70", BSP_VX10_55_70_Source },
                { "VX10-66", BSP_VX10_66_Source },
                { "VX15", BSP_VX15_Source },
                { "VX59", BSP_VX59_Source },
                { "WS3625", BSP_WS3625_Source },
                { "WS4125", BSP_WS4125_Source },
                { "XP3", BSP_XP3_Source },
                { "Z599196", BSP_Z599196_Source },
                { "Z902161", BSP_Z902161_Source },
                { "Z914262", BSP_Z914262_Source },
                { "Z918265", BSP_Z918265_Source },
                { "Z921438", BSP_Z921438_Source }
            };

        #endregion

        #region === Constructeur ===

        // A compléter

        #endregion

        #region === Méthodes publiques ===

        // A compléter

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Helper d'initialisation : construit une URI absolue à partir du nom de
        /// fichier d'une section de profil de barre, en concaténant <see cref="_base"/>
        /// et le nom passé en paramètre.
        /// </summary>
        /// <param name="filename">Nom du fichier image de la section de profil de barre (ex. <c>"RE_02CJ.png"</c>).</param>
        /// <returns>URI absolue résolvant la ressource embarquée.</returns>
        private static Uri GetBarProfilSection(string filename) => new Uri(_base + filename, UriKind.Absolute);

        #endregion

    }
}
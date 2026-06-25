namespace DG244Cutting.D_Presentation.Settings
{
    /// <summary>
    /// Référentiel statique de données des drapeaux disponibles dans l'application.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Classe de référence interne à la couche Presentation, consommée
    /// exclusivement par <see cref="SE_Flag"/>. Anciennement définie dans le projet Shared,
    /// elle est désormais intégrée en propre dans DG244Cutting afin de supprimer toute
    /// dépendance externe au projet Shared.</para>
    /// <para>Objectif : Centraliser les URI des drapeaux et le dictionnaire de résolution
    /// par code pays ISO 3166-1 alpha-2. Cette classe est une donnée de référence purement
    /// statique — elle ne porte aucun état mutable.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item>Déclarer les URI absolues de chaque drapeau disponible</item>
    /// <item>Exposer le dictionnaire de résolution code pays → URI</item>
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
    internal static class RS_Flags
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Préfixe <c>pack://</c> pointant vers le dossier <c>Resources/Flags</c>
        /// de l'assembly courante <c>DG244Cutting</c>. Constante compilée afin de
        /// supprimer toute dépendance à l'ordre d'initialisation des champs
        /// statiques.
        /// </summary>
        private const string _base = "pack://application:,,,/DG244Cutting;component/D_Presentation/Resources/Flags/";

        #endregion

        #region === Dépendances privées ===

        // A compléter

        #endregion

        #region === Propriétés publiques ===

        /// <summary>URI du drapeau par défaut utilisé en cas de repli (drapeau United Kingdom).</summary>
        public static readonly Uri DefaultFlag_Source = GetFlag("RE_united_kingdom.png");

        public static readonly Uri Flag_Afghanistan_Source = GetFlag("RE_afghanistan.png");
        public static readonly Uri Flag_Aland_islands_Source = GetFlag("RE_aland_islands.png");
        public static readonly Uri Flag_Albania_Source = GetFlag("RE_albania.png");
        public static readonly Uri Flag_Algeria_Source = GetFlag("RE_algeria.png");
        public static readonly Uri Flag_American_samoa_Source = GetFlag("RE_american_samoa.png");
        public static readonly Uri Flag_Andorra_Source = GetFlag("RE_andorra.png");
        public static readonly Uri Flag_Angola_Source = GetFlag("RE_angola.png");
        public static readonly Uri Flag_Anguilla_Source = GetFlag("RE_anguilla.png");
        public static readonly Uri Flag_Antarctica_Source = GetFlag("RE_antarctica.png");
        public static readonly Uri Flag_Antigua_and_barbuda_Source = GetFlag("RE_antigua_and_barbuda.png");
        public static readonly Uri Flag_Argentina_Source = GetFlag("RE_argentina.png");
        public static readonly Uri Flag_Armenia_Source = GetFlag("RE_armenia.png");
        public static readonly Uri Flag_Aruba_Source = GetFlag("RE_aruba.png");
        public static readonly Uri Flag_Ascension_island_Source = GetFlag("RE_ascension_island.png");
        public static readonly Uri Flag_Australia_Source = GetFlag("RE_australia.png");
        public static readonly Uri Flag_Austria_Source = GetFlag("RE_austria.png");
        public static readonly Uri Flag_Azerbaijan_Source = GetFlag("RE_azerbaijan.png");
        public static readonly Uri Flag_Bahamas_Source = GetFlag("RE_bahamas.png");
        public static readonly Uri Flag_Bahrain_Source = GetFlag("RE_bahrain.png");
        public static readonly Uri Flag_Bangladesh_Source = GetFlag("RE_bangladesh.png");
        public static readonly Uri Flag_Barbados_Source = GetFlag("RE_barbados.png");
        public static readonly Uri Flag_Belarus_Source = GetFlag("RE_belarus.png");
        public static readonly Uri Flag_Belgium_Source = GetFlag("RE_belgium.png");
        public static readonly Uri Flag_Belize_Source = GetFlag("RE_belize.png");
        public static readonly Uri Flag_Benin_Source = GetFlag("RE_benin.png");
        public static readonly Uri Flag_Bermuda_Source = GetFlag("RE_bermuda.png");
        public static readonly Uri Flag_Bhutan_Source = GetFlag("RE_bhutan.png");
        public static readonly Uri Flag_Bolivia_Source = GetFlag("RE_bolivia.png");
        public static readonly Uri Flag_Bosnia_and_herzegovina_Source = GetFlag("RE_bosnia_and_herzegovina.png");
        public static readonly Uri Flag_Botswana_Source = GetFlag("RE_botswana.png");
        public static readonly Uri Flag_Bouvet_island_Source = GetFlag("RE_bouvet_island.png");
        public static readonly Uri Flag_Brazil_Source = GetFlag("RE_brazil.png");
        public static readonly Uri Flag_British_indian_ocean_territory_Source = GetFlag("RE_british_indian_ocean_territory.png");
        public static readonly Uri Flag_British_virgin_islands_Source = GetFlag("RE_british_virgin_islands.png");
        public static readonly Uri Flag_Brunei_Source = GetFlag("RE_brunei.png");
        public static readonly Uri Flag_Bulgaria_Source = GetFlag("RE_bulgaria.png");
        public static readonly Uri Flag_Burkina_faso_Source = GetFlag("RE_burkina_faso.png");
        public static readonly Uri Flag_Burundi_Source = GetFlag("RE_burundi.png");
        public static readonly Uri Flag_Cambodia_Source = GetFlag("RE_cambodia.png");
        public static readonly Uri Flag_Cameroon_Source = GetFlag("RE_cameroon.png");
        public static readonly Uri Flag_Canada_Source = GetFlag("RE_canada.png");
        public static readonly Uri Flag_Canary_islands_Source = GetFlag("RE_canary_islands.png");
        public static readonly Uri Flag_Cape_verde_Source = GetFlag("RE_cape_verde.png");
        public static readonly Uri Flag_Caribbean_netherlands_Source = GetFlag("RE_caribbean_netherlands.png");
        public static readonly Uri Flag_Cayman_islands_Source = GetFlag("RE_cayman_islands.png");
        public static readonly Uri Flag_Central_african_republic_Source = GetFlag("RE_central_african_republic.png");
        public static readonly Uri Flag_Ceuta_and_melilla_Source = GetFlag("RE_ceuta_and_melilla.png");
        public static readonly Uri Flag_Chad_Source = GetFlag("RE_chad.png");
        public static readonly Uri Flag_Chile_Source = GetFlag("RE_chile.png");
        public static readonly Uri Flag_China_Source = GetFlag("RE_china.png");
        public static readonly Uri Flag_Christmas_island_Source = GetFlag("RE_christmas_island.png");
        public static readonly Uri Flag_Clipperton_island_Source = GetFlag("RE_clipperton_island.png");
        public static readonly Uri Flag_Cocos_keeling_islands_Source = GetFlag("RE_cocos_keeling_islands.png");
        public static readonly Uri Flag_Colombia_Source = GetFlag("RE_colombia.png");
        public static readonly Uri Flag_Comoros_Source = GetFlag("RE_comoros.png");
        public static readonly Uri Flag_Congo_brazzaville_Source = GetFlag("RE_congo_brazzaville.png");
        public static readonly Uri Flag_Congo_kinshasa_Source = GetFlag("RE_congo_kinshasa.png");
        public static readonly Uri Flag_Cook_islands_Source = GetFlag("RE_cook_islands.png");
        public static readonly Uri Flag_Costa_rica_Source = GetFlag("RE_costa_rica.png");
        public static readonly Uri Flag_Cote_divoire_Source = GetFlag("RE_cote_divoire.png");
        public static readonly Uri Flag_Croatia_Source = GetFlag("RE_croatia.png");
        public static readonly Uri Flag_Cuba_Source = GetFlag("RE_cuba.png");
        public static readonly Uri Flag_Curacao_Source = GetFlag("RE_curacao.png");
        public static readonly Uri Flag_Cyprus_Source = GetFlag("RE_cyprus.png");
        public static readonly Uri Flag_Czechia_Source = GetFlag("RE_czechia.png");
        public static readonly Uri Flag_Denmark_Source = GetFlag("RE_denmark.png");
        public static readonly Uri Flag_Diego_garcia_Source = GetFlag("RE_diego_garcia.png");
        public static readonly Uri Flag_Djibouti_Source = GetFlag("RE_djibouti.png");
        public static readonly Uri Flag_Dominica_Source = GetFlag("RE_dominica.png");
        public static readonly Uri Flag_Dominican_republic_Source = GetFlag("RE_dominican_republic.png");
        public static readonly Uri Flag_Ecuador_Source = GetFlag("RE_ecuador.png");
        public static readonly Uri Flag_Egypt_Source = GetFlag("RE_egypt.png");
        public static readonly Uri Flag_El_salvador_Source = GetFlag("RE_el_salvador.png");
        public static readonly Uri Flag_Equatorial_guinea_Source = GetFlag("RE_equatorial_guinea.png");
        public static readonly Uri Flag_Eritrea_Source = GetFlag("RE_eritrea.png");
        public static readonly Uri Flag_Estonia_Source = GetFlag("RE_estonia.png");
        public static readonly Uri Flag_Eswatini_Source = GetFlag("RE_eswatini.png");
        public static readonly Uri Flag_Ethiopia_Source = GetFlag("RE_ethiopia.png");
        public static readonly Uri Flag_European_union_Source = GetFlag("RE_european_union.png");
        public static readonly Uri Flag_Falkland_islands_Source = GetFlag("RE_falkland_islands.png");
        public static readonly Uri Flag_Faroe_islands_Source = GetFlag("RE_faroe_islands.png");
        public static readonly Uri Flag_Fiji_Source = GetFlag("RE_fiji.png");
        public static readonly Uri Flag_Finland_Source = GetFlag("RE_finland.png");
        public static readonly Uri Flag_France_Source = GetFlag("RE_france.png");
        public static readonly Uri Flag_French_guiana_Source = GetFlag("RE_french_guiana.png");
        public static readonly Uri Flag_French_polynesia_Source = GetFlag("RE_french_polynesia.png");
        public static readonly Uri Flag_French_southern_territories_Source = GetFlag("RE_french_southern_territories.png");
        public static readonly Uri Flag_Gabon_Source = GetFlag("RE_gabon.png");
        public static readonly Uri Flag_Gambia_Source = GetFlag("RE_gambia.png");
        public static readonly Uri Flag_Georgia_Source = GetFlag("RE_georgia.png");
        public static readonly Uri Flag_Germany_Source = GetFlag("RE_germany.png");
        public static readonly Uri Flag_Ghana_Source = GetFlag("RE_ghana.png");
        public static readonly Uri Flag_Gibraltar_Source = GetFlag("RE_gibraltar.png");
        public static readonly Uri Flag_Greece_Source = GetFlag("RE_greece.png");
        public static readonly Uri Flag_Greenland_Source = GetFlag("RE_greenland.png");
        public static readonly Uri Flag_Grenada_Source = GetFlag("RE_grenada.png");
        public static readonly Uri Flag_Guadeloupe_Source = GetFlag("RE_guadeloupe.png");
        public static readonly Uri Flag_Guam_Source = GetFlag("RE_guam.png");
        public static readonly Uri Flag_Guatemala_Source = GetFlag("RE_guatemala.png");
        public static readonly Uri Flag_Guernsey_Source = GetFlag("RE_guernsey.png");
        public static readonly Uri Flag_Guinea_Source = GetFlag("RE_guinea.png");
        public static readonly Uri Flag_Guinea_bissau_Source = GetFlag("RE_guinea_bissau.png");
        public static readonly Uri Flag_Guyana_Source = GetFlag("RE_guyana.png");
        public static readonly Uri Flag_Haiti_Source = GetFlag("RE_haiti.png");
        public static readonly Uri Flag_Heard_and_mcdonald_islands_Source = GetFlag("RE_heard_and_mcdonald_islands.png");
        public static readonly Uri Flag_Honduras_Source = GetFlag("RE_honduras.png");
        public static readonly Uri Flag_Hong_kong_sar_china_Source = GetFlag("RE_hong_kong_sar_china.png");
        public static readonly Uri Flag_Hungary_Source = GetFlag("RE_hungary.png");
        public static readonly Uri Flag_Iceland_Source = GetFlag("RE_iceland.png");
        public static readonly Uri Flag_India_Source = GetFlag("RE_india.png");
        public static readonly Uri Flag_Indonesia_Source = GetFlag("RE_indonesia.png");
        public static readonly Uri Flag_Iran_Source = GetFlag("RE_iran.png");
        public static readonly Uri Flag_Iraq_Source = GetFlag("RE_iraq.png");
        public static readonly Uri Flag_Ireland_Source = GetFlag("RE_ireland.png");
        public static readonly Uri Flag_Isle_of_man_Source = GetFlag("RE_isle_of_man.png");
        public static readonly Uri Flag_Israel_Source = GetFlag("RE_israel.png");
        public static readonly Uri Flag_Italy_Source = GetFlag("RE_italy.png");
        public static readonly Uri Flag_Jamaica_Source = GetFlag("RE_jamaica.png");
        public static readonly Uri Flag_Japan_Source = GetFlag("RE_japan.png");
        public static readonly Uri Flag_Jersey_Source = GetFlag("RE_jersey.png");
        public static readonly Uri Flag_Jordan_Source = GetFlag("RE_jordan.png");
        public static readonly Uri Flag_Kazakhstan_Source = GetFlag("RE_kazakhstan.png");
        public static readonly Uri Flag_Kenya_Source = GetFlag("RE_kenya.png");
        public static readonly Uri Flag_Kiribati_Source = GetFlag("RE_kiribati.png");
        public static readonly Uri Flag_Kosovo_Source = GetFlag("RE_kosovo.png");
        public static readonly Uri Flag_Kuwait_Source = GetFlag("RE_kuwait.png");
        public static readonly Uri Flag_Kyrgyzstan_Source = GetFlag("RE_kyrgyzstan.png");
        public static readonly Uri Flag_Laos_Source = GetFlag("RE_laos.png");
        public static readonly Uri Flag_Latvia_Source = GetFlag("RE_latvia.png");
        public static readonly Uri Flag_Lebanon_Source = GetFlag("RE_lebanon.png");
        public static readonly Uri Flag_Lesotho_Source = GetFlag("RE_lesotho.png");
        public static readonly Uri Flag_Liberia_Source = GetFlag("RE_liberia.png");
        public static readonly Uri Flag_Libya_Source = GetFlag("RE_libya.png");
        public static readonly Uri Flag_Liechtenstein_Source = GetFlag("RE_liechtenstein.png");
        public static readonly Uri Flag_Lithuania_Source = GetFlag("RE_lithuania.png");
        public static readonly Uri Flag_Luxembourg_Source = GetFlag("RE_luxembourg.png");
        public static readonly Uri Flag_Macao_sar_china_Source = GetFlag("RE_macao_sar_china.png");
        public static readonly Uri Flag_Madagascar_Source = GetFlag("RE_madagascar.png");
        public static readonly Uri Flag_Malawi_Source = GetFlag("RE_malawi.png");
        public static readonly Uri Flag_Malaysia_Source = GetFlag("RE_malaysia.png");
        public static readonly Uri Flag_Maldives_Source = GetFlag("RE_maldives.png");
        public static readonly Uri Flag_Mali_Source = GetFlag("RE_mali.png");
        public static readonly Uri Flag_Malta_Source = GetFlag("RE_malta.png");
        public static readonly Uri Flag_Marshall_islands_Source = GetFlag("RE_marshall_islands.png");
        public static readonly Uri Flag_Martinique_Source = GetFlag("RE_martinique.png");
        public static readonly Uri Flag_Mauritania_Source = GetFlag("RE_mauritania.png");
        public static readonly Uri Flag_Mauritius_Source = GetFlag("RE_mauritius.png");
        public static readonly Uri Flag_Mayotte_Source = GetFlag("RE_mayotte.png");
        public static readonly Uri Flag_Mexico_Source = GetFlag("RE_mexico.png");
        public static readonly Uri Flag_Micronesia_Source = GetFlag("RE_micronesia.png");
        public static readonly Uri Flag_Moldova_Source = GetFlag("RE_moldova.png");
        public static readonly Uri Flag_Monaco_Source = GetFlag("RE_monaco.png");
        public static readonly Uri Flag_Mongolia_Source = GetFlag("RE_mongolia.png");
        public static readonly Uri Flag_Montenegro_Source = GetFlag("RE_montenegro.png");
        public static readonly Uri Flag_Montserrat_Source = GetFlag("RE_montserrat.png");
        public static readonly Uri Flag_Morocco_Source = GetFlag("RE_morocco.png");
        public static readonly Uri Flag_Mozambique_Source = GetFlag("RE_mozambique.png");
        public static readonly Uri Flag_Myanmar_burma_Source = GetFlag("RE_myanmar_burma.png");
        public static readonly Uri Flag_Namibia_Source = GetFlag("RE_namibia.png");
        public static readonly Uri Flag_Nauru_Source = GetFlag("RE_nauru.png");
        public static readonly Uri Flag_Netherlands_Source = GetFlag("RE_netherlands.png");
        public static readonly Uri Flag_New_caledonia_Source = GetFlag("RE_new_caledonia.png");
        public static readonly Uri Flag_New_zealand_Source = GetFlag("RE_new_zealand.png");
        public static readonly Uri Flag_Nicaragua_Source = GetFlag("RE_nicaragua.png");
        public static readonly Uri Flag_Niger_Source = GetFlag("RE_niger.png");
        public static readonly Uri Flag_Nigeria_Source = GetFlag("RE_nigeria.png");
        public static readonly Uri Flag_Niue_Source = GetFlag("RE_niue.png");
        public static readonly Uri Flag_Norfolk_island_Source = GetFlag("RE_norfolk_island.png");
        public static readonly Uri Flag_Northern_mariana_islands_Source = GetFlag("RE_northern_mariana_islands.png");
        public static readonly Uri Flag_North_korea_Source = GetFlag("RE_north_korea.png");
        public static readonly Uri Flag_North_macedonia_Source = GetFlag("RE_north_macedonia.png");
        public static readonly Uri Flag_Norway_Source = GetFlag("RE_norway.png");
        public static readonly Uri Flag_Oman_Source = GetFlag("RE_oman.png");
        public static readonly Uri Flag_Pakistan_Source = GetFlag("RE_pakistan.png");
        public static readonly Uri Flag_Palau_Source = GetFlag("RE_palau.png");
        public static readonly Uri Flag_Palestinian_territories_Source = GetFlag("RE_palestinian_territories.png");
        public static readonly Uri Flag_Panama_Source = GetFlag("RE_panama.png");
        public static readonly Uri Flag_Papua_new_guinea_Source = GetFlag("RE_papua_new_guinea.png");
        public static readonly Uri Flag_Paraguay_Source = GetFlag("RE_paraguay.png");
        public static readonly Uri Flag_Peru_Source = GetFlag("RE_peru.png");
        public static readonly Uri Flag_Philippines_Source = GetFlag("RE_philippines.png");
        public static readonly Uri Flag_Pitcairn_islands_Source = GetFlag("RE_pitcairn_islands.png");
        public static readonly Uri Flag_Poland_Source = GetFlag("RE_poland.png");
        public static readonly Uri Flag_Portugal_Source = GetFlag("RE_portugal.png");
        public static readonly Uri Flag_Puerto_rico_Source = GetFlag("RE_puerto_rico.png");
        public static readonly Uri Flag_Qatar_Source = GetFlag("RE_qatar.png");
        public static readonly Uri Flag_Reunion_Source = GetFlag("RE_reunion.png");
        public static readonly Uri Flag_Romania_Source = GetFlag("RE_romania.png");
        public static readonly Uri Flag_Russia_Source = GetFlag("RE_russia.png");
        public static readonly Uri Flag_Rwanda_Source = GetFlag("RE_rwanda.png");
        public static readonly Uri Flag_Samoa_Source = GetFlag("RE_samoa.png");
        public static readonly Uri Flag_San_marino_Source = GetFlag("RE_san_marino.png");
        public static readonly Uri Flag_Sao_tome_and_principe_Source = GetFlag("RE_sao_tome_and_principe.png");
        public static readonly Uri Flag_Saudi_arabia_Source = GetFlag("RE_saudi_arabia.png");
        public static readonly Uri Flag_Senegal_Source = GetFlag("RE_senegal.png");
        public static readonly Uri Flag_Serbia_Source = GetFlag("RE_serbia.png");
        public static readonly Uri Flag_Seychelles_Source = GetFlag("RE_seychelles.png");
        public static readonly Uri Flag_Sierra_leone_Source = GetFlag("RE_sierra_leone.png");
        public static readonly Uri Flag_Singapore_Source = GetFlag("RE_singapore.png");
        public static readonly Uri Flag_Sint_maarten_Source = GetFlag("RE_sint_maarten.png");
        public static readonly Uri Flag_Slovakia_Source = GetFlag("RE_slovakia.png");
        public static readonly Uri Flag_Slovenia_Source = GetFlag("RE_slovenia.png");
        public static readonly Uri Flag_Solomon_islands_Source = GetFlag("RE_solomon_islands.png");
        public static readonly Uri Flag_Somalia_Source = GetFlag("RE_somalia.png");
        public static readonly Uri Flag_South_africa_Source = GetFlag("RE_south_africa.png");
        public static readonly Uri Flag_South_georgia_Source = GetFlag("RE_south_georgia.png");
        public static readonly Uri Flag_South_korea_Source = GetFlag("RE_south_korea.png");
        public static readonly Uri Flag_South_sandwich_islands_Source = GetFlag("RE_south_sandwich_islands.png");
        public static readonly Uri Flag_South_sudan_Source = GetFlag("RE_south_sudan.png");
        public static readonly Uri Flag_Spain_Source = GetFlag("RE_spain.png");
        public static readonly Uri Flag_Sri_lanka_Source = GetFlag("RE_sri_lanka.png");
        public static readonly Uri Flag_St_barthelemy_Source = GetFlag("RE_st_barthelemy.png");
        public static readonly Uri Flag_St_helena_Source = GetFlag("RE_st_helena.png");
        public static readonly Uri Flag_St_kitts_and_nevis_Source = GetFlag("RE_st_kitts_and_nevis.png");
        public static readonly Uri Flag_St_lucia_Source = GetFlag("RE_st_lucia.png");
        public static readonly Uri Flag_St_martin_Source = GetFlag("RE_st_martin.png");
        public static readonly Uri Flag_St_pierre_and_miquelon_Source = GetFlag("RE_st_pierre_and_miquelon.png");
        public static readonly Uri Flag_St_vincent_and_grenadines_Source = GetFlag("RE_st_vincent_and_grenadines.png");
        public static readonly Uri Flag_Sudan_Source = GetFlag("RE_sudan.png");
        public static readonly Uri Flag_Suriname_Source = GetFlag("RE_suriname.png");
        public static readonly Uri Flag_Svalbard_and_jan_mayen_Source = GetFlag("RE_svalbard_and_jan_mayen.png");
        public static readonly Uri Flag_Sweden_Source = GetFlag("RE_sweden.png");
        public static readonly Uri Flag_Switzerland_Source = GetFlag("RE_switzerland.png");
        public static readonly Uri Flag_Syria_Source = GetFlag("RE_syria.png");
        public static readonly Uri Flag_Taiwan_Source = GetFlag("RE_taiwan.png");
        public static readonly Uri Flag_Tajikistan_Source = GetFlag("RE_tajikistan.png");
        public static readonly Uri Flag_Tanzania_Source = GetFlag("RE_tanzania.png");
        public static readonly Uri Flag_Thailand_Source = GetFlag("RE_thailand.png");
        public static readonly Uri Flag_Timor_leste_Source = GetFlag("RE_timor_leste.png");
        public static readonly Uri Flag_Togo_Source = GetFlag("RE_togo.png");
        public static readonly Uri Flag_Tokelau_Source = GetFlag("RE_tokelau.png");
        public static readonly Uri Flag_Tonga_Source = GetFlag("RE_tonga.png");
        public static readonly Uri Flag_Trinidad_and_tobago_Source = GetFlag("RE_trinidad_and_tobago.png");
        public static readonly Uri Flag_Tristan_da_cunha_Source = GetFlag("RE_tristan_da_cunha.png");
        public static readonly Uri Flag_Tunisia_Source = GetFlag("RE_tunisia.png");
        public static readonly Uri Flag_Turkey_Source = GetFlag("RE_turkey.png");
        public static readonly Uri Flag_Turkmenistan_Source = GetFlag("RE_turkmenistan.png");
        public static readonly Uri Flag_Turks_and_caicos_islands_Source = GetFlag("RE_turks_and_caicos_islands.png");
        public static readonly Uri Flag_Tuvalu_Source = GetFlag("RE_tuvalu.png");
        public static readonly Uri Flag_Uganda_Source = GetFlag("RE_uganda.png");
        public static readonly Uri Flag_Ukraine_Source = GetFlag("RE_ukraine.png");
        public static readonly Uri Flag_United_arab_emirates_Source = GetFlag("RE_united_arab_emirates.png");
        public static readonly Uri Flag_United_kingdom_Source = GetFlag("RE_united_kingdom.png");
        public static readonly Uri Flag_United_nations_Source = GetFlag("RE_united_nations.png");
        public static readonly Uri Flag_United_states_Source = GetFlag("RE_united_states.png");
        public static readonly Uri Flag_Uruguay_Source = GetFlag("RE_uruguay.png");
        public static readonly Uri Flag_Us_outlying_islands_Source = GetFlag("RE_us_outlying_islands.png");
        public static readonly Uri Flag_Us_virgin_islands_Source = GetFlag("RE_us_virgin_islands.png");
        public static readonly Uri Flag_Uzbekistan_Source = GetFlag("RE_uzbekistan.png");
        public static readonly Uri Flag_Vanuatu_Source = GetFlag("RE_vanuatu.png");
        public static readonly Uri Flag_Venezuela_Source = GetFlag("RE_venezuela.png");
        public static readonly Uri Flag_Vietnam_Source = GetFlag("RE_vietnam.png");
        public static readonly Uri Flag_Wallis_and_futuna_Source = GetFlag("RE_wallis_and_futuna.png");
        public static readonly Uri Flag_Western_Source = GetFlag("RE_western.png");
        public static readonly Uri Flag_Yemen_Source = GetFlag("RE_yemen.png");
        public static readonly Uri Flag_Zambia_Source = GetFlag("RE_zambia.png");
        public static readonly Uri Flag_Zimbabwe_Source = GetFlag("RE_zimbabwe.png");

        /// <summary>
        /// Dictionnaire associant un code pays ISO 3166-1 alpha-2 à l'URI du drapeau correspondant.
        /// </summary>
        public static readonly IReadOnlyDictionary<string, Uri> ReferenceFlag =
            new Dictionary<string, Uri>
            {
                { "AF", Flag_Afghanistan_Source },
                { "AX", Flag_Aland_islands_Source },
                { "AL", Flag_Albania_Source },
                { "DZ", Flag_Algeria_Source },
                { "AS", Flag_American_samoa_Source },
                { "AD", Flag_Andorra_Source },
                { "AO", Flag_Angola_Source },
                { "AI", Flag_Anguilla_Source },
                { "AQ", Flag_Antarctica_Source },
                { "AG", Flag_Antigua_and_barbuda_Source },
                { "AR", Flag_Argentina_Source },
                { "AM", Flag_Armenia_Source },
                { "AW", Flag_Aruba_Source },
                { "AC", Flag_Ascension_island_Source },
                { "AU", Flag_Australia_Source },
                { "AT", Flag_Austria_Source },
                { "AZ", Flag_Azerbaijan_Source },
                { "BS", Flag_Bahamas_Source },
                { "BH", Flag_Bahrain_Source },
                { "BD", Flag_Bangladesh_Source },
                { "BB", Flag_Barbados_Source },
                { "BY", Flag_Belarus_Source },
                { "BE", Flag_Belgium_Source },
                { "BZ", Flag_Belize_Source },
                { "BJ", Flag_Benin_Source },
                { "BM", Flag_Bermuda_Source },
                { "BT", Flag_Bhutan_Source },
                { "BO", Flag_Bolivia_Source },
                { "BA", Flag_Bosnia_and_herzegovina_Source },
                { "BW", Flag_Botswana_Source },
                { "BV", Flag_Bouvet_island_Source },
                { "BR", Flag_Brazil_Source },
                { "IO", Flag_British_indian_ocean_territory_Source },
                { "VG", Flag_British_virgin_islands_Source },
                { "BN", Flag_Brunei_Source },
                { "BG", Flag_Bulgaria_Source },
                { "BF", Flag_Burkina_faso_Source },
                { "BI", Flag_Burundi_Source },
                { "KH", Flag_Cambodia_Source },
                { "CM", Flag_Cameroon_Source },
                { "CA", Flag_Canada_Source },
                { "IC", Flag_Canary_islands_Source },
                { "CV", Flag_Cape_verde_Source },
                { "BQ", Flag_Caribbean_netherlands_Source },
                { "KY", Flag_Cayman_islands_Source },
                { "CF", Flag_Central_african_republic_Source },
                { "EA", Flag_Ceuta_and_melilla_Source },
                { "TD", Flag_Chad_Source },
                { "CL", Flag_Chile_Source },
                { "CN", Flag_China_Source },
                { "CX", Flag_Christmas_island_Source },
                { "CP", Flag_Clipperton_island_Source },
                { "CC", Flag_Cocos_keeling_islands_Source },
                { "CO", Flag_Colombia_Source },
                { "KM", Flag_Comoros_Source },
                { "CG", Flag_Congo_brazzaville_Source },
                { "CD", Flag_Congo_kinshasa_Source },
                { "CK", Flag_Cook_islands_Source },
                { "CR", Flag_Costa_rica_Source },
                { "CI", Flag_Cote_divoire_Source },
                { "HR", Flag_Croatia_Source },
                { "CU", Flag_Cuba_Source },
                { "CW", Flag_Curacao_Source },
                { "CY", Flag_Cyprus_Source },
                { "CZ", Flag_Czechia_Source },
                { "DK", Flag_Denmark_Source },
                { "DG", Flag_Diego_garcia_Source },
                { "DJ", Flag_Djibouti_Source },
                { "DM", Flag_Dominica_Source },
                { "DO", Flag_Dominican_republic_Source },
                { "EC", Flag_Ecuador_Source },
                { "EG", Flag_Egypt_Source },
                { "SV", Flag_El_salvador_Source },
                { "GQ", Flag_Equatorial_guinea_Source },
                { "ER", Flag_Eritrea_Source },
                { "EE", Flag_Estonia_Source },
                { "SZ", Flag_Eswatini_Source },
                { "ET", Flag_Ethiopia_Source },
                { "EU", Flag_European_union_Source },
                { "FK", Flag_Falkland_islands_Source },
                { "FO", Flag_Faroe_islands_Source },
                { "FJ", Flag_Fiji_Source },
                { "FI", Flag_Finland_Source },
                { "FR", Flag_France_Source },
                { "GF", Flag_French_guiana_Source },
                { "PF", Flag_French_polynesia_Source },
                { "TF", Flag_French_southern_territories_Source },
                { "GA", Flag_Gabon_Source },
                { "GM", Flag_Gambia_Source },
                { "GE", Flag_Georgia_Source },
                { "DE", Flag_Germany_Source },
                { "GH", Flag_Ghana_Source },
                { "GI", Flag_Gibraltar_Source },
                { "GR", Flag_Greece_Source },
                { "GL", Flag_Greenland_Source },
                { "GD", Flag_Grenada_Source },
                { "GP", Flag_Guadeloupe_Source },
                { "GU", Flag_Guam_Source },
                { "GT", Flag_Guatemala_Source },
                { "GG", Flag_Guernsey_Source },
                { "GN", Flag_Guinea_Source },
                { "GW", Flag_Guinea_bissau_Source },
                { "GY", Flag_Guyana_Source },
                { "HT", Flag_Haiti_Source },
                { "HM", Flag_Heard_and_mcdonald_islands_Source },
                { "HN", Flag_Honduras_Source },
                { "HK", Flag_Hong_kong_sar_china_Source },
                { "HU", Flag_Hungary_Source },
                { "IS", Flag_Iceland_Source },
                { "IN", Flag_India_Source },
                { "ID", Flag_Indonesia_Source },
                { "IR", Flag_Iran_Source },
                { "IQ", Flag_Iraq_Source },
                { "IE", Flag_Ireland_Source },
                { "IM", Flag_Isle_of_man_Source },
                { "IL", Flag_Israel_Source },
                { "IT", Flag_Italy_Source },
                { "JM", Flag_Jamaica_Source },
                { "JP", Flag_Japan_Source },
                { "JE", Flag_Jersey_Source },
                { "JO", Flag_Jordan_Source },
                { "KZ", Flag_Kazakhstan_Source },
                { "KE", Flag_Kenya_Source },
                { "KI", Flag_Kiribati_Source },
                { "XK", Flag_Kosovo_Source },
                { "KW", Flag_Kuwait_Source },
                { "KG", Flag_Kyrgyzstan_Source },
                { "LA", Flag_Laos_Source },
                { "LV", Flag_Latvia_Source },
                { "LB", Flag_Lebanon_Source },
                { "LS", Flag_Lesotho_Source },
                { "LR", Flag_Liberia_Source },
                { "LY", Flag_Libya_Source },
                { "LI", Flag_Liechtenstein_Source },
                { "LT", Flag_Lithuania_Source },
                { "LU", Flag_Luxembourg_Source },
                { "MO", Flag_Macao_sar_china_Source },
                { "MG", Flag_Madagascar_Source },
                { "MW", Flag_Malawi_Source },
                { "MY", Flag_Malaysia_Source },
                { "MV", Flag_Maldives_Source },
                { "ML", Flag_Mali_Source },
                { "MT", Flag_Malta_Source },
                { "MH", Flag_Marshall_islands_Source },
                { "MQ", Flag_Martinique_Source },
                { "MR", Flag_Mauritania_Source },
                { "MU", Flag_Mauritius_Source },
                { "YT", Flag_Mayotte_Source },
                { "MX", Flag_Mexico_Source },
                { "FM", Flag_Micronesia_Source },
                { "MD", Flag_Moldova_Source },
                { "MC", Flag_Monaco_Source },
                { "MN", Flag_Mongolia_Source },
                { "ME", Flag_Montenegro_Source },
                { "MS", Flag_Montserrat_Source },
                { "MA", Flag_Morocco_Source },
                { "MZ", Flag_Mozambique_Source },
                { "MM", Flag_Myanmar_burma_Source },
                { "NA", Flag_Namibia_Source },
                { "NR", Flag_Nauru_Source },
                { "NL", Flag_Netherlands_Source },
                { "NC", Flag_New_caledonia_Source },
                { "NZ", Flag_New_zealand_Source },
                { "NI", Flag_Nicaragua_Source },
                { "NE", Flag_Niger_Source },
                { "NG", Flag_Nigeria_Source },
                { "NU", Flag_Niue_Source },
                { "NF", Flag_Norfolk_island_Source },
                { "MP", Flag_Northern_mariana_islands_Source },
                { "KP", Flag_North_korea_Source },
                { "MK", Flag_North_macedonia_Source },
                { "NO", Flag_Norway_Source },
                { "OM", Flag_Oman_Source },
                { "PK", Flag_Pakistan_Source },
                { "PW", Flag_Palau_Source },
                { "PS", Flag_Palestinian_territories_Source },
                { "PA", Flag_Panama_Source },
                { "PG", Flag_Papua_new_guinea_Source },
                { "PY", Flag_Paraguay_Source },
                { "PE", Flag_Peru_Source },
                { "PH", Flag_Philippines_Source },
                { "PN", Flag_Pitcairn_islands_Source },
                { "PL", Flag_Poland_Source },
                { "PT", Flag_Portugal_Source },
                { "PR", Flag_Puerto_rico_Source },
                { "QA", Flag_Qatar_Source },
                { "RE", Flag_Reunion_Source },
                { "RO", Flag_Romania_Source },
                { "RU", Flag_Russia_Source },
                { "RW", Flag_Rwanda_Source },
                { "WS", Flag_Samoa_Source },
                { "SM", Flag_San_marino_Source },
                { "ST", Flag_Sao_tome_and_principe_Source },
                { "SA", Flag_Saudi_arabia_Source },
                { "SN", Flag_Senegal_Source },
                { "RS", Flag_Serbia_Source },
                { "SC", Flag_Seychelles_Source },
                { "SL", Flag_Sierra_leone_Source },
                { "SG", Flag_Singapore_Source },
                { "SX", Flag_Sint_maarten_Source },
                { "SK", Flag_Slovakia_Source },
                { "SI", Flag_Slovenia_Source },
                { "SB", Flag_Solomon_islands_Source },
                { "SO", Flag_Somalia_Source },
                { "ZA", Flag_South_africa_Source },
                { "GS", Flag_South_georgia_Source },
                { "KR", Flag_South_korea_Source },
                { "SS", Flag_South_sudan_Source },
                { "ES", Flag_Spain_Source },
                { "LK", Flag_Sri_lanka_Source },
                { "BL", Flag_St_barthelemy_Source },
                { "SH", Flag_St_helena_Source },
                { "KN", Flag_St_kitts_and_nevis_Source },
                { "LC", Flag_St_lucia_Source },
                { "MF", Flag_St_martin_Source },
                { "PM", Flag_St_pierre_and_miquelon_Source },
                { "VC", Flag_St_vincent_and_grenadines_Source },
                { "SD", Flag_Sudan_Source },
                { "SR", Flag_Suriname_Source },
                { "SJ", Flag_Svalbard_and_jan_mayen_Source },
                { "SE", Flag_Sweden_Source },
                { "CH", Flag_Switzerland_Source },
                { "SY", Flag_Syria_Source },
                { "TW", Flag_Taiwan_Source },
                { "TJ", Flag_Tajikistan_Source },
                { "TZ", Flag_Tanzania_Source },
                { "TH", Flag_Thailand_Source },
                { "TL", Flag_Timor_leste_Source },
                { "TG", Flag_Togo_Source },
                { "TK", Flag_Tokelau_Source },
                { "TO", Flag_Tonga_Source },
                { "TT", Flag_Trinidad_and_tobago_Source },
                { "TN", Flag_Tunisia_Source },
                { "TR", Flag_Turkey_Source },
                { "TM", Flag_Turkmenistan_Source },
                { "TC", Flag_Turks_and_caicos_islands_Source },
                { "TV", Flag_Tuvalu_Source },
                { "UG", Flag_Uganda_Source },
                { "UA", Flag_Ukraine_Source },
                { "AE", Flag_United_arab_emirates_Source },
                { "GB", Flag_United_kingdom_Source },
                { "US", Flag_United_states_Source },
                { "UY", Flag_Uruguay_Source },
                { "UZ", Flag_Uzbekistan_Source },
                { "VU", Flag_Vanuatu_Source },
                { "VE", Flag_Venezuela_Source },
                { "VN", Flag_Vietnam_Source },
                { "WF", Flag_Wallis_and_futuna_Source },
                { "EH", Flag_Western_Source },
                { "YE", Flag_Yemen_Source },
                { "ZM", Flag_Zambia_Source },
                { "ZW", Flag_Zimbabwe_Source }
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
        /// fichier d'un drapeau, en concaténant <see cref="_base"/> et le nom passé
        /// en paramètre.
        /// </summary>
        /// <param name="filename">Nom du fichier image du drapeau (ex. <c>"france.png"</c>).</param>
        /// <returns>URI absolue résolvant la ressource embarquée.</returns>
        private static Uri GetFlag(string filename) => new Uri(_base + filename, UriKind.Absolute);

        #endregion

    }
}
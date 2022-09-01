Configuration
============
This section describe the setup for Documentation Generator Equipment Integration Test Equipment Type

Driver Definitions
=================
The following Automation Drivers are referenced on the Automation Controller as:
- **SecsGemEquipment** for the Automation Driver **BrukerInsightCAPDriver**;

- **RFIDReader** for the Automation Driver **HermosLFM4xReaderDriver**;

Drivers contain the information regarding all the items needed for the automation of the equipment.
## Automation Driver Definition - BrukerInsightCAPDriver
### Protocol
The protocol used by the Automation Driver referenced on the Automation Controller is named **ProtocolNamePlaceHolder** and used the package **ProtocolPackagePlaceHolder**, using version **ProtocolVersionPlaceHolder** at the time of writing.

### Properties

The table below describes the workflow properties

|Name | ID | Type | Equipment Type | Description 
:------------ | -------: | :-------- | :---------- | :-------- 
 DV_ADVTIPQUAL_f4EDGE_HT_L | 10001 | Decimal | F4 | * Advanced Tip Qual Analysis: Tip Vertical Edge Height - left side
 DV_ADVTIPQUAL_f4EDGE_HT_R | 10002 | Decimal | F4 | * Advanced Tip Qual Analysis:
 DV_ADVTIPQUAL_f4EDGE_RAD_L | 10003 | Decimal | F4 | * Advanced Tip Qual Analysis:
 DV_ADVTIPQUAL_f4EDGE_RAD_R | 10004 | Decimal | F4 | * Advanced Tip Qual Analysis:
 DV_ADVTIPQUAL_f4EFF_LEN_L | 10005 | Decimal | F4 | * Advanced Tip Qual Analysis:
 DV_ADVTIPQUAL_f4EFF_LEN_R | 10006 | Decimal | F4 | * Advanced Tip Qual Analysis:
 DV_ADVTIPQUAL_f4LINE_NUM | 10007 | Decimal | F4 | * Advanced Tip Qual Analysis:
 DV_ADVTIPQUAL_f4OL_END_L | 10008 | Decimal | F4 | * Advanced Tip Qual Analysis:
 DV_ADVTIPQUAL_f4OL_END_R | 10009 | Decimal | F4 | * Advanced Tip Qual Analysis:
 DV_ADVTIPQUAL_f4OL_INIT_L | 10010 | Decimal | F4 | * Advanced Tip Qual Analysis:
 DV_ADVTIPQUAL_f4OL_INIT_R | 10011 | Decimal | F4 | * Advanced Tip Qual Analysis:
 DV_ADVTIPQUAL_f4OVRHNG_L | 10012 | Decimal | F4 | * Advanced Tip Qual Analysis:
 DV_ADVTIPQUAL_f4OVRHNG_R | 10013 | Decimal | F4 | * Advanced Tip Qual Analysis:
 DV_ADVTIPQUAL_f4OVRHT_L | 10014 | Decimal | F4 | * Advanced Tip Qual Analysis:
 DV_ADVTIPQUAL_f4OVRHT_R | 10015 | Decimal | F4 | * Advanced Tip Qual Analysis:
 DV_ADVTIPQUAL_f4SHCAP_L | 10016 | Decimal | F4 | * Advanced Tip Qual Analysis:
 DV_ADVTIPQUAL_f4SHCAP_R | 10017 | Decimal | F4 | * Advanced Tip Qual Analysis:
 DV_ADVTIPQUAL_f4TIP_STIFF | 10018 | Decimal | F4 | * Advanced Tip Qual Analysis:
 DV_ADVTIPQUAL_u2AUTOPROGRAM_COUNT | 10019 | Integer | U2 | * Advanced Tip Qual Analysis:
 DV_AUTOSTEP_f4STEP_HT_AR | 10020 | Decimal | F8 | * AutoStep Analysis: Step Height(Array)
 DV_AUTOSTEP_f4STEP_WIDTH_AR | 10021 | Decimal | F8 | * AutoStep Analysis: Step Width(Array)
 DV_AUTOSTEP_u2AUTOPROGRAM_COUNT | 10022 | Integer | U2 | * AutoStep Analysis: Autoprogram counter
 DV_AUTOSTEPHEIGHT_f4HEIGHT | 10023 | Decimal | F4 | * AutoStep Height Analysis: Height
 DV_AUTOSTEPHEIGHT_f4LEFTX | 10024 | Decimal | F4 | * AutoStep Height Analysis:
 DV_AUTOSTEPHEIGHT_f4RIGHTX | 10025 | Decimal | F4 | * AutoStep Height Analysis:
 DV_AUTOSTEPHEIGHT_f4SIGMA | 10026 | Decimal | F4 | * AutoStep Height Analysis:
 DV_AUTOSTEPHEIGHT_f4WIDTH | 10027 | Decimal | F4 | * AutoStep Height Analysis:
 DV_AUTOSTEPHEIGHT_u2AUTOPROGRAM_COUNT | 10028 | Integer | U2 | * AutoStep Height Analysis:
 DV_BUMP_f4AVG_HT | 10050 | Decimal | F4 | * Bump Analysis: Average Height
 DV_BUMP_f4BACKGROUND_RMS | 10051 | Decimal | F4 | * Bump Analysis:
 DV_BUMP_f4CENTER_OF_MASS | 10052 | Decimal | F4 | * Bump Analysis:
 DV_BUMP_f4DEPTH | 10053 | Decimal | F4 | * Bump Analysis:
 DV_BUMP_f4DIAMETER | 10054 | Decimal | F4 | * Bump Analysis:
 DV_BUMP_f4HT_SIGMA | 10055 | Decimal | F4 | * Bump Analysis:
 DV_BUMP_f4NUMBER_BUMPS | 10056 | Decimal | F4 | * Bump Analysis:
 DV_BUMP_f4PEAK_HT | 10057 | Decimal | F4 | * Bump Analysis:
 DV_BUMP_f4WEAR_AREA | 10058 | Decimal | F4 | * Bump Analysis:
 DV_BUMP_f4WEAR_DEPTH | 10059 | Decimal | F4 | * Bump Analysis:
 DV_BUMP_szTYPE | 10060 | String | A | * Bump Analysis:
 DV_BUMP_u2AUTOPROGRAM_COUNT | 10061 | Integer | U2 | * Bump Analysis:
 DV_CIS_f4AXISFAST | 10062 | Decimal | F4 | * CIS Analysis: X Axis
 DV_CIS_f4AXISSLOW | 10063 | Decimal | F4 | * CIS Analysis:
 DV_CIS_f4AXISZ | 10064 | Decimal | F4 | * CIS Analysis:
 DV_CIS_f4BOTANGLELEFT | 10065 | Decimal | F4 | * CIS Analysis:
 DV_CIS_f4BOTANGLERIGHT | 10066 | Decimal | F4 | * CIS Analysis:
 DV_CIS_f4HEIGHT | 10067 | Decimal | F4 | * CIS Analysis:
 DV_CIS_f4MAXHEIGHT | 10068 | Decimal | F4 | * CIS Analysis:
 DV_CIS_f4MIDANGLELEFT | 10069 | Decimal | F4 | * CIS Analysis:
 DV_CIS_f4MIDANGLERIGHT | 10070 | Decimal | F4 | * CIS Analysis:
 DV_CIS_f4RMSERR | 10071 | Decimal | F4 | * CIS Analysis:
 DV_CIS_f4TOPANGLELEFT | 10072 | Decimal | F4 | * CIS Analysis:
 DV_CIS_f4TOPANGLERIGHT | 10073 | Decimal | F4 | * CIS Analysis:
 DV_CIS_f4TYPE | 10074 | Decimal | F4 | * CIS Analysis:
 DV_CIS_f4WIDTH | 10075 | Decimal | F4 | * CIS Analysis:
 DV_CIS_f4XBOTTOM | 10076 | Decimal | F4 | * CIS Analysis:
 DV_CIS_f4XCENTER | 10077 | Decimal | F4 | * CIS Analysis:
 DV_CIS_f4XTOP | 10078 | Decimal | F4 | * CIS Analysis:
 DV_CIS_f4YBOTTOM | 10079 | Decimal | F4 | * CIS Analysis:
 DV_CIS_f4YCENTER | 10080 | Decimal | F4 | * CIS Analysis:
 DV_CIS_f4YTOP | 10081 | Decimal | F4 | * CIS Analysis:
 DV_CIS_u2AUTOPROGRAM_COUNT | 10082 | Integer | U2 | * CIS Analysis:
 DV_DEPTH_f4FILTER_CUTOFF | 10097 | Decimal | F4 | * Depth Analysis: Filter cutoff
 DV_DEPTH_f4MAX_PEAK | 10098 | Decimal | F4 | * Depth Analysis: Maximum Peak
 DV_DEPTH_f4MIN_PEAK | 10099 | Decimal | F4 | * Depth Analysis: Minimum Peak
 DV_GRAIN_f4GRAIN_HEIGHT | 10112 | Decimal | F4 | * Grain Size Analysis: Grain height
 DV_GRAIN_f4GRAIN_SIZE_MAX | 10113 | Decimal | F4 | * Grain Size Analysis: Grain size max
 DV_GRAIN_f4GRAIN_SIZE_MEAN | 10114 | Decimal | F4 | * Grain Size Analysis: Grain size mean
 DV_GRAIN_f4GRAIN_SIZE_MIN | 10115 | Decimal | F4 | * Grain Size Analysis: Grain size min
 DV_GRAIN_f4GRAIN_SIZE_STD_DEV | 10116 | Decimal | F4 | * Grain Size Analysis: Grain size std dev
 DV_GRAIN_f4GRAIN_THRESHOLD_HEIGHT | 10117 | Decimal | F4 | * Grain Size Analysis: Grain height
 DV_GRAIN_f4HIST_PCNT | 10118 | Decimal | F4 | * Grain Size Analysis: Hist %
 DV_GRAIN_f4MEAN_AT_CURSOR | 10119 | Decimal | F4 | * Grain Size Analysis: Mean at cursor
 DV_GRAIN_f4NUM_GRAINS | 10120 | Decimal | F4 | * Grain Size Analysis: Number of grains
 DV_GRAIN_f4NUM_GRAINS_AT_CURSOR | 10121 | Decimal | F4 | * Grain Size Analysis: Number of grains at cursor
 DV_GRAIN_f4STD_DEV_AT_CURSOR | 10122 | Decimal | F4 | * Grain Size Analysis: Std dev at cursor
 DV_GRAIN_f4SUBSTRATE_DEPTH | 10123 | Decimal | F4 | * Grain Size Analysis: Substate depth
 DV_GRAIN_u2AUTOPROGRAM_COUNT | 10124 | Integer | U2 | * Grain Analysis: Autoprogram counter
 DV_LAYERS_f4FILTER_CUTOFF | 10138 | Decimal | F4 | * Layers Analysis: Filter cutoff
 DV_LAYERS_f4LAYER_AT_MAX | 10139 | Decimal | F4 | * Layers Analysis: Layer at max
 DV_LAYERS_f4LAYER1 | 10140 | Decimal | F4 | * Layers Analysis: Layer 1
 DV_LAYERS_f4LAYER1_REL_SIZE | 10141 | Decimal | F4 | * Layers Analysis: Layer 1 relative size (%)
 DV_LAYERS_f4LAYER2 | 10142 | Decimal | F4 | * Layers Analysis: Layer 2
 DV_LAYERS_f4LAYER2_REL_SIZE | 10143 | Decimal | F4 | * Layers Analysis: Layer 2 relative size (%)
 DV_LAYERS_f4LAYER3 | 10144 | Decimal | F4 | * Layers Analysis: Layer 3
 DV_LAYERS_f4LAYER3_REL_SIZE | 10145 | Decimal | F4 | * Layers Analysis: Layer 3 relative size (%)
 DV_LAYERS_f4LAYER4 | 10146 | Decimal | F4 | * Layers Analysis: Layer 4
 DV_LAYERS_f4LAYER4_REL_SIZE | 10147 | Decimal | F4 | * Layers Analysis: Layer 4 relative size (%)
 DV_LAYERS_f4REFERENCE_LEVEL | 10148 | Decimal | F4 | * Layers Analysis: Reference Level
 DV_LAYERS_f4TOTAL_PEAKS | 10149 | Decimal | F4 | * Layers Analysis: Total peaks
 DV_LAYERS_u2AUTOPROGRAM_COUNT | 10150 | Integer | U2 | * Layers Analysis: Autoprogram counter
 DV_LDA_f4DEPTH_A | 10151 | Decimal | F4 | * LDA Analysis: Depth A
 DV_LDA_f4DEPTH_B | 10152 | Decimal | F4 | * LDA Analysis: Depth B
 DV_LDA_f4DEPTH_C | 10153 | Decimal | F4 | * LDA Analysis: Depth C
 DV_LDA_f4DEPTH_D | 10154 | Decimal | F4 | * LDA Analysis: Depth D
 DV_LDA_f4HEIGHT_A | 10155 | Decimal | F4 | * LDA Analysis: Height A
 DV_LDA_f4HEIGHT_B | 10156 | Decimal | F4 | * LDA Analysis: Height B
 DV_LDA_f4HEIGHT_C | 10157 | Decimal | F4 | * LDA Analysis: Height C
 DV_LDA_f4HEIGHT_D | 10158 | Decimal | F4 | * LDA Analysis: Height D
 DV_LDA_f4HEIGHT_REF | 10159 | Decimal | F4 | * LDA Analysis: Height Ref
 DV_LDA_f4RANGE_A | 10160 | Decimal | F4 | * LDA Analysis: Range A
 DV_LDA_f4RANGE_B | 10161 | Decimal | F4 | * LDA Analysis: Range B
 DV_LDA_f4RANGE_C | 10162 | Decimal | F4 | * LDA Analysis: Range C
 DV_LDA_f4RANGE_D | 10163 | Decimal | F4 | * LDA Analysis: Range D
 DV_LDA_f4RANGE_REF | 10164 | Decimal | F4 | * LDA Analysis: Range Ref
 DV_LDA_u2AUTOPROGRAM_COUNT | 10165 | Integer | U2 | * LDA Analysis: Autoprogram counter
 DV_LINEROUGH_f4LEFTSTDDEV | 10166 | Decimal | F4 | * LineEdge Roughness Analysis: Left Edge Standard Deviation
 DV_LINEROUGH_f4MAX | 10167 | Decimal | F4 | * LineEdge Roughness Analysis:
 DV_LINEROUGH_f4MIN | 10168 | Decimal | F4 | * LineEdge Roughness Analysis:
 DV_LINEROUGH_f4RANGE | 10169 | Decimal | F4 | * LineEdge Roughness Analysis:
 DV_LINEROUGH_f4RESOLUTION | 10170 | Decimal | F4 | * LineEdge Roughness Analysis:
 DV_LINEROUGH_f4RIGHTSTDDEV | 10171 | Decimal | F4 | * LineEdge Roughness Analysis:
 DV_LINEROUGH_f4STDDEV | 10172 | Decimal | F4 | * LineEdge Roughness Analysis:
 DV_LINEROUGH_u2AUTOPROGRAM_COUNT | 10173 | Integer | U2 | * LineEdge Roughness Analysis:
 DV_PITCH_f4LINE_NUM | 10191 | Decimal | F4 | * Pitch Analysis: Line Number
 DV_PITCH_f4PITCH | 10192 | Decimal | F4 | * Pitch Analysis: Pitch
 DV_PITCH_u2AUTOPROGRAM_COUNT | 10193 | Integer | U2 | * Pitch Analysis: Autoprogram counter
 DV_PMR_f4CD1 | 10194 | Decimal | F4 | * PMR Analysis: CD 1
 DV_PMR_f4CD2 | 10195 | Decimal | F4 | * PMR Analysis:
 DV_PMR_f4CD3 | 10196 | Decimal | F4 | * PMR Analysis:
 DV_PMR_f4LEFT_ANGLE | 10197 | Decimal | F4 | * PMR Analysis:
 DV_PMR_f4MAX_WIDTH_HEIGHT | 10198 | Decimal | F4 | * PMR Analysis:
 DV_PMR_f4MIN_WIDTH_HEIGHT | 10199 | Decimal | F4 | * PMR Analysis:
 DV_PMR_f4MINMAX_HEIGHT | 10200 | Decimal | F4 | * PMR Analysis:
 DV_PMR_f4POLE_HEIGHT | 10201 | Decimal | F4 | * PMR Analysis:
 DV_PMR_f4RIGHT_ANGLE | 10202 | Decimal | F4 | * PMR Analysis:
 DV_PMR_f4TYPE | 10203 | Decimal | F4 | * PMR Analysis:
 DV_PMR_u2AUTOPROGRAM_COUNT | 10204 | Integer | U2 | * PMR Analysis:
 DV_PROFILER_f4DISHING | 10205 | Decimal | F4 | * Profiler Analysis: CMP Dishing
 DV_PROFILER_f4EROSION | 10206 | Decimal | F4 | * Profiler Analysis: CMP Erosion
 DV_PROFILER_f4MEAN_ROUGHNESS_RA | 10207 | Decimal | F4 | * Profiler Analysis: Mean Roughness Ra
 DV_PROFILER_f4MEANWAVINESS_WA | 10208 | Decimal | F4 | * Profiler Analysis: Mean Waviness Wa
 DV_PROFILER_f4RMS_RQ | 10209 | Decimal | F4 | * Profiler Analysis: RMS Rq
 DV_PROFILER_f4RMS_WQ | 10210 | Decimal | F4 | * Profiler Analysis: RMS Wq
 DV_PROFILER_f4SLOPE | 10211 | Decimal | F4 | * Profiler Analysis: Slope
 DV_PROFILER_f4STEPHT | 10212 | Decimal | F4 | * Profiler Analysis: Step Height
 DV_PROFILER_f4ZRANGE | 10213 | Decimal | F4 | * Profiler Analysis: Z-Range
 DV_PROFILER_u2AUTOPROGRAM_COUNT | 10214 | Integer | U2 | * Profiler Analysis: Autoprogram counter
 DV_RECIPE_u1TYPE | 10220 | Integer | U1 | * Recipe Type
 DV_ROUGH_f410_PT_MEAN_RZ | 10221 | Decimal | F4 | * Roughness Analysis: 10 pt mean (Rz)
 DV_ROUGH_f4AREA_THRESH_ABS | 10222 | Decimal | F4 | * Roughness Analysis: Area threshold abs
 DV_ROUGH_f4AREA_THRESH_PCNT_RMS | 10223 | Decimal | F4 | * Roughness Analysis: Area threshold (%rms)
 DV_ROUGH_f4AREA_THRESH_REF | 10224 | Decimal | F4 | * Roughness Analysis: Area threshold ref
 DV_ROUGH_f4AV_MAX_DEPTH_RVM | 10225 | Decimal | F4 | * Roughness Analysis: Av max depth (rvm)
 DV_ROUGH_f4AV_MAX_HEIGHT_RPM | 10226 | Decimal | F4 | * Roughness Analysis: Average max height rpm
 DV_ROUGH_f4BOX_X_DIM | 10227 | Decimal | F4 | * Roughness Analysis: Box x dimension
 DV_ROUGH_f4BOX_Y_DIM | 10228 | Decimal | F4 | * Roughness Analysis: Box y dimension
 DV_ROUGH_f4IMG_MEAN | 10229 | Decimal | F4 | * Roughness Analysis: Image mean
 DV_ROUGH_f4IMG_PROJ_SURF_AREA | 10230 | Decimal | F4 | * Roughness Analysis: Image projected surface area
 DV_ROUGH_f4IMG_RA | 10231 | Decimal | F4 | * Roughness Analysis: Image Ra
 DV_ROUGH_f4IMG_RAW_MEAN | 10232 | Decimal | F4 | * Roughness Analysis: Image raw mean
 DV_ROUGH_f4IMG_RMAX | 10233 | Decimal | F4 | * Roughness Analysis: Image Rmax
 DV_ROUGH_f4IMG_RMS_RQ | 10234 | Decimal | F4 | * Roughness Analysis: Image rms (Rq)
 DV_ROUGH_f4IMG_SAE | 10235 | Decimal | F4 | * Roughness Analysis: Image SAE
 DV_ROUGH_f4IMG_SURF_AREA | 10236 | Decimal | F4 | * Roughness Analysis: Image surface area
 DV_ROUGH_f4IMG_SURF_AREA_DIFF | 10237 | Decimal | F4 | * Roughness Analysis: Image surface area diff (%)
 DV_ROUGH_f4IMG_Z_RANGE | 10238 | Decimal | F4 | * Roughness Analysis: Image z range
 DV_ROUGH_f4KURTOSIS | 10239 | Decimal | F4 | * Roughness Analysis: Kurtosis
 DV_ROUGH_f4LINE_DENSITY | 10240 | Decimal | F4 | * Roughness Analysis: Line density
 DV_ROUGH_f4MAX_DEPTH_RV | 10241 | Decimal | F4 | * Roughness Analysis: Max depth (Rv)
 DV_ROUGH_f4MAX_HEIGHT_RMAX | 10242 | Decimal | F4 | * Roughness Analysis: Max height rmax
 DV_ROUGH_f4MAX_PEAK_HEIGHT_RP | 10243 | Decimal | F4 | * Roughness Analysis: Max peak height (Rp)
 DV_ROUGH_f4MEAN | 10244 | Decimal | F4 | * Roughness Analysis: Mean
 DV_ROUGH_f4MEAN_ROUGH_RA | 10245 | Decimal | F4 | * Roughness Analysis: Mean rough ra
 DV_ROUGH_f4PEAK_COUNT | 10246 | Decimal | F4 | * Roughness Analysis: Peak count
 DV_ROUGH_f4PEAK_THRESH | 10247 | Decimal | F4 | * Roughness Analysis: Peak threshold
 DV_ROUGH_f4PEAK_THRESH_PCNT_RMS | 10248 | Decimal | F4 | * Roughness Analysis: Peak threshold (%rms)
 DV_ROUGH_f4PEAK_THRESH_REF | 10249 | Decimal | F4 | * Roughness Analysis: Peak threshold ref
 DV_ROUGH_f4PROJ_SURF_AREA | 10250 | Decimal | F4 | * Roughness Analysis: Projected surface area
 DV_ROUGH_f4RAW_MEAN | 10251 | Decimal | F4 | * Roughness Analysis: Raw mean
 DV_ROUGH_f4RMS_RQ | 10252 | Decimal | F4 | * Roughness Analysis: Rms Rq
 DV_ROUGH_f4SAE | 10253 | Decimal | F4 | * Roughness Analysis: SAE
 DV_ROUGH_f4SKEWNESS | 10254 | Decimal | F4 | * Roughness Analysis: Skewness
 DV_ROUGH_f4SUMMIT_COUNT | 10255 | Decimal | F4 | * Roughness Analysis: Summit count
 DV_ROUGH_f4SUMMIT_CURV | 10256 | Decimal | F4 | * Roughness Analysis: Summit curv
 DV_ROUGH_f4SUMMIT_CUTOFF | 10257 | Decimal | F4 | * Roughness Analysis: Summit cutoff
 DV_ROUGH_f4SUMMIT_CUTOFF_PCNT_RMS | 10258 | Decimal | F4 | * Roughness Analysis: Summit cutoff (%rms)
 DV_ROUGH_f4SUMMIT_CUTOFF_REF | 10259 | Decimal | F4 | * Roughness Analysis: Summit cutoff ref
 DV_ROUGH_f4SUMMIT_DENSITY | 10260 | Decimal | F4 | * Roughness Analysis: Summit density
 DV_ROUGH_f4SUMMIT_DIA | 10261 | Decimal | F4 | * Roughness Analysis: Summit dia
 DV_ROUGH_f4SUMMIT_RAD_CURV | 10262 | Decimal | F4 | * Roughness Analysis: Summit rad curv
 DV_ROUGH_f4SUMMIT_SLOPE | 10263 | Decimal | F4 | * Roughness Analysis: Summit slope
 DV_ROUGH_f4SUMMIT_THRESH | 10264 | Decimal | F4 | * Roughness Analysis: Summit threshold
 DV_ROUGH_f4SUMMIT_THRESH_PCNT_RMS | 10265 | Decimal | F4 | * Roughness Analysis: Summit threshold (%rms)
 DV_ROUGH_f4SUMMIT_THRESH_REF | 10266 | Decimal | F4 | * Roughness Analysis: Summit threshold ref
 DV_ROUGH_f4SURF_AREA | 10267 | Decimal | F4 | * Roughness Analysis: Surface area
 DV_ROUGH_f4SURF_AREA_DIFF | 10268 | Decimal | F4 | * Roughness Analysis: Surface area difference
 DV_ROUGH_f4Z_RANGE | 10269 | Decimal | F4 | * Roughness Analysis: Z range
 DV_ROUGH_u2AUTOPROGRAM_COUNT | 10270 | Integer | U2 | * Roughness Analysis: Autoprogram counter
 DV_SECTION_f4FIRST_ANGLE | 10271 | Decimal | F4 | * Section Analysis: First angle
 DV_SECTION_f4FIRST_HORZ_DIST_L | 10272 | Decimal | F4 | * Section Analysis: First horizontal distance (L)
 DV_SECTION_f4FIRST_SURF_DIST | 10273 | Decimal | F4 | * Section Analysis: First surface distance
 DV_SECTION_f4FIRST_VERT_DIST | 10274 | Decimal | F4 | * Section Analysis: First vertical distance
 DV_SECTION_f4L | 10275 | Decimal | F4 | * Section Analysis: L
 DV_SECTION_f4LC | 10276 | Decimal | F4 | * Section Analysis: lc
 DV_SECTION_f4PAIR | 10277 | Decimal | F4 | * Section Analysis:  Pair Number (0-2)
 DV_SECTION_f4RA_LC | 10278 | Decimal | F4 | * Section Analysis: Ra (lc)
 DV_SECTION_f4RADIUS | 10279 | Decimal | F4 | * Section Analysis: Radius
 DV_SECTION_f4RMAX | 10280 | Decimal | F4 | * Section Analysis: Rmax
 DV_SECTION_f4RMS | 10281 | Decimal | F4 | * Section Analysis: RMS
 DV_SECTION_f4RZ | 10282 | Decimal | F4 | * Section Analysis: Rz
 DV_SECTION_f4RZ_COUNT | 10283 | Decimal | F4 | * Section Analysis: Rz count
 DV_SECTION_f4SIGMA | 10284 | Decimal | F4 | * Section Analysis: Sigma
 DV_SECTION_f4SPECTRAL_FREQ | 10285 | Decimal | F4 | * Section Analysis: Spectral frequency
 DV_SECTION_f4SPECTRAL_PERIOD | 10286 | Decimal | F4 | * Section Analysis: Spectral period
 DV_SECTION_f4SPECTRAL_RMS_AMP | 10287 | Decimal | F4 | * Section Analysis: Spectral RMS amp
 DV_SECTION_u2AUTOPROGRAM_COUNT | 10288 | Integer | U2 | * Section Analysis: Autoprogram counter
 DV_SITE_f4COLUMN | 10289 | Decimal | F4 | * Site Report Info: site location: column
 DV_SITE_f4ROW | 10290 | Decimal | F4 | * Site Report Info: site location: row
 DV_SITE_f8X_COORD | 10295 | Decimal | F8 | * Site Report Info: site location: x
 DV_SITE_f8X_M20_COORDINATE | 10296 | Decimal | F8 | * Site Report Info: M20 site location: x
 DV_SITE_f8Y_COORD | 10297 | Decimal | F8 | * Site Report Info: site location: y
 DV_SITE_f8Y_M20_COORDINATE | 10298 | Decimal | F8 | * Site Report Info: M20 site location: y
 DV_SITE_szAUTOPROGRAM_NAME | 10299 | String | A | * Site Autoprogram name
 DV_SITE_szMASTERSITENAME | 10300 | String | A | * Master Site name
 DV_SITE_szSITENAME | 10301 | String | A | * Site name
 DV_SITE_szSUBSTRATE_LOTID | 10302 | String | A | * Site Substrate Lotid from Bind
 DV_SITE_szSUBSTRATE_WAFERID | 10303 | String | A | * Site Substrate Waferid from Bind
 DV_SITE_u1SITE | 10304 | Integer | U1 | * Site Report Info: site number of site data
 DV_SITE_u1SLOT | 10305 | Integer | U1 | * Site Report Info: Slot of site data
 DV_SITE_u4TOTAL_SITES_PER_WAFER | 10306 | Integer | U4 | * Site Report Info: total number of sites per wafer
 DV_STEP_HEIGHT_f4STEP_HEIGHT | 10311 | Decimal | F4 | * Step Height Analysis: Step height
 DV_STEP_HEIGHT_u2AUTOPROGRAM_COUNT | 10312 | Integer | U2 | * Step Ht Analysis: Autoprogram counter
 DV_SWR_f4KURTOSIS | 10317 | Decimal | F4 | * Sidewall Roughness Analysis: Kurtosis
 DV_SWR_f4RA | 10318 | Decimal | F4 | * Sidewall Roughness Analysis: Ra
 DV_SWR_f4RQ | 10319 | Decimal | F4 | * Sidewall Roughness Analysis: Rq
 DV_SWR_f4SKEWNESS | 10320 | Decimal | F4 | * Sidewall Roughness Analysis: Skewness
 DV_SWR_u2AUTOPROGRAM_COUNT | 10321 | Integer | U2 | * Side Wall Roughness Analysis: Autoprogram counter
 DV_TRENCHDEPTH_f4NUM__MEAS | 10336 | Decimal | F4 | * Trench Depth Analysis: Number of Measurments
 DV_TRENCHDEPTH_f4STD_DEV | 10337 | Decimal | F4 | * Trench Depth Analysis: Standard Deviation
 DV_TRENCHDEPTH_f4TRENCH_DEPTH | 10338 | Decimal | F4 | * Trench Depth Analysis: Trench Depth
 DV_TRENCHDEPTHu2AUTOPROGRAM_COUNT | 10339 | Integer | U2 | * Trench Depth Analysis: Autoprogram counter
 DV_VEH_f4RA | 10340 | Decimal | F4 | * Vertical Edge Height Analysis: VEH
 DV_VEH_f4TYPE | 10341 | Decimal | F4 | * Vertical Edge Height Analysis: Type
 DV_VEH_u2AUTOPROGRAM_COUNT | 10342 | Integer | U2 | * Vertical Edge Height Analysis: Autoprogram counter
 DV_VIA_f4BOTSTDERR | 10343 | Decimal | F4 | * Via Analysis: Bottom Std. Err.
 DV_VIA_f4BOTTOMLEFTCURVATURE | 10344 | Decimal | F4 | * Via Analysis:
 DV_VIA_f4BOTTOMLEFTOFFSET | 10345 | Decimal | F4 | * Via Analysis:
 DV_VIA_f4BOTTOMRIGHTCURVATURE | 10346 | Decimal | F4 | * Via Analysis:
 DV_VIA_f4BOTTOMRIGHTOFFSET | 10347 | Decimal | F4 | * Via Analysis:
 DV_VIA_f4BOTWIDTH | 10348 | Decimal | F4 | * Via Analysis:
 DV_VIA_f4HEIGHT | 10349 | Decimal | F4 | * Via Analysis:
 DV_VIA_f4MIDDLELEFTCURVATURE | 10350 | Decimal | F4 | * Via Analysis:
 DV_VIA_f4MIDDLELEFTOFFSET | 10351 | Decimal | F4 | * Via Analysis:
 DV_VIA_f4MIDDLERIGHTCURVATURE | 10352 | Decimal | F4 | * Via Analysis:
 DV_VIA_f4MIDDLERIGHTOFFSET | 10353 | Decimal | F4 | * Via Analysis:
 DV_VIA_f4MIDSTDERR | 10354 | Decimal | F4 | * Via Analysis:
 DV_VIA_f4MIDWIDTH | 10355 | Decimal | F4 | * Via Analysis:
 DV_VIA_f4TOPLEFTCURVATURE | 10356 | Decimal | F4 | * Via Analysis:
 DV_VIA_f4TOPLEFTOFFSET | 10357 | Decimal | F4 | * Via Analysis:
 DV_VIA_f4TOPRIGHTCURVATURE | 10358 | Decimal | F4 | * Via Analysis:
 DV_VIA_f4TOPRIGHTOFFSET | 10359 | Decimal | F4 | * Via Analysis:
 DV_VIA_f4TOPSTDERR | 10360 | Decimal | F4 | * Via Analysis:
 DV_VIA_f4TOPWIDTH | 10361 | Decimal | F4 | * Via Analysis:
 DV_VIA_u1BOTTOMCDTYPE | 10362 | Integer | U1 | * Via Analysis:
 DV_VIA_u1MIDDLECDTYPE | 10363 | Integer | U1 | * Via Analysis:
 DV_VIA_u1TOPCDTYPE | 10364 | Integer | U1 | * Via Analysis:
 DV_VIA_u2AUTOPROGRAM_COUNT | 10365 | Integer | U2 | * Via Analysis:
 DV_VIA_u2SLOWSCANWINDOWBEGIN | 10366 | Integer | U2 | * Via Analysis:
 DV_VIA_u2SLOWSCANWINDOWEND | 10367 | Integer | U2 | * Via Analysis:
 DV_DEPTH_f4MAX_PEAK_FWHM | 10383 | Decimal | F4 | * Depth Analysis: Max Peak FWHM
 DV_LAYERS_f4LAYER1_FWHM | 10384 | Decimal | F4 | * Layers Analysis: Layer 1 FWHM
 DV_LAYERS_f4LAYER2_FWHM | 10385 | Decimal | F4 | * Layers Analysis: Layer 2 FWHM
 DV_LAYERS_f4LAYER3_FWHM | 10386 | Decimal | F4 | * Layers Analysis: Layer 3 FWHM
 DV_LAYERS_f4LAYER4_FWHM | 10387 | Decimal | F4 | * Layers Analysis: Layer 4 FWHM
 AlarmCode | 30120 | Binary | BI | * SECS Alarm ID for the last alarm event that was triggered (set or cleared)
 AlarmText | 30121 | String | A | * SECS Alarm ID for the last alarm event that was triggered (set or cleared)
 AlarmID | 30122 | Integer | U4 | * SECS Alarm ID for the last alarm event that was triggered (set or cleared)
 PPChangeName | 30140 | String | A | * Name of the recipe that was changed
 PPChangeStatus | 30141 | Integer | I2 | * Type of change made to the recipe
 TosControlState | 30201 | Integer | U1 | * Current control state for the connection
 TosClock | 30202 | String | A | * GEM Clock
 TosProcessState | 30210 | String | A | * ProcessState
 TosPreviousProcessState | 30211 | String | A | * PreviousProcessState
 AlarmsSet | 30220 | Object | L | * List of alarms currently set
 AlarmsEnabled | 30221 | Object | L | * List of alarms currently enabled
 EventsEnabled | 30230 | Object | L | * EventsEnabled
 TosPPID | 30240 | String | A | * Name of the recipe that is being executed
 TosOperatorCommand | 30150 | String | A | * The operator command issued on the equipment by an operator while the equipment was in remote control.
 TosECIDChanged | 30160 | Integer | U4 | * Holds the name of attribute representing an equipment constant that was changed directly on the equipment by an operator
 PRJobID | 40100 | String | A | * Object Identifier for this object (same as name)
 PRJobState | 40102 | Integer | U1 | * A unique sub-state of the job according to the ProcessJob state model.
 TosCarrierID | 87100 | String | A | * Object Identifier for this object (same as name)
 CarrierAccessingStatus | 87101 | Integer | U1 | * Current accessing state of the carrier by the equipment.
 CarrierContentMap | 87103 | Object | L | * Content of the carrier
 TosSlotMap | 87105 | Object | L | * Slot map for the carrier
 CarrierSlotMapStatus | 87106 | Integer | U1 | * Current state of slot map verification.
 CarrierIDStatus | 87109 | Integer | U2 | * Current state of the Carrier ID verification.
 TosPortID | 87110 | Integer | U4 | * Port ID for the latest loadport event
 PortAccessMode | 87111 | Integer | U1 | * Access Mode of the loadport in the latest loadport event
 PortAssociationState | 87112 | Integer | U2 | * Association State of the loadport in the latest loadport event
 PortReservationState | 87113 | Integer | U2 | * Reservation State of the loadport in the latest loadport event
 PortTransferState | 87114 | Integer | U1 | * Load Port Transfer State of the loadport in the latest loadport event
 SubstID | 90101 | String | A | * Object Identifier for this object
 SubstLocID | 90104 | String | A | * Location of the substate
 SubstProcState | 90107 | Integer | U1 | * Processing state of the substrate.
 SubstState | 90109 | Integer | U1 | * Transport state of the substrate.
 SubstLocSubstLocID | 90112 | String | A | * Object Identifier for this object
 SubstLocSubstLocState | 90113 | Integer | U1 | * Substrate location state for the location.
 SubstLocSubstID | 90114 | String | A | * Identifier of the substrate in the substate location
 CtrlJobID | 94100 | String | A | * Object Identifier for this object (same as name)
 CtrlJobState | 94110 | Integer | U1 | * Current state of the ControlJob.
 EstablishCommunicationsTimeout | 30001 | Integer | U2 | * Timeout between attempts to establish communication
 TimeFormat | 30002 | Integer | U1 | * Format to report time in
 AnnotatedEventReport | 30030 | Boolean | BO | * Sending annotated event report
 EnableSpooling | 30050 | Boolean | BO | * Indicates if spooling is currently enabled on the connection
 MaxSpoolTransmit | 30051 | Integer | U4 | * Maximum spooled messages to send in each request
 OverWriteSpool | 30052 | Boolean | BO | * Indicates if old messages are overwritten or new messages are discarded when the spool becomes full
 LoadPort.1.CassetteSize | 87001 | Integer | I4 | * The number of slots in the load port
 LoadPort.2.CassetteSize | 87002 | Integer | I4 | * The number of slots in the load port
 LoadPort.1.WaferSize | 87006 | Integer | I4 | * Gets and sets the WaferSize value for this LP. (Unknown = 0; TwentyFiveMM = 25; FiftyOneMM = 51; SeventySixMM = 76; OneHundredMM = 100; OneHundredTwentyFiveMM = 125; OneHundredTwentySevenMM = 127; OneHundredFiftyMM = 150; OneHundredSixtyMM = 160; TwoHundredMM = 200; ThreeHundredMM = 300; FourHundredFiftyMM = 450)
 LoadPort.2.WaferSize | 87007 | Integer | I4 | * Gets and sets the WaferSize value for this LP. (Unknown = 0; TwentyFiveMM = 25; FiftyOneMM = 51; SeventySixMM = 76; OneHundredMM = 100; OneHundredTwentyFiveMM = 125; OneHundredTwentySevenMM = 127; OneHundredFiftyMM = 150; OneHundredSixtyMM = 160; TwoHundredMM = 200; ThreeHundredMM = 300; FourHundredFiftyMM = 450)
 BypassReadID | 87010 | Boolean | BO | * Bypass Carrier Identification of the Carrier Id when the carrier Id reader is unavailable or not installed
 CarrierHoldControl | 87020 | Integer | U1 | * Carrier Hold Control specifies if the carrier is held at the read/write position until specified by the host
 ClampControl | 87030 | Integer | U1 | * Clamp Control specifies if the carrier remains clamped when in Auto until AMHS commands the release
 CarrierHandoffService.TP1 | 87060 | Integer | I4 | * E84 TP1 timeout
 CarrierHandoffService.TP2 | 87061 | Integer | I4 | * E84 TP2 timeout
 CarrierHandoffService.TP3 | 87062 | Integer | I4 | * E84 TP3 timeout
 CarrierHandoffService.TP4 | 87063 | Integer | I4 | * E84 TP4 timeout
 CarrierHandoffService.TP5 | 87064 | Integer | I4 | * E84 TP5 timeout
 CarrierHandoffService.TP6 | 87065 | Integer | I4 | * E84 TP6 timeout
 CarrierHandoffService.TD0 | 87066 | Integer | I4 | * E84 TD0 timeout
 CarrierHandoffService.TD1 | 87067 | Integer | I4 | * E84 TD1 timeout
 CarrierHandoffService.ManualModeSensorLogicTimeout | 87068 | Integer | I4 | * Configures the sensor logic timeout in seconds to use when the LP is in manual access mode. A value of zero will disable sensor logic timeouts in manual mode.
 CarrierHandoffService.AutoModeSensorLogicTimeout | 87069 | Integer | I4 | * Configures the sensor logic timeout in seconds to use when the LP is in auto access mode. A value of zero will disable sensor logic timeouts in auto mode.
 SubstrateReaderEnabled | 90001 | Boolean | BO | *
 SetUpName | 94001 | String | A | *
 SystemModeService.JobModeControlState | 1000001 | Integer | I4 | * Configures the E30 control state to transition to when PTO enters job mode. (Local = 0; Remote = 1)
 LightTowerService.ControlMode | 1000002 | Integer | I4 | * Indicates who is in control of light tower. (Host = 0; Equipment = 1)
 LightTowerService.BuzzerEnabled | 1000003 | Boolean | BO | * Indicates buzzer enable state.
 SubstSource | 90108 | String | A | * SubstSource Source location of the substrate (for batch locations this includes the location name
 AcquiredID | 90120 | String | A | * AcquiredID ID read from the substrate Id reader on the tool
 DV_DEPTH_f4DEPTH_AT_MAX | 10096 | Decimal | F4 | * Depth Analysis: Depth at max
 DV_DEPTH_f4PEAK_TO_PEAK | 10100 | Decimal | F4 | * Depth Analysis: Peak to peak
 DV_DEPTH_f4TOTAL_PEAKS | 10101 | Decimal | F4 | * Depth Analysis: Total peaks
 DV_DEPTH_u2AUTOPROGRAM_COUNT | 10102 | Integer | U2 | * Depth Analysis: Autoprogram counter
 DV_DEPTH_f4MIN_PEAK_FWHM | 10382 | Decimal | F4 | * Depth Analysis: Min Peak FWHM
 EPT_STATE | 116103 | Integer | U1 | * EPTState The current EPT state of this equipment or EPT module.  This parameter merges the EPTTracker E116 EPTState attribute and the EPTState DV (since EDA identifies instances with the event sourceID)

### Events

#### CE_ROUGHNESS_ANALYSIS_DATA_AVAIL

Event for * ProcessChamber.CE_ROUGHNESS_ANALYSIS_DATA_AVAIL. ID: **15300**
Linked Report Id: **1000**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosCarrierID | 87100 | String | Yes | A | * Object Identifier for this object (same as name)
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 SubstLocID | 90104 | String | Yes | A | * Location of the substate
 SubstID | 90101 | String | Yes | A | * Object Identifier for this object
 CtrlJobID | 94100 | String | Yes | A | * Object Identifier for this object (same as name)
 PRJobID | 40100 | String | Yes | A | * Object Identifier for this object (same as name)
 DV_RECIPE_u1TYPE | 10220 | Integer | Yes | U1 | * Recipe Type
 DV_ROUGH_f410_PT_MEAN_RZ | 10221 | Decimal | Yes | F4 | * Roughness Analysis: 10 pt mean (Rz)
 DV_ROUGH_f4AREA_THRESH_ABS | 10222 | Decimal | Yes | F4 | * Roughness Analysis: Area threshold abs
 DV_ROUGH_f4AREA_THRESH_PCNT_RMS | 10223 | Decimal | Yes | F4 | * Roughness Analysis: Area threshold (%rms)
 DV_ROUGH_f4AREA_THRESH_REF | 10224 | Decimal | Yes | F4 | * Roughness Analysis: Area threshold ref
 DV_ROUGH_f4AV_MAX_DEPTH_RVM | 10225 | Decimal | Yes | F4 | * Roughness Analysis: Av max depth (rvm)
 DV_ROUGH_f4AV_MAX_HEIGHT_RPM | 10226 | Decimal | Yes | F4 | * Roughness Analysis: Average max height rpm
 DV_ROUGH_f4BOX_X_DIM | 10227 | Decimal | Yes | F4 | * Roughness Analysis: Box x dimension
 DV_ROUGH_f4BOX_Y_DIM | 10228 | Decimal | Yes | F4 | * Roughness Analysis: Box y dimension
 DV_ROUGH_f4IMG_MEAN | 10229 | Decimal | Yes | F4 | * Roughness Analysis: Image mean
 DV_ROUGH_f4IMG_PROJ_SURF_AREA | 10230 | Decimal | Yes | F4 | * Roughness Analysis: Image projected surface area
 DV_ROUGH_f4IMG_RA | 10231 | Decimal | Yes | F4 | * Roughness Analysis: Image Ra
 DV_ROUGH_f4IMG_RAW_MEAN | 10232 | Decimal | Yes | F4 | * Roughness Analysis: Image raw mean
 DV_ROUGH_f4IMG_RMAX | 10233 | Decimal | Yes | F4 | * Roughness Analysis: Image Rmax
 DV_ROUGH_f4IMG_RMS_RQ | 10234 | Decimal | Yes | F4 | * Roughness Analysis: Image rms (Rq)
 DV_ROUGH_f4IMG_SAE | 10235 | Decimal | Yes | F4 | * Roughness Analysis: Image SAE
 DV_ROUGH_f4IMG_SURF_AREA | 10236 | Decimal | Yes | F4 | * Roughness Analysis: Image surface area
 DV_ROUGH_f4IMG_SURF_AREA_DIFF | 10237 | Decimal | Yes | F4 | * Roughness Analysis: Image surface area diff (%)
 DV_ROUGH_f4IMG_Z_RANGE | 10238 | Decimal | Yes | F4 | * Roughness Analysis: Image z range
 DV_ROUGH_f4KURTOSIS | 10239 | Decimal | Yes | F4 | * Roughness Analysis: Kurtosis
 DV_ROUGH_f4LINE_DENSITY | 10240 | Decimal | Yes | F4 | * Roughness Analysis: Line density
 DV_ROUGH_f4MAX_DEPTH_RV | 10241 | Decimal | Yes | F4 | * Roughness Analysis: Max depth (Rv)
 DV_ROUGH_f4MAX_HEIGHT_RMAX | 10242 | Decimal | Yes | F4 | * Roughness Analysis: Max height rmax
 DV_ROUGH_f4MAX_PEAK_HEIGHT_RP | 10243 | Decimal | Yes | F4 | * Roughness Analysis: Max peak height (Rp)
 DV_ROUGH_f4MEAN | 10244 | Decimal | Yes | F4 | * Roughness Analysis: Mean
 DV_ROUGH_f4MEAN_ROUGH_RA | 10245 | Decimal | Yes | F4 | * Roughness Analysis: Mean rough ra
 DV_ROUGH_f4PEAK_COUNT | 10246 | Decimal | Yes | F4 | * Roughness Analysis: Peak count
 DV_ROUGH_f4PEAK_THRESH | 10247 | Decimal | Yes | F4 | * Roughness Analysis: Peak threshold
 DV_ROUGH_f4PEAK_THRESH_PCNT_RMS | 10248 | Decimal | Yes | F4 | * Roughness Analysis: Peak threshold (%rms)
 DV_ROUGH_f4PEAK_THRESH_REF | 10249 | Decimal | Yes | F4 | * Roughness Analysis: Peak threshold ref
 DV_ROUGH_f4PROJ_SURF_AREA | 10250 | Decimal | Yes | F4 | * Roughness Analysis: Projected surface area
 DV_ROUGH_f4RAW_MEAN | 10251 | Decimal | Yes | F4 | * Roughness Analysis: Raw mean
 DV_ROUGH_f4RMS_RQ | 10252 | Decimal | Yes | F4 | * Roughness Analysis: Rms Rq
 DV_ROUGH_f4SAE | 10253 | Decimal | Yes | F4 | * Roughness Analysis: SAE
 DV_ROUGH_f4SKEWNESS | 10254 | Decimal | Yes | F4 | * Roughness Analysis: Skewness
 DV_ROUGH_f4SUMMIT_COUNT | 10255 | Decimal | Yes | F4 | * Roughness Analysis: Summit count
 DV_ROUGH_f4SUMMIT_CURV | 10256 | Decimal | Yes | F4 | * Roughness Analysis: Summit curv
 DV_ROUGH_f4SUMMIT_CUTOFF | 10257 | Decimal | Yes | F4 | * Roughness Analysis: Summit cutoff
 DV_ROUGH_f4SUMMIT_CUTOFF_PCNT_RMS | 10258 | Decimal | Yes | F4 | * Roughness Analysis: Summit cutoff (%rms)
 DV_ROUGH_f4SUMMIT_CUTOFF_REF | 10259 | Decimal | Yes | F4 | * Roughness Analysis: Summit cutoff ref
 DV_ROUGH_f4SUMMIT_DENSITY | 10260 | Decimal | Yes | F4 | * Roughness Analysis: Summit density
 DV_ROUGH_f4SUMMIT_DIA | 10261 | Decimal | Yes | F4 | * Roughness Analysis: Summit dia
 DV_ROUGH_f4SUMMIT_RAD_CURV | 10262 | Decimal | Yes | F4 | * Roughness Analysis: Summit rad curv
 DV_ROUGH_f4SUMMIT_SLOPE | 10263 | Decimal | Yes | F4 | * Roughness Analysis: Summit slope
 DV_ROUGH_f4SUMMIT_THRESH | 10264 | Decimal | Yes | F4 | * Roughness Analysis: Summit threshold
 DV_ROUGH_f4SUMMIT_THRESH_PCNT_RMS | 10265 | Decimal | Yes | F4 | * Roughness Analysis: Summit threshold (%rms)
 DV_ROUGH_f4SUMMIT_THRESH_REF | 10266 | Decimal | Yes | F4 | * Roughness Analysis: Summit threshold ref
 DV_ROUGH_f4SURF_AREA | 10267 | Decimal | Yes | F4 | * Roughness Analysis: Surface area
 DV_ROUGH_f4SURF_AREA_DIFF | 10268 | Decimal | Yes | F4 | * Roughness Analysis: Surface area difference
 DV_ROUGH_f4Z_RANGE | 10269 | Decimal | Yes | F4 | * Roughness Analysis: Z range
 DV_ROUGH_u2AUTOPROGRAM_COUNT | 10270 | Integer | Yes | U2 | * Roughness Analysis: Autoprogram counter
 DV_SITE_f4COLUMN | 10289 | Decimal | Yes | F4 | * Site Report Info: site location: column
 DV_SITE_f4ROW | 10290 | Decimal | Yes | F4 | * Site Report Info: site location: row
 DV_SITE_f8X_COORD | 10295 | Decimal | Yes | F8 | * Site Report Info: site location: x
 DV_SITE_f8X_M20_COORDINATE | 10296 | Decimal | Yes | F8 | * Site Report Info: M20 site location: x
 DV_SITE_f8Y_COORD | 10297 | Decimal | Yes | F8 | * Site Report Info: site location: y
 DV_SITE_f8Y_M20_COORDINATE | 10298 | Decimal | Yes | F8 | * Site Report Info: M20 site location: y
 DV_SITE_szAUTOPROGRAM_NAME | 10299 | String | Yes | A | * Site Autoprogram name
 DV_SITE_szMASTERSITENAME | 10300 | String | Yes | A | * Master Site name
 DV_SITE_szSITENAME | 10301 | String | Yes | A | * Site name
 DV_SITE_szSUBSTRATE_LOTID | 10302 | String | Yes | A | * Site Substrate Lotid from Bind
 DV_SITE_szSUBSTRATE_WAFERID | 10303 | String | Yes | A | * Site Substrate Waferid from Bind
 DV_SITE_u1SITE | 10304 | Integer | Yes | U1 | * Site Report Info: site number of site data
 DV_SITE_u1SLOT | 10305 | Integer | Yes | U1 | * Site Report Info: Slot of site data
 DV_SITE_u4TOTAL_SITES_PER_WAFER | 10306 | Integer | Yes | U4 | * Site Report Info: total number of sites per wafer

#### CE_DEPTH_ANALYSIS_DATA_AVAIL

Event for * ProcessChamber.CE_DEPTH_ANALYSIS_DATA_AVAIL. ID: **15301**
Linked Report Id: **1000**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosCarrierID | 87100 | String | Yes | A | * Object Identifier for this object (same as name)
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 SubstLocID | 90104 | String | Yes | A | * Location of the substate
 SubstID | 90101 | String | Yes | A | * Object Identifier for this object
 CtrlJobID | 94100 | String | Yes | A | * Object Identifier for this object (same as name)
 PRJobID | 40100 | String | Yes | A | * Object Identifier for this object (same as name)
 DV_DEPTH_f4DEPTH_AT_MAX | 10096 | Decimal | Yes | F4 | * Depth Analysis: Depth at max
 DV_DEPTH_f4FILTER_CUTOFF | 10097 | Decimal | Yes | F4 | * Depth Analysis: Filter cutoff
 DV_DEPTH_f4MAX_PEAK | 10098 | Decimal | Yes | F4 | * Depth Analysis: Maximum Peak
 DV_DEPTH_f4MIN_PEAK | 10099 | Decimal | Yes | F4 | * Depth Analysis: Minimum Peak
 DV_DEPTH_f4PEAK_TO_PEAK | 10100 | Decimal | Yes | F4 | * Depth Analysis: Peak to peak
 DV_DEPTH_f4TOTAL_PEAKS | 10101 | Decimal | Yes | F4 | * Depth Analysis: Total peaks
 DV_DEPTH_u2AUTOPROGRAM_COUNT | 10102 | Integer | Yes | U2 | * Depth Analysis: Autoprogram counter
 DV_RECIPE_u1TYPE | 10220 | Integer | Yes | U1 | * Recipe Type
 DV_SITE_f4COLUMN | 10289 | Decimal | Yes | F4 | * Site Report Info: site location: column
 DV_SITE_f4ROW | 10290 | Decimal | Yes | F4 | * Site Report Info: site location: row
 DV_SITE_f8X_COORD | 10295 | Decimal | Yes | F8 | * Site Report Info: site location: x
 DV_SITE_f8X_M20_COORDINATE | 10296 | Decimal | Yes | F8 | * Site Report Info: M20 site location: x
 DV_SITE_f8Y_COORD | 10297 | Decimal | Yes | F8 | * Site Report Info: site location: y
 DV_SITE_f8Y_M20_COORDINATE | 10298 | Decimal | Yes | F8 | * Site Report Info: M20 site location: y
 DV_SITE_szAUTOPROGRAM_NAME | 10299 | String | Yes | A | * Site Autoprogram name
 DV_SITE_szMASTERSITENAME | 10300 | String | Yes | A | * Master Site name
 DV_SITE_szSITENAME | 10301 | String | Yes | A | * Site name
 DV_SITE_szSUBSTRATE_LOTID | 10302 | String | Yes | A | * Site Substrate Lotid from Bind
 DV_SITE_szSUBSTRATE_WAFERID | 10303 | String | Yes | A | * Site Substrate Waferid from Bind
 DV_SITE_u1SITE | 10304 | Integer | Yes | U1 | * Site Report Info: site number of site data
 DV_SITE_u1SLOT | 10305 | Integer | Yes | U1 | * Site Report Info: Slot of site data
 DV_SITE_u4TOTAL_SITES_PER_WAFER | 10306 | Integer | Yes | U4 | * Site Report Info: total number of sites per wafer
 DV_DEPTH_f4MIN_PEAK_FWHM | 10382 | Decimal | Yes | F4 | * Depth Analysis: Min Peak FWHM
 DV_DEPTH_f4MAX_PEAK_FWHM | 10383 | Decimal | Yes | F4 | * Depth Analysis: Max Peak FWHM

#### CE_LINE_EDGE_ROUGHNESS_ANALYSIS_DATA_AVAIL

Event for * ProcessChamber.CE_LINE_EDGE_ROUGHNESS_ANALYSIS_DATA_AVAIL. ID: **15314**

There are no Automation Events Properties defined for this Automation Event

#### CE_TRENCHROUGHNESS_ANALYSIS_DATA_AVAIL

Event for * ProcessChamber.CE_TRENCHROUGHNESS_ANALYSIS_DATA_AVAIL. ID: **15331**

There are no Automation Events Properties defined for this Automation Event

#### TosProcessStateChange

Event for * ProcessStateChange. ID: **30200**
Linked Report Id: **100**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosProcessState | 30210 | String | Yes | A | * ProcessState
 TosPreviousProcessState | 30211 | String | Yes | A | * PreviousProcessState

#### ProcessSMTrans1

Event for * Event raised when transition #1 of this state machine occurs. ID: **30201**
Linked Report Id: **100**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosProcessState | 30210 | String | Yes | A | * ProcessState
 TosPreviousProcessState | 30211 | String | Yes | A | * PreviousProcessState

#### ProcessSMTrans2

Event for * Event raised when transition #2 of this state machine occurs. ID: **30202**
Linked Report Id: **100**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosProcessState | 30210 | String | Yes | A | * ProcessState
 TosPreviousProcessState | 30211 | String | Yes | A | * PreviousProcessState

#### ProcessSMTrans3

Event for * Event raised when transition #3 of this state machine occurs. ID: **30203**
Linked Report Id: **100**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosProcessState | 30210 | String | Yes | A | * ProcessState
 TosPreviousProcessState | 30211 | String | Yes | A | * PreviousProcessState

#### ProcessSMTrans10

Event for * Event raised when transition #10 of this state machine occurs. ID: **30210**
Linked Report Id: **100**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosProcessState | 30210 | String | Yes | A | * ProcessState
 TosPreviousProcessState | 30211 | String | Yes | A | * PreviousProcessState

#### ProcessSMTrans11

Event for * Event raised when transition #11 of this state machine occurs. ID: **30211**
Linked Report Id: **100**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosProcessState | 30210 | String | Yes | A | * ProcessState
 TosPreviousProcessState | 30211 | String | Yes | A | * PreviousProcessState

#### TosEquipmentOffline

Event for * Equipment control state changed to the Offline state. ID: **30501**

There are no Automation Events Properties defined for this Automation Event

#### TosControlStateLocal

Event for * Equipment control state changed to the Online Local state. ID: **30502**

There are no Automation Events Properties defined for this Automation Event

#### TosControlStateRemote

Event for * Equipment control state changed to the Online Remote state. ID: **30503**

There are no Automation Events Properties defined for this Automation Event

#### TosMaterialReceived

Event for * The MaterialReceived event is raised when a carrier arrives at a load port.. ID: **30513**
Linked Report Id: **120**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event

#### TosMaterialRemoved

Event for * The MaterialRemoved event is raised when a carrier is removed from a load port.. ID: **30514**
Linked Report Id: **120**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event

#### RecipeChanged

Event for * Creation; modification; or deletion of a process program by the operator. ID: **30540**

There are no Automation Events Properties defined for this Automation Event

#### TosOperatorEquipmentConstantChange

Event for * Operator changed an equipment constant while in remote mode. ID: **30560**

There are no Automation Events Properties defined for this Automation Event

#### TosOperatorCommandIssued

Event for * Operator issued a command while in remote control mode. ID: **30565**

There are no Automation Events Properties defined for this Automation Event

#### PJSM1_NOSTATE_QUEUED

Event for * The processing resource accepts a ProcessJob create request. ID: **40501**
Linked Report Id: **3000**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 40100 | String | Yes | A | * Object Identifier for this object (same as name)
 PRJobState | 40102 | Integer | Yes | U1 | * A unique sub-state of the job according to the ProcessJob state model.

#### PJSM2_QUEUED_SETTINGUP

Event for * The processing resource has been allocated to the ProcessJob. ID: **40502**
Linked Report Id: **3000**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 40100 | String | Yes | A | * Object Identifier for this object (same as name)
 PRJobState | 40102 | Integer | Yes | U1 | * A unique sub-state of the job according to the ProcessJob state model.

#### PJSM3_SETTINGUP_WAITINGFORSTART

Event for * Job material is present and processing resource is ready to start the ProcessJob and PRProcessStart attribute is not set. ID: **40503**
Linked Report Id: **3000**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 40100 | String | Yes | A | * Object Identifier for this object (same as name)
 PRJobState | 40102 | Integer | Yes | U1 | * A unique sub-state of the job according to the ProcessJob state model.

#### PROCESSING_STARTED

Event for * Material is present and ready for processing. PRProcessStart attribute is set. ID: **40504**
Linked Report Id: **3000**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 40100 | String | Yes | A | * Object Identifier for this object (same as name)
 PRJobState | 40102 | Integer | Yes | U1 | * A unique sub-state of the job according to the ProcessJob state model.

#### PJSM5_WAITINGFORSTART_PROCESSING

Event for * Job start directive. ID: **40505**
Linked Report Id: **3000**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 40100 | String | Yes | A | * Object Identifier for this object (same as name)
 PRJobState | 40102 | Integer | Yes | U1 | * A unique sub-state of the job according to the ProcessJob state model.

#### PROCESSING_COMPLETED

Event for * Material processing complete. ID: **40506**
Linked Report Id: **3000**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 40100 | String | Yes | A | * Object Identifier for this object (same as name)
 PRJobState | 40102 | Integer | Yes | U1 | * A unique sub-state of the job according to the ProcessJob state model.

#### PJSM7_PROCESSCOMPLETE_NOSTATE

Event for * Job material departs from the equipment or the ProcessJob becomes extinct because the ProcessJob. ID: **40507**
Linked Report Id: **3000**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 40100 | String | Yes | A | * Object Identifier for this object (same as name)
 PRJobState | 40102 | Integer | Yes | U1 | * A unique sub-state of the job according to the ProcessJob state model.

#### PJSM8_EXECUTING_PAUSING

Event for * The processing resource initiated a process pause action. ID: **40508**
Linked Report Id: **3000**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 40100 | String | Yes | A | * Object Identifier for this object (same as name)
 PRJobState | 40102 | Integer | Yes | U1 | * A unique sub-state of the job according to the ProcessJob state model.

#### PJSM9_PAUSING_PAUSED

Event for * The processing resource paused the job. ID: **40509**
Linked Report Id: **3000**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 40100 | String | Yes | A | * Object Identifier for this object (same as name)
 PRJobState | 40102 | Integer | Yes | U1 | * A unique sub-state of the job according to the ProcessJob state model.

#### PJSM10_PAUSE_EXECUTING

Event for * Process Job resumed event. ID: **40510**
Linked Report Id: **3000**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 40100 | String | Yes | A | * Object Identifier for this object (same as name)
 PRJobState | 40102 | Integer | Yes | U1 | * A unique sub-state of the job according to the ProcessJob state model.

#### PJSM11_EXECUTING_STOPPING

Event for * The processing resource initiated a process stop action. ID: **40511**
Linked Report Id: **3000**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 40100 | String | Yes | A | * Object Identifier for this object (same as name)
 PRJobState | 40102 | Integer | Yes | U1 | * A unique sub-state of the job according to the ProcessJob state model.

#### PJSM12_PAUSE_STOPPING

Event for * The processing resource initiated a process stop action. ID: **40512**
Linked Report Id: **3000**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 40100 | String | Yes | A | * Object Identifier for this object (same as name)
 PRJobState | 40102 | Integer | Yes | U1 | * A unique sub-state of the job according to the ProcessJob state model.

#### PJSM13_EXECUTING_ABORTING

Event for * The processing resource initiated a process abort action. ID: **40513**
Linked Report Id: **3000**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 40100 | String | Yes | A | * Object Identifier for this object (same as name)
 PRJobState | 40102 | Integer | Yes | U1 | * A unique sub-state of the job according to the ProcessJob state model.

#### PJSM14_STOPPING_ABORTING

Event for * The processing resource initiated a process abort action. ID: **40514**
Linked Report Id: **3000**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 40100 | String | Yes | A | * Object Identifier for this object (same as name)
 PRJobState | 40102 | Integer | Yes | U1 | * A unique sub-state of the job according to the ProcessJob state model.

#### PJSM15_PAUSE_ABORTING

Event for * The processing resource initiated a process abort action. ID: **40515**
Linked Report Id: **3000**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 40100 | String | Yes | A | * Object Identifier for this object (same as name)
 PRJobState | 40102 | Integer | Yes | U1 | * A unique sub-state of the job according to the ProcessJob state model.

#### PJSM16_ABORTING_NOSTATE

Event for * The abort procedure is completed. ID: **40516**
Linked Report Id: **3000**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 40100 | String | Yes | A | * Object Identifier for this object (same as name)
 PRJobState | 40102 | Integer | Yes | U1 | * A unique sub-state of the job according to the ProcessJob state model.

#### ProcessingStopped

Event for * The stop procedure is completed. ID: **40517**
Linked Report Id: **3000**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 40100 | String | Yes | A | * Object Identifier for this object (same as name)
 PRJobState | 40102 | Integer | Yes | U1 | * A unique sub-state of the job according to the ProcessJob state model.

#### PJSM18_QUEUED_NOSTATE

Event for * Cancel; Abort; or Stop command received. ID: **40518**
Linked Report Id: **3000**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 40100 | String | Yes | A | * Object Identifier for this object (same as name)
 PRJobState | 40102 | Integer | Yes | U1 | * A unique sub-state of the job according to the ProcessJob state model.

#### TosCarrierClosed

Event for * Carrier access door closed. ID: **87503**

There are no Automation Events Properties defined for this Automation Event

#### TosCarrierClamped

Event for * Carrier is clamped to the load port. ID: **87504**

There are no Automation Events Properties defined for this Automation Event

#### TosCarrierOpened

Event for * Carrier access door opened. ID: **87507**

There are no Automation Events Properties defined for this Automation Event

#### TosCarrierUnclamped

Event for * Carrier unclamped from the load port. ID: **87509**

There are no Automation Events Properties defined for this Automation Event

#### AccessSMTrans01_initialized

Event for * Access mode initialized event. ID: **87521**
Linked Report Id: **3050**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 PortAccessMode | 87111 | Integer | Yes | U1 | * Access Mode of the loadport in the latest loadport event

#### AccessSMTrans02_Auto

Event for * Host or operator has executed a Change Access service with the value of Auto. ID: **87522**
Linked Report Id: **3050**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 PortAccessMode | 87111 | Integer | Yes | U1 | * Access Mode of the loadport in the latest loadport event

#### AccessSMTrans03_Manual

Event for * Host or operator has executed a Change Access service with the value of Manual. ID: **87523**
Linked Report Id: **3050**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 PortAccessMode | 87111 | Integer | Yes | U1 | * Access Mode of the loadport in the latest loadport event

#### COSM1_NOSTATE_CARRIER

Event for * Carrier is instantiated. ID: **87531**
Linked Report Id: **3020**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 CarrierIDStatus | 87109 | Integer | Yes | U2 | * Current state of the Carrier ID verification.
 TosCarrierID | 87100 | String | Yes | A | * Object Identifier for this object (same as name)
 CarrierSlotMapStatus | 87106 | Integer | Yes | U1 | * Current state of slot map verification.

#### COSM2_NOSTATE_IDNOTREAD

Event for * Bind or Carrier Notification service received. ID: **87532**
Linked Report Id: **3020**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 CarrierIDStatus | 87109 | Integer | Yes | U2 | * Current state of the Carrier ID verification.
 TosCarrierID | 87100 | String | Yes | A | * Object Identifier for this object (same as name)
 CarrierSlotMapStatus | 87106 | Integer | Yes | U1 | * Current state of slot map verification.

#### COSM3_NOSTATE_IDWAITINGFORHOST

Event for * A carrier ID not currently existing at the equipment is successfully read or a carrier ID read is successful but an equipment based verification failed. ID: **87533**
Linked Report Id: **3020**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 CarrierIDStatus | 87109 | Integer | Yes | U2 | * Current state of the Carrier ID verification.
 TosCarrierID | 87100 | String | Yes | A | * Object Identifier for this object (same as name)
 CarrierSlotMapStatus | 87106 | Integer | Yes | U1 | * Current state of slot map verification.

#### COSM4_NOSTATE_IDVERIFICATIONOK

Event for * ProceedWithCarrier service is received after an ID read fail or unknown carrier. Carrier is created from the ProceedWithCarrier. ID: **87534**
Linked Report Id: **3020**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 CarrierIDStatus | 87109 | Integer | Yes | U2 | * Current state of the Carrier ID verification.
 TosCarrierID | 87100 | String | Yes | A | * Object Identifier for this object (same as name)
 CarrierSlotMapStatus | 87106 | Integer | Yes | U1 | * Current state of slot map verification.

#### COSM5_NOSTATE_IDVERIFICATIONFAIL

Event for * CancelCarrier service is received after an ID read fail or unknown carrier. Carrier is created from the CancelCarrier. ID: **87535**
Linked Report Id: **3020**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 CarrierIDStatus | 87109 | Integer | Yes | U2 | * Current state of the Carrier ID verification.
 TosCarrierID | 87100 | String | Yes | A | * Object Identifier for this object (same as name)
 CarrierSlotMapStatus | 87106 | Integer | Yes | U1 | * Current state of slot map verification.

#### COSM6_IDNOTREAD_IDVERIFICATIONOK

Event for * Carrier ID is read successfully and the equipment has verified the carrier ID successfully. ID: **87536**
Linked Report Id: **3020**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 CarrierIDStatus | 87109 | Integer | Yes | U2 | * Current state of the Carrier ID verification.
 TosCarrierID | 87100 | String | Yes | A | * Object Identifier for this object (same as name)
 CarrierSlotMapStatus | 87106 | Integer | Yes | U1 | * Current state of slot map verification.

#### COSM7_IDNOTREAD_IDWAITINGFORHOST

Event for * Carrier ID was read unsuccessful. ID: **87537**
Linked Report Id: **3020**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 CarrierIDStatus | 87109 | Integer | Yes | U2 | * Current state of the Carrier ID verification.
 TosCarrierID | 87100 | String | Yes | A | * Object Identifier for this object (same as name)
 CarrierSlotMapStatus | 87106 | Integer | Yes | U1 | * Current state of slot map verification.

#### COSM8_IDWAITINGFORHOST_IDVERIFICATIONOK

Event for * A ProceedWithCarrier service is received. ID: **87538**
Linked Report Id: **3020**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 CarrierIDStatus | 87109 | Integer | Yes | U2 | * Current state of the Carrier ID verification.
 TosCarrierID | 87100 | String | Yes | A | * Object Identifier for this object (same as name)
 CarrierSlotMapStatus | 87106 | Integer | Yes | U1 | * Current state of slot map verification.

#### COSM9_IDWAITINGFORHOST_IDVERIFICATIONFAIL

Event for * CancelCarrier service is received. ID: **87539**
Linked Report Id: **3020**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 CarrierIDStatus | 87109 | Integer | Yes | U2 | * Current state of the Carrier ID verification.
 TosCarrierID | 87100 | String | Yes | A | * Object Identifier for this object (same as name)
 CarrierSlotMapStatus | 87106 | Integer | Yes | U1 | * Current state of slot map verification.

#### COSM10_IDNOTREAD_IDWAITINGFORHOST

Event for * BypassReadID variable is set to False. ID: **87540**
Linked Report Id: **3020**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 CarrierIDStatus | 87109 | Integer | Yes | U2 | * Current state of the Carrier ID verification.
 TosCarrierID | 87100 | String | Yes | A | * Object Identifier for this object (same as name)
 CarrierSlotMapStatus | 87106 | Integer | Yes | U1 | * Current state of slot map verification.

#### COSM11_IDNOTREAD_IDVERIFICATIONOK

Event for * BypassReadID variable is set to True. ID: **87541**
Linked Report Id: **3020**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 CarrierIDStatus | 87109 | Integer | Yes | U2 | * Current state of the Carrier ID verification.
 TosCarrierID | 87100 | String | Yes | A | * Object Identifier for this object (same as name)
 CarrierSlotMapStatus | 87106 | Integer | Yes | U1 | * Current state of slot map verification.

#### COSM12_NOSTATE_SLOTMAPNOTREAD

Event for * Carrier is instantiated. ID: **87542**
Linked Report Id: **3030**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 TosCarrierID | 87100 | String | Yes | A | * Object Identifier for this object (same as name)
 CarrierContentMap | 87103 | Object | Yes | L | * Content of the carrier
 TosSlotMap | 87105 | Object | Yes | L | * Slot map for the carrier

#### COSM13_SLOTMAPNOTREAD_SLOTMAPVERIFICATIONOK

Event for * Slot map is read and verified by the equipment. ID: **87543**
Linked Report Id: **3030**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 TosCarrierID | 87100 | String | Yes | A | * Object Identifier for this object (same as name)
 CarrierContentMap | 87103 | Object | Yes | L | * Content of the carrier
 TosSlotMap | 87105 | Object | Yes | L | * Slot map for the carrier

#### TosSlotMapReadSuccessful

Event for * Slot map read successfully and the equipment is waiting for host verification. ID: **87544**
Linked Report Id: **3030**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 TosCarrierID | 87100 | String | Yes | A | * Object Identifier for this object (same as name)
 CarrierContentMap | 87103 | Object | Yes | L | * Content of the carrier
 TosSlotMap | 87105 | Object | Yes | L | * Slot map for the carrier

#### COSM15_SLOTMAPWAITINGFORHOST_SLOTMAPVERIFICATIONOK

Event for * ProceedWithCarrier service is received. ID: **87545**
Linked Report Id: **3030**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 TosCarrierID | 87100 | String | Yes | A | * Object Identifier for this object (same as name)
 CarrierContentMap | 87103 | Object | Yes | L | * Content of the carrier
 TosSlotMap | 87105 | Object | Yes | L | * Slot map for the carrier

#### COSM16_SLOTMAPWAITINGFORHOST_SLOTMAPVERIFICATIONFAIL

Event for * CancelCarrier service is received. Prepare carrier for unload. ID: **87546**
Linked Report Id: **3030**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 TosCarrierID | 87100 | String | Yes | A | * Object Identifier for this object (same as name)
 CarrierContentMap | 87103 | Object | Yes | L | * Content of the carrier
 TosSlotMap | 87105 | Object | Yes | L | * Slot map for the carrier

#### COSM17_NOSTATE_NOTACCESSED

Event for * Carrier is instantiated. ID: **87547**
Linked Report Id: **3040**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosCarrierID | 87100 | String | Yes | A | * Object Identifier for this object (same as name)
 CarrierAccessingStatus | 87101 | Integer | Yes | U1 | * Current accessing state of the carrier by the equipment.

#### COSM18_NOTACCESSED_INACCESS

Event for * Equipment starts accessing the carrier. ID: **87548**
Linked Report Id: **3040**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosCarrierID | 87100 | String | Yes | A | * Object Identifier for this object (same as name)
 CarrierAccessingStatus | 87101 | Integer | Yes | U1 | * Current accessing state of the carrier by the equipment.

#### COSM19_INACCESS_CARRIERCOMPLETE

Event for * Equipment finishes accessing the carrier normally. ID: **87549**
Linked Report Id: **3040**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosCarrierID | 87100 | String | Yes | A | * Object Identifier for this object (same as name)
 CarrierAccessingStatus | 87101 | Integer | Yes | U1 | * Current accessing state of the carrier by the equipment.

#### COSM20_INACCESS_CARRIERSTOPPED

Event for * Equipment finishes accessing the carrier abnormally. ID: **87550**
Linked Report Id: **3040**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosCarrierID | 87100 | String | Yes | A | * Object Identifier for this object (same as name)
 CarrierAccessingStatus | 87101 | Integer | Yes | U1 | * Current accessing state of the carrier by the equipment.

#### COSM21_CARRIER_NOSTATE

Event for * Carrier is unloaded from the equipment. ID: **87551**
Linked Report Id: **3040**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosCarrierID | 87100 | String | Yes | A | * Object Identifier for this object (same as name)
 CarrierAccessingStatus | 87101 | Integer | Yes | U1 | * Current accessing state of the carrier by the equipment.

#### LPCASM1_NOSTATE_NOTASSOCIATED

Event for * System initialized. ID: **87560**
Linked Report Id: **3080**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 TosCarrierID | 87100 | String | Yes | A | * Object Identifier for this object (same as name)
 PortAssociationState | 87112 | Integer | Yes | U2 | * Association State of the loadport in the latest loadport event

#### LPCASM2_NOTASSOCIATED_ASSOCIATED

Event for * Host sends a Bind when the port is unoccupied. ID: **87561**
Linked Report Id: **3080**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 TosCarrierID | 87100 | String | Yes | A | * Object Identifier for this object (same as name)
 PortAssociationState | 87112 | Integer | Yes | U2 | * Association State of the loadport in the latest loadport event

#### LPCASM3_ASSOCIATED_NOTASSOCIATED

Event for * Host sends a CancelBind to the equipment before the carrier arrives or before a transfer sequence has started. ID: **87562**
Linked Report Id: **3080**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 TosCarrierID | 87100 | String | Yes | A | * Object Identifier for this object (same as name)
 PortAssociationState | 87112 | Integer | Yes | U2 | * Association State of the loadport in the latest loadport event

#### LPCASM4_ASSOCIATED_ASSOCIATED

Event for * Equipment based verification fails and the carrier assumes the ID value from the carrier that is on the load port. ID: **87563**
Linked Report Id: **3080**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 TosCarrierID | 87100 | String | Yes | A | * Object Identifier for this object (same as name)
 PortAssociationState | 87112 | Integer | Yes | U2 | * Association State of the loadport in the latest loadport event

#### LPRSM1_NOSTATE_NOTRESERVED

Event for * System initialized. ID: **87571**
Linked Report Id: **3060**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 PortReservationState | 87113 | Integer | Yes | U2 | * Reservation State of the loadport in the latest loadport event

#### LPRSM2_NOTRESERVED_RESERVED

Event for * Host or operator sends a ReserveAtPort or Bind to the equipment; or a CarrierOut occurs on the equipment. ID: **87572**
Linked Report Id: **3060**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 PortReservationState | 87113 | Integer | Yes | U2 | * Reservation State of the loadport in the latest loadport event

#### LPRSM3_RESERVED_NOTRESERVED

Event for * Host or operator sends a CancelBind or a CancelReservationAtPort; or a carrier arrives at the reserved port. ID: **87573**
Linked Report Id: **3060**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 PortReservationState | 87113 | Integer | Yes | U2 | * Reservation State of the loadport in the latest loadport event

#### LPTSM1_NOSTATE_INSERVICE

Event for * Transfer state initialized event. ID: **87581**
Linked Report Id: **3070**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 PortTransferState | 87114 | Integer | Yes | U1 | * Load Port Transfer State of the loadport in the latest loadport event

#### LPTSM2_OUTOFSERVICE_INSERVICE

Event for * Host or operator has invoked the ChangeServiceState service for this load port with a value of OutOfService. ID: **87582**
Linked Report Id: **3070**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 PortTransferState | 87114 | Integer | Yes | U1 | * Load Port Transfer State of the loadport in the latest loadport event

#### LPTSM3_INSERVICE_OUTOFSERVICE

Event for * Host or operator has invoked the ChangeServiceState service for this load port with a value of InService. ID: **87583**
Linked Report Id: **3070**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 PortTransferState | 87114 | Integer | Yes | U1 | * Load Port Transfer State of the loadport in the latest loadport event

#### LPTSM4_INSERVICE_TRANSFERREADY_OR_TRANSFERBLOCKED

Event for * In Service to Transfer Ready or Transfer Blocked event. ID: **87584**
Linked Report Id: **3070**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 PortTransferState | 87114 | Integer | Yes | U1 | * Load Port Transfer State of the loadport in the latest loadport event

#### LPTSM5_TRANSFERREADY_READYTOLOAD_OR_READYTOUNLOAD

Event for * Transfer Ready to Ready to Load or Ready to Unload event. ID: **87585**
Linked Report Id: **3070**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 PortTransferState | 87114 | Integer | Yes | U1 | * Load Port Transfer State of the loadport in the latest loadport event

#### LPTSM6_READYTOLOAD_TRANSFERBLOCKED

Event for * Equipment recognizes the logical indication of the start of a manual load transfer. ID: **87586**
Linked Report Id: **3070**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 PortTransferState | 87114 | Integer | Yes | U1 | * Load Port Transfer State of the loadport in the latest loadport event

#### LPTSM7_READYTOUNLOAD_TRANSFERBLOCKED

Event for * Equipment recognizes the logical indication of the start of a manual unload transfer. ID: **87587**
Linked Report Id: **3070**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 PortTransferState | 87114 | Integer | Yes | U1 | * Load Port Transfer State of the loadport in the latest loadport event

#### POD_REMOVED

Event for * Manual carrier unload transfer has completed and the Load Port is now empty and ready for load transfer. ID: **87588**
Linked Report Id: **3070**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 PortTransferState | 87114 | Integer | Yes | U1 | * Load Port Transfer State of the loadport in the latest loadport event

#### LPTSM9_TRANSFERBLOCKED_READYTOUNLOAD

Event for * Processing for substrates contained with the carrier has complete; or a CancelCarrier/CancelCarrierAtPort service has been issued. ID: **87589**
Linked Report Id: **3070**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 PortTransferState | 87114 | Integer | Yes | U1 | * Load Port Transfer State of the loadport in the latest loadport event

#### LPTSM10_TRANSFERBLOCKED_TRANSFERREADY

Event for * Transfer was unsuccessful and the carrier has not loaded or unloaded. ID: **87590**
Linked Report Id: **3070**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TosPortID | 87110 | Integer | Yes | U4 | * Port ID for the latest loadport event
 PortTransferState | 87114 | Integer | Yes | U1 | * Load Port Transfer State of the loadport in the latest loadport event

#### SOSM1_NOSTATE_ATSOURCE

Event for * Substrate object is registered. ID: **90501**
Linked Report Id: **3100**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 90101 | String | Yes | A | * Object Identifier for this object
 SubstLocID | 90104 | String | Yes | A | * Location of the substate
 SubstState | 90109 | Integer | Yes | U1 | * Transport state of the substrate.
 SubstProcState | 90107 | Integer | Yes | U1 | * Processing state of the substrate.
 TosClock | 30202 | String | Yes | A | * GEM Clock

#### SOSM2_ATSOURCE_ATWORK

Event for * Substrate is taken from the source substrate location and placed into the equipment. ID: **90502**
Linked Report Id: **3100**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 90101 | String | Yes | A | * Object Identifier for this object
 SubstLocID | 90104 | String | Yes | A | * Location of the substate
 SubstState | 90109 | Integer | Yes | U1 | * Transport state of the substrate.
 SubstProcState | 90107 | Integer | Yes | U1 | * Processing state of the substrate.
 TosClock | 30202 | String | Yes | A | * GEM Clock

#### SOSM3_ATWORK_ATSOURCE

Event for * Substrate has moved to the source substrate location. ID: **90503**
Linked Report Id: **3100**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 90101 | String | Yes | A | * Object Identifier for this object
 SubstLocID | 90104 | String | Yes | A | * Location of the substate
 SubstState | 90109 | Integer | Yes | U1 | * Transport state of the substrate.
 SubstProcState | 90107 | Integer | Yes | U1 | * Processing state of the substrate.
 TosClock | 30202 | String | Yes | A | * GEM Clock

#### SOSM4_ATWORK_ATWORK

Event for * Substrate has moved out from current equipment substrate location towards a new equipment substrate location. ID: **90504**
Linked Report Id: **3100**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 90101 | String | Yes | A | * Object Identifier for this object
 SubstLocID | 90104 | String | Yes | A | * Location of the substate
 SubstState | 90109 | Integer | Yes | U1 | * Transport state of the substrate.
 SubstProcState | 90107 | Integer | Yes | U1 | * Processing state of the substrate.
 TosClock | 30202 | String | Yes | A | * GEM Clock

#### SOSM5_ATWORK_ATDESTINATION

Event for * Substrate is moved to the destination substrate location. ID: **90505**
Linked Report Id: **3100**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 90101 | String | Yes | A | * Object Identifier for this object
 SubstLocID | 90104 | String | Yes | A | * Location of the substate
 SubstState | 90109 | Integer | Yes | U1 | * Transport state of the substrate.
 SubstProcState | 90107 | Integer | Yes | U1 | * Processing state of the substrate.
 TosClock | 30202 | String | Yes | A | * GEM Clock

#### SOSM6_ATDESTINATION_ATWORK

Event for * Substrate is taken from the destination substrate location and placed into the equipment. ID: **90506**
Linked Report Id: **3100**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 90101 | String | Yes | A | * Object Identifier for this object
 SubstLocID | 90104 | String | Yes | A | * Location of the substate
 SubstState | 90109 | Integer | Yes | U1 | * Transport state of the substrate.
 SubstProcState | 90107 | Integer | Yes | U1 | * Processing state of the substrate.
 TosClock | 30202 | String | Yes | A | * GEM Clock

#### SOSM7_ATDESTINATION_NOSTATE

Event for * Substrate is removed from the equipment by a normal transfer process event. ID: **90507**
Linked Report Id: **3100**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 90101 | String | Yes | A | * Object Identifier for this object
 SubstLocID | 90104 | String | Yes | A | * Location of the substate
 SubstState | 90109 | Integer | Yes | U1 | * Transport state of the substrate.
 SubstProcState | 90107 | Integer | Yes | U1 | * Processing state of the substrate.
 TosClock | 30202 | String | Yes | A | * GEM Clock

#### SOSM8_ATDESTINATION_ATSOURCE

Event for * User informs or the equipment detects that the substrate is At Source. ID: **90508**
Linked Report Id: **3100**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 90101 | String | Yes | A | * Object Identifier for this object
 SubstLocID | 90104 | String | Yes | A | * Location of the substate
 SubstState | 90109 | Integer | Yes | U1 | * Transport state of the substrate.
 SubstProcState | 90107 | Integer | Yes | U1 | * Processing state of the substrate.
 TosClock | 30202 | String | Yes | A | * GEM Clock

#### SOSM9_ANYSTATE_NOSTATE

Event for * User informs or equipment detects the substrate has been removed event. ID: **90509**
Linked Report Id: **3100**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 90101 | String | Yes | A | * Object Identifier for this object
 SubstLocID | 90104 | String | Yes | A | * Location of the substate
 SubstState | 90109 | Integer | Yes | U1 | * Transport state of the substrate.
 SubstProcState | 90107 | Integer | Yes | U1 | * Processing state of the substrate.
 TosClock | 30202 | String | Yes | A | * GEM Clock

#### SOSM10_NOSTATE_NEEDSPROCESSING

Event for * Substrate object is created. ID: **90510**
Linked Report Id: **3100**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 90101 | String | Yes | A | * Object Identifier for this object
 SubstLocID | 90104 | String | Yes | A | * Location of the substate
 SubstState | 90109 | Integer | Yes | U1 | * Transport state of the substrate.
 SubstProcState | 90107 | Integer | Yes | U1 | * Processing state of the substrate.
 TosClock | 30202 | String | Yes | A | * GEM Clock

#### WaferStart

Event for * Substrate starts actual processing. ID: **90511**
Linked Report Id: **3100**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 90101 | String | Yes | A | * Object Identifier for this object
 SubstLocID | 90104 | String | Yes | A | * Location of the substate
 SubstState | 90109 | Integer | Yes | U1 | * Transport state of the substrate.
 SubstProcState | 90107 | Integer | Yes | U1 | * Processing state of the substrate.
 TosClock | 30202 | String | Yes | A | * GEM Clock

#### WaferEnd

Event for * Substrate processing complete (successful or unsuccessfull) event. ID: **90512**
Linked Report Id: **3100**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 90101 | String | Yes | A | * Object Identifier for this object
 SubstLocID | 90104 | String | Yes | A | * Location of the substate
 SubstState | 90109 | Integer | Yes | U1 | * Transport state of the substrate.
 SubstProcState | 90107 | Integer | Yes | U1 | * Processing state of the substrate.
 TosClock | 30202 | String | Yes | A | * GEM Clock

#### SOSM13_INPROCESS_NEEDSPROCESSING

Event for * The process associated with the processing instructions was completed and the substrate is to be processed again. ID: **90513**
Linked Report Id: **3100**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 90101 | String | Yes | A | * Object Identifier for this object
 SubstLocID | 90104 | String | Yes | A | * Location of the substate
 SubstState | 90109 | Integer | Yes | U1 | * Transport state of the substrate.
 SubstProcState | 90107 | Integer | Yes | U1 | * Processing state of the substrate.
 TosClock | 30202 | String | Yes | A | * GEM Clock

#### SOSM14_NEEDSPROCESSING_PROCESSINGCOMPLETE

Event for SOSM14_NEEDSPROCESSING_PROCESSINGCOMPLETE. ID: **90514**
Linked Report Id: **3100**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 90101 | String | Yes | A | * Object Identifier for this object
 SubstLocID | 90104 | String | Yes | A | * Location of the substate
 SubstState | 90109 | Integer | Yes | U1 | * Transport state of the substrate.
 SubstProcState | 90107 | Integer | Yes | U1 | * Processing state of the substrate.
 TosClock | 30202 | String | Yes | A | * GEM Clock

#### SOSM17_NOTCONFIRMED_CONFIRMED

Event for * Substrate was successfully read and SubstID provided in the ContentMap matched the AcquiredID read by the equipment. ID: **90517**
Linked Report Id: **3110**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 90101 | String | Yes | A | * Object Identifier for this object
 SubstLocID | 90104 | String | Yes | A | * Location of the substate
 SubstSource | 90108 | String | Yes | A | * SubstSource Source location of the substrate (for batch locations this includes the location name
 AcquiredID | 90120 | String | Yes | A | * AcquiredID ID read from the substrate Id reader on the tool
 TosClock | 30202 | String | Yes | A | * GEM Clock

#### SOSM18_NOTCONFIRMED_WAITINGFORHOST

Event for * Substrate Id was successfully read but the acquired ID is different from the one the equipment used to instantiate the substrate object. ID: **90518**
Linked Report Id: **3110**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 90101 | String | Yes | A | * Object Identifier for this object
 SubstLocID | 90104 | String | Yes | A | * Location of the substate
 SubstSource | 90108 | String | Yes | A | * SubstSource Source location of the substrate (for batch locations this includes the location name
 AcquiredID | 90120 | String | Yes | A | * AcquiredID ID read from the substrate Id reader on the tool
 TosClock | 30202 | String | Yes | A | * GEM Clock

#### SOSM19_NOTCONFIRMED_WAITINGFORHOST

Event for * Substrate ID was successfully read but the acquired ID is different from the one the equipment used to instantiate the substrate object. ID: **90519**
Linked Report Id: **3110**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 90101 | String | Yes | A | * Object Identifier for this object
 SubstLocID | 90104 | String | Yes | A | * Location of the substate
 SubstSource | 90108 | String | Yes | A | * SubstSource Source location of the substrate (for batch locations this includes the location name
 AcquiredID | 90120 | String | Yes | A | * AcquiredID ID read from the substrate Id reader on the tool
 TosClock | 30202 | String | Yes | A | * GEM Clock

#### SOSM20_WAITINGFORHOST_CONFIRMED

Event for * Equipment has received a ProceedWithSubstrate. ID: **90520**
Linked Report Id: **3110**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 90101 | String | Yes | A | * Object Identifier for this object
 SubstLocID | 90104 | String | Yes | A | * Location of the substate
 SubstSource | 90108 | String | Yes | A | * SubstSource Source location of the substrate (for batch locations this includes the location name
 AcquiredID | 90120 | String | Yes | A | * AcquiredID ID read from the substrate Id reader on the tool
 TosClock | 30202 | String | Yes | A | * GEM Clock

#### SOSM21_WAITINGFORHOST_CONFIRMATIONFAILED

Event for * Equipment has received a CancelSubstrate. ID: **90521**
Linked Report Id: **3110**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 90101 | String | Yes | A | * Object Identifier for this object
 SubstLocID | 90104 | String | Yes | A | * Location of the substate
 SubstSource | 90108 | String | Yes | A | * SubstSource Source location of the substrate (for batch locations this includes the location name
 AcquiredID | 90120 | String | Yes | A | * AcquiredID ID read from the substrate Id reader on the tool
 TosClock | 30202 | String | Yes | A | * GEM Clock

#### SLOSM1_UNOCCUPIED_OCCUPIED

Event for * Substrate moves into the substrate location. ID: **90531**
Linked Report Id: **3090**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstLocSubstID | 90114 | String | Yes | A | * Identifier of the substrate in the substate location
 SubstLocSubstLocID | 90112 | String | Yes | A | * Object Identifier for this object
 SubstLocSubstLocState | 90113 | Integer | Yes | U1 | * Substrate location state for the location.

#### SLOSM2_OCCUPIED_UNOCCUPIED

Event for * Substrate moves off the location. ID: **90532**
Linked Report Id: **3090**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstLocSubstID | 90114 | String | Yes | A | * Identifier of the substrate in the substate location
 SubstLocSubstLocID | 90112 | String | Yes | A | * Object Identifier for this object
 SubstLocSubstLocState | 90113 | Integer | Yes | U1 | * Substrate location state for the location.

#### CJSM1_NOSTATE_QUEUED

Event for * Receive Create command from host or operator through operator console. ID: **94501**
Linked Report Id: **3010**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CtrlJobID | 94100 | String | Yes | A | * Object Identifier for this object (same as name)
 CtrlJobState | 94110 | Integer | Yes | U1 | * Current state of the ControlJob.

#### CJSM2_QUEUED_NOSTATE

Event for * Receive Cancel; Abort; or Stop command from host or operator through operator console. ID: **94502**
Linked Report Id: **3010**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CtrlJobID | 94100 | String | Yes | A | * Object Identifier for this object (same as name)
 CtrlJobState | 94110 | Integer | Yes | U1 | * Current state of the ControlJob.

#### CJSM3_QUEUED_SELECTED

Event for * Processing resource has capacity to begin work on the next ControlJob. ID: **94503**
Linked Report Id: **3010**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CtrlJobID | 94100 | String | Yes | A | * Object Identifier for this object (same as name)
 CtrlJobState | 94110 | Integer | Yes | U1 | * Current state of the ControlJob.

#### CJSM4_SELECTED_QUEUED

Event for * Receive De-select command from host or operator through operator console and materials for the ControlJob have not arrived yet. ID: **94504**
Linked Report Id: **3010**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CtrlJobID | 94100 | String | Yes | A | * Object Identifier for this object (same as name)
 CtrlJobState | 94110 | Integer | Yes | U1 | * Current state of the ControlJob.

#### CJSM5_SELECTED_EXECUTING

Event for * Material for the first ProcessJob arrives or in the case where the first ProcessJob does not require material; this transition shall be taken as soon as the processing resource for that. ID: **94505**
Linked Report Id: **3010**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CtrlJobID | 94100 | String | Yes | A | * Object Identifier for this object (same as name)
 CtrlJobState | 94110 | Integer | Yes | U1 | * Current state of the ControlJob.

#### CJSM6_SELECTED_WAITINGFORSTART

Event for Material for the first ProcessJob arrives or in the case where the first ProcessJob does not require material; this transition shall be taken as soon as the processing resource for that. ID: **94506**
Linked Report Id: **3010**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CtrlJobID | 94100 | String | Yes | A | * Object Identifier for this object (same as name)
 CtrlJobState | 94110 | Integer | Yes | U1 | * Current state of the ControlJob.

#### CJSM7_WAITINGFORSTART_EXECUTING

Event for * User Start command received. ID: **94507**
Linked Report Id: **3010**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CtrlJobID | 94100 | String | Yes | A | * Object Identifier for this object (same as name)
 CtrlJobState | 94110 | Integer | Yes | U1 | * Current state of the ControlJob.

#### CJSM8_EXECUTING_PAUSED

Event for * Received Pause command from host or operator through operator console or a ControlJob PauseEvent has occurred. ID: **94508**
Linked Report Id: **3010**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CtrlJobID | 94100 | String | Yes | A | * Object Identifier for this object (same as name)
 CtrlJobState | 94110 | Integer | Yes | U1 | * Current state of the ControlJob.

#### CJSM9_PAUSED_EXECUTING

Event for * Received Resume command from host or operator through operator console. ID: **94509**
Linked Report Id: **3010**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CtrlJobID | 94100 | String | Yes | A | * Object Identifier for this object (same as name)
 CtrlJobState | 94110 | Integer | Yes | U1 | * Current state of the ControlJob.

#### CJSM10_EXECUTING_COMPLETED

Event for * All the ProcessJobs specified for the ControlJob have completed. ID: **94510**
Linked Report Id: **3010**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CtrlJobID | 94100 | String | Yes | A | * Object Identifier for this object (same as name)
 CtrlJobState | 94110 | Integer | Yes | U1 | * Current state of the ControlJob.

#### CJSM11_ACTIVE_COMPLETED

Event for * Equipment received a CJStop command from the host or operator or generated a CJStop internally. ID: **94511**
Linked Report Id: **3010**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CtrlJobID | 94100 | String | Yes | A | * Object Identifier for this object (same as name)
 CtrlJobState | 94110 | Integer | Yes | U1 | * Current state of the ControlJob.

#### CJSM12_ACTIVE_COMPLETED

Event for * Equipment received a CJAbort command from the host or operator or generated a CJAbort internally. ID: **94512**
Linked Report Id: **3010**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CtrlJobID | 94100 | String | Yes | A | * Object Identifier for this object (same as name)
 CtrlJobState | 94110 | Integer | Yes | U1 | * Current state of the ControlJob.

#### CJSM13_COMPLETED_NOSTATE

Event for * ControlJob is deleted. ID: **94513**
Linked Report Id: **3010**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CtrlJobID | 94100 | String | Yes | A | * Object Identifier for this object (same as name)
 CtrlJobState | 94110 | Integer | Yes | U1 | * Current state of the ControlJob.

#### EquipmentEPTStateChangeEvent

Event for * Equipment Performance State Model for Equipment. ID: **116500**
Linked Report Id: **180**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 EPT_STATE | 116103 | Integer | Yes | U1 | * EPTState The current EPT state of this equipment or EPT module.  This parameter merges the EPTTracker E116 EPTState attribute and the EPTState DV (since EDA identifies instances with the event sourceID)

### Commands

#### ABORT

Command for Aborts currently executing E30 job or cancels pending PP-Select.. ID: **ABORT**

There are no Automation Command Parameters defined for this Automation Command

#### CHANGE_TIP

Command for Tip Centering. ID: **CHANGE_TIP**

There are no Automation Command Parameters defined for this Automation Command

#### GetRecipeSites

Command for Comma delimited list of recipe. ID: **GetRecipeSites**

There are no Automation Command Parameters defined for this Automation Command

#### Local

Command for Remote command to change the control state to Local. ID: **Local**

There are no Automation Command Parameters defined for this Automation Command

#### PAUSE

Command for Pauses currently executing E30 job.. ID: **PAUSE**

There are no Automation Command Parameters defined for this Automation Command

#### PP-SELECT

Command for Slots to process.. ID: **PP-SELECT**

There are no Automation Command Parameters defined for this Automation Command

#### Remote

Command for Remote command to change the control state to Remote. ID: **Remote**

There are no Automation Command Parameters defined for this Automation Command

#### RESETERRORPROCESSINGSTATUS

Command for Id of the Substrate whose Error ProcessingStatus should be reset.. ID: **RESETERRORPROCESSINGSTATUS**

There are no Automation Command Parameters defined for this Automation Command

#### ResetLightTower

Command for Turns off all host controlled lights and then recalculates the light tower state. ID: **ResetLightTower**

There are no Automation Command Parameters defined for this Automation Command

#### RESUME

Command for Resumes paused E30 job.. ID: **RESUME**

There are no Automation Command Parameters defined for this Automation Command

#### RUN_UTILITY_RECIPE

Command for Utility Recipe Type. ID: **RUN_UTILITY_RECIPE**

There are no Automation Command Parameters defined for this Automation Command

#### RUNCOMPLETE

Command for Multiple Run Mode Run Complete command.. ID: **RUNCOMPLETE**

There are no Automation Command Parameters defined for this Automation Command

#### RUNRECIPE

Command for Registration Required. ID: **RUNRECIPE**

There are no Automation Command Parameters defined for this Automation Command

#### RUNSTART

Command for Align Angle. ID: **RUNSTART**

There are no Automation Command Parameters defined for this Automation Command

#### SetLightTower

Command for Off = 0; On = 1; Flash = 2; FlashFast = 3; FlashSlow = 4; Unknown = -1. ID: **SetLightTower**

There are no Automation Command Parameters defined for this Automation Command

#### START

Command for Starts pending E30 job.. ID: **START**

There are no Automation Command Parameters defined for this Automation Command

#### STOP

Command for Stops currently executing E30 job or cancels pending PP-Select.. ID: **STOP**

There are no Automation Command Parameters defined for this Automation Command

#### TransferComplete

Command for The load port to recover. ID: **TransferComplete**

There are no Automation Command Parameters defined for this Automation Command

#### TransferRetry

Command for The load port to recover. ID: **TransferRetry**

There are no Automation Command Parameters defined for this Automation Command

## Automation Driver Definition - HermosLFM4xReaderDriver
Information on this driver is contained on an additional driver related document.


# Name Generators

## Custom Name Generators

The following name generators were created to support customer requirements.

| Table                     | Description       |
| ------                    | ------            |
| [CustomGenerateSplitLotNames](/cmf.custom.help/techspec>artifacts>namegenerators>CustomGenerateSplitLotNames) | This generator is executed when a Material is split, and generates a name for the splitted Materials. It calls the DEE action **[CustomGenerateSplitLotNames](/AMSOsram/tecspecs>artifacts>deeactions>CustomGenerateSplitLotNames)**. |
| [Custom Generate Split Lot Names Name Generator](/cmf.custom.help/techspec>artifacts>namegenerators>CustomGenerateSplitLotNamesNameGenerator) | This generator is used when spliting a Material.

On the splits the name generation should always keep the original lot name adding a 2-digit counter:

* T2143001 -> T2143001.01 [Original Lot Name].[2 digit counter] |
| [CustomProductionLotNameGenerator](/cmf.custom.help/techspec>artifacts>namegenerators>CustomProductionLotNameGenerator) | Custom Name Generator for Production Lot. |



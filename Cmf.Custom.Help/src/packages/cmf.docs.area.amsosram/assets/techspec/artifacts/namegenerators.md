# Name Generators

## Custom Name Generators

The following name generators were created to support customer requirements.

| Table                     | Description       |
| ------                    | ------            |
| [Custom Generate Split Lot Names Name Generator](/AMSOsram/techspec>artifacts>namegenerators>CustomGenerateSplitLotNamesNameGenerator) | This generator is used when spliting a Material.

On the splits the name generation should always keep the original lot name adding a 2-digit counter:

* T2143001 -> T2143001.01 [Original Lot Name].[2 digit counter] | 
| [Custom Production Lot Name Generator](/AMSOsram/techspec>artifacts>namegenerators>CustomProductionLotNameGenerator) | Name generator for Production Lot Names. This generator is used when creating the main material.

The name generation should be as follows:
- [Site][2 digits for the fiscal year][2 digits for the fiscal week][Alphanumeric running number]
- R2143001….. R214300A
For the Site it is used the constant T.

*Note:*
Alphanumeric need to exclude the following letters B,D,E,G,I,J,K,O,P,Q,S,V,W,Y,Z | 



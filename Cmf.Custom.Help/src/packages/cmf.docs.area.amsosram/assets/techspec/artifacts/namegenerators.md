# Name Generators

## Custom Name Generators

The following name generators were created to support customer requirements.

| Table                     | Description       |
| ------                    | ------            |
| [Custom Generate Split Lot Names Name Generator](/AMSOsram/techspec>artifacts>namegenerators>CustomGenerateSplitLotNamesNameGenerator) | This generator is used when spliting a Material.

On the splits the name generation should take the first 8 digits of the parent material followed by two digits that are alphanumeric running numbers:

* UA00000100 -> UA00000101 [8 digits of parent material][2 digit alphanumeric running number] | 
| [Custom Production Lot Name Generator](/AMSOsram/techspec>artifacts>namegenerators>CustomProductionLotNameGenerator) | Name generator for Production Lot Names. This generator is used when creating the main material.

The name generation should be as follows:
- [First Letter of Site ID][First Letter of Facility ID][6 digits of alphanumeric running number][2 digit alphanumeric split lot counter]
- UA00000100
The split lot counter is allways 00.

*Note:*
Alphanumeric need to exclude the following letters B,D,E,G,I,J,K,O,P,Q,S,V,W,Y,Z | 



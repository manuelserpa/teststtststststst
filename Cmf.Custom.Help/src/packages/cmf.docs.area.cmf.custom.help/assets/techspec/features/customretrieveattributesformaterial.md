# CustomRetrieveAttributesForMaterial

## Requirement Specification
Mechanism to return one or more *Materials* attributes and it's *SubMaterials* attributes.

## Design Specification

### Relevant Artifacts
The table below describes the properties for this entity type:

Name          | Type      | Is Mandatory | Data Type | Description 
:------------ | :-------- | :----------: | :-------- | :-----------

### How it works
The system processes the input and gradually fills up the output datastructure. From this the system serializes the XML output that is returned as a string.

* Example Input:  
		"$type": "Cmf.Custom.amsOSRAM.Orchestration.InputObjects.CustomGetMaterialAttributesInput, Cmf.Custom.amsOSRAM.Orchestration",  
		"MaterialList":"TestMaterialWithAttributes",  
		"AttributeList": "",  
		"IncludeSubMaterials": "",  
		"SubMaterialAttributeList": ""  
	
* Example output:
```xml
<?xml version=\"1.0\" encoding=\"utf-16\"?>
<CustomGetMaterialAttributes xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">
	<Material>
		<Name>TestMaterialWithAttributes</Name>
		<Form>Lot</Form>
		<Attributes>
			<Attribute Name=\"GoodsReceiptNo\">70112202</Attribute>
			<Attribute Name=\"GoodsReceiptDate\">20221107</Attribute>
		</Attributes>
		<SubMaterials>
			<Material>
				<Name>TestSubMaterialWithAttributes_1</Name>
				<Form>Logical Wafer</Form>
				<Attributes>
					<Attribute Name=\"GoodsReceiptNo\">41112202</Attribute>
					<Attribute Name=\"GoodsReceiptDate\">20221114</Attribute>
				</Attributes>
			</Material>
			<Material>
				<Name>TestSubMaterialWithAttributes_2</Name>
				<Form>Logical Wafer</Form>
				<Attributes>
					<Attribute Name=\"GoodsReceiptNo\">20221114</Attribute>
					<Attribute Name=\"GoodsReceiptDate\">20221114</Attribute>
				</Attributes>
			</Material>
		</SubMaterials>
	</Material>
</CustomGetMaterialAttributes>
```

### Assumptions
N/A

## Work items

Upon receiving an input the system will create an XML output with the requested materials attributes.

User Story | Type | Title | Description|
:---|:---|:---|:---|
194972 | User Story | CM Outbound Interface to retrieve Wafer Certificate Parameter values - MaterialAttributes/GetByMaterial | 
import { ComponentFramework } from "cmf.core/src/core";
import Cmf, { CMFMap, System } from "cmf.lbos";

export interface CustomGetResourceLoadPortsDataQueryInputs {
    sourceEntityId: string
    containerAssociationType?: string
    loadPortStateModelStateId?: string
}

export const CustomGetResourceLoadPortsDataQueryParameters = ({
    sourceEntityId, containerAssociationType, loadPortStateModelStateId
}: CustomGetResourceLoadPortsDataQueryInputs): Cmf.Foundation.BusinessObjects.QueryObject.QueryParameterCollection => {
    const queryParameters = new Cmf.Foundation.BusinessObjects.QueryObject.QueryParameterCollection();

    const parameter_0 = new Cmf.Foundation.BusinessObjects.QueryObject.QueryParameter();
    parameter_0.Name = "ParentResource";
    parameter_0.Value = sourceEntityId;
    parameter_0.Direction = System.Data.ParameterDirection.Input;
    parameter_0.FilterType = Cmf.Foundation.BusinessObjects.QueryObject.Enums.FilterType.Normal;
    parameter_0.IsOptional = false;

    queryParameters.push(parameter_0);

    if (containerAssociationType) {
        const parameter_1 = new Cmf.Foundation.BusinessObjects.QueryObject.QueryParameter();
        parameter_1.Name = "ContainerResourceAssociationType";
        parameter_1.Value = containerAssociationType;
        parameter_1.Direction = System.Data.ParameterDirection.Input;
        parameter_1.FilterType = Cmf.Foundation.BusinessObjects.QueryObject.Enums.FilterType.Normal;
        parameter_1.IsOptional = true

        queryParameters.push(parameter_1);
    }

    if (loadPortStateModelStateId) {
        const parameter_2 = new Cmf.Foundation.BusinessObjects.QueryObject.QueryParameter();
        parameter_2.Name = "LoadPortStateModelStateId";
        parameter_2.Value = loadPortStateModelStateId;
        parameter_2.Direction = System.Data.ParameterDirection.Input;
        parameter_2.FilterType = Cmf.Foundation.BusinessObjects.QueryObject.Enums.FilterType.Normal;
        parameter_2.IsOptional = true;

        queryParameters.push(parameter_2);
    }

    return queryParameters;
}

export const CustomGetResourceLoadPortsData = async (
    framework: ComponentFramework, parameters: CustomGetResourceLoadPortsDataQueryInputs) => {
    const input = new Cmf.Foundation.BusinessOrchestration.QueryManagement.InputObjects.ExecuteQueryByNameInput();
    input.Name = "CustomGetResourceLoadPortsData";
    input.QueryParameters = CustomGetResourceLoadPortsDataQueryParameters(parameters);

    const output = await framework.sandbox.lbo.call(input) as
        Cmf.Foundation.BusinessOrchestration.QueryManagement.OutputObjects.ExecuteQueryOutput;

    const loadPorts = new Cmf.Navigo.BusinessObjects.SubResourceCollection();

    if (output == null || output.NgpDataSet == null || output.NgpDataSet["T_Result"] == null) {
        return loadPorts;
    }

    for (const row of output.NgpDataSet["T_Result"]) {
        const loadPort = new Cmf.Navigo.BusinessObjects.SubResource();
        loadPort.Id = row["TargetEntityId"];
        loadPort.Name = row["TargetEntityName"];

        const attributes = new Cmf.Foundation.BusinessObjects.AttributeCollection();
        attributes.set("IsLoadPortInUse", row["TargetEntityIsLoadPortInUse"] === 'True')

        loadPort.Attributes = attributes;
        loadPorts.push(loadPort);
    }

    return loadPorts;
}

export const CustomGetProductsWithProductGroupQuery = () => {
    const fieldCollection = new Cmf.Foundation.BusinessObjects.QueryObject.FieldCollection();

    // Field field_0
    const field_0 = new Cmf.Foundation.BusinessObjects.QueryObject.Field();
    field_0.Alias = "Id";
    field_0.ObjectName = "Product";
    field_0.ObjectAlias = "Product_1";
    field_0.IsUserAttribute = false;
    field_0.Name = "Id";
    field_0.Position = 0;
    field_0.Sort = Cmf.Foundation.Common.FieldSort.NoSort;

    // Field field_1
    const field_1 = new Cmf.Foundation.BusinessObjects.QueryObject.Field();
    field_1.Alias = "DefinitionId";
    field_1.ObjectName = "Product";
    field_1.ObjectAlias = "Product_1";
    field_1.IsUserAttribute = false;
    field_1.Name = "DefinitionId";
    field_1.Position = 1;
    field_1.Sort = Cmf.Foundation.Common.FieldSort.NoSort;

    // Field field_2
    const field_2 = new Cmf.Foundation.BusinessObjects.QueryObject.Field();
    field_2.Alias = "Revision";
    field_2.ObjectName = "Product";
    field_2.ObjectAlias = "Product_1";
    field_2.IsUserAttribute = false;
    field_2.Name = "Revision";
    field_2.Position = 2;
    field_2.Sort = Cmf.Foundation.Common.FieldSort.NoSort;

    // Field field_3
    const field_3 = new Cmf.Foundation.BusinessObjects.QueryObject.Field();
    field_3.Alias = "Name";
    field_3.ObjectName = "Product";
    field_3.ObjectAlias = "Product_1";
    field_3.IsUserAttribute = false;
    field_3.Name = "Name";
    field_3.Position = 3;
    field_3.Sort = Cmf.Foundation.Common.FieldSort.NoSort;

    // Field field_4
    const field_4 = new Cmf.Foundation.BusinessObjects.QueryObject.Field();
    field_4.Alias = "ProductGroup_Id";
    field_4.ObjectName = "ProductGroup";
    field_4.ObjectAlias = "Product_ProductGroup_2";
    field_4.IsUserAttribute = false;
    field_4.Name = "Id";
    field_4.Position = 4;
    field_4.Sort = Cmf.Foundation.Common.FieldSort.NoSort;

    // Field field_5
    const field_5 = new Cmf.Foundation.BusinessObjects.QueryObject.Field();
    field_5.Alias = "ProductGroup_Name";
    field_5.ObjectName = "ProductGroup";
    field_5.ObjectAlias = "Product_ProductGroup_2";
    field_5.IsUserAttribute = false;
    field_5.Name = "Name";
    field_5.Position = 6;
    field_5.Sort = Cmf.Foundation.Common.FieldSort.NoSort;

    // Field field_6
    const field_6 = new Cmf.Foundation.BusinessObjects.QueryObject.Field();
    field_6.Alias = "ProductGroup_Revision";
    field_6.ObjectName = "ProductGroup";
    field_6.ObjectAlias = "Product_ProductGroup_2";
    field_6.IsUserAttribute = false;
    field_6.Name = "Revision";
    field_6.Position = 7;
    field_6.Sort = Cmf.Foundation.Common.FieldSort.NoSort;

    // Field field_7
    const field_7: Cmf.Foundation.BusinessObjects.QueryObject.Field = new Cmf.Foundation.BusinessObjects.QueryObject.Field();
    field_7.Alias = "Description";
    field_7.ObjectName = "Product";
    field_7.ObjectAlias = "Product_1";
    field_7.IsUserAttribute = false;
    field_7.Name = "Description";
    field_7.Position = 8;
    field_7.Sort = Cmf.Foundation.Common.FieldSort.NoSort;

    const filterCollection = new Cmf.Foundation.BusinessObjects.QueryObject.FilterCollection();
    fieldCollection.push(field_0);
    fieldCollection.push(field_1);
    fieldCollection.push(field_2);
    fieldCollection.push(field_3);
    fieldCollection.push(field_4);
    fieldCollection.push(field_5);
    fieldCollection.push(field_6);
    fieldCollection.push(field_7);

    // Relation relation_0
    const relation_0 = new Cmf.Foundation.BusinessObjects.QueryObject.Relation();
    relation_0.Alias = "";
    relation_0.IsRelation = false;
    relation_0.Name = "";
    relation_0.SourceEntity = "Product";
    relation_0.SourceEntityAlias = "Product_1",
        relation_0.SourceJoinType = Cmf.Foundation.BusinessObjects.QueryObject.Enums.JoinType.InnerJoin;
    relation_0.SourceProperty = "ProductGroupId";
    relation_0.TargetEntity = "ProductGroup";
    relation_0.TargetEntityAlias = "Product_ProductGroup_2";
    relation_0.TargetJoinType = Cmf.Foundation.BusinessObjects.QueryObject.Enums.JoinType.InnerJoin;
    relation_0.TargetProperty = "Id";

    const relationCollection = new Cmf.Foundation.BusinessObjects.QueryObject.RelationCollection();
    relationCollection.push(relation_0);

    const query = new Cmf.Foundation.BusinessObjects.QueryObject.QueryObject();
    query.Description = "";
    query.EntityTypeName = "Product";
    query.Name = "CustomGetProductsWithProductGroup";
    query.Query = new Cmf.Foundation.BusinessObjects.QueryObject.Query();
    query.Query.Distinct = false;
    query.Query.Filters = filterCollection;
    query.Query.Fields = fieldCollection;
    query.Query.Relations = relationCollection;

    return query;
}

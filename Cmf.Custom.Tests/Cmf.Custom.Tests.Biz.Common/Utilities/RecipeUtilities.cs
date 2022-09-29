using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.SmartTables;
using Cmf.Foundation.BusinessOrchestration.TableManagement.InputObjects;
using Cmf.Navigo.BusinessObjects;

namespace Cmf.Custom.Tests.Biz.Common.Utilities
{
    public static class RecipeUtilities
    {
        public static void TerminateRecipeHierarchy(string recipeName)
        {
            if (string.IsNullOrWhiteSpace(recipeName))
            {
                return;
            }

            var recipe = new Recipe()
            {
                Name = recipeName
            };

            if (recipe.ObjectExists())
            {
                recipe.Load();

                var output = new Cmf.Navigo.BusinessOrchestration.RecipeManagement.InputObjects.LoadRecipeSubRecipesInput
                {
                    Recipe = recipe
                }.LoadRecipeSubRecipesSync();

                if (recipe.UniversalState != Foundation.Common.Base.UniversalState.Terminated)
                {
                    recipe.Terminate();
                }

                foreach (var subRecipe in output.Recipe.SubRecipes)
                {
                    TerminateRecipeHierarchy(subRecipe.ChildRecipe.Name);
                }
            }
        }

        public static void CreateMESRecipeIfItDoesNotExist(string resourceName, string recipeName, string ppid, string serviceName,
        string recipePath = null,
        List<Dictionary<string, object>> subRecipes = null,
        SubRecipeCollection subRecipeCollection = null,
        bool isTopMostRecipe = true,
        bool clearRecipeContext = true,
        Dictionary<Parameter, object> parameterCollection = null)
        {

            Resource resource = new Resource()
            {
                Name = resourceName
            };
            resource.Load();
            resource.IsRecipeManagementEnabled = true;
            resource.VerifyMaterialRecipeAtTrackIn = false;
            resource.Save();

            var recipe = new Recipe()
            {
                Name = recipeName,
                ResourceRecipeName = ppid
            };


            #region Create Recipe if it does not exist
            if (!recipe.ObjectExists())
            {
                if (!String.IsNullOrWhiteSpace(recipePath))
                {
                    byte[] binaryBody;
                    using (FileStream fileStream = File.OpenRead(recipePath))
                    {
                        binaryBody = new byte[fileStream.Length];
                        fileStream.Read(binaryBody, 0, binaryBody.Length);
                    }
                    recipe.Body = new RecipeBody()
                    {
                        Body = binaryBody
                    };

                    recipe.BodySource = RecipeBodySource.EquipmentSupplier;
                    recipe.BodyFormat = RecipeBodyFormat.Binary;
                }
                else
                {
                    recipe.BodySource = RecipeBodySource.None;
                }

                recipe.Type = "Production";

                recipe.ChangeSet = new Cmf.Foundation.BusinessOrchestration.ChangeSetManagement.InputObjects.CreateChangeSetInput()
                {
                    ChangeSet = new ChangeSet
                    {
                        Name = Cmf.TestScenarios.Others.Utilities.NewGuid(),
                        Type = "General",
                        Description = "Change Set Created for Master Data Loader",
                        MakeEffectiveOnApproval = true
                    }
                }.CreateChangeSetSync().ChangeSet;

                recipe.Create();

                recipe.Load();

                if (subRecipes != null)
                {
                    var subRecipeCollectionChildren = new SubRecipeCollection();

                    foreach (var subRecipe in subRecipes)
                    {
                        var resourceNameSubRecipe = "";
                        if (subRecipe.ContainsKey("resourceName"))
                        {
                            resourceNameSubRecipe = subRecipe["resourceName"] as string;
                        }

                        var recipeNameSubRecipe = "";
                        if (subRecipe.ContainsKey("recipeName"))
                        {
                            recipeNameSubRecipe = subRecipe["recipeName"] as string;
                        }

                        var ppidSubRecipe = "";
                        if (subRecipe.ContainsKey("ppid"))
                        {
                            ppidSubRecipe = subRecipe["ppid"] as string;
                        }

                        var serviceNameSubRecipe = "";
                        if (subRecipe.ContainsKey("serviceName"))
                        {
                            serviceNameSubRecipe = subRecipe["serviceName"] as string;
                        }

                        var recipePathSubRecipe = "";
                        if (subRecipe.ContainsKey("recipePath"))
                        {
                            recipePathSubRecipe = subRecipe["recipePath"] as string;
                        }

                        List<Dictionary<string, object>> subRecipesSubRecipe = null;
                        if (subRecipe.ContainsKey("subRecipes"))
                        {
                            subRecipesSubRecipe = subRecipe["subRecipes"] as List<Dictionary<string, object>>;
                        }

                        var subParameterCollection = new Dictionary<Parameter, object>();
                        if (subRecipe.ContainsKey("parameterCollection"))
                        {
                            subParameterCollection = subRecipe["parameterCollection"] as Dictionary<Parameter, object>;
                        }

                        //Create
                        CreateMESRecipeIfItDoesNotExist(resourceNameSubRecipe,
                            recipeNameSubRecipe,
                            ppidSubRecipe,
                            serviceNameSubRecipe,
                            recipePathSubRecipe,
                            subRecipesSubRecipe,
                            subRecipeCollectionChildren,
                            false, parameterCollection: subParameterCollection);
                    }


                    var subRecipesCall = new Cmf.Navigo.BusinessOrchestration.RecipeManagement.InputObjects.FullUpdateRecipeInput
                    {
                        FullUpdateRecipeParameters = new FullUpdateRecipeParameters
                        {
                            SubRecipesToAdd = subRecipeCollectionChildren
                        },
                        Recipe = recipe
                    }.FullUpdateRecipeSync();

                }

                if (parameterCollection != null && parameterCollection.Count > 0)
                {
                    var recipeParameterCollection = new EntityRelationCollection();
                    CmfEntityRelationCollection relEntityCollection = new CmfEntityRelationCollection();
                    var order = 1;
                    foreach (var parameterKeyValue in parameterCollection)
                    {
                        var parameter = parameterKeyValue.Key;
                        var value = parameterKeyValue.Value;
                        if (!parameter.ObjectExists())
                        {
                            parameter.ParameterScope = ParameterScope.Recipe;
                            parameter.Create();
                        }
                        else
                        {
                            parameter.Load();
                        }

                        var recipeParameters = new List<RecipeParameter>();

                        var recipeParameter = new RecipeParameter();
                        recipeParameter.Type = RecipeParameterType.Input;
                        recipeParameter.Value = value;

                        recipe.Load();
                        recipeParameter.SourceEntity = recipe;
                        recipeParameter.TargetEntity = parameter;

                        recipeParameter.IsOverridable = true;
                        recipeParameter.IsSource = true;
                        recipeParameter.LockType = Foundation.Common.LockType.FullAccess;
                        recipeParameter.Order = order;
                        recipeParameterCollection.Add(recipeParameter);
                        order++;
                        //recipeParameter.SaveRelation();
                    }

                    recipe.Load();
                    relEntityCollection.Add("RecipeParameter", recipeParameterCollection);
                    recipe.RelationCollection = relEntityCollection;


                    var recipeParameterInput = new Cmf.Navigo.BusinessOrchestration.RecipeManagement.InputObjects.FullUpdateRecipeInput
                    {
                        FullUpdateRecipeParameters = new FullUpdateRecipeParameters
                        {
                            RelationsToAdd = recipeParameterCollection
                        },
                        Recipe = recipe
                    }.FullUpdateRecipeSync();

                }



                if (subRecipeCollection != null)
                {
                    var subRecipe = new SubRecipe()
                    {
                        ChildRecipe = recipe,
                        Order = subRecipeCollection.Count + 1
                    };

                    subRecipeCollection.Add(subRecipe);
                }

                recipe.ChangeSet.Approve();
            }
            #endregion

            if (isTopMostRecipe)
            {
                SmartTable table = new GetSmartTableByNameInput { SmartTableName = "RecipeContext", LoadData = true }.GetSmartTableByNameSync().SmartTable;

                //table = new LoadSmartTableDataInput { SmartTable = (table as SmartTable) }.LoadSmartTableDataSync().SmartTable;
                DataSet ds = Cmf.TestScenarios.Others.Utilities.ToDataSet((table as SmartTable).Data);

                if (clearRecipeContext)
                {

                    var drToDelete = ds.Tables[0].AsEnumerable().FirstOrDefault(drow => drow["Service"].ToString() == serviceName
                    && drow["Resource"].ToString() == resourceName
                    && string.IsNullOrEmpty(drow["Product"].ToString())
                    && string.IsNullOrEmpty(drow["ProductGroup"].ToString())
                    && string.IsNullOrEmpty(drow["Flow"].ToString())
                    && string.IsNullOrEmpty(drow["Material"].ToString())
                    && string.IsNullOrEmpty(drow["MaterialType"].ToString())
                    && string.IsNullOrEmpty(drow["ResourceType"].ToString())
                    && string.IsNullOrEmpty(drow["Model"].ToString())
                    && string.IsNullOrEmpty(drow["RunningMode"].ToString()));
                    if (drToDelete != null)
                    {
                        var dsToDelete = ds.Clone();
                        //dsToDelete.Tables[0].Rows.Clear();
                        //dsToDelete.Tables[0].AcceptChanges();

                        dsToDelete.Tables[0].Rows.Add(drToDelete.ItemArray);

                        var inputClear = new RemoveSmartTableRowsInput { SmartTable = table, Table = Cmf.TestScenarios.Others.Utilities.FromDataSet(dsToDelete) };
                        inputClear.RemoveSmartTableRowsSync();
                    }
                }

                if (ds.Tables[0].Rows.Count > 0)
                {
                    ds.Tables[0].Rows.Clear();
                    ds.Tables[0].AcceptChanges();
                }

                DataRow dr = ds.Tables[0].NewRow();
                dr["RecipeContextId"] = 0; // Stored Proc that manages the ST data filters by Id = 0 , not -1
                dr["LastServiceHistoryId"] = -1;
                dr["LastOperationHistorySeq"] = -1;
                dr["Service"] = serviceName;
                dr["Resource"] = resourceName;
                dr["Recipe"] = recipeName;

                ds.Tables[0].Rows.Add(dr);



                try
                {


                    var input = new InsertOrUpdateSmartTableRowsInput { SmartTable = table, Table = Cmf.TestScenarios.Others.Utilities.FromDataSet(ds) };
                    var insert = input.InsertOrUpdateSmartTableRowsSync();
                }
                catch (Exception)
                {
                }
            }
        }


        public static void CreateMESRecipeFromEquipmentBody(string resourceName, string recipeName, string ppid)
        {
            var resource = new Resource() { Name = resourceName };
            resource.Load();
            resource.AutomationMode = ResourceAutomationMode.Online;
            resource.AutomationAddress = ".";
            resource.IsRecipeManagementEnabled = true;
            resource.VerifyMaterialRecipeAtTrackIn = false;
            resource.IsDownloadRecipeCapable = true;
            resource.Save();

            var recipe = new Recipe()
            {
                Name = recipeName,
                ResourceRecipeName = ppid
            };

            recipe.BodySource = RecipeBodySource.None;
            recipe.BodyFormat = RecipeBodyFormat.Binary;


            recipe.Type = "Production";


            recipe.ChangeSet = new Cmf.Foundation.BusinessOrchestration.ChangeSetManagement.InputObjects.CreateChangeSetInput()
            {
                ChangeSet = new ChangeSet
                {
                    Name = Cmf.TestScenarios.Others.Utilities.NewGuid(),
                    Type = "General",
                    Description = "Change Set Created for Master Data Loader",
                    MakeEffectiveOnApproval = true
                }
            }.CreateChangeSetSync().ChangeSet;


            recipe.Create();
            recipe.Load();

            recipe.BodySource = RecipeBodySource.DownloadedFromEquipment;
            recipe.BodyDownloadFromResourceRecipe = ppid;
            recipe.BodyDownloadFrom = resource.Name;
            var recipeUpdated = new Cmf.Navigo.BusinessOrchestration.RecipeManagement.InputObjects.FullUpdateRecipeInput()
            {
                Recipe = recipe

            }.FullUpdateRecipeSync();

            recipe.ChangeSet.Approve();
        }

        public static List<Dictionary<string, object>> BuildRecipeList(int numberOfSubRecipes, string resourceName,
            string recipeName,
            string ppid,
            string serviceName)
        {
            var subRecipes = new List<Dictionary<string, object>>();

            for (int i = 1; i <= numberOfSubRecipes; i++)
            {
                var subRecipe = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
                subRecipe.Add("resourceName", resourceName);
                subRecipe.Add("recipeName", recipeName + "_" + i + "_Sub");
                subRecipe.Add("ppid", ppid + "_" + i + "_Sub");
                subRecipe.Add("serviceName", serviceName);
                subRecipe.Add("recipePath", ".\\RecipeBinaryFiles\\HEX-TEST");
                subRecipes.Add(subRecipe);
            }

            return subRecipes;
        }

    }
}

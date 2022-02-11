using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using cmConnect.TestFramework.SystemRest.Utilities;
using cmConnect.TestFramework.EquipmentSimulator.Drivers;
using Cmf.SECS.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AMSOsramEIAutomaticTests.IoT.Common
{
    public class RecipeData
    {
        public byte[] Body;
        public string Checksum;

        public RecipeData(string body)
        {
            Body = System.Text.Encoding.ASCII.GetBytes(body);
        }

        public void SetBody(byte[] newBody)
        {
            Body = newBody;
        }

        public string GetBody()
        {
            return (System.Text.Encoding.ASCII.GetString(Body));
        }
    }

    public class RecipeManagement
    {
        protected SecsGemEquipment m_Equipment;
        protected Dictionary<string, RecipeData> m_Recipes = new Dictionary<string, RecipeData>();

        public List<string> ErrorsOccurred = new List<string>();

        /// <summary>Should an exception be thrown if an attempt of changing the body is requested</summary>
        public bool AllowNewBody { get; set; }
        public bool FailOnNewBody { get; set; }

        public bool UtilSetBody { get; set; } = false;
        public bool UtilGetBody { get; set; } = false;

        public bool RecipeExistsOnList = false;

        public bool ReplyWithCorrectBody = false;

        public bool ValidateRecipeSize = false;

        public int RecipeSize = 0;

        public byte[] RecipeBody;

        public int PadSize = 0;

        public Dictionary<string, RecipeData> ToolRecipes { get { return (m_Recipes); } }

        public EventHandler OnNewBody;
        public bool TrimmedRecipeName = true;
        public RecipeManagement(SecsGemEquipment equipment)
        {
            m_Equipment = equipment;
            AllowNewBody = true;
            FailOnNewBody = false;

            m_Equipment.RegisterOnMessage("S7F19", OnGetAllRecipes);
            m_Equipment.RegisterOnMessage("S7F5", OnGetBody);
            m_Equipment.RegisterOnMessage("S7F3", OnSetBody);
            m_Equipment.RegisterOnMessage("S7F1", OnSetBodyInquiry);
            
        }

        public virtual void SetRecipe(string recipeName, string body)
        {
            recipeName = (TrimmedRecipeName ? recipeName.Trim() : recipeName);

            if (!m_Recipes.ContainsKey(recipeName))
                m_Recipes.Add(recipeName, new RecipeData(""));

            m_Recipes[recipeName] = new RecipeData(body);
        }

        public virtual void SetRecipe(string recipeName, byte[] body)
        {
            recipeName = (TrimmedRecipeName ? recipeName.Trim() : recipeName);
            if (!m_Recipes.ContainsKey(recipeName))
                m_Recipes.Add(recipeName, new RecipeData(""));

            m_Recipes[recipeName].SetBody(body);
        }

        public string GetRecipe(string recipeName)
        {
            recipeName = (TrimmedRecipeName ? recipeName.Trim() : recipeName);
            if (!m_Recipes.ContainsKey(recipeName))
                m_Recipes.Add(recipeName, new RecipeData(""));

            return (m_Recipes[recipeName].GetBody());
        }

        public void ClearRecipes()
        {
            m_Recipes.Clear();
        }

        public int RecipeCount()
        {
            return (m_Recipes.Count);
        }

        public virtual void SendRecipeBody(string recipeName)
        {
            recipeName = (TrimmedRecipeName ? recipeName.Trim() : recipeName);
            Log(string.Format("Recipe '{0}' Send recipe body request received", recipeName));
            if (m_Recipes.ContainsKey(recipeName))
            {
                SecsTransaction secsTransaction = m_Equipment.Library.GetTransaction("S7F3").Duplicate();
                m_Equipment.SetValue(secsTransaction.Primary.Item.GetChildList()[0], SecsItem.ItemType.Ascii, recipeName);
                m_Equipment.SetValue(secsTransaction.Primary.Item.GetChildList()[1], SecsItem.ItemType.Binary, m_Recipes[recipeName].Body);

                // Send the message in a while...
                ThreadPool.QueueUserWorkItem(delegate
                {
                    Thread.Sleep(1000);

                    secsTransaction.Send();
                }, null);

                Log(string.Format("Recipe '{0}' Send recipe body request complet", recipeName));
            }
            else
            {
                string error = string.Format("Recipe '{0}' doesn't exist in the equipment", recipeName);
                Log(error);
                throw new Exception(error);
            }
        }

        private bool OnGetAllRecipes(SecsMessage request, SecsMessage reply)
        {
            Log(string.Format("Get all recipes request received"));
            reply.Item.Clear();

            foreach (var recipe in m_Recipes)
            {
                SecsItem newRecipe = new SecsItem();
                m_Equipment.SetValue(newRecipe, SecsItem.ItemType.Ascii, recipe.Key.PadRight(PadSize));

                reply.Item.Add(newRecipe);
            }
          
            UtilSetBody = true;
            Log(string.Format("Get all recipes request complete"));
            return (true);
        }

        private bool OnSetBody(SecsMessage request, SecsMessage reply)
        {
            try
            {
                string recipeName = request.Item[0].GetValue().ToString();
                Log(string.Format("Recipe '{0}' Set body received", recipeName));

                if (PadSize != 0 && recipeName.Length < PadSize)
                {
                    string error = string.Format("PPID for Recipe '{0}' was not properly padded", recipeName);
                    Log(error);
                    throw new Exception(error);
                }

                recipeName = (TrimmedRecipeName ? recipeName.Trim() : recipeName);
                byte[] body = request.Item[1].GetValue() as byte[];

                if (!AllowNewBody  || RecipeExistsOnList)
                {
                    string error = string.Format("Definition of a new body for '{0}' should have not occurred", recipeName);
                    ErrorsOccurred.Add(error);
                    Log(error);
                    throw new Exception(error);
                }

                if (ReplyWithCorrectBody && !RecipeBody.SequenceEqual(body))
                {
                    for (int i = 0; i < RecipeBody.Length; i++)
                    {
                        if (!RecipeBody[i].Equals(body[i]))
                        {
                            string error = "RecipeBody wrongly sent by host";
                            Log(error);
                            throw new Exception(error);
                        }
                    }

                }

                if (!m_Recipes.ContainsKey(recipeName))
                    m_Recipes.Add(recipeName, new RecipeData(""));

                m_Recipes[recipeName].SetBody(body);

                m_Equipment.SetValue(reply.Item, SecsItem.ItemType.Binary, FailOnNewBody ? 0x01 : 0x00);
                Log(string.Format("Recipe '{0}' Set body complete", recipeName));

            }
            catch
            {
                //fail if something goes wrong on validation
                m_Equipment.SetValue(reply.Item, SecsItem.ItemType.Binary, 0x01);
            }
            return (true);
        }
        private bool OnSetBodyInquiry(SecsMessage request, SecsMessage reply)
        {
            try
            {
                string recipeName = request.Item[0].GetValue().ToString();
                Log(string.Format("Recipe '{0}' Set Body Inquiry received", recipeName));

                if (PadSize != 0 && recipeName.Length < PadSize)
                {
                    string error = string.Format("PPID for Recipe '{0}' was not properly padded", recipeName);
                    Log(error);
                    throw new Exception(error);
                }

                recipeName = (TrimmedRecipeName ? recipeName.Trim() : recipeName);
                int bodysize = 0;


                if (!Int32.TryParse(request.Item[1].GetValue()?.ToString(), out bodysize))
                {
                    string error = "Size of recipe body cannot be null";
                    Log(error);
                    throw new Exception(error);
                }

                if (ValidateRecipeSize && RecipeSize != bodysize && RecipeSize != 0)
                {
                    string error = "Size of recipe body wrongly sent by host";
                    Log(error);
                    throw new Exception(error);
                    
                }


                if (!AllowNewBody || RecipeExistsOnList)
                {
                    ErrorsOccurred.Add(string.Format("Definition of a new body for '{0}' with size '{1}' should have not occurred", recipeName, bodysize));
                    Log(ErrorsOccurred.Last());
                    throw new Exception(ErrorsOccurred.Last());
                }


                m_Equipment.SetValue(reply.Item, SecsItem.ItemType.Binary, FailOnNewBody ? 0x01 : 0x00);
                Log(string.Format("Recipe '{0}' Set Body Inquiry complete", recipeName));

            }
            catch
            {
                //fail if something goes wrong on validation
                m_Equipment.SetValue(reply.Item, SecsItem.ItemType.Binary, 0x01);
            }
            return (true);
        }

        protected virtual bool OnGetBody(SecsMessage request, SecsMessage reply)
        {
            string recipe = request.Item.GetValue().ToString();
            Log(String.Format("{0}: Get Body for recipe {1}", DateTime.UtcNow.ToString(),recipe));
            if (PadSize != 0 && recipe.Length < PadSize)
            {
                string error = string.Format("PPID for Recipe '{0}' was not properly padded", recipe);
                Log(error);
                throw new Exception(error);
            }

            recipe = (TrimmedRecipeName ? recipe.Trim() : recipe);

            reply.Item.Clear();
            if (m_Recipes.ContainsKey(recipe))
            {
                SecsItem recipeName = new SecsItem();
                m_Equipment.SetValue(recipeName, SecsItem.ItemType.Ascii, recipe);
                reply.Item.Add(recipeName);

                SecsItem recipeBody = new SecsItem();
                m_Equipment.SetValue(recipeBody, SecsItem.ItemType.Binary, m_Recipes[recipe].Body);
                reply.Item.Add(recipeBody);
            }

            UtilGetBody = true;
            return (true);
        }
        protected void Log(string log)
        {
            m_Equipment.Environment.Log(String.Format("{0}:{1}",DateTime.UtcNow.ToString("hh:mm:ss.fff"), log));
        }

    }

   
}

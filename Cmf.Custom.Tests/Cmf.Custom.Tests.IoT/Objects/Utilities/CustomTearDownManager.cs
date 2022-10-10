using Cmf.Custom.TestUtilities;
using Cmf.Custom.TestUtilities.TerminationHandlers;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.Common.Base;
using Cmf.Navigo.BusinessObjects;
using System.Collections.Generic;
using System.Linq;

namespace amsOSRAMEIAutomaticTests.Objects.Utilities
{
    /// <summary>
    /// Custom termination handler, inherits from a default one.
    /// </summary>
    internal class CustomMaterialTerminationHandler : MaterialTerminationHandler
    {
        public override void AfterTerminate(Entity obj)
        {
            base.AfterTerminate(obj);
            // Reject Received Integration Entries after terminating Materials as they might create outbound messages
            //MQBrokerIntegrationScenario.RejectReceivedOrFailedIntegrationEntries();
        }
    }

    /// <summary>
    /// Custom teardown manager, registering custom termination handlers
    /// </summary>
    public class CustomTeardownManager : TestTeardownManager
    {
        protected override void RegisterHandlers()
        {
            // Call base.RegisterHandlers() to register the default handlers
            base.RegisterHandlers();

            // Replace material handler
            TestTeardownManager.TerminationHandlers["Cmf.Navigo.BusinessObjects.Material"] = new CustomMaterialTerminationHandler();
        }

        // current priority given to entity types with no preset priority. every time a new entity
        // type is pushed, this value is decreased, so that sequential tear down is simulated
        private long _RunningPriority = 999999999;

        // entity tear down priorities
        private Dictionary<string, long> _TeardownPriorities = new Dictionary<string, long>()
        {
            { "Cmf.Navigo.BusinessObjects.Material", 1 }
            , { "Cmf.Navigo.BusinessObjects.Resource", 2 }
        };

        // inner tear down manager dictionary
        Dictionary<string, Cmf.TestScenarios.TestTeardownManager> _TestTeardownManagerDictionary = new Dictionary<string, Cmf.TestScenarios.TestTeardownManager>();


        /// <summary>
        /// Pushes an entity to the Tear Down Manager
        /// </summary>
        /// <param name="entity">Entity instance to be added to the stack of objects subject to teardown</param>
        public new void Push(Cmf.Foundation.BusinessObjects.Entity entity)
        {
            // initialize teardown manager for entity type if not yet done
            string entityType = entity.GetType().ToString();
            if (!_TestTeardownManagerDictionary.ContainsKey(entityType))
            {
                _TestTeardownManagerDictionary.Add(entityType, new Cmf.TestScenarios.TestTeardownManager());
            }

            // check priority of entity type
            if (!_TeardownPriorities.ContainsKey(entityType))
            {
                // add priority for entity type
                _TeardownPriorities.Add(entityType, _RunningPriority);

                // set running priority for next unknown entity type
                _RunningPriority--;
            }

            // get entity tear down manager
            Cmf.TestScenarios.TestTeardownManager testTeardownManager = _TestTeardownManagerDictionary[entityType];

            //Specific handler for production order to push materials to stack and terminate
            if (entity is ProductionOrder)
            {
                ProductionOrder po = entity as ProductionOrder;
                po.LoadMaterials();
                TerminateMaterials(testTeardownManager, po.Materials);
            }
            testTeardownManager.Push(entity);
        }

        /// <summary>
        /// Terminate a MaterialCollection 
        /// </summary>
        /// <param name="testTeardownManager"></param>
        /// <param name="materials"></param>
        private static void TerminateMaterials(Cmf.TestScenarios.TestTeardownManager testTeardownManager, MaterialCollection materials)
        {
            // materials.Load();
            foreach (Material material in materials)
            {
                if (material.SystemState == MaterialSystemState.InTransit && material.UniversalState == UniversalState.Active)
                {
                    // TODO to be reviewed
                    // material.UnShip();
                }
                if (material.HoldCount > 0)
                    material.CompleteRelease();

                if (material.SystemState == MaterialSystemState.InProcess &&
                    material.UniversalState != Cmf.Foundation.Common.Base.UniversalState.Terminated)
                {
                    material.Abort();
                }
            }
        }

        /// <summary>
        /// Custom method to push a list of entities to the teardown stack
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        public void Push<T>(List<T> entities) where T : Entity
        {
            // If entity is Material
            if (typeof(T) == typeof(Material))
            {
                string materialType = "Cmf.Navigo.BusinessObjects.Material";

                if (!_TestTeardownManagerDictionary.ContainsKey(materialType))
                {
                    _TestTeardownManagerDictionary.Add(materialType, new Cmf.TestScenarios.TestTeardownManager());
                }
                MaterialCollection materials = new MaterialCollection();
                materials.AddRange(entities.Select(x => x as Material));
                // get entity tear down manager
                Cmf.TestScenarios.TestTeardownManager testTeardownManager = _TestTeardownManagerDictionary[materialType];
                TerminateMaterials(testTeardownManager, materials);
            }
            else
            {
                foreach (var entity in entities)
                {
                    Push(entity);
                }
            }
        }

        /// <summary>
        /// Terminates all stored objects sequentially. Invokes also FHTestTeardownManager.TearDownSequentially
        /// </summary>
        public new void TearDownSequentially()
        {
            // get all entity types for sequential tear down
            // ordered by priority
            var orderedEntities = _TestTeardownManagerDictionary
                .OrderBy(E => _TeardownPriorities[E.Key])
                .Select((E, I) => new { Index = I, TTM = E.Value });
            Dictionary<int, Cmf.TestScenarios.TestTeardownManager> orderedFinal = orderedEntities.ToDictionary(E => E.Index, E => E.TTM);
            for (int iPos = 0; iPos < orderedFinal.Count; iPos++)
            {
                orderedFinal[iPos].TearDownSequentially();
            }
        }
    }
}

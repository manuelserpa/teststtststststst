using Cmf.Foundation.BusinessObjects.Abstractions;
using Cmf.Navigo.BusinessObjects.Abstractions;
using System.Linq;

namespace Cmf.Custom.amsOSRAM.Common.Extensions
{
    /// <summary>
    /// Extensions to extend material functionalities
    /// </summary>
	public static class MaterialExtensions
	{
        public static IArea GetArea(this IMaterial instance)
        {
            if (instance.Id <= 0)
            {
                instance.Load();
            }

            if (instance.Step.Id <= 0)
            {
                instance.Step.Load();
            }

            instance.Step.LoadRelations(Cmf.Navigo.Common.Constants.StepArea, 1);
            return ((IStepArea)instance.Step.RelationCollection[Navigo.Common.Constants.StepArea].FirstOrDefault((IEntityRelation rel) => ((IStepArea)rel).TargetEntity.Facility.Id == instance.Facility.Id))?.TargetEntity;
        }
    }
}

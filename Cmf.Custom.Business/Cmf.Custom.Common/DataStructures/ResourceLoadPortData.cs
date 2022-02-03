using Cmf.Navigo.BusinessObjects;
using System;

namespace Cmf.Custom.AMSOsram.Common.DataStructures
{
	public class ResourceLoadPortData
	{
		/// <summary>
		/// The parent resource id
		/// </summary>
		public long ParentResourceId { get; set; }

		/// <summary>
		/// The parent resource name
		/// </summary>
		public string ParentResourceName { get; set; }

		/// <summary>
		/// The load port id
		/// </summary>
		public long LoadPortId { get; set; }

		/// <summary>
		/// The load port name
		/// </summary>
		public string LoadPortName { get; set; }

		/// <summary>
		/// The load port in use attribute
		/// </summary>
		public bool LoadPortInUse { get; set; }

		/// <summary>
		/// The load port (resource), load port type. Eg. 'Input'
		/// </summary>
		public LoadPortType? LoadPortLoadPortType { get; set; }

		/// <summary>
		/// The load port modified on date
		/// </summary>
		public DateTime LoadPortModifiedOn { get; set; }

		/// <summary>
		/// The load port state model state id (name)
		/// </summary>
		public string LoadPortStateModelStateId { get; set; }

		/// <summary>
		/// The container id
		/// </summary>
		public long ContainerId { get; set; }

		/// <summary>
		/// The container name
		/// </summary>
		public string ContainerName { get; set; }

		/// <summary>
		/// The container type
		/// </summary>
		public string ContainerType { get; set; }

		/// <summary>
		/// The container total positions
		/// </summary>
		public int ContainerTotalPositions { get; set; }

		/// <summary>
		/// The container used positions
		/// </summary>
		public int ContainerUsedPositions { get; set; }

		/// <summary>
		/// The container resource association type. Eg. 'Docked Container'
		/// </summary>
		public ContainerResourceAssociationType? ContainerResourceAssociationType { get; set; }

		/// <summary>
		/// The container 'Lot' attribute. Lot name used for a compose scenario.
		/// </summary>
		public string ContainerLotAttribute { get; set; }

		/// <summary>
		/// The container 'Product' attribute
		/// </summary>
		public string ContainerProductAttribute { get; set; }

		/// <summary>
		/// The container 'MapContainerNeeded' attribute
		/// </summary>
		public bool ContainerMapContainerNeededAttribute { get; set; }

		/// <summary>
		/// The container 'TransportRequested' attribute
		/// </summary>
		public bool ContainerTransportRequestedAttribute { get; set; }

		/// <summary>
		/// The parent material (lot) id for a wafer associated to a container docked on the load port
		/// </summary>
		public long ParentMaterialId { get; set; }

		/// <summary>
		/// The parent material (lot) name for a wafer associated to a container docked on the load port
		/// </summary>
		public string ParentMaterialName { get; set; }
	}
}
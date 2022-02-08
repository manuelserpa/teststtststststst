using Cmf.Navigo.BusinessObjects;
using System;
using System.Runtime.Serialization;

namespace Cmf.Custom.AMSOsram.Common.DataStructures
{
	/// <summary>
	/// Support class that represents a grouping of the resource and the load port
	/// </summary>
	[DataContract]
	public class ResourceLoadPortData
	{
		/// <summary>
		/// The parent resource id
		/// </summary>
		[DataMember]
		public long ParentResourceId { get; set; }

		/// <summary>
		/// The parent resource name
		/// </summary>
		[DataMember]
		public string ParentResourceName { get; set; }

		/// <summary>
		/// The load port id
		/// </summary>
		[DataMember]
		public long LoadPortId { get; set; }

		/// <summary>
		/// The load port name
		/// </summary>
		[DataMember]
		public string LoadPortName { get; set; }

		/// <summary>
		/// The load port in use attribute
		/// </summary>
		[DataMember]
		public bool LoadPortInUse { get; set; }

		/// <summary>
		/// The load port (resource), load port type. Eg. 'Input'
		/// </summary>
		[DataMember]
		public LoadPortType? LoadPortLoadPortType { get; set; }

		/// <summary>
		/// The load port modified on date
		/// </summary>
		[DataMember]
		public DateTime LoadPortModifiedOn { get; set; }

		/// <summary>
		/// The load port state model state id (name)
		/// </summary>
		[DataMember]
		public string LoadPortStateModelStateId { get; set; }

		/// <summary>
		/// The container id
		/// </summary>
		[DataMember]
		public long ContainerId { get; set; }

		/// <summary>
		/// The container name
		/// </summary>
		[DataMember]
		public string ContainerName { get; set; }

		/// <summary>
		/// The container type
		/// </summary>
		[DataMember]
		public string ContainerType { get; set; }

		/// <summary>
		/// The container total positions
		/// </summary>
		[DataMember]
		public int ContainerTotalPositions { get; set; }

		/// <summary>
		/// The container used positions
		/// </summary>
		[DataMember]
		public int ContainerUsedPositions { get; set; }

		/// <summary>
		/// The container resource association type. Eg. 'Docked Container'
		/// </summary>
		[DataMember]
		public ContainerResourceAssociationType? ContainerResourceAssociationType { get; set; }

		/// <summary>
		/// The container 'Lot' attribute. Lot name used for a compose scenario.
		/// </summary>
		[DataMember]
		public string ContainerLotAttribute { get; set; }

		/// <summary>
		/// The container 'Product' attribute
		/// </summary>
		[DataMember]
		public string ContainerProductAttribute { get; set; }

		/// <summary>
		/// The container 'MapContainerNeeded' attribute
		/// </summary>
		[DataMember]
		public bool ContainerMapContainerNeededAttribute { get; set; }

		/// <summary>
		/// The container 'TransportRequested' attribute
		/// </summary>
		[DataMember]
		public bool ContainerTransportRequestedAttribute { get; set; }

		/// <summary>
		/// The parent material (lot) id for a wafer associated to a container docked on the load port
		/// </summary>
		[DataMember]
		public long ParentMaterialId { get; set; }

		/// <summary>
		/// The parent material (lot) name for a wafer associated to a container docked on the load port
		/// </summary>
		[DataMember]
		public string ParentMaterialName { get; set; }
	}
}
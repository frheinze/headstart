using Flurl.Http;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ordercloud.integrations.easypost.Exceptions;
using OrderCloud.Catalyst;

namespace ordercloud.integrations.easypost
{
	public class AddressPair : IEquatable<AddressPair>
	{
		public Address ShipFrom { get; set; }
		public Address ShipTo { get; set; }

		public bool Equals(AddressPair other)
		{
			return (ShipFrom.ID == other.ShipFrom.ID) &&
					// we still want to compare the rest of these properties to handle one time addresses
					(ShipFrom.Street1 == other?.ShipFrom.Street1) &&
					(ShipFrom.Zip == other?.ShipFrom.Zip) &&
					(ShipFrom.City == other?.ShipFrom.City) &&
					(ShipTo.Street1 == other?.ShipTo.Street1) &&
					(ShipTo.Zip == other?.ShipTo.Zip) &&
					(ShipTo.City == other?.ShipTo.City);
		}

		public override int GetHashCode()
		{
			return 1; // force Equals to be called for comparison
		}
	}

	public interface IEasyPostShippingService
	{
		Task<ShipEstimateResponse> GetRates(IEnumerable<IGrouping<AddressPair, LineItem>> groupedLineItems, EasyPostShippingProfiles profiles);
	}

	public class Grouping<TKey, TElement> : List<TElement>, IGrouping<TKey, TElement>
	{
		public Grouping(TKey key) : base() => Key = key;
		public Grouping(TKey key, int capacity) : base(capacity) => Key = key;
		public Grouping(TKey key, IEnumerable<TElement> collection)
			: base(collection) => Key = key;
		public TKey Key { get; private set; }
	}


	/// <summary>
	/// NOTE: Post-Request to EasyPost replaced with 'MockRatesForFreeShipping'. Not very smart but works for now.
	/// </summary>
	public class EasyPostShippingService : IEasyPostShippingService
	{
		private readonly EasyPostConfig _config;
		public const string FreeShipping = "FREE_SHIPPING";


		public EasyPostShippingService(EasyPostConfig config)
		{
			_config = config;
		}

		public async Task<ShipEstimateResponse> GetRates(IEnumerable<IGrouping<AddressPair, LineItem>> groupedLineItems, EasyPostShippingProfiles profiles)
		{            
			var shipEstimateResponse = new ShipEstimateResponse
			{
				ShipEstimates = groupedLineItems.Select((lineItems, index) =>
				{
						return MockRatesForFreeShipping(lineItems.ToList());
				}).ToList(),
			};
			return shipEstimateResponse;
		}

		public ShipEstimate MockRatesForFreeShipping(List<LineItem> lineItems)
        {
			var firstLi = lineItems.First();
			return new ShipEstimate
			{
				ID = FreeShipping + $"_{firstLi.SupplierID}",
				ShipMethods = new List<ShipMethod> {
					new ShipMethod {
						ID = FreeShipping + $"_{firstLi.SupplierID}",
						Cost = 0,
						Name = "FREE",
						EstimatedTransitDays = 1 // Can be overwritten by app settings
					}
				},
				ShipEstimateItems = lineItems.Select(li => new ShipEstimateItem() { LineItemID = li.ID, Quantity = li.Quantity }).ToList(),
				xp =
                {
					SupplierID = firstLi.SupplierID, // This will help with forwarding the supplier order
					ShipFromAddressID = firstLi.ShipFromAddressID  // This will help with forwarding the supplier order
                }
			};
        }				
	}
}

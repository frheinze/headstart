using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ordercloud.integrations.library;
using OrderCloud.Catalyst;
using OrderCloud.SDK;

namespace ordercloud.integrations.smartystreets
{
	public interface ISmartyStreetsCommand
	{
		Task<AddressValidation> ValidateAddress(Address address);
		Task<BuyerAddressValidation> ValidateAddress(BuyerAddress address);
		// ME endpoints
		Task<BuyerAddress> CreateMeAddress(BuyerAddress address, DecodedToken decodedToken);
		Task<BuyerAddress> SaveMeAddress(string addressID, BuyerAddress address, DecodedToken decodedToken);
		Task PatchMeAddress(string addressID, BuyerAddress patch, DecodedToken decodedToken);
		// BUYER endpoints
		Task<Address> CreateBuyerAddress(string buyerID, Address address, DecodedToken decodedToken);
		Task<Address> SaveBuyerAddress(string buyerID, string addressID, Address address, DecodedToken decodedToken);
		Task<Address> PatchBuyerAddress(string buyerID, string addressID, Address patch, DecodedToken decodedToken);
		// SUPPLIER endpoints
		Task<Address> CreateSupplierAddress(string supplierID, Address address, DecodedToken decodedToken);
		Task<Address> SaveSupplierAddress(string supplierID, string addressID, Address address, DecodedToken decodedToken);
		Task<Address> PatchSupplierAddress(string supplierID, string addressID, Address patch, DecodedToken decodedToken);
		// ADMIN endpoints
		Task<Address> CreateAdminAddress(Address address, DecodedToken decodedToken);
		Task<Address> SaveAdminAddress(string addressID, Address address, DecodedToken decodedToken);
		Task<Address> PatchAdminAddress(string addressID, Address patch, DecodedToken decodedToken);
		// ORDER endpoints
		Task<Order> SetBillingAddress(OrderDirection direction, string orderID, Address address, DecodedToken decodedToken);
		Task<Order> SetShippingAddress(OrderDirection direction, string orderID, Address address, DecodedToken decodedToken);
	}

	/// <summary>
	/// NOTE: Access to ISmartyStreetsService removed. Not very smart but works for now.
	/// </summary>
	public class SmartyStreetsCommand : ISmartyStreetsCommand
	{
		private readonly IOrderCloudClient _oc;

		public SmartyStreetsCommand(IOrderCloudClient oc)
		{
			_oc = oc;
		}

        public async Task<AddressValidation> ValidateAddress(Address address)
		{
			var response = new AddressValidation(address);
			response.ValidAddress = address;
			return response;
		}

		public async Task<BuyerAddressValidation> ValidateAddress(BuyerAddress address)
		{
			var response = new BuyerAddressValidation(address);
			response.ValidAddress = address;		
			return response;
		}

        private bool NoAddressSuggestions(AutoCompleteResponse suggestions)
        {
            return (suggestions == null || suggestions.suggestions == null || suggestions.suggestions.Count == 0);
        }

        #region Ordercloud Routes
        // ME endpoints
        public async Task<BuyerAddress> CreateMeAddress(BuyerAddress address, DecodedToken decodedToken)
		{
			var validation = await ValidateAddress(address);
			return await _oc.Me.CreateAddressAsync(validation.ValidAddress, decodedToken.AccessToken);
		}

		public async Task<BuyerAddress> SaveMeAddress(string addressID, BuyerAddress address, DecodedToken decodedToken)
		{
			var validation = await ValidateAddress(address);
			return await _oc.Me.SaveAddressAsync(addressID, validation.ValidAddress, decodedToken.AccessToken);
		}

		public async Task PatchMeAddress(string addressID, BuyerAddress patch, DecodedToken decodedToken)
		{
			var current = await _oc.Me.GetAddressAsync<BuyerAddress>(addressID, decodedToken.AccessToken);
			var patched = PatchHelper.PatchObject(patch, current);
			await ValidateAddress(patched);
			await _oc.Me.PatchAddressAsync(addressID, (PartialBuyerAddress)patch, decodedToken.AccessToken);
		}

		// BUYER endpoints
		public async Task<Address> CreateBuyerAddress(string buyerID, Address address, DecodedToken decodedToken)
		{
			var validation = await ValidateAddress(address);
			return await _oc.Addresses.CreateAsync(buyerID, validation.ValidAddress, decodedToken.AccessToken);
		}

		public async Task<Address> SaveBuyerAddress(string buyerID, string addressID, Address address, DecodedToken decodedToken)
		{
			var validation = await ValidateAddress(address);
			return await _oc.Addresses.SaveAsync(buyerID, addressID, validation.ValidAddress, decodedToken.AccessToken);
		}

		public async Task<Address> PatchBuyerAddress(string buyerID, string addressID, Address patch, DecodedToken decodedToken)
		{
			var current = await _oc.Addresses.GetAsync<Address>(buyerID, addressID, decodedToken.AccessToken);
			var patched = PatchHelper.PatchObject(patch, current);
			await ValidateAddress(patched);
			return await _oc.Addresses.PatchAsync(buyerID, addressID, patch as PartialAddress, decodedToken.AccessToken);
		}

		// SUPPLIER endpoints
		public async Task<Address> CreateSupplierAddress(string supplierID, Address address, DecodedToken decodedToken)
		{
			var validation = await ValidateAddress(address);
			return await _oc.SupplierAddresses.CreateAsync(supplierID, validation.ValidAddress, decodedToken.AccessToken);
		}

		public async Task<Address> SaveSupplierAddress(string supplierID, string addressID, Address address, DecodedToken decodedToken)
		{
			var validation = await ValidateAddress(address);
			return await _oc.SupplierAddresses.SaveAsync(supplierID, addressID, validation.ValidAddress, decodedToken.AccessToken);
		}

		public async Task<Address> PatchSupplierAddress(string supplierID, string addressID, Address patch, DecodedToken decodedToken)
		{
			var current = await _oc.SupplierAddresses.GetAsync<Address>(supplierID, addressID, decodedToken.AccessToken);
			var patched = PatchHelper.PatchObject(patch, current);
			await ValidateAddress(patched);
			return await _oc.SupplierAddresses.PatchAsync(supplierID, addressID, patch as PartialAddress, decodedToken.AccessToken);
		}

		// ADMIN endpoints
		public async Task<Address> CreateAdminAddress(Address address, DecodedToken decodedToken)
		{
			var validation = await ValidateAddress(address);
			return await _oc.AdminAddresses.CreateAsync(address, decodedToken.AccessToken);
		}

		public async Task<Address> SaveAdminAddress(string addressID, Address address, DecodedToken decodedToken)
		{
			var validation = await ValidateAddress(address);
			return await _oc.AdminAddresses.SaveAsync(addressID, validation.ValidAddress, decodedToken.AccessToken);
		}

		public async Task<Address> PatchAdminAddress(string addressID, Address patch, DecodedToken decodedToken)
		{
			var current = await _oc.AdminAddresses.GetAsync<Address>(addressID, decodedToken.AccessToken);
			var patched = PatchHelper.PatchObject(patch, current);
			await ValidateAddress(patched);
			return await _oc.AdminAddresses.PatchAsync(addressID, patch as PartialAddress, decodedToken.AccessToken);
		}

		// ORDER endpoints
		public async Task<Order> SetBillingAddress(OrderDirection direction, string orderID, Address address, DecodedToken decodedToken)
		{
			var validation = await ValidateAddress(address);
			return await _oc.Orders.SetBillingAddressAsync(direction, orderID, validation.ValidAddress, decodedToken.AccessToken);
		}

		public async Task<Order> SetShippingAddress(OrderDirection direction, string orderID, Address address, DecodedToken decodedToken)
		{
			var validation = await ValidateAddress(address);
			return await _oc.Orders.SetShippingAddressAsync(direction, orderID, validation.ValidAddress, decodedToken.AccessToken);
		}
		#endregion
	}
}

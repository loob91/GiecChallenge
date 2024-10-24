using System.Data;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AutoMapper;
using GiecChallenge.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GiecChallenge.Services
{
    public interface IPurchaseRepository
    {
        Task<List<PurchaseDto>> GetAll(Guid userId);
        Task<PurchaseDto> Get(Guid userId, Guid purchaseId);
        Task<List<PurchaseDto>> GetBetweenDate(Guid userId, DateTime dateBegin, DateTime dateEnd);
        Task<double> GetCO2BetweenDate(Guid userId, DateTime dateBegin, DateTime dateEnd);
        Task Create(Guid userId, PurchaseDto purchaseDto);
        Task Update(Guid userId, PurchaseDto purchaseDto);
        Task Delete(Guid userId, Guid purchaseId);
        Task DeleteLine(Guid userId, Guid purchaseLineId);
        Task<PurchaseLaRucheImportReturnDto> ImportLaRuchePurchase(Guid userId, PurchaseLaRucheDto purchaseLaRuche);
    }

    public class PurchaseService : IPurchaseRepository
    {
        private readonly ILogger<PurchaseService> _logger;
        private readonly IMapper _mapper;
        private readonly GiecChallengeContext _context;

        public PurchaseService(ILogger<PurchaseService> logger,
                              IMapper mapper,
                              GiecChallengeContext context) {
            this._logger = logger;
            this._mapper = mapper;
            this._context = context;
        }

        public async Task<List<PurchaseDto>> GetAll(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.id == userId);
            if (user == null)
                throw new Exception("Nice try");
            var purchases = await GetPurchasesWithInclude().Where(p => p.user.id == userId).ToListAsync();
            return getDtos(purchases, user);
        }

        public async Task<PurchaseDto> Get(Guid userId, Guid purchaseId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.id == userId);
            if (user == null)
                throw new Exception("Nice try");
            var purchase = await GetPurchasesWithInclude().FirstOrDefaultAsync(p => p.user.id == userId && p.id == purchaseId);

            if (purchase == null) {
                throw new Exception("Purchase does not exist");
            }

            return getDto(purchase, user);
        }

        public async Task<List<PurchaseDto>> GetBetweenDate(Guid userId, DateTime dateBegin, DateTime dateEnd)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.id == userId);
            if (user == null)
                throw new Exception("Nice try");
            var purchases = await _context.Purchases.Include(b => b.products).ThenInclude(b => b.product).Where(p => p.user.id == userId && p.datePurchase >= dateBegin && p.datePurchase <= dateEnd).ToListAsync();
            return getDtos(purchases, user);
        }

        public async Task<double> GetCO2BetweenDate(Guid userId, DateTime dateBegin, DateTime dateEnd)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.id == userId);
            if (user == null)
                throw new Exception("Nice try");
            var purchases = await _context.Purchases.Include(b => b.products).ThenInclude(b => b.product).Where(p => p.user.id == userId && p.datePurchase >= dateBegin && p.datePurchase <= dateEnd).ToListAsync();
            double CO2Emissions = purchases.Sum(p => p.products.Sum(pro => pro.product.CO2 * pro.quantity));
            return CO2Emissions;
        }


        public async Task Create(Guid userId, PurchaseDto purchaseDto)
        {   
            Purchase purchase = _mapper.Map<Purchase>(purchaseDto);

            purchase = await getPurchaseFromDto(userId, purchase, purchaseDto);

            purchase.id = Guid.NewGuid();
            
            foreach (ProductPurchase products in purchase.products) {
                _context.ProductPurchases.Add(products);
            }

            await _context.Purchases.AddAsync(purchase);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid userId, Guid purchaseID)
        {   
            Purchase? purchase = await _context.Purchases.FirstOrDefaultAsync(p => p.id == purchaseID && p.user.id == userId);

            if (purchase == null) {
                throw new Exception("Purchase does not exist");
            }

            _context.Purchases.Remove(purchase);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteLine(Guid userId, Guid purchaseLineID)
        {   
            ProductPurchase? productPurchase = await _context.ProductPurchases.FirstOrDefaultAsync(p => p.id == purchaseLineID && p.purchase.user.id == userId);

            if (productPurchase == null) {
                throw new Exception("Product purchase does not exist");
            }

            _context.ProductPurchases.Remove(productPurchase);

            if (!productPurchase.purchase.products.Any())
                _context.Purchases.Remove(productPurchase.purchase);

            await _context.SaveChangesAsync();
        }


        public async Task Update(Guid userId, PurchaseDto purchaseDto)
        {   
            Purchase? purchase = await _context.Purchases.Include(p => p.products).FirstOrDefaultAsync(p => p.id == purchaseDto.id);

            if (purchase == null) {
                throw new Exception("Purchase does not exist");
            }

            purchase = await getPurchaseFromDto(userId, purchase, purchaseDto);

            _context.Purchases.Update(purchase);
            
            foreach (ProductPurchase products in purchase.products) {
                _context.ProductPurchases.Add(products);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<PurchaseLaRucheImportReturnDto> ImportLaRuchePurchase(Guid userId, PurchaseLaRucheDto purchaseLaRuche)
        {   
            List<ProductPurchaseDto> productToRetrieve = new List<ProductPurchaseDto>();

            Purchase purchase = new Purchase();
            purchase.datePurchase = purchaseLaRuche.datePurchase;
            purchase.user = await GetUserInLaRocheImport(purchaseLaRuche, userId);
            Currency currency = await _context.Currencies.FirstAsync(c => c.ISOCode == "EUR");

            foreach (string productLine in purchaseLaRuche.command.Split(new[] { "\n \n\n"}, StringSplitOptions.None)) {
                string[] productCaracteristics = productLine.Trim().Split("\n");
                ProductPurchase productPurchase = await GetProductPurchaseFromLine(productCaracteristics, currency);
                if (productPurchase.product == null) {
                    ProductPurchaseDto purchaseDto = _mapper.Map<ProductPurchaseDto>(productPurchase);
                    purchaseDto.product = productCaracteristics[0];
                    productToRetrieve.Add(purchaseDto);
                }
                else if (purchase.products.Any(pp => pp.product.id == productPurchase.id)) {
                    ProductPurchase alreadyInImport = purchase.products.First(pp => pp.product.id == productPurchase.id);
                    alreadyInImport.price += productPurchase.price;
                    alreadyInImport.quantity += productPurchase.quantity;
                }
                else
                    purchase.products.Add(productPurchase);
            }

            purchase.id = Guid.NewGuid();
            
            await _context.Purchases.AddAsync(purchase);
            
            foreach (ProductPurchase products in purchase.products) {
                _context.ProductPurchases.Add(products);
            }

            await _context.SaveChangesAsync();
            
            return new PurchaseLaRucheImportReturnDto() {
                id = purchase.id,
                productsToTranslate = productToRetrieve
            };
        }

        private Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<GiecChallenge.Models.Purchase, GiecChallenge.Models.Currency> GetPurchasesWithInclude() {
            return this._context.Purchases.Include(b => b.products).ThenInclude(b => b.product).ThenInclude(b => b.names).ThenInclude(b => b.language)
                                          .Include(b => b.user)
                                          .Include(b => b.products).ThenInclude(b => b.currency);
        }

        private async Task<Purchase> getPurchaseFromDto(Guid userId, Purchase purchase, PurchaseDto purchaseDto) {
            purchase.user = await GetPurchaseUserFromDto(userId, purchase, purchaseDto);
            purchase.products = new List<ProductPurchase>();
            _context.CarbonLoans.RemoveRange(await _context.CarbonLoans.Where(cl => cl.productPurchase.purchase == purchase).ToListAsync());
            _context.Purchases.RemoveRange(await _context.Purchases.Where(cl => cl.initialPurchase == purchase).ToListAsync());

            foreach (ProductPurchaseDto purchaseProductDto in purchaseDto.products) {
                ProductPurchase purchaseProduct = await GetProductPurchaseFromDto(purchaseProductDto, purchase);
                if (!string.IsNullOrEmpty(purchaseProductDto.translation) && 
                    !_context.ProductUserTranslations.Any(put => put.product == purchaseProduct.product && put.name == purchaseProductDto.translation)) {
                    _context.ProductUserTranslations.Add(new ProductUserTranslation() {
                        id = new Guid(),
                        user = purchase.user,
                        product = purchaseProduct.product,
                        name = purchaseProductDto.translation
                    });
                }
                if (!purchase.products.Any(p => p.product == purchaseProduct.product)) {
                    purchaseProduct.purchase = purchase;
                    if (purchaseProduct.id == Guid.Empty) {
                        purchaseProduct.id = Guid.NewGuid();
                        purchaseProduct.CO2Cost = purchaseProduct.product.CO2;
                        purchase.products.Add(purchaseProduct);
                    }
                }
                else {
                    purchaseProduct = purchase.products.First(p => p.product == purchaseProduct.product);
                    purchaseProduct.price += purchaseProductDto.price;
                    purchaseProduct.quantity += purchaseProductDto.quantity;
                    purchaseProduct.CO2Cost += purchaseProduct.product.CO2;
                }
            }

            foreach (ProductPurchase productPurchase in purchase.products.Where(pp => pp.product.amortization > 0).ToList()) {
                await addLoan(purchase, productPurchase);
            }
            
            return purchase;
        }

        private List<PurchaseDto> getDtos(List<Purchase> purchases, User user) {
            List<PurchaseDto> allPurchases = new List<PurchaseDto>();
            foreach (Purchase purchase in purchases) {   
                allPurchases.Add(getDto(purchase, user));
            }
            return allPurchases;
        }

        private PurchaseDto getDto(Purchase purchase, User user) {
            var purchaseDto = _mapper.Map<PurchaseDto>(purchase);
            foreach (ProductPurchase product in purchase.products) {
                purchaseDto.products.Add(_mapper.Map<ProductPurchaseDto>(product, opts: opt => opt.Items["language"] = user.favoriteLanguage));
                purchaseDto.CO2Cost += product.CO2Cost;
                purchaseDto.WaterCost += product.product.water;
            }
            return purchaseDto;
        }

        private List<PurchaseDto> getPurchase(List<Purchase> purchases, User user) {
            List<PurchaseDto> allPurchases = new List<PurchaseDto>();
            foreach (Purchase purchase in purchases) {        
                var purchaseDto = _mapper.Map<PurchaseDto>(purchases, opts: opt => opt.Items["language"] = user.favoriteLanguage);
                foreach (ProductPurchase product in purchase.products) {
                    purchaseDto.products.Add(_mapper.Map<ProductPurchaseDto>(product.product, opts: opt => opt.Items["language"] = user.favoriteLanguage));
                }
                allPurchases.Add(purchaseDto);
            }
            return allPurchases;
        }

        private async Task<User> GetUserInLaRocheImport(PurchaseLaRucheDto purchaseLaRuche, Guid userId) {
            if (purchaseLaRuche.datePurchase > DateTime.Now) {
                throw new Exception("Purchase date can't be in the future");
            }

            if (string.IsNullOrEmpty(purchaseLaRuche.command))
                throw new Exception("No product selected");
            
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.id == userId);

            if (user == null)
                throw new Exception("Nice try");
            return user;
        }

        private async Task<ProductPurchase> GetProductPurchaseFromLine(string[] productCaracteristics, Currency currency) {
            Product? product = await _context.Products.Include(b => b.names).Include(b => b.subgroup).ThenInclude(b => b.names).Where(al => al.names.Any(aln => aln.name.ToLower() == productCaracteristics[0].ToLower() && aln.language.ISOCode == "FR")).FirstOrDefaultAsync();
            double quantity = 0;
            double price = 0;
            if (product == null) {
                product = await _context.ProductUserTranslations.Where(put => put.name.ToLower() == productCaracteristics[0].ToLower()).Include(put => put.product).ThenInclude(b => b.names).ThenInclude(b => b.language).Include(put => put.product).ThenInclude(b => b.subgroup).ThenInclude(b => b.names).Select(s => s.product).FirstOrDefaultAsync();
            }

            try {
                quantity = GetProductQuantity(
                    String.Join("", Regex.Matches(productCaracteristics[1].Replace("(", "").Replace(")", ""), @"([0-9]{0,4}\.?[0-9]{0,2}\s?×?)").Select(m => m.Value).ToList()).Replace("×", "*"), 
                    Regex.Matches(productCaracteristics[1], @"([a-zA-Z]*)").Where(m => !string.IsNullOrEmpty(m.Value)).Select(m => m.Value).FirstOrDefault());
            }
            catch {
                throw new Exception(string.Concat("Product quantity for ", productCaracteristics[0], " not correct"));
            }

            try {
                price = double.Parse(productCaracteristics[2].Replace("€", "").Trim(), CultureInfo.InvariantCulture);
            }
            catch {
                throw new Exception(string.Concat("Product price for ", productCaracteristics[0], " not correct"));
            }

            return new ProductPurchase() {
                id = Guid.NewGuid(),
                product = product,
                currency = currency,
                price = price,
                quantity = quantity,
                CO2Cost = product != null ? product.CO2 / (product.amortization + 1) : 0
            };
        } 

        private async Task addLoan(Purchase purchase, ProductPurchase productPurchase) {
            CarbonLoan loan = new CarbonLoan();
            loan.id = Guid.NewGuid();
            loan.user = purchase.user;
            loan.productPurchase = productPurchase;
            loan.dateBegin = purchase.datePurchase;
            loan.dateEnd = purchase.datePurchase.AddMonths(productPurchase.product.amortization);
            await _context.CarbonLoans.AddAsync(loan);
            int i = 1;
            while (i < productPurchase.product.amortization) {
                Purchase newPurchase = new Purchase() {
                    id = Guid.NewGuid(),
                    user = purchase.user,
                    datePurchase = purchase.datePurchase.AddMonths(i),
                    initialPurchase = purchase
                };
                ProductPurchase newProductPurchase = new ProductPurchase() {
                    id = Guid.NewGuid(),
                    product = productPurchase.product,
                    currency = productPurchase.currency,
                    purchase = newPurchase,
                    price = 0,
                    quantity = productPurchase.quantity,
                    CO2Cost = productPurchase.product.CO2 / (productPurchase.product.amortization + 1)
                };
                newPurchase.products.Add(newProductPurchase);
                await _context.Purchases.AddAsync(newPurchase);
                await _context.ProductPurchases.AddAsync(newProductPurchase);
                i++;
            }
        }

        private async Task<ProductPurchase> GetProductPurchaseFromDto(ProductPurchaseDto purchaseProductDto, Purchase purchase) {
            ProductPurchase purchaseProduct = _mapper.Map<ProductPurchase>(purchaseProductDto);
            if (Guid.TryParse(purchaseProductDto.id, out Guid purchaseId) && purchase.products.Any(p => p.id == purchaseId))
                purchaseProduct.id = purchaseId;
            if (purchaseProductDto.price < 0)
                throw new Exception("Price must be superior than 0");
            if (!Guid.TryParse(purchaseProductDto.product, out Guid productId))
                throw new Exception("Product does not exist");
            var product = await _context.Products.FirstOrDefaultAsync(p => p.id == productId);
            if (product == null)
                throw new Exception("Product does not exist");
            purchaseProduct.product = product;
            Currency? currency = null;
            if (Guid.TryParse(purchaseProductDto.currencyIsoCode, out Guid currencyId))
                currency = await _context.Currencies.FirstOrDefaultAsync(p => p.id == currencyId);
            else
                currency = await _context.Currencies.FirstOrDefaultAsync(p => p.ISOCode == purchaseProductDto.currencyIsoCode);
            if (currency == null)
                throw new Exception("Currency does not exist");
            purchaseProduct.currency = currency;
            return purchaseProduct;
        }

        private async Task<User> GetPurchaseUserFromDto(Guid userId, Purchase purchase, PurchaseDto purchaseDto) {
            if (purchase.user.id == Guid.Empty) {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.id == userId);

                if (user == null)
                    throw new Exception("Nice try");

                purchase.user = user;

                if (!purchaseDto.products.Any()) {
                    throw new Exception("No product selected");
                }
            }
            else if (purchase.user.id != userId) {
                throw new Exception("Purchase does not exist");
            }

            if (!purchaseDto.products.Any())
                throw new Exception("No product selected");
            return purchase.user;
        }

        private double GetProductQuantity(string quantityString, string unit) {
            double finalQuantity = Convert.ToDouble(new DataTable().Compute(quantityString,null));
            if (unit == "g" || unit == "cl")
                finalQuantity = finalQuantity / 1000;
            return finalQuantity;
        }
    }
}
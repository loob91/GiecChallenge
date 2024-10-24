using AutoMapper;
using GiecChallenge.Models;
using Microsoft.EntityFrameworkCore;

namespace GiecChallenge.Services
{
    public interface IProductRepository
    {
        Task<List<ProductDto>> GetAllProducts();
        Task<ProductDto?> GetProduct(Guid id);
        Task<List<ProductDto>> GetProducts(string name, string languageCode);
        Task<List<ProductDto>> GetProductsByGroup(string groupId);
        Task<List<ProductDto>> GetProductsBySubGroup(string subGroupId);
        Task Create(ProductDto ProductDto);
        Task Update(Guid id, ProductDto ProductDto);
        Task CreateTranslation(ProductUserTranslationDTO translationDTO);
        Task UpdateTranslation(ProductUserTranslationDTO translationDTO, Guid userId);
        Task DeleteTranslation(ProductUserTranslationDTO translationDTO, Guid userId);
    }

    public class ProductService : IProductRepository
    {
        private readonly ILogger<ProductService> _logger;
        private readonly IMapper _mapper;
        private readonly GiecChallengeContext _context;

        public ProductService(ILogger<ProductService> logger,
                              IMapper mapper,
                              GiecChallengeContext context) {
            this._logger = logger;
            this._mapper = mapper;
            this._context = context;
        }

        public async Task<List<ProductDto>> GetAllProducts()
        {
            var allProducts = await GetProductsWithInclude().ToListAsync();
            return _mapper.Map<List<Product>, List<ProductDto>>(allProducts);
        }

        public async Task<ProductDto?> GetProduct(Guid id)
        {
            Product? product = await GetProductsWithInclude().FirstOrDefaultAsync(gr => gr.id == id);
            if (product == null)
                throw new Exception(string.Concat("Product does not exist"));
            return _mapper.Map<Product, ProductDto>(product);
        }

        public async Task<List<ProductDto>> GetProductsByGroup(string groupId)
        {
            if (!Guid.TryParse(groupId, out Guid group)) {
                throw new Exception(string.Concat("Group ", groupId, " doesn't exist"));
            }
            var result = await _context.Products.Include(b => b.subgroup).ThenInclude(b => b.Groupe).Where(p => p.subgroup.Groupe.id == group).ToListAsync();
            return _mapper.Map<List<ProductDto>>(result).ToList();
        }

        public async Task<List<ProductDto>> GetProductsBySubGroup(string subGroupId)
        {
            if (!Guid.TryParse(subGroupId, out Guid subGroup)) {
                throw new Exception(string.Concat("Sub group ", subGroupId, " doesn't exist"));
            }
            var result = await _context.Products.Include(b => b.subgroup).Where(p => p.subgroup.id == subGroup).ToListAsync();
            return _mapper.Map<List<ProductDto>>(result).ToList();
        }

        public async Task<List<ProductDto>> GetProducts(string name, string languageCode)
        {
            var result = await _context.ProductUserTranslations.Where(put => put.name.ToLower() == name.ToLower()).Include(put => put.product).ThenInclude(b => b.names).ThenInclude(b => b.language).Include(put => put.product).ThenInclude(b => b.subgroup).ThenInclude(b => b.names).Include(put => put.product).ThenInclude(b => b.subgroup).ThenInclude(b => b.Groupe).ThenInclude(b => b.names).Select(s => new Tuple<int, Product>(1, s.product)).Take(10).ToListAsync();
            result.AddRange(await GetProductsWithInclude().Where(al => al.names.Any(aln => aln.name.ToLower() == name.ToLower() && aln.language.ISOCode == languageCode)).Take(10).Select(s => new Tuple<int, Product>(2, s)).ToListAsync());
            result.AddRange(await GetProductsWithInclude().Where(al => al.names.Any(aln => aln.name.ToLower().StartsWith(name.ToLower()) && aln.language.ISOCode == languageCode)).Take(10).Select(s => new Tuple<int, Product>(3, s)).ToListAsync());
            result.AddRange(await GetProductsWithInclude().Where(al => al.names.Any(aln => aln.name.ToLower().Contains(name.ToLower()) && aln.language.ISOCode == languageCode)).Take(10).Select(s => new Tuple<int, Product>(4, s)).ToListAsync());
            return _mapper.Map<List<ProductDto>>(result.OrderBy(s => s.Item1).Select(s => s.Item2).Take(10).Distinct().ToList()).ToList();
        }

        public async Task Create(ProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto, opts: opt => opt.Items["language"] = productDto.language);

            product.names = await GetNames(productDto, product);

            product.subgroup = await getSubGroup(productDto);

            foreach (ProductLanguage productLanguage in product.names) {
                if (await GetProductsWithInclude().AnyAsync(p => 
                        p.names.Any(pn => pn.name.ToLower() == productLanguage.name.ToLower() && pn.language.id == productLanguage.language.id) && 
                        p.subgroup.id == product.subgroup.id
                    )) 
                    throw new Exception(string.Concat("Product already exists"));
            }
            
            await _context.Products.AddAsync(product, default);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Guid id, ProductDto productDto)
        {   
            var product = await this._context.Products.FirstOrDefaultAsync(al => al.id == id);

            if (product == new Product() || product == null)
                throw new Exception("Product does not exist");

            _mapper.Map(productDto, product, opts: opt => opt.Items["language"] = productDto.language);

            product.names = await GetNames(productDto, product);
            product.subgroup = await getSubGroup(productDto);

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task CreateTranslation(ProductUserTranslationDTO translationDTO) {
            var translation = _mapper.Map<ProductUserTranslation>(translationDTO);

            var user = await _context.Users.FirstOrDefaultAsync(l => l.id.ToString() == translationDTO.user);
            if (user == null)
                throw new Exception(string.Concat("User does not exist"));

            var product = await _context.Products.FirstOrDefaultAsync(p => p.id.ToString() == translationDTO.product);
            if (product == null)
                throw new Exception(string.Concat("Product does not exist"));

            translation.user = user;
            translation.product = product;
            
            await _context.ProductUserTranslations.AddAsync(translation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTranslation(ProductUserTranslationDTO translationDTO, Guid userId) {
            if (!Guid.TryParse(translationDTO.id, out Guid translationId))
                throw new Exception(string.Concat("Translation does not exist"));

            var translation = await this._context.ProductUserTranslations.FirstOrDefaultAsync(al => al.id == translationId && al.user.id == userId);

            if (translation == new ProductUserTranslation() || translation == null)
                throw new Exception("Translation doesn't exist");

            translation.name = translationDTO.name;

            _context.ProductUserTranslations.Update(translation);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTranslation(ProductUserTranslationDTO translationDTO, Guid user) {
            var translation = await _context.ProductUserTranslations.FirstOrDefaultAsync(p => p.id.ToString() == translationDTO.id && p.user.id == user);
            if (translation == null)
                throw new Exception(string.Concat("Translation does not exist"));
            
            _context.ProductUserTranslations.Remove(translation);
            await _context.SaveChangesAsync();
        }

        private Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<GiecChallenge.Models.Product, GiecChallenge.Models.Language> GetProductsWithInclude() {
            return this._context.Products.Include(b => b.names).ThenInclude(b => b.language).Include(b => b.subgroup).ThenInclude(b => b.names).ThenInclude(b => b.language).Include(b => b.subgroup).ThenInclude(b => b.Groupe).ThenInclude(b => b.names).ThenInclude(b => b.language);
        }

        private async Task<List<ProductLanguage>> GetNames(ProductDto productDto, Product product) {
            var productLanguages = new List<ProductLanguage>();
            
            foreach (ProductNamesDto name in productDto.names) {
                if (!Guid.TryParse(name.language, out Guid languageId)) {
                    throw new Exception(string.Concat("Language ", name.language, " doesn't exist"));
                }
                var currentLanguageToInsert = await _context.Languages.SingleOrDefaultAsync(l => l.id == languageId);
                if (currentLanguageToInsert == null)
                    throw new Exception(string.Concat("Language ", name.language, " doesn't exist"));
                if (product.names.Any(l => l.language.ISOCode.ToLower() == name.language.ToLower()))
                    product.names.First(l => l.language.ISOCode.ToLower() == name.language.ToLower()).name = name.name;
                else 
                    product.names.Add(_mapper.Map<ProductLanguage>(name, opts: opt => { 
                        opt.Items["language"] = currentLanguageToInsert;
                    }));
            }

            return product.names;
        }

        private async Task<ProductSubGroup> getSubGroup(ProductDto product) {
            if (!Guid.TryParse(product.group, out Guid groupId))
                throw new Exception(string.Concat("Group does not exist"));

            var subGroup = await this._context.ProductSubGroups.FirstOrDefaultAsync(pg => pg.id == groupId);

            if (subGroup == null)
                throw new Exception(string.Concat("Group does not exist"));
            return subGroup;
        }
    }
}
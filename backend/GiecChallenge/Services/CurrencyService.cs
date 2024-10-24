using AutoMapper;
using GiecChallenge.Models;
using Microsoft.EntityFrameworkCore;

namespace GiecChallenge.Services
{
    public interface ICurrencyRepository
    {
        Task<List<CurrencyDto>> GetAllCurrencies();
        Task<CurrencyDto?> GetCurrency(Guid id);
        Task<CurrencyDto?> GetCurrencyByISO(string isoCode);
        Task<List<CurrencyDto>> GetCurrencies(string name, string languageCode);
        Task Create(CurrencyDto CurrencyDto);
        Task Update(Guid id, CurrencyDto CurrencyDto);
    }

    public class Currencieservice : ICurrencyRepository
    {
        private readonly ILogger<Currencieservice> _logger;
        private readonly IMapper _mapper;
        private readonly GiecChallengeContext _context;

        public Currencieservice(ILogger<Currencieservice> logger,
                              IMapper mapper,
                              GiecChallengeContext context) {
            this._logger = logger;
            this._mapper = mapper;
            this._context = context;
        }

        public async Task<List<CurrencyDto>> GetAllCurrencies()
        {
            var allCurrencies = await GetCurrenciesWithInclude().ToListAsync();
            return _mapper.Map<List<Currency>, List<CurrencyDto>>(allCurrencies);
        }

        public async Task<CurrencyDto?> GetCurrency(Guid id)
        {
            Currency? Currency = await GetCurrenciesWithInclude().FirstOrDefaultAsync(gr => gr.id == id);
            if (Currency == null)
                throw new Exception(string.Concat("Currency does not exist"));
            return _mapper.Map<Currency, CurrencyDto>(Currency);
        }

        public async Task<CurrencyDto?> GetCurrencyByISO(string isoCode)
        {
            Currency? Currency = await GetCurrenciesWithInclude().FirstOrDefaultAsync(gr => gr.ISOCode == isoCode);
            if (Currency == null)
                throw new Exception(string.Concat("Currency does not exist"));
            return _mapper.Map<Currency, CurrencyDto>(Currency);
        }

        public async Task<List<CurrencyDto>> GetCurrencies(string name, string languageCode)
        {
                var result = await GetCurrenciesWithInclude().Where(al => al.names.Any(aln => aln.name.ToLower() == name.ToLower() && aln.language.ISOCode == languageCode)).Select(s => new Tuple<int, Currency>(1, s)).ToListAsync();
                result.AddRange(await GetCurrenciesWithInclude().Where(al => al.names.Any(aln => aln.name.ToLower().StartsWith(name.ToLower()) && aln.language.ISOCode == languageCode)).Select(s => new Tuple<int, Currency>(2, s)).ToListAsync());
                result.AddRange(await GetCurrenciesWithInclude().Where(al => al.names.Any(aln => aln.name.ToLower().Contains(name.ToLower()) && aln.language.ISOCode == languageCode)).Select(s => new Tuple<int, Currency>(3, s)).ToListAsync());
                return _mapper.Map<List<CurrencyDto>>(result.OrderBy(s => s.Item1).Select(s => s.Item2).Distinct().ToList()).ToList();
        }

        public async Task Create(CurrencyDto currencyDto)
        {
            var Currency = _mapper.Map<Currency>(currencyDto, opts: opt => opt.Items["language"] = currencyDto.language);

            if (_context.Currencies.Any(c => c.ISOCode == currencyDto.ISOCode))
                    throw new Exception(string.Concat("Currency already exists"));

            Currency.names = await GetNames(currencyDto, Currency);

            foreach (CurrencyLanguage CurrencyLanguage in Currency.names) {
                if (await _context.CurrencyLanguages.Include(b => b.language).AnyAsync(pgl => pgl.language.ISOCode.ToLower() == CurrencyLanguage.language.ISOCode.ToLower() && pgl.name.ToLower() == CurrencyLanguage.name.ToLower()))
                    throw new Exception(string.Concat("Currency already exists"));
            }
            
            await _context.Currencies.AddAsync(Currency, default);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Guid id, CurrencyDto currencyDto)
        {   
            var Currency = await this._context.Currencies.FirstOrDefaultAsync(al => al.id == id);

            if (Currency == new Currency() || Currency == null)
                throw new Exception("Currency does not exist");

            _mapper.Map(currencyDto, Currency, opts: opt => opt.Items["language"] = currencyDto.language);

            Currency.names = await GetNames(currencyDto, Currency);

            _context.Currencies.Update(Currency);
            await _context.SaveChangesAsync();
        }

        private Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<GiecChallenge.Models.Currency, GiecChallenge.Models.Language> GetCurrenciesWithInclude() {
            return this._context.Currencies.Include(b => b.names).ThenInclude(b => b.language);
        }

        private async Task<List<CurrencyLanguage>> GetNames(CurrencyDto currencyDto, Currency Currency) {
            var CurrencyLanguages = new List<CurrencyLanguage>();
            
            foreach (CurrencyNamesDto name in currencyDto.names) {
                var currentLanguageToInsert = await _context.Languages.FirstOrDefaultAsync(l => l.ISOCode.ToLower() == name.language.ToLower());
                if (currentLanguageToInsert == null)
                    throw new Exception(string.Concat("Language ", name.language, " doesn't exist"));
                if (Currency.names.Any(l => l.language.ISOCode.ToLower() == name.language.ToLower()))
                    Currency.names.First(l => l.language.ISOCode.ToLower() == name.language.ToLower()).name = name.name;
                else 
                    Currency.names.Add(_mapper.Map<CurrencyLanguage>(name, opts: opt => { 
                        opt.Items["language"] = currentLanguageToInsert;
                    }));
            }

            return Currency.names;
        }
    }
}
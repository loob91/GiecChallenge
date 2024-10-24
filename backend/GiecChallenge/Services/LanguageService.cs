using AutoMapper;
using GiecChallenge.Models;
using Microsoft.EntityFrameworkCore;

namespace GiecChallenge.Services
{
    public interface ILanguageRepository
    {
        Task<List<LanguageDto>> GetAllLanguages();
        Task<LanguageDto?> GetLanguage(Guid id);
        Task<List<LanguageDto>> GetLanguages(string name, string languageCode);
        Task Create(LanguageDto LanguageDto);
        Task Update(Guid id, LanguageDto LanguageDto);
    }

    public class LanguageService : ILanguageRepository
    {
        private readonly ILogger<LanguageService> _logger;
        private readonly IMapper _mapper;
        private readonly GiecChallengeContext _context;

        public LanguageService(ILogger<LanguageService> logger,
                              IMapper mapper,
                              GiecChallengeContext context) {
            this._logger = logger;
            this._mapper = mapper;
            this._context = context;
        }

        public async Task<List<LanguageDto>> GetAllLanguages()
        {
            var allLanguages = await GetLanguagesWithInclude().ToListAsync();
            return _mapper.Map<List<Language>, List<LanguageDto>>(allLanguages);
        }

        public async Task<LanguageDto?> GetLanguage(Guid id)
        {
            Language? Language = await GetLanguagesWithInclude().FirstOrDefaultAsync(gr => gr.id == id);
            if (Language == null)
                throw new Exception(string.Concat("Language does not exist"));
            return _mapper.Map<Language, LanguageDto>(Language);
        }

        public async Task<List<LanguageDto>> GetLanguages(string name, string languageCode)
        {
                var result = await GetLanguagesWithInclude().Where(al => al.names.Any(aln => aln.name.ToLower() == name.ToLower())).Select(s => new Tuple<int, Language>(1, s)).ToListAsync();
                result.AddRange(await GetLanguagesWithInclude().Where(al => al.names.Any(aln => aln.name.ToLower().StartsWith(name.ToLower()))).Select(s => new Tuple<int, Language>(2, s)).ToListAsync());
                result.AddRange(await GetLanguagesWithInclude().Where(al => al.names.Any(aln => aln.name.ToLower().Contains(name.ToLower()))).Select(s => new Tuple<int, Language>(3, s)).ToListAsync());
                return _mapper.Map<List<LanguageDto>>(result.OrderBy(s => s.Item1).Select(s => s.Item2).Distinct().ToList()).ToList();
        }

        public async Task Create(LanguageDto languageDto)
        {
            var language = _mapper.Map<Language>(languageDto, opts: opt => opt.Items["language"] = languageDto.language);

            language.names = await GetNames(languageDto, language);

            foreach (LanguageLanguage languageLanguage in language.names) {
                if (await _context.LanguageLanguages.Include(b => b.language).AnyAsync(pgl => pgl.language.ISOCode.ToLower() == languageLanguage.language.ISOCode.ToLower() && pgl.name.ToLower() == languageLanguage.name.ToLower()))
                    throw new Exception(string.Concat("Language already exists"));
            }

            await _context.Languages.AddAsync(language);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Guid id, LanguageDto languageDto)
        {   
            var language = await this._context.Languages.Include(b =>b.names).ThenInclude(b => b.language).FirstOrDefaultAsync(al => al.id == id);

            if (language == new Language() || language == null)
                throw new Exception("Language does not exist");

            _mapper.Map(languageDto, language, opts: opt => opt.Items["language"] = languageDto.language);

            language.names = await GetNames(languageDto, language);

            _context.Languages.Update(language);
            await _context.SaveChangesAsync();
        }

        private Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<GiecChallenge.Models.Language, GiecChallenge.Models.Language> GetLanguagesWithInclude() {
            return this._context.Languages.Include(b => b.names).ThenInclude(b => b.language);
        }

        private async Task<List<LanguageLanguage>> GetNames(LanguageDto languageDto, Language language) {
            foreach (LanguageNamesDto name in languageDto.names) {
                var currentLanguageToInsert = name.language == language.ISOCode ?
                                                language :
                                                await _context.Languages.Include(b => b.names).SingleOrDefaultAsync(l => l.ISOCode.ToLower() == name.language.ToLower());
                if (currentLanguageToInsert == null)
                    throw new Exception(string.Concat("Language ", name.language, " doesn't exist"));
                if (language.names.Any(l => l.language.ISOCode.ToLower() == name.language.ToLower()))
                    language.names.First(l => l.language.ISOCode == name.language).name = name.name;
                else 
                    language.names.Add(_mapper.Map<LanguageLanguage>(name, opts: opt => { 
                        opt.Items["language"] = currentLanguageToInsert;
                    }));
            }
            return language.names;
        }
    }
}
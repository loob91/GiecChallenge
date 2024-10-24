using AutoMapper;
using GiecChallenge.Models;
using Microsoft.EntityFrameworkCore;

namespace GiecChallenge.Services
{
    public interface IAlimentRepository
    {
        Task<List<AlimentDto>> GetAllAliments();
        Task<AlimentDto?> GetAliment(string code);
        Task<List<AlimentDto>> GetAliments(string name);
        Task Create(AlimentDto alimentDto);
        Task Update(AlimentDto aliment);
    }

    public class AlimentService : IAlimentRepository
    {
        private readonly ILogger<AlimentService> _logger;
        private readonly IMapper _mapper;
        private readonly GiecChallengeContext _context;

        public AlimentService(ILogger<AlimentService> logger,
                              IMapper mapper,
                              GiecChallengeContext context) {
            this._logger = logger;
            this._mapper = mapper;
            this._context = context;
        }

        public async Task<List<AlimentDto>> GetAllAliments()
        {
            var languageFR = await getLanguageFR();
            var allAliments = await _context.Aliments.Include(b => b.names).Include(b => b.subgroup).Include(b => b.subgroup.names).ToListAsync();
            return _mapper.Map<List<Aliment>, List<AlimentDto>>(allAliments, opts: opt => opt.Items["language"] = languageFR);
        }

        public async Task<AlimentDto?> GetAliment(string code)
        {
            var languageFR = await getLanguageFR();
            Aliment? aliment = await this._context.Aliments.Include(b => b.names).Include(b => b.subgroup).Include(b => b.subgroup.names).FirstOrDefaultAsync(al => al.ciqual == code);
            if (aliment == null)
                throw new Exception(string.Concat(code, " does not exist"));
            return _mapper.Map<Aliment, AlimentDto>(aliment, opts: opt => opt.Items["language"] = languageFR);
        }

        public async Task<List<AlimentDto>> GetAliments(string name)
        {
            var languageFR = await getLanguageFR();
            var result = await _context.ProductUserTranslations.Where(put => put.name == name).Include(p => p.product).ThenInclude(b => b.names).ThenInclude(b => b.language).Include(b => b.product).ThenInclude(b => b.subgroup).ThenInclude(b => b.names).Select(s => new Tuple<int, Aliment>(1, (Aliment)s.product)).ToListAsync();
            result.AddRange(await _context.Aliments.Include(b => b.names).Include(b => b.subgroup).ThenInclude(b => b.names).Where(al => al.names.Any(aln => aln.name.ToLower() == name.ToLower())).Select(s => new Tuple<int, Aliment>(1, s)).ToListAsync());
            result.AddRange(await _context.Aliments.Include(b => b.names).Include(b => b.subgroup).ThenInclude(b => b.names).Where(al => al.names.Any(aln => aln.name.ToLower().StartsWith(name.ToLower()))).Select(s => new Tuple<int, Aliment>(2, s)).ToListAsync());
            result.AddRange(await _context.Aliments.Include(b => b.names).Include(b => b.subgroup).ThenInclude(b => b.names).Where(al => al.names.Any(aln => aln.name.ToLower().Contains(name.ToLower()))).Select(s => new Tuple<int, Aliment>(3, s)).ToListAsync());
            return _mapper.Map<List<Aliment>, List<AlimentDto>>(result.OrderBy(s => s.Item1).Select(s => s.Item2).Distinct().ToList(), opts: opt => opt.Items["language"] = languageFR).ToList();
        }

        public async Task Create(AlimentDto alimentDto)
        {
            var languageFR = await getLanguageFR();
            if (await _context.Aliments.AnyAsync(x => alimentDto.ciqual_code.Equals(x.ciqual)))
                throw new Exception(alimentDto.ciqual_code + " already exists");

            var aliment = _mapper.Map<Aliment>(alimentDto, opts: opt => opt.Items["language"] = languageFR);

            var subgroup = await _context.ProductSubGroups.FirstOrDefaultAsync(g => g.names.Any(gn => gn.name.Equals(alimentDto.groupe)));

            if (subgroup == null) {
                subgroup = await CreateSubGroup(alimentDto);
                await _context.ProductSubGroups.AddAsync(subgroup, default);
            }

            aliment.names.Add(new ProductLanguage() { id = new Guid(), name = alimentDto.nom_francais, language = languageFR });

            aliment.subgroup = subgroup;

            await _context.Aliments.AddAsync(aliment, default);
            await _context.SaveChangesAsync();
        }

        public async Task Update(AlimentDto alimentDto)
        {   
            var languageFR = await getLanguageFR();
            var aliment = await this._context.Aliments.FirstOrDefaultAsync(al => al.ciqual == alimentDto.ciqual_code);

            if (aliment == new Aliment() || aliment == null)
                throw new Exception("Aliment does not exist");

            var subgroup = await _context.ProductSubGroups.FirstOrDefaultAsync(g => g.names.Any(gn => gn.name.Equals(alimentDto.groupe)));

            if (subgroup == null) {
                subgroup = await CreateSubGroup(alimentDto);
                await _context.ProductSubGroups.AddAsync(subgroup, default);
            }

            aliment.subgroup = subgroup;
            
            var names = aliment.names.FirstOrDefault(aln => aln.language.ISOCode == languageFR.ISOCode);
            if (names == null)
                aliment.names.Add(new ProductLanguage() { id = new Guid(), language = languageFR, name = alimentDto.nom_francais });
            else
                names.name = alimentDto.nom_francais;

            _mapper.Map(alimentDto, aliment, opts: opt => opt.Items["language"] = languageFR);
            _context.Aliments.Update(aliment);
            await _context.SaveChangesAsync();
        }

        private async Task<ProductSubGroup> CreateSubGroup(AlimentDto alimentDto) {
            var languageFR = await getLanguageFR();
            ProductGroup group = await _context.ProductGroups.FirstAsync(pg => pg.names.Any(pgn => pgn.name.Equals("Aliment") && pgn.language == languageFR));
            return new ProductSubGroup() { 
                id = new Guid(),
                names = new List<ProductSubGroupLanguage>() {
                    new ProductSubGroupLanguage() {
                        id = new Guid(),
                        language = languageFR,
                        name = alimentDto.groupe
                    }
                },
                Groupe = group
            };
        }

        private async Task<Language> getLanguageFR() {
            return await _context.Languages.FirstAsync(l => l.ISOCode == "FR");
        }
    }
}
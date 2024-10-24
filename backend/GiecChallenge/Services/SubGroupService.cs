using AutoMapper;
using GiecChallenge.Models;
using Microsoft.EntityFrameworkCore;

namespace GiecChallenge.Services
{
    public interface ISubGroupRepository
    {
        Task<List<SubGroupDto>> GetAllSubGroups();
        Task<SubGroupDto?> GetSubGroup(Guid id);
        Task<List<SubGroupDto>> GetSubGroups(string name, string languageCode);
        Task Create(SubGroupDto SubGroupDto);
        Task Update(Guid id, SubGroupDto SubGroupDto);
    }

    public class SubGroupService : ISubGroupRepository
    {
        private readonly ILogger<SubGroupService> _logger;
        private readonly IMapper _mapper;
        private readonly GiecChallengeContext _context;

        public SubGroupService(ILogger<SubGroupService> logger,
                              IMapper mapper,
                              GiecChallengeContext context) {
            this._logger = logger;
            this._mapper = mapper;
            this._context = context;
        }

        public async Task<List<SubGroupDto>> GetAllSubGroups()
        {
            var allSubGroups = await GetSubGroupsWithInclude().ToListAsync();
            return _mapper.Map<List<ProductSubGroup>, List<SubGroupDto>>(allSubGroups);
        }

        public async Task<SubGroupDto?> GetSubGroup(Guid id)
        {
            ProductSubGroup? subgroup = await GetSubGroupsWithInclude().FirstOrDefaultAsync(gr => gr.id == id);
            if (subgroup == null)
                throw new Exception(string.Concat("Sub Group does not exist"));
            return _mapper.Map<ProductSubGroup, SubGroupDto>(subgroup);
        }

        public async Task<List<SubGroupDto>> GetSubGroups(string name, string languageCode)
        {
                var result = await GetSubGroupsWithInclude().Where(al => al.names.Any(aln => aln.name.ToLower() == name.ToLower())).Select(s => new Tuple<int, ProductSubGroup>(1, s)).ToListAsync();
                result.AddRange(await GetSubGroupsWithInclude().Where(al => al.names.Any(aln => aln.name.ToLower().StartsWith(name.ToLower()))).Select(s => new Tuple<int, ProductSubGroup>(2, s)).ToListAsync());
                result.AddRange(await GetSubGroupsWithInclude().Where(al => al.names.Any(aln => aln.name.ToLower().Contains(name.ToLower()))).Select(s => new Tuple<int, ProductSubGroup>(3, s)).ToListAsync());
                return _mapper.Map<List<SubGroupDto>>(result.OrderBy(s => s.Item1).Select(s => s.Item2).Distinct().ToList()).ToList();
        }

        public async Task Create(SubGroupDto subGroupDto)
        {
            var subgroup = _mapper.Map<ProductSubGroup>(subGroupDto, opts: opt => opt.Items["language"] = subGroupDto.language);

            if (!Guid.TryParse(subGroupDto.group, out Guid groupId))
                throw new Exception(string.Concat(subGroupDto.group, " is not valid"));

            foreach (SubGroupNamesDto names in subGroupDto.names) {
                if (await _context.ProductSubGroups.Include(b => b.Groupe).ThenInclude(b => b.names).ThenInclude(b => b.language).AnyAsync(psg => psg.names.Any(psgn => psgn.name.ToLower() == names.name.ToLower() && psg.Groupe.id == groupId)))
                    throw new Exception(string.Concat("Sub Group already exists"));
            }

            subgroup.names = await GetNames(subGroupDto, subgroup);
            subgroup.Groupe = await getGroup(subGroupDto, groupId);
            
            await _context.ProductSubGroups.AddAsync(subgroup, default);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Guid id, SubGroupDto subGroupDto)
        {   
            if (!Guid.TryParse(subGroupDto.group, out Guid groupId))
                throw new Exception(string.Concat(subGroupDto.group, " is not valid"));

            var subGroup = await this._context.ProductSubGroups.FirstOrDefaultAsync(al => al.id == id);

            if (subGroup == new ProductSubGroup() || subGroup == null)
                throw new Exception("Sub Group does not exist");

            subGroup = _mapper.Map(subGroupDto, subGroup, opts: opt => opt.Items["language"] = subGroupDto.language);

            subGroup.names = await GetNames(subGroupDto, subGroup);
            if (!string.IsNullOrEmpty(subGroupDto.group))
                subGroup.Groupe = await getGroup(subGroupDto, groupId);

            _context.ProductSubGroups.Update(subGroup);
            await _context.SaveChangesAsync();
        }

        private Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<GiecChallenge.Models.ProductSubGroup, GiecChallenge.Models.Language> GetSubGroupsWithInclude() {
            return this._context.ProductSubGroups.Include(b => b.names).ThenInclude(b => b.language).Include(b => b.Groupe).ThenInclude(b => b.names).ThenInclude(b => b.language);
        }

        private async Task<List<ProductSubGroupLanguage>> GetNames(SubGroupDto SubGroupDto, ProductSubGroup subGroup) {            
            foreach (SubGroupNamesDto name in SubGroupDto.names) {
                if (!Guid.TryParse(name.language, out Guid languageId))
                    throw new Exception(string.Concat(name.language, " is not valid"));
                var currentLanguageToInsert = await _context.Languages.FirstOrDefaultAsync(l => l.id == languageId);
                if (currentLanguageToInsert == null)
                    throw new Exception(string.Concat("Language ", name.language, " doesn't exist"));
                if (subGroup.names.Any(l => l.language.ISOCode.ToLower() == name.language.ToLower()))
                    subGroup.names.First(l => l.language.ISOCode.ToLower() == name.language.ToLower()).name = name.name;
                else 
                    subGroup.names.Add(_mapper.Map<ProductSubGroupLanguage>(name, opts: opt => { 
                        opt.Items["language"] = currentLanguageToInsert;
                    }));
            }

            return subGroup.names;
        }

        private async Task<ProductGroup> getGroup(SubGroupDto subGroupDto, Guid groupId) {
            var group = await this._context.ProductGroups.FirstOrDefaultAsync(pg => pg.id == groupId);

            if (group == null)
                throw new Exception(string.Concat("Group does not exist"));
            return group;
        }
    }
}
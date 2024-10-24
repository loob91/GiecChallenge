using AutoMapper;
using GiecChallenge.Models;
using Microsoft.EntityFrameworkCore;

namespace GiecChallenge.Services
{
    public interface IGroupRepository
    {
        Task<List<GroupDto>> GetAllGroups();
        Task<GroupDto?> GetGroup(Guid id);
        Task<List<GroupDto>> GetGroups(string name, string languageCode);
        Task Create(GroupDto GroupDto);
        Task Update(Guid id, GroupDto GroupDto);
    }

    public class GroupService : IGroupRepository
    {
        private readonly ILogger<GroupService> _logger;
        private readonly IMapper _mapper;
        private readonly GiecChallengeContext _context;

        public GroupService(ILogger<GroupService> logger,
                              IMapper mapper,
                              GiecChallengeContext context) {
            this._logger = logger;
            this._mapper = mapper;
            this._context = context;
        }

        public async Task<List<GroupDto>> GetAllGroups()
        {
            var allGroups = await GetGroupsWithInclude().ToListAsync();
            return _mapper.Map<List<ProductGroup>, List<GroupDto>>(allGroups);
        }

        public async Task<GroupDto?> GetGroup(Guid id)
        {
            ProductGroup? group = await GetGroupsWithInclude().FirstOrDefaultAsync(gr => gr.id == id);
            if (group == null)
                throw new Exception(string.Concat("Group does not exist"));
            return _mapper.Map<ProductGroup, GroupDto>(group);
        }

        public async Task<List<GroupDto>> GetGroups(string name, string languageCode)
        {
                var result = await GetGroupsWithInclude().Where(al => al.names.Any(aln => aln.name.ToLower() == name.ToLower() && aln.language.ISOCode == languageCode)).Select(s => new Tuple<int, ProductGroup>(1, s)).ToListAsync();
                result.AddRange(await GetGroupsWithInclude().Where(al => al.names.Any(aln => aln.name.ToLower().StartsWith(name.ToLower()) && aln.language.ISOCode == languageCode)).Select(s => new Tuple<int, ProductGroup>(2, s)).ToListAsync());
                result.AddRange(await GetGroupsWithInclude().Where(al => al.names.Any(aln => aln.name.ToLower().Contains(name.ToLower()) && aln.language.ISOCode == languageCode)).Select(s => new Tuple<int, ProductGroup>(3, s)).ToListAsync());
                return _mapper.Map<List<GroupDto>>(result.OrderBy(s => s.Item1).Select(s => s.Item2).Distinct().ToList()).ToList();
        }

        public async Task Create(GroupDto groupDto)
        {
            var group = _mapper.Map<ProductGroup>(groupDto, opts: opt => opt.Items["language"] = groupDto.language);

            group.names = await GetNames(groupDto, group);

            foreach (ProductGroupLanguage groupLanguage in group.names) {
                if (await _context.ProductGroupLanguages.Include(b => b.language).AnyAsync(pgl => pgl.language.ISOCode.ToLower() == groupLanguage.language.ISOCode.ToLower() && pgl.name.ToLower() == groupLanguage.name.ToLower()))
                    throw new Exception(string.Concat("Group already exists"));
            }
            
            await _context.ProductGroups.AddAsync(group, default);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Guid id, GroupDto groupDto)
        {   
            var group = await this._context.ProductGroups.FirstOrDefaultAsync(al => al.id == id);

            if (group == new ProductGroup() || group == null)
                throw new Exception("Group does not exist");

            _mapper.Map(groupDto, group, opts: opt => opt.Items["language"] = groupDto.language);

            group.names = await GetNames(groupDto, group);

            _context.ProductGroups.Update(group);
            await _context.SaveChangesAsync();
        }

        private Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<GiecChallenge.Models.ProductGroup, GiecChallenge.Models.Language> GetGroupsWithInclude() {
            return this._context.ProductGroups.Include(b => b.names).ThenInclude(b => b.language);
        }

        private async Task<List<ProductGroupLanguage>> GetNames(GroupDto groupDto, ProductGroup group) {
            var groupLanguages = new List<ProductGroupLanguage>();
            
            foreach (GroupNamesDto name in groupDto.names) {
                if (!Guid.TryParse(name.language, out Guid languageId))
                    throw new Exception(string.Concat(name.language, " is not valid"));
                var currentLanguageToInsert = await _context.Languages.FirstOrDefaultAsync(l => l.id == languageId);
                if (currentLanguageToInsert == null)
                    throw new Exception(string.Concat("Language ", name.language, " doesn't exist"));
                if (group.names.Any(l => l.id == languageId))
                    group.names.First(l => l.language.id == languageId).name = name.name;
                else 
                    group.names.Add(_mapper.Map<ProductGroupLanguage>(name, opts: opt => { 
                        opt.Items["language"] = currentLanguageToInsert;
                    }));
            }

            return group.names;
        }
    }
}
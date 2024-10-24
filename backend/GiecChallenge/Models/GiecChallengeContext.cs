using Microsoft.EntityFrameworkCore;

namespace GiecChallenge.Models
{
    public class GiecChallengeContext : DbContext
    {
        public GiecChallengeContext(DbContextOptions<GiecChallengeContext> options):base(options) {  
            base.Database.EnsureCreated();
        }

        public virtual void SetEntityStateModified(Purchase purchase)
        {
            base.Entry(purchase).State = EntityState.Modified;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Aliment>();
            modelBuilder.Entity<CarbonLoan>();
            modelBuilder.Entity<Currency>();
            modelBuilder.Entity<CurrencyLanguage>();
            modelBuilder.Entity<Language>();
            modelBuilder.Entity<LanguageLanguage>();
            modelBuilder.Entity<Product>();
            modelBuilder.Entity<ProductGroup>();
            modelBuilder.Entity<ProductGroupLanguage>();
            modelBuilder.Entity<ProductLanguage>();
            modelBuilder.Entity<ProductPurchase>();
            modelBuilder.Entity<ProductSubGroup>();
            modelBuilder.Entity<ProductSubGroupLanguage>();
            modelBuilder.Entity<ProductUserTranslation>();
            modelBuilder.Entity<Purchase>();
            modelBuilder.Entity<User>();
            modelBuilder.Entity<UserGroup>();
        }

        public virtual DbSet<Aliment> Aliments { get; set; } = null!;
        public virtual DbSet<CarbonLoan> CarbonLoans { get; set; } = null!;
        public virtual DbSet<Currency> Currencies { get; set; } = null!;
        public virtual DbSet<CurrencyLanguage> CurrencyLanguages { get; set; } = null!;
        public virtual DbSet<Language> Languages { get; set; } = null!;
        public virtual DbSet<LanguageLanguage> LanguageLanguages { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductGroup> ProductGroups { get; set; } = null!;
        public virtual DbSet<ProductGroupLanguage> ProductGroupLanguages { get; set; } = null!;
        public virtual DbSet<ProductLanguage> ProductLanguages { get; set; } = null!;
        public virtual DbSet<ProductPurchase> ProductPurchases { get; set; } = null!;
        public virtual DbSet<ProductSubGroup> ProductSubGroups { get; set; } = null!;
        public virtual DbSet<ProductSubGroupLanguage> ProductSubGroupLanguages { get; set; } = null!;
        public virtual DbSet<ProductUserTranslation> ProductUserTranslations { get; set; } = null!;
        public virtual DbSet<Purchase> Purchases { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserGroup> UserGroups { get; set; } = null!;
    }
}
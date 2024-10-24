using Microsoft.EntityFrameworkCore;
using GiecChallenge.Models;
using GiecChallenge.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

builder.Services.AddControllers(
    options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

builder.Services.AddEntityFrameworkNpgsql().AddDbContext<GiecChallengeContext>(opt =>
          opt.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddCors(p =>
    p.AddPolicy("CORSPolicy", x => x.AllowAnyHeader()
                            .AllowAnyMethod()
                            .SetIsOriginAllowed(_ => true)
                            .AllowCredentials())
);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
  {
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
  });

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = configuration["JWT:ValidAudience"],
        ValidIssuer = configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]!))
    };
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.All;
});

builder.Services.AddAuthorization();

addScoped();

var app = builder.Build();

app.UseForwardedHeaders();

app.UseHttpsRedirection();

app.UseRouting();

app.Use((ctx, next) =>
{
    var headers = ctx.Response.Headers;
    headers["Access-Control-Allow-Origin"] = configuration["OriginAllowed"];
    headers["Access-Control-Allow-Credentials"] = "true";
    headers["Access-Control-Allow-Headers"] = "Content-Type, X-CSRF-Token, X-Requested-With, Accept, Accept-Version, Content-Length, Content-MD5, Date, X-Api-Version, X-File-Name";
    headers["Access-Control-Allow-Methods"] = "POST,GET,PUT,PATCH,DELETE,OPTIONS";
    headers["cc"] = "test";

   return next();
});

app.UseCors("CORSPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.MapSwagger();

app.Run();

void addScoped() {
    builder.Services.AddScoped<IAlimentRepository, AlimentService>();
    builder.Services.AddScoped<IProductRepository, ProductService>();
    builder.Services.AddScoped<ICurrencyRepository, Currencieservice>();
    builder.Services.AddScoped<IGroupRepository, GroupService>();
    builder.Services.AddScoped<ISubGroupRepository, SubGroupService>();
    builder.Services.AddScoped<ILanguageRepository, LanguageService>();
    builder.Services.AddScoped<IUserRepository, UserService>();
    builder.Services.AddScoped<IPurchaseRepository, PurchaseService>();
}
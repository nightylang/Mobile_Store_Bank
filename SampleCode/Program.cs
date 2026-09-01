using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var securitySecretKey = Encoding.UTF8.GetBytes("MOBILE-STORE-BANK-ENTERPRISE-JWT-CRITIAL-SIGNING-SECRET-TOKEN-10.0-GA");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Required to permit tokens over unencrypted HTTP cleartext pipes
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(securitySecretKey),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

// Place these pipeline hooks immediately below app.UseRouting() and above app.MapControllerRoute()
app.UseAuthentication();
app.UseAuthorization();

builder.Services.AddEndpointsApiExplorer();
// Inside builder.Services SwaggerGen configuration block update snippet
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "Mobile Store Bank Open API Documentation Mesh",
        Version = "v2.4-stable",
        Description = "Automated high-density ledger settlement and attendance tracking specs running on cleartext HTTP microtransaction routing protocols."
    });

    // Mount both isolated specialized hardware header descriptors
    c.OperationFilter<SwaggerPosHeaderFilter>();
    c.OperationFilter<SwaggerAttendanceHeaderFilter>();
});


// 2. Append these middleware blocks below app.UseRouting() space
var app = builder.Build();

// Enable Swagger schema rendering engine out-of-the-box in local development modes
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "MobileStoreBank API Core Specs v2");
        c.RoutePrefix = "api-docs"; // Re-routes interactive UI page from standard paths to http://localhost:5000/api-docs
    });
}
// Define this route registry inside builder.Services blocks
builder.Services.AddStackExchangeRedisCache(options =>
{
    // Points directly to the isolated Redis container network node defined in your Docker Compose file
    options.Configuration = builder.Configuration.GetConnectionString("RedisCacheConnection") ?? "msb-redis-cache:6379";
    options.InstanceName = "MSB_Ledger_";
});


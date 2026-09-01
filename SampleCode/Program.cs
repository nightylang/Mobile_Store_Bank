builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "Mobile Store Bank Open API Documentation Mesh",
        Version = "v2.4-stable",
        Description = "Automated high-density ledger settlement endpoint specifications designed for cleartext HTTP microtransaction routing protocols."
    });

    // Mount the operation filter to append required header parameters on endpoints natively
    c.OperationFilter<SwaggerPosHeaderFilter>();
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

using Microsoft.OpenApi.Models;
using MyClassLibrary.DataAccessMethods;
using MyClassLibrary.Extensions;
using MyClassLibrary.LocalServerMethods.Interfaces;
using MyClassLibrary.LocalServerMethods.Models;
using System.Reflection;
using TheWhaddonShowClassLibrary.Models;
using TheWhaddonShowClassLibrary.SampleData;
using TheWhaddonShowTesting.Tests;


var builder = WebApplication.CreateBuilder(args);



//TODO Tidy Up Program.cs and check Appsettings etc.
//TODO Add PersonUpdate and SCriptItemUpdate Controllers.
//TODO Add in HealthChecks and Logging
//TODO Get Version 1 working
//TODO Setup Postman Testing
//TODO Setup testing of SqlDataAccess Layer utilising this API either using ROCP access or preferably Test Web Host for authentication to http clients
//Create monitoring dashboard.

builder.ConfigureWebAPIAuthentication_AzureAdB2C();

builder.ByPassAuthenticationIfInDevelopment();






builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts =>
{
	var title = "The Whaddon Show API";
	var description = "Provides data access to The Whaddon Show's central database.";
	var contact = new OpenApiContact()
	{
		Name = "Mark Carter",
		Email = "magcarter@hotmail.co.uk"
		//TODO - add in home page to OpenApiContact
	};

	opts.SwaggerDoc("v1 (deprecated)", new OpenApiInfo()
	{
		Version = "v1",
		Title = title,
		Description = description,
		Contact = contact
	});
	opts.SwaggerDoc("v2", new OpenApiInfo()
	{
		Version = "v2",
		Title = title,
		Description = description,
		Contact = contact
	});

	var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	opts.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
});
builder.Services.AddApiVersioning(opts =>
{
	opts.AssumeDefaultVersionWhenUnspecified = true;
	opts.DefaultApiVersion = new(2, 0);
	opts.ReportApiVersions = true;
});

builder.Services.AddApiVersioning().AddApiExplorer(opts =>
{
	opts.GroupNameFormat = "'v'VVV";
	opts.SubstituteApiVersionInUrl = true;
});


builder.Services.AddHealthChecks();


builder.Services.AddSingleton<ISqlDataAccess, SqlDataAccess>();
builder.Services.AddSingleton(typeof(IServerDataAccess<>), typeof(ServerSQLConnector<>));
builder.Services.AddSingleton(typeof(IServerAPIControllerService<>), typeof(ServerAPIControllerService<>));
builder.Services.AddSingleton(typeof(ISampleDataProvider<>), typeof(SampleDataProvider<>));

var app = builder.Build();



var helper = new Helper(
			app.Services.GetRequiredService<IServerDataAccess<PartUpdate>>(),
			app.Services.GetRequiredService<IServerDataAccess<PersonUpdate>>(),
			app.Services.GetRequiredService<IServerDataAccess<ScriptItemUpdate>>()
		);

helper.ResetSampleData();


// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI(opts =>
{
	opts.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");
	opts.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
	opts.RoutePrefix = string.Empty;

});
//}

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); ;
app.MapHealthChecks("/health");


app.Run();

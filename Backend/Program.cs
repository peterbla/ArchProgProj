using System.Security.Cryptography.X509Certificates;

using Backend.DatabaseModels;
using Backend.Services;
using Backend;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Models;

using Supabase;

const string supabaseUrl = "https://jjctsjkcpqocbinkucvv.supabase.co";
const string supabaseApiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImpqY3RzamtjcHFvY2Jpbmt1Y3Z2Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzYwOTM5MDAsImV4cCI6MjA1MTY2OTkwMH0.HyYXyn1a-SePbP_4U3gds4gWf42-IBOe3K8N56cHrps";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ProductService>();
builder.Services.AddSingleton<UserService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
          new string[] { }
        }
    });
});

builder.Services.AddSingleton<TokenService>();
builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;
    config.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(TokenService.GenerateSecretByte()),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("user", policy => policy.RequireRole("user"));
});

SupabaseOptions supabaseOptions = new()
{
    AutoRefreshToken = true,
    AutoConnectRealtime = true,
};
builder.Services.AddSingleton(provider => new Supabase.Client(supabaseUrl, supabaseApiKey, supabaseOptions));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseAuthentication();
app.UseAuthorization();

# region Login
app.MapPost("/login", async ([FromBody] UserCredentials userCredentials, TokenService tokenService) =>
{
    return await Task.Run(Results<NotFound<string>, Ok<UserCredentialsWithToken>> () =>
    {
        User? expectedUser = MockDatabase.users.Find(u => u.Name == userCredentials.Name);

        if (expectedUser == null)
        {
            return TypedResults.NotFound("Invalid username or password");
        }

        if (expectedUser.PasswordHash != UserService.HashPassword(userCredentials.Password))
        {
            return TypedResults.NotFound("Invalid username or password");
        }

        var token = "Bearer " + tokenService.GenerateToken(expectedUser);

        userCredentials.Password = String.Empty;

        return TypedResults.Ok(new UserCredentialsWithToken { User = userCredentials, Token = token });
    });

});
#endregion

#region Products
app.MapGet("/products", async (ProductService productService) =>
{
    List<Product> allProducts = await productService.GetAllProducts();
    return TypedResults.Ok(allProducts);
})
.RequireAuthorization("user")
.WithName("Get Products")
.WithOpenApi();

app.MapGet("/products/{id}", async ([FromRoute] int id) =>
{
    return await Task.Run(Results<NotFound, Ok<Product>> () =>
    {
        Product? product = MockDatabase.products.Find(pr => pr.Id == id);

        if (product == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(product);
    });
})
.RequireAuthorization("user")
.WithName("Get Single Product")
.WithOpenApi();

app.MapPost("/products", async ([FromBody] NewProduct newProduct) =>
{
    return await Task.Run(Results<BadRequest<string>, Created<Product>> () =>
    {
        Product product = new()
        {
            Id = MockDatabase.products.Count + 1,
            Name = newProduct.Name,
            Energy = newProduct.Energy,
            Fat = newProduct.Fat,
            Saturates = newProduct.Saturates,
            Carbohydrate = newProduct.Carbohydrate,
            Sugars = newProduct.Sugars,
            Fibre = newProduct.Fibre,
            Protein = newProduct.Protein,
            Salt = newProduct.Salt
        };
        MockDatabase.products.Add(product);
        return TypedResults.Created($"/products/{product.Id}", product);
    });
})
.RequireAuthorization("user")
.WithName("Add New Product")
.WithOpenApi();
#endregion

#region MealEntries
app.MapGet("/meals", async (HttpContext httpContext) =>
{
    return await Task.Run(() =>
    {
        User user = MockDatabase.GetUserFromHttpContext(httpContext);
        return TypedResults.Ok(MockDatabase.eatingHistory);
    });
})
.RequireAuthorization("user")
.WithName("Get User's Meals")
.WithOpenApi();

app.MapGet("/meals/{mealId}", async (HttpContext httpContext, [FromRoute] int mealId) =>
{
    return await Task.Run(Results<NotFound, Ok<MealEntry>> () =>
    {
        User user = MockDatabase.GetUserFromHttpContext(httpContext);
        MealEntry? mealEntry = MockDatabase.eatingHistory.Find(me => me.Id == mealId);
        if (mealEntry == null)
        {
            return TypedResults.NotFound();
        }
        return TypedResults.Ok(mealEntry);
    });
})
.RequireAuthorization("user")
.WithName("Get User's Single Meal")
.WithOpenApi();

app.MapPost("/meals", async ([FromBody] NewMealEntry eating, HttpContext httpContext) =>
{
    return await Task.Run(Results<BadRequest<string>, Created<MealEntry>> () =>
    {

        MealEntry mealEntry = new()
        {
            Id = MockDatabase.eatingHistory.Count + 1,
            UserId = MockDatabase.GetUserFromHttpContext(httpContext).Id,
            Date = eating.Date,
            MealType = eating.MealType
        };

        MockDatabase.eatingHistory.Add(mealEntry);
        return TypedResults.Created($"/meals/{mealEntry.Id}", mealEntry);
    });
})
.RequireAuthorization("user")
.WithName("Add New Empty Meal")
.WithOpenApi();

app.MapGet("/meals/{mealId}/products", async (HttpContext httpContext, [FromRoute] int mealId) =>
{
    return await Task.Run(Results<NotFound, Ok<List<ProductInMeal>>> () =>
    {
        User user = MockDatabase.GetUserFromHttpContext(httpContext);
        MealEntry? mealEntry = MockDatabase.eatingHistory.Find(me => me.Id == mealId);
        if (mealEntry == null)
        {
            return TypedResults.NotFound();
        }
        return TypedResults.Ok(MockDatabase.productsInMeals);
    });
})
.RequireAuthorization("user")
.WithName("Get User's Single Meal's Products")
.WithOpenApi();

app.MapPost("/meals/{mealId}/products", async ([FromBody] NewProductInMeal newProductInMeal, HttpContext httpContext, [FromRoute] int mealId) =>
{
    return await Task.Run(Results<NotFound, Created<ProductInMeal>> () =>
    {
        User user = MockDatabase.GetUserFromHttpContext(httpContext);
        MealEntry? mealEntry = MockDatabase.eatingHistory.Find(me => me.Id == mealId);
        if (mealEntry == null)
        {
            return TypedResults.NotFound();
        }

        ProductInMeal productInMeal = new()
        {
            Id = MockDatabase.productsInMeals.Count + 1,
            ProductId = newProductInMeal.ProductId,
            MealEntryId = mealId,
            AmountG = newProductInMeal.AmountG,
        };

        MockDatabase.productsInMeals.Add(productInMeal);
        return TypedResults.Created($"", productInMeal);
    });
})
.RequireAuthorization("user")
.WithName("Add Product To a Meal")
.WithOpenApi();


#endregion

#region WeightsHistory
app.MapGet("/weightHistory", async (HttpContext httpContext) =>
{
    return await Task.Run(() =>
    {
        User user = MockDatabase.GetUserFromHttpContext(httpContext);
        return TypedResults.Ok(MockDatabase.weightHistory1);
    });
})
.RequireAuthorization("user")
.WithName("Get User's Weight History")
.WithOpenApi();

app.MapGet("/weightHistory/{weightId}", async (HttpContext httpContext, [FromRoute] int weightId) =>
{
    return await Task.Run(() =>
    {
        User user = MockDatabase.GetUserFromHttpContext(httpContext);
        return TypedResults.Ok(MockDatabase.weightHistory1[weightId]);
    });
})
.RequireAuthorization("user")
.WithName("Get User's Single Weight")
.WithOpenApi();

app.MapPost("/weightHistory", async ([FromBody] NewWeightHistory newWeightHistory, HttpContext httpContext) =>
{
    return await Task.Run(Results<BadRequest<string>, Created<WeightHistory>> () =>
    {

        WeightHistory weightHistory = new()
        {
            Id = MockDatabase.weightHistory1.Count + 1,
            UserId = MockDatabase.GetUserFromHttpContext(httpContext).Id,
            
        };

        MockDatabase.weightHistory1.Add(weightHistory);
        return TypedResults.Created($"/weightHistory/{weightHistory.Id}", weightHistory);
    });
})
.RequireAuthorization("user")
.WithName("Add New Weight To User's History")
.WithOpenApi();

#endregion

app.Run();

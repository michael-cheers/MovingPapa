using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Rewrite;
using MovingPapa.DB;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddScoped<MovingpapaContext>();
builder.Services.Configure<GzipCompressionProvider>(o => { });
builder.Services.AddResponseCompression(opt =>
{
    opt.EnableForHttps = true;
    opt.Providers.Add<GzipCompressionProvider>();
});
builder.Services.AddWebOptimizer(pipeline =>
{
    pipeline.MinifyHtmlFiles();
    pipeline.MinifyJsFiles();
    pipeline.MinifyCssFiles();
});
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseResponseCompression();
app.UseHttpsRedirection();
app.UseRewriter(new RewriteOptions().AddRewrite("^service-area/(bradford|toronto|oshawa|scarborough|pickering|vaughan|oakville|hamilton|celadon|brampton|mississauga|milton|st-catherines|landon|kingston|burlington|markham|richmond-hill|new-market|barrie|aurora|kitchener|waterloo|vancouver|burnaby|richmond|surrey|ottawa|gatineau|saskatchewan|winnipeg|saskatoon|halifax|regina|calgary|edmonton|kelowna|victoria|vancouver-island|new-brunswick)|(((residential|seniors|commercial)-moving|packing|storage))$", "index.html", true));
app.UseDefaultFiles();
//app.UseWebOptimizer();
app.UseStaticFiles();
app.UseRouting();
app.UseForwardedHeaders();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

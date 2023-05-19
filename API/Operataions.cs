using Library.Data;
using Library.Models;
using Message = Library.Models.Message;
using Product = Library.Models.Product;

namespace API
{
    public static class Operataions
    {
        public static WebApplication UseOperations(this WebApplication application)
        {
            application.MapGet("/products", (ObservabilityContext observabilityContext) =>
            {
                return Results.Ok(observabilityContext.Products.Take(10).ToList());
            });

            application.MapPost("/finalize", (ObservabilityContext observabilityContext, Product product) =>
            {
                if (product is null)
                    return Results.BadRequest();

                product.Status = Message.FinalizedCode;

                observabilityContext.Add(product);
                observabilityContext.SaveChanges();

                return Results.Ok(product);
            });

            application.MapPost("/supply", (ObservabilityContext observabilityContext, List<string> workstations) =>
            {
                if (workstations.Count == 0)
                    return Results.BadRequest();

                List<Supply> supplies = new();

                foreach (string workstation in workstations)
                {
                    if (string.IsNullOrEmpty(workstation))
                        continue;

                    Supply supply = new() { WorkStation = workstation };
                    supplies.Add(supply);
                    observabilityContext.Add(supply);
                }

                observabilityContext.SaveChanges();

                return Results.Ok(supplies);
            });

            application.MapPost("/approve", (ObservabilityContext observabilityContext, List<Product> products) =>
            {
                if (products is null || products.Count == 0)
                    return Results.BadRequest();

                foreach (Product product in products)
                {
                    product.Status = Message.ApproveCode;
                    observabilityContext.Add(product);
                }

                observabilityContext.SaveChanges();

                return Results.Ok(products);
            });

            application.MapGet("/error", () =>
            {
                throw new Exception("Error API simulate");
            });

            return application;
        }
    }
}
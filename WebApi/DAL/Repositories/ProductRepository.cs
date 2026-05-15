using System;
using System.Text;
using Dapper;
using WebApi.DAL;

public class ProductRepository(UnitOfWork unitOfWork) : IProductRepository
{
    public async Task<V1ProductDal[]> Query(QueryProductsDalModel model, CancellationToken token)
    {
        var offset = (model.Page - 1) * model.PageSize;
        
        var sql = new StringBuilder(@"
            select 
                id,
                title,
                description,
                category,
                price_cents,
                price_currency,
                product_url,
                stock,
                discount_percent,
                created_at,
                updated_at
            from products
            where 1=1
        ");

        var param = new DynamicParameters();

        if (!string.IsNullOrEmpty(model.Category))
        {
            param.Add("Category", model.Category);
            sql.Append(" and category = @Category");
        }

        if (model.MinPriceCents.HasValue)
        {
            param.Add("MinPriceCents", model.MinPriceCents.Value);
            sql.Append(" and price_cents >= @MinPriceCents");
        }

        if (model.MaxPriceCents.HasValue)
        {
            param.Add("MaxPriceCents", model.MaxPriceCents.Value);
            sql.Append(" and price_cents <= @MaxPriceCents");
        }

        if (model.InStock.HasValue && model.InStock.Value)
        {
            sql.Append(" and stock > 0");
        }

        sql.Append(" order by created_at desc");
        sql.Append($" limit {model.PageSize} offset {offset}");

        var conn = await unitOfWork.GetConnection(token);
        var res = await conn.QueryAsync<V1ProductDal>(new CommandDefinition(
            sql.ToString(), param, cancellationToken: token));

        return res.ToArray();
    }

    public async Task<long> QueryTotal(QueryProductsDalModel model, CancellationToken token)
    {
        var sql = new StringBuilder("select count(*) from products where 1=1");
        var param = new DynamicParameters();

        if (!string.IsNullOrEmpty(model.Category))
        {
            param.Add("Category", model.Category);
            sql.Append(" and category = @Category");
        }

        if (model.MinPriceCents.HasValue)
        {
            param.Add("MinPriceCents", model.MinPriceCents.Value);
            sql.Append(" and price_cents >= @MinPriceCents");
        }

        if (model.MaxPriceCents.HasValue)
        {
            param.Add("MaxPriceCents", model.MaxPriceCents.Value);
            sql.Append(" and price_cents <= @MaxPriceCents");
        }

        if (model.InStock.HasValue && model.InStock.Value)
        {
            sql.Append(" and stock > 0");
        }

        var conn = await unitOfWork.GetConnection(token);
        var count = await conn.ExecuteScalarAsync<long>(new CommandDefinition(
            sql.ToString(), param, cancellationToken: token));

        return count;
    }

    public async Task<V1ProductDal> GetById(long id, CancellationToken token)
    {
        var sql = @"
            select 
                id,
                title,
                description,
                category,
                price_cents,
                price_currency,
                product_url,
                stock,
                discount_percent,
                created_at,
                updated_at
            from products
            where id = @Id
        ";

        var conn = await unitOfWork.GetConnection(token);
        var res = await conn.QueryFirstOrDefaultAsync<V1ProductDal>(new CommandDefinition(
            sql, new { Id = id }, cancellationToken: token));

        return res;
    }

    public async Task<V1ProductDal> BulkInsert(V1ProductDal product, CancellationToken token)
    {
        var sql = @"
            insert into products 
            (
                title,
                description,
                category,
                price_cents,
                price_currency,
                product_url,
                stock,
                discount_percent,
                created_at,
                updated_at
            )
            values
            (
                @Title,
                @Description,
                @Category,
                @PriceCents,
                @PriceCurrency,
                @ProductUrl,
                @Stock,
                @DiscountPercent,
                @CreatedAt,
                @UpdatedAt
            )
            returning 
                id,
                title,
                description,
                category,
                price_cents,
                price_currency,
                product_url,
                stock,
                discount_percent,
                created_at,
                updated_at
        ";

        var conn = await unitOfWork.GetConnection(token);
        var res = await conn.QueryFirstAsync<V1ProductDal>(new CommandDefinition(
            sql, product, cancellationToken: token));

        return res;
    }
}

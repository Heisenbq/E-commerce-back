using FluentMigrator;

[Migration(5)]
public class CreateProductTable : Migration
{
    public override void Up()
    {
        var sql = @"
            create table if not exists products (
                id bigserial not null primary key,
                title text not null,
                description text,
                category text,
                price_cents bigint not null,
                price_currency text not null default 'RUB',
                product_url text,
                stock integer not null default 0,
                discount_percent integer default 0,
                created_at timestamp with time zone not null,
                updated_at timestamp with time zone not null
            );

            create index if not exists idx_products_category on products (category);
            create index if not exists idx_products_created_at on products (created_at);

            create type v1_product as (
                id bigint,
                title text,
                description text,
                category text,
                price_cents bigint,
                price_currency text,
                product_url text,
                stock integer,
                discount_percent integer,
                created_at timestamp with time zone,
                updated_at timestamp with time zone
            );
        ";
        
        Execute.Sql(sql);
    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}

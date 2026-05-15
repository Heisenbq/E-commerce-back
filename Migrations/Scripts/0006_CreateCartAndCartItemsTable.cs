using FluentMigrator;

[Migration(6)]
public class CreateCartAndCartItemsTable : Migration
{
    public override void Up()
    {
        var sql = @"
            create table if not exists carts (
                id bigserial not null primary key,
                user_id bigint not null references users(id) on delete cascade,
                created_at timestamp with time zone not null,
                updated_at timestamp with time zone not null
            );

            create index if not exists idx_carts_user_id on carts (user_id);

            create table if not exists cart_items (
                id bigserial not null primary key,
                cart_id bigint not null references carts(id) on delete cascade,
                product_id bigint not null references products(id) on delete cascade,
                quantity integer not null,
                created_at timestamp with time zone not null,
                updated_at timestamp with time zone not null
            );

            create index if not exists idx_cart_items_cart_id on cart_items (cart_id);
            create index if not exists idx_cart_items_product_id on cart_items (product_id);

            create type v1_cart as (
                id bigint,
                user_id bigint,
                created_at timestamp with time zone,
                updated_at timestamp with time zone
            );

            create type v1_cart_item as (
                id bigint,
                cart_id bigint,
                product_id bigint,
                quantity integer,
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

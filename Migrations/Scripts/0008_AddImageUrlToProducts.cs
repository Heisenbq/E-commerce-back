using FluentMigrator;

[Migration(8)]
public class AddImageUrlToProducts : Migration
{
    public override void Up()
    {
        var sql = @"
            ALTER TABLE products ADD COLUMN IF NOT EXISTS image_url text;

            UPDATE products SET image_url = 'https://images.unsplash.com/photo-1510557880182-3d4d3cba35a5?w=400&h=400&fit=crop' WHERE id = 1;
            UPDATE products SET image_url = 'https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=400&h=400&fit=crop' WHERE id = 2;
            UPDATE products SET image_url = 'https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=400&h=400&fit=crop' WHERE id = 3;
            UPDATE products SET image_url = 'https://images.unsplash.com/photo-1588423771073-b8903fbb85b5?w=400&h=400&fit=crop' WHERE id = 4;
            UPDATE products SET image_url = 'https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=400&h=400&fit=crop' WHERE id = 5;
            UPDATE products SET image_url = 'https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?w=400&h=400&fit=crop' WHERE id = 6;
            UPDATE products SET image_url = 'https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=400&h=400&fit=crop' WHERE id = 7;
            UPDATE products SET image_url = 'https://images.unsplash.com/photo-1542272604-787c3835535d?w=400&h=400&fit=crop' WHERE id = 8;
            UPDATE products SET image_url = 'https://images.unsplash.com/photo-1551488831-00ddcb6c6bd3?w=400&h=400&fit=crop' WHERE id = 9;
            UPDATE products SET image_url = 'https://images.unsplash.com/photo-1608231387042-66d1773070a5?w=400&h=400&fit=crop' WHERE id = 10;
            UPDATE products SET image_url = 'https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=400&h=400&fit=crop' WHERE id = 11;
            UPDATE products SET image_url = 'https://images.unsplash.com/photo-1585515320310-259814833e62?w=400&h=400&fit=crop' WHERE id = 12;
            UPDATE products SET image_url = 'https://images.unsplash.com/photo-1555041469-a586c61ea9bc?w=400&h=400&fit=crop' WHERE id = 13;
            UPDATE products SET image_url = 'https://images.unsplash.com/photo-1558618047-3c8c76ca7d13?w=400&h=400&fit=crop' WHERE id = 14;
            UPDATE products SET image_url = 'https://images.unsplash.com/photo-1544716278-ca5e3f4abd8c?w=400&h=400&fit=crop' WHERE id = 15;
            UPDATE products SET image_url = 'https://images.unsplash.com/photo-1532012197267-da84d127e765?w=400&h=400&fit=crop' WHERE id = 16;
            UPDATE products SET image_url = 'https://images.unsplash.com/photo-1512820790803-83ca734da794?w=400&h=400&fit=crop' WHERE id = 17;
            UPDATE products SET image_url = 'https://images.unsplash.com/photo-1534438327276-14e5300c3a48?w=400&h=400&fit=crop' WHERE id = 18;
            UPDATE products SET image_url = 'https://images.unsplash.com/photo-1571019613454-1cb2f99b2d8b?w=400&h=400&fit=crop' WHERE id = 19;
            UPDATE products SET image_url = 'https://images.unsplash.com/photo-1532298229144-0ec0c57515c7?w=400&h=400&fit=crop' WHERE id = 20;

            ALTER TYPE v1_product ADD ATTRIBUTE image_url text;
        ";

        Execute.Sql(sql);
    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}

using FluentMigrator;

[Migration(7)]
public class AddUniqueConstraintToCartsUserId : Migration
{
    public override void Up()
    {
        Execute.Sql("ALTER TABLE carts ADD CONSTRAINT carts_user_id_unique UNIQUE (user_id);");
    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}

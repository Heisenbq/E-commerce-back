using FluentMigrator;

[Migration(4)]
public class CreateUserTable : Migration
{
    public override void Up()
    {
        var sql = @"
            create table if not exists users (
                id bigserial not null primary key,
                username text not null unique,
                email text not null unique,
                password_hash text not null,
                created_at timestamp with time zone not null,
                updated_at timestamp with time zone not null
            );

            create index if not exists idx_users_username on users (username);
            create index if not exists idx_users_email on users (email);
        ";
        
        Execute.Sql(sql);
    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}

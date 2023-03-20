create schema [web] authorization [dbo];
go

create user [webapp] with password = 'Super_Str0ng*P4ZZword!'
go

grant execute on schema::[web] to [webapp]
go

GRANT SELECT ON OBJECT::dbo.ToDo TO [webapp];
GRANT INSERT ON OBJECT::dbo.ToDo TO [webapp];
GRANT UPDATE ON OBJECT::dbo.ToDo TO [webapp];
GRANT DELETE ON OBJECT::dbo.ToDo TO [webapp];
go


CREATE TABLE dbo.ToDo (
    Id uniqueidentifier primary key,
    [order] int null,
    title nvarchar(200) not null,
    [url] nvarchar(200) not null,
    completed bit not null
);
GO


CREATE PROCEDURE [web].[DeleteToDoById]
    @Id nvarchar(100)
AS
    DECLARE @UID UNIQUEIDENTIFIER = TRY_CAST(@ID AS uniqueidentifier)
    IF @UId IS NOT NULL AND @Id != ''
    BEGIN
        DELETE FROM dbo.ToDo WHERE Id = @UID;
    END
    ELSE
    BEGIN
        DELETE FROM dbo.ToDo WHERE @ID = '';
    END

    SELECT Id, [order], title, url, completed FROM dbo.ToDo
GO

CREATE PROCEDURE [web].[DeleteToDo]
AS
    DELETE FROM dbo.ToDo;

    SELECT Id, [order], title, url, completed FROM dbo.ToDo
GO

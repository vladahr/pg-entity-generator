# pg-entity-generator
.NET scaffold entities from Postgres database
# This project designed to generate CSharp classes based on table definitions from PostgreSQL db.
1. Get .NET6 https://dotnet.microsoft.com/en-us/download/dotnet/6.0
2. Right click on a project and choose Publish, then select profile according to your OS ( win/linux/osx x64 ) and click finish.
3. Go to bin\Release\net6.0\publish\win-x64 folder and find PgEntityGenerator.exe
4. Run exe with specified arguments: schema_name table_name connection_string

Example to run via powershell:
.\PgEntityGenerator.exe **"Server=localhost;Port=5432;Database=test;User ID=postgres;Password=****;CommandTimeout=300;" "tschema" "table" **

5. Find \*Entity.cs \*Map.cs files next to exe file.

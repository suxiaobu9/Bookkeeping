dotnet tool install --global dotnet-ef --version 6.0.1

dotnet ef dbcontext scaffold "Server=;Database=;user id=;password=" Microsoft.EntityFrameworkCore.SqlServer 
dotnet ef dbcontext scaffold "Server=;Database=;user id=;password=" Microsoft.EntityFrameworkCore.SqlServer --force
dotnet ef dbcontext scaffold "Server=;Database=;user id=;password=" Microsoft.EntityFrameworkCore.SqlServer -o Model
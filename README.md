# Reproduce FCS bug

Require .NET Core Sdk v2.1.401 installed

```bash
# first restore the repro project
dotnet restore repro

# build and run, to typecheck with FCS that repro project
dotnet build

dotnet run -f netcoreapp2.1
```

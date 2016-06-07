set V=2.10.4
nuget push VirtoCommerce.ContentModule.Client.%V%.nupkg -Source nuget.org -ApiKey %1
nuget push VirtoCommerce.ContentModule.Data.%V%.nupkg -Source nuget.org -ApiKey %1
pause

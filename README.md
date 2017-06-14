Dave's Deep Date Updater
================

Uses database metadata to generate deep date update procedures for SQL Server.

Available as a [NuGet Package](https://www.nuget.org/packages/Daves.DeepDateUpdater/).

I use this in conjunction with my [deep copy generator](https://github.com/davghouse/Daves.DeepDataDuplicator) to keep template organizations in my multi-tenant database up to date.
Dependencies are discovered in the same way, but instead of copying data we update all date type columns by the specified delta.
